﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Common.Auth
{
    public interface ICurrentUserContext
    {
        /// <summary>
        /// 可以用来区分请求上下文是User,System等
        /// </summary>
        string ClientType { get; set; }
        string User { get; set; }
        List<string> Roles { get; set; }
        List<string> Permissions { get; set; }

        [System.Text.Json.Serialization.JsonIgnore, Newtonsoft.Json.JsonIgnore]
        List<Claim> Claims { get; set; }
    }

    public class CurrentUserContext : ICurrentUserContext
    {
        public static string UserKey = ClaimTypes.Name;
        public static string RoleKey = ClaimTypes.Role;
        public static string ClientTypeKey = "ClientType";
        public static string PermissionKey = "Permission";


        private string _clientType = "";
        public string ClientType
        {
            get => _clientType ??= GetClaimsValues(ClientTypeKey).FirstOrDefault();
            set => _clientType = value;
        }

        private string _user;
        public string User
        {
            get => _user ??= GetClaimsValues(UserKey).FirstOrDefault();
            set => _user = value;
        }

        private List<string> _roles = null;
        public List<string> Roles
        {
            get => _roles ??= GetClaimsValues(RoleKey);
            set => _roles = value;
        }

        private List<string> _permissions = null;

        public List<string> Permissions
        {
            get => _permissions ??= GetClaimsValues(PermissionKey);
            set => _permissions = value;
        }

        public List<Claim> Claims { get; set; } = new List<Claim>();

        public List<string> GetClaimsValues(string claimType)
        {
            return Claims.Where(x => x.Type == claimType).Select(x => x.Value).ToList();
        }

        public override string ToString()
        {
            return $"User:[{User}],Roles:[{Roles.JoinToOneValue()}],Permissions:[{Permissions.JoinToOneValue()}],Claims({Claims.Count}):[{Claims.Select(x => x.GetShortTypeName()).JoinToOneValue()}]";
        }

        public static CurrentUserContext Empty = new CurrentUserContext();
    }

    public interface ICurrentUserContextService
    {
        ICurrentUserContext GetCurrentUserContext();
    }

    public static class CurrentUserContextExtensions
    {
        public static bool IsLogin(this ICurrentUserContext userContext)
        {
            return !string.IsNullOrWhiteSpace(userContext.User);
        }

        /// <summary>
        /// 为了显示方便，截取Uri的最后一截儿
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static string GetShortTypeName(this Claim claim)
        {
            if (string.IsNullOrWhiteSpace(claim.Type))
            {
                return claim.Type;
            }
            return claim.Type.Split('/').LastOrDefault();
        }
    }
}
