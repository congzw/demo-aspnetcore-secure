﻿using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks
{
    /// <summary>
    /// 扩展点: 检测逻辑的提供者接口
    /// </summary>
    public interface IPermissionCheckLogicProvider
    {
        int Order { get; set; }
        Task<bool> ShouldCareAsync(CurrentUserContext userContext, PermissionCheckContext permissionCheckContext);
        Task<PermissionCheckResult> CheckPermissionAsync(CurrentUserContext userContext, PermissionCheckContext permissionCheckContext);
    }
}