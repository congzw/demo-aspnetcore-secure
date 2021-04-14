# Common.Auth

## Authentication

JwtAndCookie

## Authorization 

PermissionChecks

## PermissionChecks的设计思路

- 默认标识：[AllowAnonymous]或[Authorize]，使用默认的实现逻辑
- 没有默认标识的，落入Fallback Policy，即采用PermissionChecks的验证思路

PermissionChecks的验证思路：

- 非注册的控制点 => Allowed
- 注册的控制点，使用投票服务(PermissionCheckVoteService)计算结果

PermissionCheckVoteService支持多种逻辑联合投票：

- RoleBased(主体权限控制)
- ResourceBased(客体权限控制)
- DemoBased(演示用的自定义权限控制逻辑)
- 其他自定义逻辑

### RoleBased(主体权限控制)的实现思路

RBAC规则表达式
- 匿名可访问
- 登录可访问
- 角色可访问