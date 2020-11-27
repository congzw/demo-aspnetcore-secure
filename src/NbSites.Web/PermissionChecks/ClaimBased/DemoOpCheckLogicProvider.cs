using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NbSites.Web.PermissionChecks.ClaimBased
{
    public class DemoOpCheckLogicProvider : IPermissionCheckLogicProvider
    {
        private readonly IPermissionCheckDebugHelper _debugHelper;
        private readonly ILogger<DemoOpCheckLogicProvider> _logger;

        public DemoOpCheckLogicProvider(IPermissionCheckDebugHelper debugHelper, ILogger<DemoOpCheckLogicProvider> logger)
        {
            _debugHelper = debugHelper;
            _logger = logger;
        }

        public int Order { get; set; }

        public bool ShouldCare(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            //Provider按需进行处理
            return permissionCheckContext.MatchPermissionId(KnownPermissionIds.DemoOp);
        }

        public Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            //此示例演示基于自身Claims可以完成判断逻辑的场景
            var msg = "需要授权: " + KnownPermissionIds.DemoOp;
            if (userContext.Permissions.MyContains(KnownPermissionIds.DemoOp))
            {
                var allowedCheckResult = PermissionCheckResult.Allowed
                    .WithMessage(msg)
                    .WithData(KnownPermissionIds.DemoOp);
                _debugHelper.AppendPermissionCheckResults(allowedCheckResult);
                _logger.LogInformation(allowedCheckResult.Message);
                return Task.FromResult(allowedCheckResult);
            }

            var forbiddenResult = PermissionCheckResult.Forbidden
                .WithMessage(msg)
                .WithData(KnownPermissionIds.DemoOp);
            _debugHelper.AppendPermissionCheckResults(forbiddenResult);
            _logger.LogInformation(forbiddenResult.Message);

            return Task.FromResult(forbiddenResult);
        }

    }
}
