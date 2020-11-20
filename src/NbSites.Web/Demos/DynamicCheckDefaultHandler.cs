using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace NbSites.Web.Demos
{
    public class DynamicCheckDefaultHandler : AuthorizationHandler<DynamicCheckRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DynamicCheckDefaultHandler> _logger;
        private readonly DynamicCheckService _dynamicCheckFeatureService;

        public DynamicCheckDefaultHandler(IHttpContextAccessor httpContextAccessor, ILogger<DynamicCheckDefaultHandler> logger, DynamicCheckService dynamicCheckFeatureService)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _dynamicCheckFeatureService = dynamicCheckFeatureService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicCheckRequirement requirement)
        {
            var actionDescriptor = GetControllerActionDescriptor(context);
            if (actionDescriptor == null)
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation("actionDescriptor >>>>>>>>> " + actionDescriptor.DisplayName);
            var roleClaims = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            var userName = context.User.Identity.Name;

            var results = new List<MessageResult>();
            var checkFeatureContext = new DynamicCheckContext();
            checkFeatureContext.User = userName;
            checkFeatureContext.Roles = roleClaims.Select(x => x.Value).ToList();

            var checkFeatureAttributes = GetCheckFeatureAttributes(context);
            if (checkFeatureAttributes.Count == 0)
            {
                //todo: try to find by Namespace.Controller.Action as Id
                //NbSites.Web.Controllers.SimpleController.LoginOp (NbSites.Web)
                var actionId = actionDescriptor.DisplayName.Split().FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(actionId))
                {
                    var checkResult = _dynamicCheckFeatureService.IsAllowed(actionId, checkFeatureContext);
                    checkResult.Data = actionId;
                    results.Add(checkResult);
                    DynamicCheckDebugHelper.Instance.CheckRuleResults = results;
                    _logger.LogInformation(checkResult.Message);
                    if (checkResult.Success)
                    {
                        context.Succeed(requirement);
                    }
                }
                return Task.CompletedTask;
            }

            foreach (var checkFeatureAttribute in checkFeatureAttributes)
            {
                var id = checkFeatureAttribute.Id;
                var checkResult = _dynamicCheckFeatureService.IsAllowed(id, checkFeatureContext);
                checkResult.Data = id;
                results.Add(checkResult);
                _logger.LogInformation(checkResult.Message);
            }

            DynamicCheckDebugHelper.Instance.CheckRuleResults = results;

            if (results.All(x => x.Success))
            {
                context.Succeed(requirement);
            }

            //todo: check permission by setting: op:[roles]
            return Task.CompletedTask;
        }

        private ActionDescriptor GetControllerActionDescriptor(AuthorizationHandlerContext context)
        {
            //another impl: not use IHttpContextAccessor
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