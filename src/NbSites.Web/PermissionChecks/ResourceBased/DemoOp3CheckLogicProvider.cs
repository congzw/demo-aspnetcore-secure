using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;

namespace NbSites.Web.PermissionChecks.ResourceBased
{
    public class DemoOp3CheckLogicProvider : IResourceBasedCheckLogicProvider
    {
        private readonly IPermissionCheckDebugHelper _debugHelper;
        private readonly ILogger<DemoOp3CheckLogicProvider> _logger;

        public DemoOp3CheckLogicProvider(IPermissionCheckDebugHelper debugHelper, ILogger<DemoOp3CheckLogicProvider> logger)
        {
            _debugHelper = debugHelper;
            _logger = logger;
        }

        public int Order { get; set; }

        public bool ShouldCare(ICurrentUserContext userContext, OperationAuthorizationRequirement requirement, object resource)
        {
            //根据需要自行添加逻辑
            return requirement.Name.MyEquals(Operations.Delete.Name) && resource is Org;
        }

        public Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, OperationAuthorizationRequirement requirement, object resource)
        {
            //此示例演示基于资源的客体授权，由代码主动调用
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));
            if (requirement == null) throw new ArgumentNullException(nameof(requirement));
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            if (resource is Org theOrg)
            {
                //todo: 检测当前用户身份有删除该资源的权限
                //模拟只有删除777的权限
                if (theOrg.OrgId == "777")
                {
                    return Task.FromResult(PermissionCheckResult.Allowed.WithMessage($"{userContext.User}删除777的权限 => Allowed"));
                }
            }

            //return Task.FromResult(PermissionCheckResult.Forbidden.WithMessage($"{userContext.User}删除777的权限 => Forbidden")); //其他LogicHandler允许也没用
            return Task.FromResult(PermissionCheckResult.NotSure.WithMessage($"{userContext.User}删除777的权限 => NotSure")); //不置可否，由其他LogicHandler决定
        }
    }
}
