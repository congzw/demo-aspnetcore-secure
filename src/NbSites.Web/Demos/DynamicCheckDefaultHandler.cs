﻿using System;
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
    public class DynamicCheckHandlerDefault : AuthorizationHandler<DynamicCheckRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DynamicCheckHandlerDefault> _logger;
        private readonly DynamicCheckRulePool _rbacCheckPool;
        private readonly ICurrentUserContext _currentUserContext;

        public DynamicCheckHandlerDefault(IHttpContextAccessor httpContextAccessor, 
            ILogger<DynamicCheckHandlerDefault> logger, 
            DynamicCheckRulePool dynamicCheckFeatureService, 
            ICurrentUserContext currentUserContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _rbacCheckPool = dynamicCheckFeatureService;
            _currentUserContext = currentUserContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicCheckRequirement requirement)
        {
            var actionDescriptor = GetControllerActionDescriptor(context);
            if (actionDescriptor == null)
            {
                return;
            }
            
            _logger.LogInformation("HandleRequirementAsync >>>>>>>>> " + actionDescriptor.DisplayName);
            
            var results = new List<MessageResult>();
            var checkFeatureIds = TryGetCheckFeatureIds(context, actionDescriptor);
            if (checkFeatureIds.Count == 0)
            {
                //不需要控制的点，放行
                context.Succeed(requirement);
                return;
            }
            //<<<RBAC CHECK BEGIN
            foreach (var checkFeatureId in checkFeatureIds)
            {
                var checkResult = _rbacCheckPool.IsAllowed(checkFeatureId, _currentUserContext);
                checkResult.Data = checkFeatureId;
                results.Add(checkResult);
                _logger.LogInformation(checkResult.Message);
            }
            if (results.Any(x => !x.Success))
            {
                return;
            }

            context.Succeed(requirement);
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