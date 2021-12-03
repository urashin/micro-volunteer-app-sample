using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Utf8Json;
using WebRequestConstant;


public class WebRequest : MonoBehaviour
{
    private const string AWS_HOST = WebRequestConstant.WebRequestConstant.AWS_HOST;
    private const string AWS_PORT = WebRequestConstant.WebRequestConstant.AWS_PORT;

    public EState CurrentState { get; private set; } = EState.Init;

	public bool Finished { get; private set; } = false; 
	public string ResultJson { get; private set; }

    public BaseResponse ResultObject;


    #region CallApi
    /// <summary>
    /// 通信開始指示
    /// </summary>
    public void CallApi(string token)
    {
        Debug.Log("wel__come token: " + token);

        var method = "GET";
        var endpoint = "/v1/user/login";
        var param = new SendParam();
        param.userId = "my@email.org";
        param.password = "mypass";

        StartCoroutine(AsyncWebRequest<WebRequestConstant.TestJsonData>(method, endpoint, GenerateSendData(param)));
    }

    public void CallLoginApi(string email, string password)
    {
        var method = "GET";
        var endpoint = "/v1/user/login";
        var param = new LoginRequest();
        param.userId = email;
        param.password = password;

        StartCoroutine(AsyncWebRequest<WebRequestConstant.TestJsonData>(method, endpoint, GenerateSendData(param)));
    }

    public void CallCheckInApi(CheckInRequest param)
    {
        var method = "POST";
        var endpoint = "/v1/matching/checkin";

        StartCoroutine(AsyncWebRequest<CheckInResponse>(method, endpoint, GenerateSendData(param)));
    }

    public void CallHandicapRegisterApi(HandicapRegisterRequest param)
    {
        var method = "POST";
        var endpoint = "/v1/user/handicap/register";

        StartCoroutine(AsyncWebRequest<HandicapRegisterResponse>(method, endpoint, GenerateSendData(param)));
    }

    public void CallGetHandicapApi(GetHandicapListRequest param)
    {
        var method = "GET";
        var endpoint = "/v1/user/handicaplist";

        StartCoroutine(AsyncWebRequest<GetHandicapListResponse>(method, endpoint, GenerateSendData(param)));
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
            Debug.LogError(request.downloadHandler.text);
            Finished = true;
            yield break;
        }

        Debug.Log("request.downloadHandler.text:" + request.downloadHandler.text);
        ResultJson = request.downloadHandler.text;
        ResultObject = JsonSerializer.Deserialize<T>(ResultJson);
        ResultObject._RawJson = ResultJson; // debug log 用
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
