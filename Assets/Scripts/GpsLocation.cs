using System.Collections;
using UnityEngine;

public class GpsLocation : MonoBehaviour
{
    public static GpsLocation Instance { set; get; }

    public float latitude;
    public float longitude;
    public float altitude;

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
        latitude = 123;
        longitude = 456;
        altitude = 789;
        return;
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        //GPS location
        if (CheckPermission("android.permission.ACCESS_FINE_LOCATION") == false)
		{
            CurrentState = EState.PermissionError;
            return;
		}
        // Network location
        if (CheckPermission("android.permission.ACCESS_COARSE_LOCATION") == false)
		{
            CurrentState = EState.PermissionError;
            return;
		}

        StartCoroutine(StartLocationService());
#endif
    }

    static bool CheckPermission(string permission)
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var compat = new AndroidJavaClass("android.support.v4.app.ActivityCompat"))
        {
            var check = compat.CallStatic<int>("checkSelfPermission", activity, permission);

            if (check == 0) return true;

            int REQUEST_CODE = 1;
            compat.CallStatic("requestPermissions", activity, new string[] { permission }, REQUEST_CODE);

            //再チェック
            check = compat.CallStatic<int>("checkSelfPermission", activity, permission);
            if (check == 0) return true;
        }
        return false;
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            CurrentState = EState.PermissionError;
            yield break;
        }

        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait <= 0)
        {
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            CurrentState = EState.Error;
            yield break;
        }

        CurrentState = EState.ServiceStart;
        while (true)
        {
            latitude = Input.location.lastData.latitude; // 緯度
            longitude = Input.location.lastData.longitude; // 軽度
            altitude = Input.location.lastData.altitude; // 高度
            yield return new WaitForSeconds(10);
        }
    }
}
