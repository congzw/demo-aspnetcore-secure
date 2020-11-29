# 简单的权限控制系统

鉴权系统通常涉及两部分内容：认证和授权。此权限控制仅关注授权部分。认证相关可集成任意实现：

- 默认实现：ASP.NET Core Identity
- 自己实现：当前DEMO采用自己的实现（演示用户bob，密码检测都ok，claims随意造）
- 第三方认证系统：IdentityServer4，Azure Active Directory等

## 主要设计思路

权限控制的设计参照了群体投票思路。
权限控制可以由多个单元共同进行，控制系统保证每个单元都有发言权。
每个单元的断言逻辑分为三类：

- NotSure：不置可否（参考其他单元的意见）
- Allowed：我同意
- Forbidden：我不同意

群体单元的共同判定逻辑，跟一票否决的群体投票类似。具体可以参看测试用例(PermissionCheckResultSpec)。

## 权限控制的分类

权限控制的分类，可以根据操作者和被操作者的维度，分为主体权限（用户、角色，声明等）、客体权限（资源、对象等）。
本系统目前支持三种授权模型，逻辑相对独立，控制力度逐渐增强，可按需选择使用。

- 基于用户角色（RBAC）
- 基于声明(CBAC)
- 基于资源


### 基于用户角色

预置了一个简化的基于字符串的RBAC规则表达式，这样系统可以存储和加载这些规则，来改变控制行为。

规则里包含三个概念：

- PermissionId 代表一个逻辑上的最小的授权单元的ID
- AllowedUsers 代表运行的用户列表
- AllowedRoles 代表运行的用户列表

用户、角色二者是或关系, 它们的规则表达式的值都可以是用','间隔表达多个，值之间也是或关系

规则表达式的值，有三种情况:

- ""		=>	NONE 
- "*"		=>	ANY
- "A,B"	=>	A或B

常见的规则模板如下，具体可以参见测试用例（RoleBasedCheckLogicSpec）：
PermissionId,				AllowedUsers,	AllowedRoles	说明
------------------------------------------------------------------
GuestOp						*				*				匿名可用
LoginOp						忽略的任意值		*				登录可用
LoginOp						*				忽略的任意值		登录可用
UsersOrRolesOp				bob,alice		Admin,Super		特定用户或角色可用
FooOp						bob,alice						特定用户可用
UsersOrRolesOp								Admin,Super		特定角色可用
...

### 基于声明

以用户的Claims信息来判断，其实User, Role, Permission都是主体的某些特定Claims。此类行为通常由拦截中间件自动触发。

### 基于资源

以资源为依据的判定，例如要删除一个文档，系统希望只有文档作者才能操作。则必须要先得到Document的记录，Document.Author值来判定。此类行为通常由程序主动触发。

## 支持的扩展点

系统实现了基本的权限判定，如果现有不能覆盖实际的需要，还可以直接使用原生的控制进行扩展。具体参看[Resource-based authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased)

目前系统支持如下扩展点：

- IPermissionRuleActionProvider：	实现此接口，来提供自定义的控制规则(Rules, Actions)
- IPermissionCheckLogicProvider：	实现此接口，来提供自定义的主体控制逻辑（ClaimBased：User, Roles, Permissions, Any Other Claims...）
- IResourceBasedCheckLogicProvider：实现此接口，来提供自定义的客体控制逻辑（ResourceBased）