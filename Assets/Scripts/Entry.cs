using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Entry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI VersionText;
    [SerializeField] WebRequestTest WebRequest;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ApiCallTest());
    }

    private IEnumerator ApiCallTest()
	{
        // API通信開始
        WebRequest.CallApi();
        // 終了待ち
        while (true)
		{
            if (WebRequest.Finished == false) yield return null;
            else break;
        }

        Debug.Log("WebRequest.CurrentState:" + WebRequest.CurrentState);

        if (WebRequest.CurrentState != WebRequestTest.EState.Success)
		{
            Debug.LogError("通信エラー");
            yield return null;
		}

        // 受信した通信結果jsonをデコードしてみる
        var jsonDecode = new JsonDecodeTest();
        var decodeData = jsonDecode.Test(WebRequest.ResultJson);
        Debug.Log("result:" + decodeData.token);
        VersionText.text = decodeData.token;
    }
}
