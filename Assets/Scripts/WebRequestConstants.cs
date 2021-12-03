namespace WebRequestConstant
{
    public enum EState
	{
        Init,
        Connecting,
        Success,
        Error
	}

    public static class WebRequestConstant
    {
        public const string AWS_HOST = "http://ec2-54-95-14-86.ap-northeast-1.compute.amazonaws.com";
        public const string AWS_PORT = "8080";
    }

    #region Dto

    /// <summary>
    /// 送信パラメータ
    /// </summary>
    public class BaseRequest { }
    public class BaseResponse { }

    public class SendParam : BaseRequest
    {
        public string userId;
        public string password;
    }
    public class TestJsonData : BaseResponse  // こぴぺ
    {
        public string token { get; set; }
    }
    public class LoginRequest : BaseRequest
    {
        public string userId;
        public string password;
    }
    public class CheckInRequest : BaseRequest
    {
        public string token;
        public string x_geometry;
        public string y_geometry;
    }
    public class CheckInResponse : BaseResponse
    {
        public string result;
    }
    #endregion

}
