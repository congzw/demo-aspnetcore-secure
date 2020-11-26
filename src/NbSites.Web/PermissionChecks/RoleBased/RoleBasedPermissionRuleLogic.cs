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
                return PermissionCheckResult.Allowed.WithMessage("没有定义规则 => 放行");
            }

            if (checkContext.CheckPermissionIds == null || checkContext.CheckPermissionIds.Count == 0)
            {
                return PermissionCheckResult.Allowed.WithMessage("没有指定需要检测的PermissionId => 放行");
            }

            if (!checkContext.MatchPermissionId(rule.PermissionId))
            {
                return PermissionCheckResult.NoCare.WithMessage($"规则不匹配 => 无法判断: {rule.PermissionId} ? [{string.Join(',', checkContext.CheckPermissionIds)}]");
            }

            var userContext = checkContext.CurrentUserContext;
            if (rule.NeedGuest())
            {
                return PermissionCheckResult.Allowed.WithMessage("访客规则 => 满足");
            }

            var hasLogin = userContext.IsLogin();
            if (!hasLogin)
            {
                return PermissionCheckResult.Forbidden.WithMessage("需要登录 => 不满足");
            }

            if (rule.NeedLogin())
            {
                return PermissionCheckResult.Allowed.WithMessage("需要登录 => 满足");
            }

            var msg = $"指定用户或角色: ctx:[{userContext.User}],[{userContext.Roles.MyJoin()}] + rule:[{rule.AllowedUsers}],[{rule.AllowedRoles}]";
            if (rule.NeedUsersOrRoles(userContext.User, userContext.Roles.MyJoin()))
            {
                return PermissionCheckResult.Allowed.WithMessage(msg + " => 满足");
            }

            return PermissionCheckResult.Forbidden.WithMessage(msg + " => 不满足");
        }
    }
}