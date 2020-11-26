using System.Linq;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public class RoleBasedPermissionRule
    {
        public string PermissionId { get; set; }
        public string AllowedUsers { get; set; }
        public string AllowedRoles { get; set; }

        private PermissionRuleExpression _allowedUsersExpression;
        public PermissionRuleExpression AllowedUsersExpression
        {
            get
            {
                return _allowedUsersExpression ??= PermissionRuleExpression.Create(AllowedUsers);
            }
        }
        
        private PermissionRuleExpression _allowedRolesExpression;
        public PermissionRuleExpression AllowedRolesExpression
        {
            get
            {
                return _allowedRolesExpression ??= PermissionRuleExpression.Create(AllowedRoles);
            }
        }

        public static RoleBasedPermissionRule Create(string permissionId, string allowedUsers, string allowedRoles)
        {
            return new RoleBasedPermissionRule() { PermissionId = permissionId, AllowedRoles = allowedRoles, AllowedUsers = allowedUsers };
        }

        public static RoleBasedPermissionRule CreateGuestRule(string permissionId)
        {
            return new RoleBasedPermissionRule()
            {
                PermissionId = permissionId,
                AllowedRoles = PermissionRuleExpression.Any.Value,
                AllowedUsers = PermissionRuleExpression.Any.Value
            };
        }

        public static RoleBasedPermissionRule CreateLoginRule(string permissionId)
        {
            return new RoleBasedPermissionRule()
            {
                PermissionId = permissionId,
                AllowedUsers = PermissionRuleExpression.Any.Value,
                AllowedRoles = PermissionRuleExpression.None.Value
            };
        }

        /// <summary>
        /// 任何访客都可以
        /// </summary>
        /// <returns></returns>
        public bool NeedGuest()
        {
            //"*", "*"  => allowed any visitor, include guest
            if (AllowedUsersExpression.AllowAny() && AllowedRolesExpression.AllowAny())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 只要登录就可以
        /// </summary>
        /// <returns></returns>
        public bool NeedLogin()
        {
            //"*", ""  => allowed any Login user    
            //"", "*"  => allowed any Login user
            if (AllowedUsersExpression.DenyAll() && AllowedRolesExpression.AllowAny())
            {
                return true;
            }
            if (AllowedUsersExpression.AllowAny() && AllowedRolesExpression.DenyAll())
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 满足其中的某个用户
        /// </summary>
        /// <param name="usersValue"></param>
        /// <returns></returns>
        public bool NeedUsers(string usersValue)
        {
            if (AllowedUsersExpression.AllowAny())
            {
                return true;
            }

            if (AllowedUsersExpression.DenyAll())
            {
                return false;
            }

            return AllowedUsersExpression.AllowAnyOf(usersValue.MySplit().ToArray());
        }

        /// <summary>
        /// 满足其中的某个角色
        /// </summary>
        /// <param name="rolesValue"></param>
        /// <returns></returns>
        public bool NeedRoles(string rolesValue)
        {
            if (AllowedRolesExpression.AllowAny())
            {
                return true;
            }

            if (AllowedRolesExpression.DenyAll())
            {
                return false;
            }

            return AllowedRolesExpression.AllowAnyOf(rolesValue.MySplit().ToArray());
        }

        /// <summary>
        /// 满足其中的某个用户或角色
        /// </summary>
        /// <param name="usersValue"></param>
        /// <param name="rolesValue"></param>
        /// <returns></returns>
        public bool NeedUsersOrRoles(string usersValue, string rolesValue)
        {
            return NeedUsers(usersValue) || NeedRoles(rolesValue);
        }
        
        public RoleBasedPermissionRule SetNeedGuest()
        {
            var newRule = CreateGuestRule(this.PermissionId);
            this._allowedUsersExpression = null;
            this._allowedRolesExpression = null;
            this.AllowedUsers = newRule.AllowedUsers;
            this.AllowedRoles = newRule.AllowedRoles;
            return this;
        }
        public RoleBasedPermissionRule SetNeedLogin()
        {
            var newRule = CreateLoginRule(this.PermissionId);
            this._allowedUsersExpression = null;
            this._allowedRolesExpression = null;
            this.AllowedUsers = newRule.AllowedUsers;
            this.AllowedRoles = newRule.AllowedRoles;
            return this;
        }
        public RoleBasedPermissionRule SetNeedUsersOrRoles(string allowedUsers, string allowedRoles)
        {
            var newRule = Create(this.PermissionId, allowedUsers, allowedRoles);
            this._allowedUsersExpression = null;
            this._allowedRolesExpression = null;
            this.AllowedUsers = newRule.AllowedUsers;
            this.AllowedRoles = newRule.AllowedRoles;
            return this;
        }
    }
}