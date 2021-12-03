using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebRequestConstant;
using Utf8Json;


public class WebRequest : MonoBehaviour
{
    private const string AWS_HOST = WebRequestConstant.WebRequestConstant.AWS_HOST;
    private const string AWS_PORT = WebRequestConstant.WebRequestConstant.AWS_PORT;

    public EState CurrentState { get; private set; } = EState.Init;

	public bool Finished { get; private set; } = false; 
	public string ResultJson { get; private set; }

    public BaseResponse ResultObject;


    #region Dto

    /// <summary>
    /// 送信パラメータ
    /// </summary>
    private class BaseRequest { }
    public class BaseResponse { }

    private class SendParam : BaseRequest
    {
        public string userId;
        public string password;
    }
    private class TestJsonData : BaseResponse  // こぴぺ
    {
        public string token { get; set; }
    }
    private class LoginRequest : BaseRequest
    {
        public string userId;
        public string password;
    }
    private class CheckInRequest : BaseRequest
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

    #region CallApi
    /// <summary>
    /// 通信開始指示
    /// </summary>
    public void CallApi(string token)
    {
        Debug.Log("welcome token: " + token);

        var method = "GET";
        var endpoint = "/v1/user/login";
        var param = new SendParam();
        param.userId = "my@email.org";
        param.password = "mypass";

        StartCoroutine(AsyncWebRequest<TestJsonData>(method, endpoint, GenerateSendData(param)));
    }

    public void CallLoginApi(string email, string password)
    {
        var method = "GET";
        var endpoint = "/v1/user/login";
        var param = new LoginRequest();
        param.userId = email;
        param.password = password;

        StartCoroutine(AsyncWebRequest<TestJsonData>(method, endpoint, GenerateSendData(param)));
    }
    public void CallCheckInApi(string token, string x_geometry, string y_geometry)
    {
        var method = "POST";
        var endpoint = "/v1/matching/checkin";
        var param = new CheckInRequest();
        param.token = token;
        param.x_geometry = x_geometry;
        param.y_geometry = y_geometry;

        StartCoroutine(AsyncWebRequest<CheckInResponse>(method, endpoint, GenerateSendData(param)));
    }
    #endregion

    #region Methods
    public WebRequest Init()
    {
        Finished = false;
        ResultJson = null;
        ResultObject = null;
        return this;
    }

    /// <summary>
    /// 非同期通信処理本体
    /// 成功した場合、このクラスのResultJsonに文字列として保持されます
    /// </summary>
    /// <param name="token">token</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator AsyncWebRequest<T>(string method, string endpoint, byte[] sendByteData)
         where T : BaseResponse
    {
        CurrentState = EState.Connecting;

        string requestUri = AWS_HOST + "" + ":" + AWS_PORT + endpoint;
        Debug.Log("request uri:" + requestUri);

        UnityWebRequest request = new UnityWebRequest(requestUri, method);
        request.uploadHandler = new UploadHandlerRaw(sendByteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        Debug.Log("request.result:" + request.result);

        if (request.result != UnityWebRequest.Result.Success)
        {
            CurrentState = EState.Error;
            Debug.LogError(request.error);
            Finished = true;
            yield break;
        }

        Debug.Log("request.downloadHandler.text:" + request.downloadHandler.text);
        ResultJson = request.downloadHandler.text;
        ResultObject = JsonSerializer.Deserialize<T>(ResultJson);
        CurrentState = EState.Success;
        Finished = true;
    }

    /// <summary>
    /// クラスインスタンスからjsonデータを生成する
    /// </summary>
    /// <param name="sendParam">送るデータ</param>
    /// <returns></returns>
    private byte[] GenerateSendData(BaseRequest sendParam)
	{
        string sendJson = JsonUtility.ToJson(sendParam);
        Debug.Log("sendJson:" + sendJson);
        return System.Text.Encoding.UTF8.GetBytes(sendJson);
    }
    #endregion
}
