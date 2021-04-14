using System;
using System.Threading.Tasks;
using Common.Auth.PermissionChecks.ControlPoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers
{
    public class PermissionCheckVoteHandler : AuthorizationHandler<PermissionCheckRequirement>
    {
        private readonly ILogger<PermissionCheckVoteHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IControlPointService _controlPointService;
        private readonly IPermissionCheckVoteService _permissionCheckService;

        public PermissionCheckVoteHandler(ILogger<PermissionCheckVoteHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            IControlPointService controlPointService,
            IPermissionCheckVoteService permissionCheckService)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _controlPointService = controlPointService;
            _permissionCheckService = permissionCheckService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var endPointId = httpContext.GetCurrentActionId();
            if (endPointId == null)
            {
                return;
            }
            
            var checkResult = await RunCheckAsync(httpContext, endPointId, requirement);
            LogCheckResult(endPointId, checkResult);

            switch (checkResult.Category)
            {
                case PermissionCheckResultCategory.Allowed:
                    context.Succeed(requirement);
                    return;
                case PermissionCheckResultCategory.Forbidden:
                    context.Fail();
                    break;
                case PermissionCheckResultCategory.NotSure:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<PermissionCheckResult> RunCheckAsync(HttpContext httpContext, string endPointId, PermissionCheckRequirement requirement)
        {
            var permissionIds = _controlPointService.GetCurrentEndPointPermissionIds(httpContext, endPointId)?.ToArray();
            //- 非注册的控制点 => Allowed
            //- 注册的控制点，使用投票服务(PermissionCheckVoteService)计算结果
            if (permissionIds == null || permissionIds.Length == 0)
            {
                return PermissionCheckResult.Allowed.WithMessage("非注册的控制点，放行").WithTarget(endPointId);
                //return PermissionCheckResult.NotSure.WithMessage("非注册的控制点，不置可否");
            }

            var userContext = httpContext.GetCurrentUserContext();
            var registry = httpContext.RequestServices.GetService<ControlPointRegistry>();
            var checkContext = PermissionCheckContext.Create(registry, userContext, requirement, permissionIds);
            var checkResult = await _permissionCheckService.CheckAsync(checkContext);
            return checkResult.WithTarget(endPointId);
        }

        private void LogCheckResult(string endPointId, PermissionCheckResult checkResult)
        {
            var logMsg = $"{endPointId} => {checkResult.GetVoteDescription()}";
            _logger.LogInformation(logMsg);
            //PermissionCheckDebugHelper.Instance.SetLastResultDescription(endPointId + checkResult.GetVoteDescription());
            PermissionCheckDebugHelper.Instance.AppendPermissionCheckResults(checkResult);
        }
    }
}
