using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using NbSites.Web.Demos.Permissions;

namespace NbSites.Web.Demos
{
    public class DynamicCheckPermissionHandler : AuthorizationHandler<DynamicCheckRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DynamicCheckHandlerDefault> _logger;
        private readonly DynamicCheckRulePool _rbacCheckPool;
        private readonly IPermissionCheckService _permissionCheckService;
        private readonly ICurrentUserContext _currentUserContext;

        public DynamicCheckPermissionHandler(IHttpContextAccessor httpContextAccessor, 
            ILogger<DynamicCheckHandlerDefault> logger, 
            DynamicCheckRulePool dynamicCheckFeatureService, 
            IPermissionCheckService permissionCheckService,
            ICurrentUserContext currentUserContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _rbacCheckPool = dynamicCheckFeatureService;
            _permissionCheckService = permissionCheckService;
            _currentUserContext = currentUserContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicCheckRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return;
            }


            var actionDescriptor = GetControllerActionDescriptor(context);
            if (actionDescriptor == null)
            {
                return;
            }
            
            _logger.LogInformation("HandleRequirementAsync for permission >>>>>>>>> " + actionDescriptor.DisplayName);
            var checkFeatureIds = TryGetCheckFeatureIds(context, actionDescriptor);
            var permissionCheckResults = new List<PermissionCheckResult>();
            foreach (var checkFeatureId in checkFeatureIds)
            {
                var permissionCheckContext = PermissionCheckContext.Create(actionDescriptor, _httpContextAccessor.HttpContext, checkFeatureId);
                var permissionCheckResult = await _permissionCheckService.PermissionCheckAsync(_currentUserContext, permissionCheckContext);
                permissionCheckResults.Add(permissionCheckResult);
            }
            
            var combine = PermissionCheckResult.Combine(permissionCheckResults.ToArray());
            if (combine == PermissionCheckResultCategory.NoCare)
            {
                return;
            }

            if (combine == PermissionCheckResultCategory.Allowed)
            {
                context.Succeed(requirement);
                return;
            }

            if (combine == PermissionCheckResultCategory.Forbidden)
            {
                context.Fail();
            }
        }

        private List<string> TryGetCheckFeatureIds(AuthorizationHandlerContext context, ActionDescriptor actionDescriptor)
        {
            var featureIds = new List<string>();
            var checkFeatureAttributes = GetCheckFeatureAttributes(context);
            foreach (var checkFeatureAttribute in checkFeatureAttributes)
            {
                featureIds.Add(checkFeatureAttribute.Id);
            }

            var actionId = actionDescriptor.DisplayName.Split().FirstOrDefault();
            var checkRule = _rbacCheckPool.TryGetRuleByActionId(actionId);
            if (checkRule != null)
            {
                featureIds.Add(checkRule.CheckFeatureId);
            }

            return featureIds;
        }

        //todo: move to extensions helper
        private ActionDescriptor GetControllerActionDescriptor(AuthorizationHandlerContext context)
        {
            #region another impl: not use IHttpContextAccessor


            //if (context == null)
            //    throw new ArgumentNullException(nameof(context));
            //if (context.Resource is Endpoint endpoint)
            //{
            //    var actionDescriptor = endpoint.Metadata?.GetMetadata<ControllerActionDescriptor>();
            //    _logger.LogInformation("endpoint >>>>>>>>> " + actionDescriptor?.DisplayName);
            //    return actionDescriptor;
            //}
            //if (context.Resource is AuthorizationFilterContext mvcContext)
            //{
            //    // Examine MVC-specific things like routing data.
            //    _logger.LogInformation("mvcContext >>>>>>>>> " + mvcContext.ActionDescriptor.DisplayName);
            //    return mvcContext.ActionDescriptor;
            //}

            #endregion

            var httpContext = _httpContextAccessor.HttpContext;
            var endpoint = httpContext.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        }

        private IList<DynamicCheckFeatureAttribute> GetCheckFeatureAttributes(AuthorizationHandlerContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var endpoint = httpContext.GetEndpoint();

            var checkFeatureAttributes = endpoint.Metadata.Where(x => x is DynamicCheckFeatureAttribute).Cast<DynamicCheckFeatureAttribute>().ToList();
            var overrideIdValueList = checkFeatureAttributes.Select(x => x.OverrideIds).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (overrideIdValueList.Count == 0)
            {
                return checkFeatureAttributes;
            }

            var needIgnoreIds = overrideIdValueList
                .SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var featureAttributes = checkFeatureAttributes.Where(x => !needIgnoreIds.Contains(x.Id, StringComparer.OrdinalIgnoreCase))
                .ToList();

            return featureAttributes;
        }
    }
}