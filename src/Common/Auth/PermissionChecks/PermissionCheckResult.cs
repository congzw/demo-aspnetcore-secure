﻿using System.Collections.Generic;
using System.Linq;

namespace Common.Auth.PermissionChecks
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
        public string Source { get; set; }
        public string Target { get; set; }

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
        public PermissionCheckResult WithSource(string source)
        {
            Source = source;
            return this;
        }
        public PermissionCheckResult WithTarget(string target)
        {
            Target = target;
            return this;
        }

        /// <summary>
        /// 描述信息
        /// </summary>
        /// <returns></returns>
        public string GetVoteDescription()
        {
            return $"{Category} => votes:[{Message}]";
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
        /// 不置可否
        /// </summary>
        public static PermissionCheckResult NotSure => new PermissionCheckResult(PermissionCheckResultCategory.NotSure, "不置可否");

        /// <summary>
        /// 合并多个结果
        /// </summary>
        /// <param name="permissionCheckResults"></param>
        /// <returns></returns>
        public static PermissionCheckResult Combine(params PermissionCheckResult[] permissionCheckResults)
        {
            //if (permissionCheckResults == null)
            //{
            //    return Allowed.WithMessage("没有指定任何规则，放行");
            //}
            //var checkResults = permissionCheckResults.ToList();
            //if (checkResults.Count == 0)
            //{
            //    return Allowed.WithMessage("没有规则，放行");
            //}

            if (permissionCheckResults == null || permissionCheckResults.Length == 0)
            {
                return PermissionCheckResult.NotSure.WithMessage("没有发现任何规则"); ;
            }

            var checkResults = permissionCheckResults.ToList();
            if (checkResults.Count == 1)
            {
                return checkResults[0];
            }

            var combineMessage = CombineMessage(checkResults);
            if (checkResults.All(x => x.Category == PermissionCheckResultCategory.NotSure))
            {
                return NotSure.WithMessage(combineMessage).WithData(checkResults);
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
                return "没有规则";
            }
            var resultGroups = permissionCheckResults.GroupBy(x => x.Category).ToList();
            return resultGroups.Select(x => $"{x.Key}:{x.Count()}").JoinToOneValue();
        }
    }

    /// <summary>
    /// 授权结果的分类
    /// </summary>
    public enum PermissionCheckResultCategory
    {
        /// <summary>
        /// 不置可否
        /// </summary>
        NotSure = 0,
        /// <summary>
        /// 我同意
        /// </summary>
        Allowed = 1,
        /// <summary>
        /// 我不同意
        /// </summary>
        Forbidden = 2
    }

    public static class PermissionCheckResultExtensions
    {
        /// <summary>
        /// 合并多个结果
        /// </summary>
        /// <param name="permissionCheckResults"></param>
        /// <returns></returns>
        public static PermissionCheckResult Combine(this IEnumerable<PermissionCheckResult> permissionCheckResults)
        {
            //if (permissionCheckResults == null)
            //{
            //    return PermissionCheckResult.NotSure;
            //}
            return PermissionCheckResult.Combine(permissionCheckResults.ToArray());
        }
    }
}
