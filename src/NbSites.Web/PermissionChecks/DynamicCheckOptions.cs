namespace NbSites.Web.PermissionChecks
{
    public class DynamicCheckOptions
    {
        public const string SectionName = "PermissionCheck";
        /// <summary>
        /// 开启裸奔模式，所有自动的主体权限检测失效。默认false
        /// </summary>
        public bool Naked { get; set; } = false;
        /// <summary>
        /// 是否调试助手，用于Web上直接查看检测的过程信息。默认true
        /// </summary>
        public bool DebugHelperEnabled { get; set; } = true;
        /// <summary>
        /// 没有显示设置权限的请求，是否应用强制登录。默认true
        /// </summary>
        public bool RequiredLoginForUnknown { get; set; } = true;
    }
}