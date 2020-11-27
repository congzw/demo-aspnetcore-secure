using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NbSites.Web.PermissionChecks
{
    public class PermissionCheckResult
    {
        public PermissionCheckResult(PermissionCheckResultCategory category, string message)
        {
            Category = category;
            Message = message;
        }

        /// <summary>
        /// 授权结果的分类
        /// </summary>
        public PermissionCheckResultCategory Category { get; set; }
        /// <summary>
        /// 授权结果的消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 授权结果的附加数据
        /// </summary>
        public object Data { get; set; }
        public PermissionCheckResult WithMessage(string message)
        {
            Message = message;
            return this;
        }
        public PermissionCheckResult WithData(object data)
        {
            Data = data;
            return this;
        }

        /// <summary>
        /// 我同意
        /// </summary>
        public static PermissionCheckResult Allowed => new PermissionCheckResult(PermissionCheckResultCategory.Allowed, "允许");

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
                return NoCare;
            }
            var checkResults = permissionCheckResults.ToList();
            if (checkResults.Count == 0)
            {
                return NoCare;
            }

            var combineMessage = CombineMessage(checkResults);
            if (checkResults.All(x => x.Category == PermissionCheckResultCategory.NoCare))
            {
                return NoCare.WithMessage(combineMessage).WithData(checkResults);
            }

            if (checkResults.Any(x => x.Category == PermissionCheckResultCategory.Forbidden))
            {
                return Forbidden.WithMessage(combineMessage).WithData(checkResults);
            }

            if (checkResults.Any(x => x.Category == PermissionCheckResultCategory.Allowed))
            {
                return Allowed.WithMessage(combineMessage).WithData(checkResults);
            }
            
            return Forbidden.WithMessage(combineMessage).WithData(checkResults);
        }

        private static string CombineMessage(IList<PermissionCheckResult> permissionCheckResults)
        {
            if (permissionCheckResults == null || permissionCheckResults.Count == 0)
            {
                return "不关注";
            }
            var resultGroups = permissionCheckResults.GroupBy(x => x.Category).ToList();
            return resultGroups.Select(x => $"{x.Key}:{x.Count()}").MyJoin();
        }
    }

    /// <summary>
    /// 授权结果的分类
    /// </summary>
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
