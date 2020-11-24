using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace NbSites.Web.Demos
{
    public class DynamicCheckHandlerDefault : AuthorizationHandler<DynamicCheckRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DynamicCheckHandlerDefault> _logger;
        private readonly DynamicCheckRulePool _dynamicCheckFeatureService;
        private readonly CurrentUserContext _dynamicCheckContext;

        public DynamicCheckHandlerDefault(IHttpContextAccessor httpContextAccessor, ILogger<DynamicCheckHandlerDefault> logger, DynamicCheckRulePool dynamicCheckFeatureService, CurrentUserContext dynamicCheckContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _dynamicCheckFeatureService = dynamicCheckFeatureService;
            _dynamicCheckContext = dynamicCheckContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicCheckRequirement requirement)
        {
            var actionDescriptor = GetControllerActionDescriptor(context);
            if (actionDescriptor == null)
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation("actionDescriptor >>>>>>>>> " + actionDescriptor.DisplayName);
            var results = new List<MessageResult>();

            var checkFeatureAttributes = GetCheckFeatureAttributes(context);
            if (checkFeatureAttributes.Count == 0)
            {
                var actionId = actionDescriptor.DisplayName.Split().FirstOrDefault();
                var checkRule = _dynamicCheckFeatureService.TryGetRuleByActionId(actionId);
                if (checkRule == null)
                {
                    //不需要控制的点，放行
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                var checkResult = _dynamicCheckFeatureService.IsAllowed(checkRule.CheckFeatureId, _dynamicCheckContext);
                checkResult.Data = checkRule.CheckFeatureId;
                results.Add(checkResult);
                DynamicCheckDebugHelper.Instance.CheckRuleResults = results;
                _logger.LogInformation(checkResult.Message);
                if (checkResult.Success)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                return Task.CompletedTask;
            }

            foreach (var checkFeatureAttribute in checkFeatureAttributes)
            {
                var id = checkFeatureAttribute.Id;
                var checkResult = _dynamicCheckFeatureService.IsAllowed(id, _dynamicCheckContext);
                checkResult.Data = id;
                results.Add(checkResult);
                _logger.LogInformation(checkResult.Message);
            }

            DynamicCheckDebugHelper.Instance.CheckRuleResults = results;

            if (results.All(x => x.Success))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

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