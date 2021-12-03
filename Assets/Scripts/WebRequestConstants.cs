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
}
