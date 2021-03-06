﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NbSites.Web.PermissionChecks
{
    public interface ICurrentUserContext
    {
        string User { get; set; }
        List<string> Roles { get; set; }
        List<string> Permissions { get; set; }
        List<Claim> Claims { get; set; }
    }

    public class CurrentUserContext : ICurrentUserContext
    {
        public static string UserKey = ClaimTypes.Name;
        public static string RoleKey = ClaimTypes.Role;
        public static string PermissionKey = "Permission";

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

        public static CurrentUserContext Empty = new CurrentUserContext();
    }
}