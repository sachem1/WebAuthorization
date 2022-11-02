namespace Common
{
    public class ApiResponse
    {
        public int Status { get; set; } = 404;
        public string Value { get; set; } = "No Found";
        public MessageModel<string> MessageModel;

        public ApiResponse(StatusCode apiCode, string msg = null)
        {
            switch (apiCode)
            {
                case StatusCode.CODE401:
                    {
                        Status = 401;
                        Value = "您无权访问该接口，请确保已经登录!";
                    }
                    break;
                case StatusCode.CODE403:
                    {
                        Status = 403;
                        Value = "当前登录的用户信息已失效，请重新登录！";
                    }
                    break;
                case StatusCode.CODE500:
                    {
                        Status = 500;
                        Value = msg;
                    }
                    break;
            }

            MessageModel = new MessageModel<string>()
            {
                status = Status,
                message = Value,
                success = false
            };
        }
    }

    public enum StatusCode
    {
        CODE401,
        CODE403,
        CODE404,
        CODE500
    }

}
