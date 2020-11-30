# 简单的权限控制组件

鉴权系统通常涉及认证和授权两部分，此权限控制组件仅关注授权部分，认证相关可集成任意实现：

- 默认实现：ASP.NET Core Identity
- 自定义实现：比如此EMO示例（演示用户bob，任意密码检测都ok，claims按需随意造）
- 第三方认证系统：IdentityServer4，Azure Active Directory等

## 主要思路

权限控制参照了群体投票：控制可以由多个单元共同进行，系统保证每个单元都有发言权。
每个单元的断言逻辑只有三类：

- NotSure：不置可否（参考其他单元的意见）
- Allowed：我同意
- Forbidden：我不同意

系统使用的最终判定逻辑，类似于一票否决制。详细逻辑，可参看测试用例PermissionCheckResultSpec。

## 权限控制的分类

权限控制的分类，可以根据操作者和被操作者的维度，分为主体权限（用户、角色，声明等）、客体权限（资源、对象等）。
本组件目前支持三种授权模型，逻辑相对独立，控制力度逐渐增强，可按需选择使用。

- 基于用户角色（RBAC）
- 基于声明(CBAC)
- 基于资源（ResourceBAC）

### 基于用户角色

预置了基于字符串的RBAC规则表达式，供系统加载使用。这些规则可以被编辑、修改，从而让系统改变控制行为。

规则包含三个元素：
- PermissionId 代表一个逻辑上的最小的授权单元的ID
- AllowedUsers 代表运行的用户列表
- AllowedRoles 代表运行的用户列表

规则表达式的关系：
- AllowedUsers、AllowedUsers二者是或关系
- 规则表达式的值都可以是用','间隔表达多个，值之间也是或关系。

规则表达式的值，有三种情况:
- ""		=>	NONE 
- "*"		=>	ANY
- "A,B"	=>	A或B

常见的规则模板如下，具体可以参见测试用例（RoleBasedCheckLogicSpec）：
PermissionId,				AllowedUsers,	AllowedRoles	说明
------------------------------------------------------------------
GuestOp						*				*				匿名可用
LoginOp						非*的任意值		*				登录可用
LoginOp						*				非*的任意值		登录可用
UsersOrRolesOp				bob,alice		Admin,Super		特定用户或角色可用
UsersOp						bob,alice		""				特定用户可用
RolesOp						""				Admin,Super		特定角色可用
...

### 基于声明

以用户的Claims信息来判断，特别的，User, Role, Permission都是主体的某些特定Claims。此类行为通常由拦截中间件自动触发。

### 基于资源

以资源为依据的判定，此类行为通常由程序主动触发，主动提供资源对象。
以文档资源为例：如果系统要精确控制，文档只允许原作者删除，则必须向授权组件输入包含Author的文档对象，来触发判定。

## 支持的扩展点

权限控制组件目前实现了上述三种基本的权限判定。
如果仍不能覆盖实际的需要，还可以使用原生的控制进行扩展。例子可参看[Resource-based authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased)

目前组件支持如下扩展点：

- IPermissionRuleActionProvider：	实现此接口，来提供自定义的控制规则来源 (Rules, Actions)
- IPermissionCheckLogicProvider：	实现此接口，来提供自定义的主体控制逻辑（ClaimBased：User, Roles, Permissions, Any Other Claims...）
- IResourceBasedCheckLogicProvider：实现此接口，来提供自定义的客体控制逻辑（ResourceBased）