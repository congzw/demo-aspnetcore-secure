# 简单的权限控制组件

鉴权系统通常涉及认证和授权两部分，此权限控制组件仅关注授权部分，认证相关可集成任意实现：

- 默认实现：ASP.NET Core Identity
- 自定义实现：比如此DEMO示例（演示用户bob，任意密码检测都ok，claims按需随意造）
- 第三方认证系统：IdentityServer4，Azure Active Directory等

## 主要思路

权限控制参照了群体投票：控制可以由多个单元共同进行，系统保证每个单元都有发言权。
每个单元的断言逻辑只有三类：

- NotSure：不置可否（请参考其他单元的断言）
- Allowed：同意
- Forbidden：不同意

系统使用的最终判定逻辑，类似于一票否决制。详细可参看测试用例PermissionCheckResultSpec。

## 权限控制的分类

权限控制的分类，可以根据操作者和被操作者的维度，分为主体权限（用户、角色，声明等）、客体权限（资源、对象等）。
本组件目前支持三种授权模型，逻辑相对独立，控制力度逐渐增强，可按需选择使用。

- 基于用户角色（Role Based Access Control）
- 基于声明(Claims Based Access Control)
- 基于资源（Resource Based Access Control）

### 基于用户角色

预置了基于字符串的规则表达式。这些规则可以避免硬编码，支持后期编辑，从而改变控制行为。

规则表达式包含三个元素：
- PermissionId 代表一个逻辑上的最小的授权单元的ID
- AllowedUsers 代表运行的用户列表
- AllowedRoles 代表运行的用户列表

规则表达式的关系：
- 规则表达式的值都可以是用','间隔表达多个，值之间是或（OR）关系
- AllowedUsers、AllowedUsers二者是或（OR）关系

规则表达式的值，有三种情况:
- ""		=>	NONE 
- "*"		=>	ANY
- "A,B"		=>	A或B

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

以用户主体的Claims信息来判断，可以认为User, Role, Permission都是用户主体的某些特定Claims类型的具体实现。此类控制行为通常由中间件自动拦截触发。

### 基于资源

以资源为依据的判定，此类行为通常由程序主动触发，主动提供要控制的资源对象。
以文档资源为例：如果系统需要控制文档只允许原作者删除，则必须向授权组件输入包含原作者信息的文档对象，并提供自定义判断逻辑的实现组件，共同来完成授权控制。

## 支持的扩展点

权限控制组件目前支持如下扩展点：

- IPermissionRuleActionProvider：	实现此接口，提供自定义的控制规则 (Rules, Actions)
- IPermissionCheckLogicProvider：	实现此接口，提供自定义的主体控制逻辑（Claim Based：User, Roles, Permissions, Any Other Claims...）
- IResourceBasedCheckLogicProvider：实现此接口，提供自定义的客体控制逻辑（Resource Based）

如果仍不能覆盖实际的需要，还可以使用原生的控制进行扩展。例子可参看[Resource-based authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased)