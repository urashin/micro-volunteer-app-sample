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
        WebRequest.CallApi();

        while (true)
		{
            if (WebRequest.Finished == false) yield return null;
            else break;
        }

        var jsonDecode = new JsonDecodeTest();
        var decodeData = jsonDecode.Test(WebRequest.Result);
        Debug.Log("result:" + decodeData[0].title);
        VersionText.text = decodeData[0].title;
    }
}
