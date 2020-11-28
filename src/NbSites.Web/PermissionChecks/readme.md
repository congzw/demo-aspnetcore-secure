# 权限控制的设计思路

## 权限判断逻辑

多个权限控制单元共同进行，保证每个单元的发言权。断言逻辑分为三类：

- NotSure：不置可否，参考其他人的意见 => return
- Allowed：我同意，如果其他人没有明确不同意的话 => Succeed()
- Forbidden：我不同意 => Fail()

多控制单元的判定逻辑参看PermissionCheckResult.Combine方法的测试用例。

## 权限控制的分类

按照控制的精细化粒度，分为三种。各自逻辑独立，按需使用。

- 基于用户角色（RBAC）
- 基于声明
- 基于资源（非主体权限）

### 基于用户角色

用户、角色二者是或关系, 它们的值都可以是用','间隔表达多个，也是或关系
""		=>	NONE 
"*"		=>	ANY
"A,B"	=>	A或B

规则对应的PermissionId,		允许的用户,		允许的角色
PermissionId,				AllowedUsers,	AllowedRoles
---------------------------------------------------------
GuestOp						*				*
LoginOp										*
LoginOp						*				
UsersOrRolesOp				bob,alice		Admin,Super		

### 基于声明

以用户的Claims信息来判断，其实User, Role, Permission都是主体上的Claims的某种特定实现，通常由拦截自动触发

### 基于资源

以资源为依据的判定，例如文档所有者，必须基于文档记录上的Author来判定，通常由程序主动发起

## 支持的扩展点

- IPermissionRuleActionProvider：实现此接口，来提供自定义的规则(Rules, Actions)
- IPermissionCheckLogicProvider：实现此接口，来提供自定义基于主体的权限控制逻辑（User, Roles, Permissions, Claims...）
- IResourceBasedCheckLogicProvider：实现此接口，来提供自定义基于资源的权限控制逻辑(Resources)

不能覆盖的场景，可以直接使用原生的控制进行扩展。
具体参看[Resource-based authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased)
