﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers
{
    public class PermissionCheckVoteHandler : AuthorizationHandler<PermissionCheckRequirement>
    {
        private readonly ILogger<PermissionCheckVoteHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionCheckVoteService _permissionCheckService;

        public PermissionCheckVoteHandler(ILogger<PermissionCheckVoteHandler> logger, 
            IHttpContextAccessor httpContextAccessor,
            IPermissionCheckVoteService permissionCheckService)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _permissionCheckService = permissionCheckService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var currentActionId = httpContext.GetCurrentActionId();
            if (currentActionId == null)
            {
                return;
            }

            var userContext = httpContext.GetCurrentUserContext();
            var checkAttributes = httpContext.GetPermissionAttributes();
            var permissionIds = checkAttributes.Select(x => x.PermissionId).ToArray();
            var checkContext = PermissionCheckContext.Create(userContext, requirement, permissionIds);
            var checkResult = await _permissionCheckService.CheckAsync(checkContext);

            var logMsg = $"{currentActionId} => {checkResult.GetVoteDescription()}";
            _logger.LogInformation(logMsg);
            PermissionCheckDebugHelper.Instance.SetLastResultDescription(currentActionId + checkResult.GetVoteDescription());
            PermissionCheckDebugHelper.Instance.AppendPermissionCheckResults(checkResult);

            switch (checkResult.Category)
            {
                case PermissionCheckResultCategory.Allowed:
                    context.Succeed(requirement);
                    return;
                case PermissionCheckResultCategory.Forbidden:
                    context.Fail();
                    break;
                case PermissionCheckResultCategory.NotSure:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}