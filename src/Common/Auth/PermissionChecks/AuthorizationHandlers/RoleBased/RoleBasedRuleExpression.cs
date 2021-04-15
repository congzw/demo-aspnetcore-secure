using System;
using Common.Auth.PermissionChecks.Rules;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased
{
    //public class RoleBasedRuleExpression
    //{
    //    public string PermissionId { get; set; }
    //    public string AllowedUsers { get; set; }
    //    public string AllowedRoles { get; set; }

    //    public string ToExpression()
    //    {
    //        return ParseRuleExpression(this);
    //    }

    //    #region factory helpers
        
    //    public static RoleBasedRuleExpression CreateGuestRule(string permissionId)
    //    {
    //        return new RoleBasedRuleExpression()
    //        {
    //            PermissionId = permissionId,
    //            AllowedRoles = SimpleRuleExpression.Any.Value,
    //            AllowedUsers = SimpleRuleExpression.Any.Value
    //        };
    //    }
    //    public RoleBasedRuleExpression WithUsers(string allowedUsers)
    //    {
    //        this.AllowedUsers = allowedUsers;
    //        return this;
    //    }
    //    public RoleBasedRuleExpression WithRoles(string allowedRoles)
    //    {
    //        this.AllowedRoles = allowedRoles;
    //        return this;
    //    }

    //    public static string ParseRuleExpression(RoleBasedRuleExpression rule)
    //    {
    //        return $"{rule.PermissionId}::{rule.AllowedUsers}::{rule.AllowedRoles}";
    //    }
    //    public static RoleBasedRuleExpression ParseRuleExpression(string ruleExpression)
    //    {
    //        try
    //        {
    //            var values = ruleExpression.Split("::");
    //            var roleBasedRuleExpression = new RoleBasedRuleExpression();
    //            roleBasedRuleExpression.PermissionId = values[0];
    //            roleBasedRuleExpression.AllowedUsers = values[1];
    //            roleBasedRuleExpression.AllowedRoles = values[2];
    //            return roleBasedRuleExpression;
    //        }
    //        catch
    //        {
    //            var tip = "{PermissionId}::{AllowedUsers}::{AllowedRoles}";
    //            throw new Exception($"无效的权限表达式：{ruleExpression}, 格式参考：{tip}");
    //        }
    //    }

    //    #endregion

    //    #region validate helpers

    //    /// <summary>
    //    /// 满足其中的某个用户
    //    /// </summary>
    //    /// <param name="users"></param>
    //    /// <returns></returns>
    //    public bool ValidateUsers(string users)
    //    {
    //        return SimpleRuleExpression.Create(AllowedUsers).AllowAnyOfValue(users);
    //    }

    //    /// <summary>
    //    /// 满足其中的某个角色
    //    /// </summary>
    //    /// <param name="roles"></param>
    //    /// <returns></returns>
    //    public bool ValidateRoles(string roles)
    //    {
    //        return SimpleRuleExpression.Create(AllowedRoles).AllowAnyOfValue(roles);
    //    }

    //    #endregion
    //}

    public class RoleBasedRuleExpression
    {
        public RoleBasedRuleExpression(SimpleRuleExpression needUsers, SimpleRuleExpression needRoles)
        {
            NeedUsers = needUsers;
            NeedRoles = needRoles;
        }

        public SimpleRuleExpression NeedUsers { get; set; }
        public SimpleRuleExpression NeedRoles { get; set; }
        public RoleBasedRuleExpression SetNeedUsers(string needUsers)
        {
            this.NeedUsers = SimpleRuleExpression.Create(needUsers);
            return this;
        }
        public RoleBasedRuleExpression SetNeedRoles(string needRoles)
        {
            this.NeedRoles = SimpleRuleExpression.Create(needRoles);
            return this;
        }
        public string ToRule()
        {
            return ParseRule(this);
        }
        
        public static RoleBasedRuleExpression NeedGuest = new RoleBasedRuleExpression(SimpleRuleExpression.Any, SimpleRuleExpression.Any);
        public static RoleBasedRuleExpression NeedLogin = new RoleBasedRuleExpression(SimpleRuleExpression.Any, SimpleRuleExpression.None);
        public static RoleBasedRuleExpression NeedUsersOrRoles(string needUsers, string needRoles)
        {
            return new RoleBasedRuleExpression(SimpleRuleExpression.Create(needUsers), SimpleRuleExpression.Create(needRoles));
        }

        private static string splitter = "::";
        public static RoleBasedRuleExpression ParseRule(string ruleValue)
        {
            try
            {
                var values = ruleValue.Split(splitter);
                var needUsers = SimpleRuleExpression.Create(values[0]);
                var needRoles = SimpleRuleExpression.Create(values[1]);
                var theExpression = new RoleBasedRuleExpression(needUsers, needRoles);
                return theExpression;
            }
            catch
            {
                var tip = "{AllowedUsers}::{AllowedRoles}";
                throw new Exception($"无效的权限表达式：{ruleValue}, 格式参考：{tip}");
            }
        }
        public static string ParseRule(RoleBasedRuleExpression ruleExpression)
        {
            return $"{ruleExpression.NeedUsers.Value}{splitter}{ruleExpression.NeedRoles.Value}";
        }

        #region validate helpers

        /// <summary>
        /// 满足其中的某个用户
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public bool ValidateUsers(string users)
        {
            return NeedUsers.AllowAnyOfValue(users);
        }

        /// <summary>
        /// 满足其中的某个角色
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool ValidateRoles(string roles)
        {
            return NeedRoles.AllowAnyOfValue(roles);
        }

        #endregion
    }
}
