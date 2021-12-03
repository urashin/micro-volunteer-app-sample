using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebRequestConstant;


public class WebRequestTest : MonoBehaviour
{

    public EState CurrentState { get; private set; } = EState.Init;

	public bool Finished { get; private set; } = false; 
	public string ResultJson { get; private set; }
    private string AWS_HOST = WebRequestConstant.WebRequestConstant.AWS_HOST;
    private string AWS_PORT = WebRequestConstant.WebRequestConstant.AWS_PORT;


    /// <summary>
    /// 送信パラメータ
    /// </summary>
    private class SendParam
    {
        public string userId;
        public string password;
    }

    /// <summary>
    /// 通信開始指示
    /// </summary>
    public void CallApi(string token)
    {
        StartCoroutine(AsyncWebRequest(token));
    }

    /// <summary>
    /// 非同期通信処理本体
    /// 成功した場合、このクラスのResultJsonに文字列として保持されます
    /// </summary>
    /// <param name="token">token</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator AsyncWebRequest(string token)
    {
        CurrentState = EState.Connecting;

        string requestUri = AWS_HOST + "" + ":" + AWS_PORT + "/v1/user/login";
        Debug.Log("request uri:" + requestUri);

        SendParam sendParam = new SendParam();
        sendParam.userId = "my@email.org";
        sendParam.password = "mypass";
        var sendByteData = GenerateSendData(sendParam);

        UnityWebRequest request = new UnityWebRequest(requestUri, "GET");
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
        CurrentState = EState.Success;
        Finished = true;
    }

    /// <summary>
    /// クラスインスタンスからjsonデータを生成する
    /// </summary>
    /// <param name="sendParam">送るデータ</param>
    /// <returns></returns>
    private byte[] GenerateSendData(SendParam sendParam)
	{
        string sendJson = JsonUtility.ToJson(sendParam);
        Debug.Log("sendJson:" + sendJson);
        return System.Text.Encoding.UTF8.GetBytes(sendJson);
    }
}
