# 权限控制的设计思路

## 权限判断逻辑

支持多个权限控制单元共同进行权限检查。
注册的多个AuthorizationHandler都会被执行到。

关于AuthorizationHandler的判断逻辑，分为几类：

- NotSure：不置可否，参考其他人的意见 => return
- Allowed：我同意，如果其他人没有明确不同意的话 => Succeed()
- Forbidden：我不同意，必须失败 => Fail()

## 权限控制的分类

按照控制的精细化粒度，分为三种。各自逻辑独立，可以按需集成

- 基于用户角色（RBAC）
- 基于声明
- 基于资源

### 基于RBAC

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


### 基于资源


## 支持的扩展点

- IPermissionCheckLogicProvider：实现此接口，来提供自定义的逻辑
- IPermissionRuleActionProvider：实现此接口，来提供自定义的规则
