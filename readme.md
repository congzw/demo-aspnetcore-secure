# �򵥵�Ȩ�޿���ϵͳ

��Ȩϵͳͨ���漰���������ݣ���֤����Ȩ����Ȩ�޿��ƽ���ע��Ȩ���֡���֤��ؿɼ�������ʵ�֣�

- Ĭ��ʵ�֣�ASP.NET Core Identity
- �Լ�ʵ�֣���ǰDEMO�����Լ���ʵ�֣���ʾ�û�bob�������ⶼok��claims�����죩
- ��������֤ϵͳ��IdentityServer4��Azure Active Directory��

## ��Ҫ���˼·

Ȩ�޿��Ƶ���Ʋ�����Ⱥ��ͶƱ˼·��
Ȩ�޿��ƿ����ɶ����Ԫ��ͬ���У�����ϵͳ��֤ÿ����Ԫ���з���Ȩ��
ÿ����Ԫ�Ķ����߼���Ϊ���ࣺ

- NotSure�����ÿɷ񣨲ο�������Ԫ�������
- Allowed����ͬ��
- Forbidden���Ҳ�ͬ��

Ⱥ�嵥Ԫ�Ĺ�ͬ�ж��߼�����һƱ�����Ⱥ��ͶƱ���ơ�������Բο���������(PermissionCheckResultSpec)��

## Ȩ�޿��Ƶķ���

Ȩ�޿��Ƶķ��࣬���Ը��ݲ����ߺͱ������ߵ�ά�ȣ���Ϊ����Ȩ�ޣ��û�����ɫ�������ȣ�������Ȩ�ޣ���Դ������ȣ���
��ϵͳĿǰ֧��������Ȩģ�ͣ��߼���Զ�����������������ǿ���ɰ���ѡ��ʹ�á�

- �����û���ɫ��RBAC��
- ��������(CBAC)
- ������Դ


### �����û���ɫ

Ԥ����һ���򻯵Ļ����ַ�����RBAC������ʽ������ϵͳ���Դ洢�ͼ�����Щ�������ı������Ϊ��

����������������

- PermissionId ����һ���߼��ϵ���С����Ȩ��Ԫ��ID
- AllowedUsers �������е��û��б�
- AllowedRoles �������е��û��б�

�û�����ɫ�����ǻ��ϵ, ���ǵĹ�����ʽ��ֵ����������','����������ֵ֮��Ҳ�ǻ��ϵ

������ʽ��ֵ�����������:

- ""		=>	NONE 
- "*"		=>	ANY
- "A,B"	=>	A��B

�����Ĺ���ģ�����£�������Բμ�����������RoleBasedCheckLogicSpec����
PermissionId,				AllowedUsers,	AllowedRoles	˵��
------------------------------------------------------------------
GuestOp						*				*				��������
LoginOp						���Ե�����ֵ		*				��¼����
LoginOp						*				���Ե�����ֵ		��¼����
UsersOrRolesOp				bob,alice		Admin,Super		�ض��û����ɫ����
FooOp						bob,alice						�ض��û�����
UsersOrRolesOp								Admin,Super		�ض���ɫ����
...

### ��������

���û���Claims��Ϣ���жϣ���ʵUser, Role, Permission���������ĳЩ�ض�Claims��������Ϊͨ���������м���Զ�������

### ������Դ

����ԴΪ���ݵ��ж�������Ҫɾ��һ���ĵ���ϵͳϣ��ֻ���ĵ����߲��ܲ����������Ҫ�ȵõ�Document�ļ�¼��Document.Authorֵ���ж���������Ϊͨ���ɳ�������������

## ֧�ֵ���չ��

ϵͳʵ���˻�����Ȩ���ж���������в��ܸ���ʵ�ʵ���Ҫ��������ֱ��ʹ��ԭ���Ŀ��ƽ�����չ������ο�[Resource-based authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased)

Ŀǰϵͳ֧��������չ�㣺

- IPermissionRuleActionProvider��	ʵ�ִ˽ӿڣ����ṩ�Զ���Ŀ��ƹ���(Rules, Actions)
- IPermissionCheckLogicProvider��	ʵ�ִ˽ӿڣ����ṩ�Զ������������߼���ClaimBased��User, Roles, Permissions, Any Other Claims...��
- IResourceBasedCheckLogicProvider��ʵ�ִ˽ӿڣ����ṩ�Զ���Ŀ�������߼���ResourceBased��