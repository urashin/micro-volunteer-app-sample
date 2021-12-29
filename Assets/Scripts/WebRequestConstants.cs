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
        public const string AWS_HOST = "http://micro-volunteer-supporter.com";
        public const string AWS_PORT = "8080";
    }

    #region Dto

    /// <summary>
    /// 送信パラメータ
    /// </summary>
    public class BaseRequest { }
    public class BaseResponse
    {
        public string _RawJson;
    }

    public class SendParam : BaseRequest
    {
        public string userId;
        public string password;
    }
    public class TestJsonData : BaseResponse  // こぴぺ
    {
        public string token { get; set; }
    }
    // Regist
    public class UserRegistRequest : BaseRequest
    {
        public string token;
        public string email;
        public string password;
    }
    public class UserRegistResponse : BaseResponse
    {
        public string result;
    }
    // Login
    public class LoginRequest : BaseRequest
    {
        public string userId;
        public string password;
    }
    // CheckIn
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
    // HandicapRegister
    public class HandicapRegisterRequest : BaseRequest
    {
        public string token;
        public int handicap_type;
        public int handicap_level;
        public int reliability_th;
        public int severity;
        public string comment;
    }
    public class HandicapRegisterResponse : BaseResponse
    {
        public string result;
    }
    // Get HandicapList
    public class GetHandicapListRequest : BaseRequest
    {
        public string token;
    }
    public class HandicapInfoDto
    {
        public int handicapinfo_id;
        public string handicapped_id;
        public int handicap_type;
        public int handicap_level;
        public int reliability_th;
        public int severity;
        public string comment;
    }
    public class GetHandicapListResponse : BaseResponse
    {
        public HandicapInfoDto[] handicapInfoDtoList;

    }
    #endregion
}
