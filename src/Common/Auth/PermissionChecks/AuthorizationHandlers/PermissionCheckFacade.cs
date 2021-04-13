using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers
{
    public class PermissionCheckFacade : AuthorizationHandler<PermissionCheckRequirement>
    {
        private readonly ILogger<PermissionCheckFacade> _logger;
        private readonly ICurrentUserContext _userContext;
        private readonly IList<IPermissionCheckLogicProvider> _providers;

        public PermissionCheckFacade(ILogger<PermissionCheckFacade> logger, 
            IEnumerable<IPermissionCheckLogicProvider> providers, 
            ICurrentUserContext userContext)
        {
            _logger = logger;
            _userContext = userContext;
            _providers = providers.OrderBy(x => x.Order).ToList();
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckRequirement requirement)
        {
            _logger.LogInformation("should success?");
            
            //var httpContext = _httpContextAccessor.HttpContext;
            //var actionDescriptor = context.GetControllerActionDescriptor(httpContext);
            //if (actionDescriptor == null)
            //{
            //    await Task.CompletedTask;
            //    return;
            //}

            //_logger.LogInformation(actionDescriptor.DisplayName);

            //var actionId = actionDescriptor.DisplayName.Split().FirstOrDefault();
            //var permissionIds = _ruleActionPool.TryGetPermissionIdsByActionId(actionId);
            //if (permissionIds.Count == 0)
            //{
            //    //如果找到ActionId的配置，以此为准，否则以Attribute为准
            //    permissionIds = context.GetPermissionAttributes(httpContext).Select(x => x.PermissionId).ToList();
            //}

            //if (permissionIds.Count == 0)
            //{
            //    if (_snapshot.Value.RequiredLoginForUnknown)
            //    {
            //        _logger.LogInformation("没有任何对应规则，根据配置执行登录检测");
            //        permissionIds.Add(KnownPermissionIds.LoginOp);
            //    }
            //}

            //var permissionCheckContext = CreatePermissionCheckContext(context, httpContext, actionDescriptor, permissionIds, requirement);


            foreach (var logicProvider in _providers)
            {
            }

            return Task.CompletedTask;
        }

        //private PermissionCheckContext CreatePermissionCheckContext(
        //    AuthorizationHandlerContext context,
        //    HttpContext httpContext,
        //    ActionDescriptor actionDescriptor,
        //    IEnumerable<string> permissionIds,
        //    PermissionCheckRequirement requirement)
        //{
        //    var permissionCheckContext = new PermissionCheckContext(actionDescriptor, httpContext, _userContext, requirement);
        //    permissionCheckContext.AddCheckPermissionIds(permissionIds?.ToArray());
        //    return permissionCheckContext;
        //}
    }
}
