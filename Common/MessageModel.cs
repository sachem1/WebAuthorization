namespace Common
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class MessageModel<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int status { get; set; } = 200;
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool success { get; set; } = false;
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; } = "服务器异常";
        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T response { get; set; }

        /// <summary>
        ///     成功
        /// </summary>
        /// <param name="data">要返回的数据泛型</param>
        /// <returns></returns>
        public static MessageModel<T> Success(T data)
        {
            return new MessageModel<T>()
            {
                response = data ?? default(T),
                message = "",
                success = true
            };
        }

        public static MessageModel<T> Success(T data, string msg)
        {
            return new MessageModel<T>()
            {
                response = data ?? default(T),
                message = msg,
                success = true
            };
        }
        public static MessageModel<T> Success(T data, int code)
        {
            return new MessageModel<T>()
            {
                response = data ?? default(T),
                message = "",
                success = true,
                status = code
            };
        }
        public static MessageModel<T> Success(T data, string msg, int code)
        {
            return new MessageModel<T>()
            {
                response = data ?? default(T),
                message = msg,
                success = true,
                status = code
            };
        }

        /// <summary>
        ///     失败
        /// </summary>
        /// <param name="msg">定义失败信息</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static MessageModel<T> Fail(string msg, int code = 500)
        {
            return new MessageModel<T>
            {
                success = false,
                message = msg ?? string.Empty,
                status = code,
                response = default(T)
            };
        }

        public static MessageModel<T> Fail(string customMsg, string errorMessage, int code = 500)
        {
            return new MessageModel<T>
            {
                success = false,
                message = customMsg ?? string.Empty,
                status = code,
                response = default(T)
            };
        }
        public static MessageModel<T> Fail(string msg, T data, int code = 500)
        {
            return new MessageModel<T>
            {
                success = false,
                message = msg ?? string.Empty,
                status = code,
                response = data ?? default(T)
            };
        }

        public static MessageModel<T> Fail(T data, Exception ex, int code = 500)
        {
            return new MessageModel<T>
            {
                success = false,
                message = ex.Message,
                status = code,
                response = data ?? default(T)
            };
        }

    }

    public class MessageResponse<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int code { get; set; } = 200;

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool success { get; set; } = false;

        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        public T data { get; set; }
    }
}
