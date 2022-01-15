using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class GpsLocationService : MonoBehaviour
{
    public static GpsLocationService Instance { set; get; }

    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public float Altitude { get; private set; }

    public enum EState
	{
        PermissionError,
        ServiceInitializing,
        ServiceStart,
        Error
	}

    public EState CurrentState { get; private set; } = EState.ServiceInitializing;

	private void Start()
    {
        CurrentState = EState.ServiceInitializing;
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Unity Editorでの実行時はダミーデータを使う
#if UNITY_EDITOR
        CurrentState = EState.ServiceStart;
        Latitude = 123;
        Longitude = 456;
        Altitude = 789;
        return;
#endif

#if !UNITY_EDITOR && UNITY_ANDROID

        // 位置サービスが許可されているか確認
        // see: https://docs.unity3d.com/ja/2020.2/Manual/android-RequestingPermissions.html
        // memo:Network locationの場合は、ACCESS_COARSE_LOCATION
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // 要求するシステムのダイアログ表示
            StartCoroutine(WaitForPermission());
            Permission.RequestUserPermission(Permission.FineLocation);
            return;
        }

        StartCoroutine(StartLocationService());
#endif
    }

    /// <summary>
    /// 位置サービスが許可されるまで待機
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator WaitForPermission()
	{
        Debug.Log("WaitForPermission()");
        var waitObj = new WaitForEndOfFrame();

        while (Permission.HasUserAuthorizedPermission(Permission.FineLocation) == false)
		{
            yield return waitObj;
		}
        StartCoroutine(StartLocationService());
    }

    /// <summary>
    /// 位置情報取得ループ処理
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator StartLocationService()
    {
        Debug.Log("StartLocationService()");
        CurrentState = EState.ServiceStart;
        
        if (!Input.location.isEnabledByUser)
        {
            CurrentState = EState.PermissionError;
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait <= 0)
        {
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            CurrentState = EState.Error;
            Debug.Log("StartLocationService() error.");
            yield break;
        }

        CurrentState = EState.ServiceStart;
        while (true)
        {
            Latitude = Input.location.lastData.latitude;    // 緯度
            Longitude = Input.location.lastData.longitude;  // 軽度
            Altitude = Input.location.lastData.altitude;    // 高度
            // 5秒毎更新
            yield return new WaitForSeconds(5);
            Debug.Log("latitude:" + Latitude + ", longitude:" + Longitude + ", altitude:" + Altitude);
        }
    }
}
