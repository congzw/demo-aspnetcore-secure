namespace NbSites.Web.PermissionChecks.RoleBased
{
    public interface IRoleBasedPermissionRuleLogic
    {
        PermissionCheckResult CheckRule(RoleBasedPermissionRule rule, PermissionCheckContext checkContext);
    }

    public class RoleBasedPermissionRuleLogic : IRoleBasedPermissionRuleLogic
    {
        public PermissionCheckResult CheckRule(RoleBasedPermissionRule rule, PermissionCheckContext checkContext)
        {
            if (rule == null)
            {
                return PermissionCheckResult.NoCare.WithMessage("不认识的规则");
            }

            if (checkContext.CheckPermissionIds == null || checkContext.CheckPermissionIds.Count == 0)
            {
                return PermissionCheckResult.NoCare.WithMessage("没有指定需要检测的PermissionId");
            }

            if (!checkContext.MatchPermissionId(rule.PermissionId))
            {
                return PermissionCheckResult.NoCare.WithMessage($"规则不匹配: {rule.PermissionId} not in [{string.Join(',', checkContext.CheckPermissionIds)}]");
            }

            var userContext = checkContext.CurrentUserContext;

            if (rule.NeedGuest())
            {
                return PermissionCheckResult.Allowed.WithMessage("访客规则");
            }

            var hasLogin = userContext.IsLogin();
            if (!hasLogin)
            {
                return PermissionCheckResult.Forbidden.WithMessage("需要登录");
            }

            if (rule.NeedLogin())
            {
                return PermissionCheckResult.Allowed.WithMessage("登录即可");
            }

            var msg = $"指定用户或角色满足: ctx:[{userContext.User}],[{userContext.Roles.MyJoin()}] + rule:[{rule.AllowedUsers}],[{rule.AllowedRoles}]";
            if (rule.NeedUsersOrRoles(userContext.User, userContext.Roles.MyJoin()))
            {
                return PermissionCheckResult.Allowed.WithMessage(msg + " => OK");
            }

            return PermissionCheckResult.Forbidden.WithMessage(msg + " => K.O.");
        }
    }
}