namespace JwtAndCookie.Libs
{
    /// <summary>
    /// 自定义返回结果
    /// </summary>
    public class MessageResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 附加数据
        /// </summary>
        public virtual object Data { get; set; } = null;


        public static MessageResult Create(bool success, string message, object data = null)
        {
            return new MessageResult()
            {
                Success = success,
                Message = message,
                Data = data
            };
        }

        public static MessageResult CreateSuccess(string message, object data = null)
        {
            return new MessageResult() { Success = true, Message = message, Data = data };
        }

        public static MessageResult CreateFail(string message, object data = null)
        {
            return new MessageResult() { Success = false, Message = message, Data = data };
        }

        public static MessageResult ValidateResult(bool success = false, string successMessage = "验证通过", string failMessage = "验证失败")
        {
            var vr = new MessageResult
            {
                Message = success ? successMessage : failMessage,
                Success = success
            };
            return vr;
        }
    }
}
