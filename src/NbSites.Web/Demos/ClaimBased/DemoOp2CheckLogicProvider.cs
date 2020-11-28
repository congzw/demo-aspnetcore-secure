using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NbSites.Web.PermissionChecks;

namespace NbSites.Web.Demos.ClaimBased
{
    public class DemoOp2CheckLogicProvider : IPermissionCheckLogicProvider
    {
        private readonly IPermissionCheckDebugHelper _debugHelper;
        private readonly ILogger<DemoOp2CheckLogicProvider> _logger;

        public DemoOp2CheckLogicProvider(IPermissionCheckDebugHelper debugHelper, ILogger<DemoOp2CheckLogicProvider> logger)
        {
            _debugHelper = debugHelper;
            _logger = logger;
        }


        public int Order { get; set; }

        public Task<bool> ShouldCareAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            //按需决定是否需要参与
            return Task.FromResult(permissionCheckContext.MatchPermissionId(KnownPermissionIds.DemoOp2));
        }

        public async Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            //此示例演示基于额外的请求上下文，才能完成判断逻辑的场景，例如Query，Form， Cookie等
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));
            if (permissionCheckContext == null) throw new ArgumentNullException(nameof(permissionCheckContext));

            //todo: read current user's allowed scoped from database or somewhere else
            var scopeOrgIds = new List<string>(){"123", "789"};
            var currentOrgId = await TryGetCurrentOrgId(permissionCheckContext);
            if (scopeOrgIds.MyContains(currentOrgId))
            {
                var permissionCheckResult = PermissionCheckResult.Allowed.WithMessage("当前组织已授权: " + currentOrgId);
                _debugHelper.AppendPermissionCheckResults(permissionCheckResult);
                _logger.LogInformation(permissionCheckResult.Message);
                return permissionCheckResult;
            }

            var permissionCheckResult2 = PermissionCheckResult.Forbidden.WithMessage("当前组织没有授权: " + currentOrgId);
            _debugHelper.AppendPermissionCheckResults(permissionCheckResult2);
            _logger.LogInformation(permissionCheckResult2.Message);
            return permissionCheckResult2;
        }

        private static async Task<string> TryGetCurrentOrgId(PermissionCheckContext permissionCheckContext)
        {
            //or read currentOrgId from other context
            var httpContext = permissionCheckContext.HttpContext;
            if (httpContext.Request.Query.TryGetValue("orgId", out var currentOrgId))
            {
                return currentOrgId;
            }

            var form = await httpContext.Request.ReadFormAsync();
            if (form.TryGetValue("orgId", out currentOrgId))
            {
                return currentOrgId;
            }

            //or read currentOrgId from other context
            return null;
        }
    }
}
