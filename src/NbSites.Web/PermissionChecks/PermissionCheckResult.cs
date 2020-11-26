using System.Linq;

namespace NbSites.Web.PermissionChecks
{
    public class PermissionCheckResult
    {
        public PermissionCheckResult(PermissionCheckResultCategory category, string message)
        {
            Category = category;
            Message = message;
        }

        public PermissionCheckResultCategory Category { get; set; }
        public string Message { get; set; }

        public PermissionCheckResult WithMessage(string message)
        {
            Message = message;
            return this;
        }

        /// <summary>
        /// 我同意
        /// </summary>
        public static PermissionCheckResult Allowed => new PermissionCheckResult(PermissionCheckResultCategory.Allowed, "运行");

        /// <summary>
        /// 我不同意
        /// </summary>
        public static PermissionCheckResult Forbidden => new PermissionCheckResult(PermissionCheckResultCategory.Forbidden, "不允许");

        /// <summary>
        /// 我不关注，我不懂
        /// </summary>
        public static PermissionCheckResult NoCare => new PermissionCheckResult(PermissionCheckResultCategory.NoCare, "不关注");

        /// <summary>
        /// 合并多个结果
        /// </summary>
        /// <param name="permissionCheckResults"></param>
        /// <returns></returns>
        public static PermissionCheckResult Combine(params PermissionCheckResult[] permissionCheckResults)
        {
            if (permissionCheckResults == null)
            {
                return PermissionCheckResult.NoCare;
            }
            var checkResults = permissionCheckResults.ToList();
            if (checkResults.Count == 0)
            {
                return PermissionCheckResult.NoCare;
            }
            
            if (checkResults.All(x => x.Category == PermissionCheckResultCategory.NoCare))
            {
                return NoCare.WithMessage(checkResults.Select(x => x.Message).MyJoin());
            }

            if (checkResults.Any(x => x.Category == PermissionCheckResultCategory.Allowed))
            {
                return Allowed.WithMessage(checkResults.Select(x => x.Message).MyJoin());
            }
            
            return Forbidden.WithMessage(checkResults.Select(x => x.Message).MyJoin());
        }
    }

    public enum PermissionCheckResultCategory
    {
        /// <summary>
        /// 我不关注，我不懂
        /// </summary>
        NoCare = 0,
        /// <summary>
        /// 我同意
        /// </summary>
        Allowed = 1,
        /// <summary>
        /// 我不同意
        /// </summary>
        Forbidden = 2
    }
}
