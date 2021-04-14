using System;
using Common.Auth.PermissionChecks.Rules;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased
{
    public class RoleBasedRuleExpression
    {
        public string PermissionId { get; set; }
        public string AllowedUsers { get; set; }
        public string AllowedRoles { get; set; }

        public string ToExpression()
        {
            return ParseRuleExpression(this);
        }

        #region factory helpers
        
        public static RoleBasedRuleExpression CreateGuestRule(string permissionId)
        {
            return new RoleBasedRuleExpression()
            {
                PermissionId = permissionId,
                AllowedRoles = PermissionRuleExpression.Any.Value,
                AllowedUsers = PermissionRuleExpression.Any.Value
            };
        }
        public RoleBasedRuleExpression WithUsers(string allowedUsers)
        {
            this.AllowedUsers = allowedUsers;
            return this;
        }
        public RoleBasedRuleExpression WithRoles(string allowedRoles)
        {
            this.AllowedRoles = allowedRoles;
            return this;
        }

        public static string ParseRuleExpression(RoleBasedRuleExpression rule)
        {
            return $"{rule.PermissionId}::{rule.AllowedUsers}::{rule.AllowedRoles}";
        }
        public static RoleBasedRuleExpression ParseRuleExpression(string ruleExpression)
        {
            try
            {
                var values = ruleExpression.Split("::");
                var roleBasedRuleExpression = new RoleBasedRuleExpression();
                roleBasedRuleExpression.PermissionId = values[0];
                roleBasedRuleExpression.AllowedUsers = values[1];
                roleBasedRuleExpression.AllowedRoles = values[2];
                return roleBasedRuleExpression;
            }
            catch
            {
                var tip = "{PermissionId}::{AllowedUsers}::{AllowedRoles}";
                throw new Exception($"无效的权限表达式：{ruleExpression}, 格式参考：{tip}");
            }
        }

        #endregion

        #region validate helpers

        /// <summary>
        /// 满足其中的某个用户
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public bool ValidateUsers(string users)
        {
            return PermissionRuleExpression.Create(AllowedUsers).AllowAnyOfValue(users);
        }

        /// <summary>
        /// 满足其中的某个角色
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool ValidateRoles(string roles)
        {
            return PermissionRuleExpression.Create(AllowedRoles).AllowAnyOfValue(roles);
        }

        #endregion
    }
}
