using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace NbSites.Web.Demos.Permissions
{
    public interface IPermissionCheckProvider
    {
        int Order { get; set; }
        Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext);
    }

    public interface IPermissionCheckContext
    {
        ActionDescriptor ActionDescriptor { get; set; }
        HttpContext HttpContext { get; set; }
        string PermissionId { get; set; }
    }

    public class PermissionCheckContext : IPermissionCheckContext
    {
        public ActionDescriptor ActionDescriptor { get; set; }
        public HttpContext HttpContext { get; set; }
        public string PermissionId { get; set; }

        public static PermissionCheckContext Create(ActionDescriptor actionDescriptor, HttpContext httpContext, string permissionId)
        {
            return new PermissionCheckContext()
            {
                ActionDescriptor = actionDescriptor,
                HttpContext = httpContext,
                PermissionId = permissionId
            };
        }
    }
    
    public class PermissionCheckResult
    {
        public PermissionCheckResult(PermissionCheckResultCategory category, string message)
        {
            Category = category;
            Message = message;
        }

        public PermissionCheckResultCategory Category { get; set; }
        public string Message { get; set; }

        public PermissionCheckResult WithMessage(string message)
        {
            Message = message;
            return this;
        }

        public static PermissionCheckResult Allowed => new PermissionCheckResult(PermissionCheckResultCategory.Allowed, "运行");
        public static PermissionCheckResult Forbidden => new PermissionCheckResult(PermissionCheckResultCategory.Forbidden, "不允许");
        public static PermissionCheckResult NoCare => new PermissionCheckResult(PermissionCheckResultCategory.NoCare, "不关注");

        public static PermissionCheckResultCategory Combine(params PermissionCheckResult[] permissionCheckResults)
        {
            if (permissionCheckResults.Length == 0)
            {
                return PermissionCheckResultCategory.NoCare;
            }

            if (permissionCheckResults.All(x => x.Category == PermissionCheckResultCategory.NoCare))
            {
                return PermissionCheckResultCategory.NoCare;
            }

            if (permissionCheckResults.Any(x => x.Category == PermissionCheckResultCategory.Forbidden))
            {
                return PermissionCheckResultCategory.Forbidden;
            }

            return PermissionCheckResultCategory.Allowed;
        }
    }

    public enum PermissionCheckResultCategory
    {
        NoCare = 0,
        Allowed = 1,
        Forbidden = 2
    }
}