using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestTest : MonoBehaviour
{
    public bool Finished { get; private set; } = false; 
	public string Result { get; private set; }

    public void CallApi()
    {
        StartCoroutine(AsyncWebRequest());   
    }

    private IEnumerator AsyncWebRequest()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://jsonplaceholder.typicode.com/todos//");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            yield return null;
        }

        Debug.Log(request.downloadHandler.text);
        Result = request.downloadHandler.text;

        Finished = true;
    }
}
