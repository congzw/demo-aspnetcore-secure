using System;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased
{
    public class RoleBasedRuleExpression
    {
        public RoleBasedRuleExpression(AllowedRuleExpression needUsers, AllowedRuleExpression needRoles)
        {
            NeedUsers = needUsers;
            NeedRoles = needRoles;
        }

        public AllowedRuleExpression NeedUsers { get; set; }
        public AllowedRuleExpression NeedRoles { get; set; }
        public RoleBasedRuleExpression SetNeedUsers(string needUsers)
        {
            this.NeedUsers = AllowedRuleExpression.Create(needUsers);
            return this;
        }
        public RoleBasedRuleExpression SetNeedRoles(string needRoles)
        {
            this.NeedRoles = AllowedRuleExpression.Create(needRoles);
            return this;
        }
        public string ToRule()
        {
            return ParseRule(this);
        }
        
        public static RoleBasedRuleExpression NeedGuest = new RoleBasedRuleExpression(AllowedRuleExpression.Any, AllowedRuleExpression.Any);
        public static RoleBasedRuleExpression NeedLogin = new RoleBasedRuleExpression(AllowedRuleExpression.Empty, AllowedRuleExpression.Any);
        public static RoleBasedRuleExpression NeedUsersOrRoles(string needUsers, string needRoles)
        {
            return new RoleBasedRuleExpression(AllowedRuleExpression.Create(needUsers), AllowedRuleExpression.Create(needRoles));
        }

        private static string splitter_or = "||";
        public static RoleBasedRuleExpression ParseRule(string ruleValue)
        {
            try
            {
                var values = ruleValue.Split(splitter_or);
                var theExpression = new RoleBasedRuleExpression(AllowedRuleExpression.Create(values[0]), AllowedRuleExpression.Create(values[1]));
                return theExpression;
            }
            catch
            {
                var tip = "{AllowedUsers}||{AllowedRoles}";
                throw new Exception($"无效的权限表达式：{ruleValue}, 格式参考：{tip}");
            }
        }
        public static string ParseRule(RoleBasedRuleExpression ruleExpression)
        {
            return $"{ruleExpression.NeedUsers.Value}{splitter_or}{ruleExpression.NeedRoles.Value}";
        }

        #region validate helpers
        
        /// <summary>
        /// 任何访客都可以
        /// </summary>
        /// <returns></returns>
        public bool ValidateNeedGuest()
        {
            //"*", "*"  => allowed any visitor, include guest
            if (NeedUsers.AllowAny() && NeedRoles.AllowAny())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 只要登录就可以
        /// </summary>
        /// <returns></returns>
        public bool ValidateNeedLogin()
        {
            //"*", ""  => allowed any Login user    
            //"", "*"  => allowed any Login user
            if (NeedUsers.AllowExist() && NeedRoles.AllowAny())
            {
                return true;
            }
            if (NeedUsers.AllowAny() && NeedRoles.AllowExist())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 满足其中的某个用户
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public bool ValidateNeedAnyOfUsers(string users)
        {
            return NeedUsers.AllowAnyOfValue(users);
        }

        /// <summary>
        /// 满足其中的某个角色
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool ValidateNeedAnyOfRoles(string roles)
        {
            return NeedRoles.AllowAnyOfValue(roles);
        }
        
        /// <summary>
        /// 满足其中的某个用户或角色
        /// </summary>
        /// <param name="usersValue"></param>
        /// <param name="rolesValue"></param>
        /// <returns></returns>
        public bool ValidateNeedAnyOfUsersOrRoles(string usersValue, string rolesValue)
        {
            return ValidateNeedAnyOfUsers(usersValue) || ValidateNeedAnyOfRoles(rolesValue);
        }

        #endregion
    }
}
