using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using WebRequestConstant;

/// <summary>
/// アプリ本体
/// </summary>
public class Entry : MonoBehaviour
{
	#region Inspector

	[SerializeField] TextMeshProUGUI VersionText;
    [SerializeField] Utility utility;
    [SerializeField] WebRequest WebRequest;
    [SerializeField] ProcessDeepLinkMngr processDeepLinkMngr;

    [SerializeField] Button GpsCheckinButton;
    [SerializeField] Button QrCheckinButton;
    [SerializeField] Button HelpForMeButton;
    [SerializeField] Button ConfigButton;

    [SerializeField] GameObject LoadingPanel;
    [SerializeField] TextMeshProUGUI PanelMessage;

    [SerializeField] SelectDialog SelectYesCancelDialog;
    [SerializeField] EvalutionScreen EvalutionScreenDialog;
    [SerializeField] ActionHistoryWindow ActionHistoryWindow;

    #endregion

    #region
    private string m_token = "";
	#endregion

	#region Method

	private void Awake()
	{
        LoadingPanel.SetActive(false);

        SelectYesCancelDialog.Hide();
        EvalutionScreenDialog.Hide();
        ActionHistoryWindow.Hide();

        // 端末にtokenが保存されているかを調べる
        var token = PlayerPrefs.GetString("token", "");
        if (token == "")
		{
            var msg = "tokenが端末に保存されていないので未登録ユーザ";
            Debug.Log(msg);
            VersionText.text = msg;
		}
        else
        {
            var msg = "token保存済み、登録ユーザ: " + token;
            Debug.Log(msg);
            VersionText.text = msg;
            m_token = token;
        }
    }

	// Start is called before the first frame update
	void Start()
    {
        GpsCheckinButton.onClick.AddListener(OnClickGpsCheckinButton);
        QrCheckinButton.onClick.AddListener(OnClickQrCheckinButton);

        HelpForMeButton.onClick.AddListener(OnClickHelpForMeButton);
        // HelpForMeButtonは、checkinするまで無効
        HelpForMeButton.interactable = false;

        ConfigButton.onClick.AddListener(OnclickActionHistoryButton);

        // handle deeplink
        var deeplink = processDeepLinkMngr.deeplinkURL;
        #if DEBUG
        deeplink = "http://example.com/sns-register?sns_id=U1234&token=11223344-5678-abcd-ef01-23456789abcd";
        deeplink = "http://example.com/sns-register?sns_id=U6de8fd67fdbfc2ea917cd5cfe7d58d51&token=392b6a7a-a495-4bf8-8fc9-7a723808e925";
        #endif
        if (deeplink != ProcessDeepLinkMngr.NoDeeplink)
        {
            var res = Utility.ParseDeepLink(deeplink);
            var text = "";
            text = "cmd = " + res.Endpoint;

            foreach(var kv in res.QueryDictionary)
            {
                text += "\n - [" + kv.Key + "] `" + kv.Value + "`";
            }
            VersionText.text = text;
            m_token = res.QueryDictionary["token"];
            SaveToken(m_token);
        }

    }


    /// <summary>
    /// GPSでチェックインボタンクリック
    /// </summary>
    private void OnClickGpsCheckinButton()
    {
        // とりあえず同じ処理
        OnClickQrCheckinButton();
    }

    /// <summary>
    /// QRコードでチェックインボタンクリック
    /// </summary>
    private void OnClickQrCheckinButton()
    {
        if (m_token == "")
        {
            // open LINE Bot account
            Application.OpenURL("https://line.me/R/ti/p/@378tqgzf");
            return;
        }
        LoadingPanel.SetActive(true);
        PanelMessage.text = "チェックイン中です...";
        StartCoroutine(AsyncCheckin());
    }

    /// <summary>
    /// 歯車アイコンがクリックされた時、履歴画面を表示する
    /// </summary>
    private void OnclickActionHistoryButton()
	{
        ActionHistoryWindow.OpenWindow();
    }

    /// <summary>
    /// 取得したTokenを端末に保存する（保存先はセキュアでない領域です）
    /// </summary>
    /// <param name="token"></param>
    private void SaveToken(string token)
    {
        PlayerPrefs.SetString("token", token);
        PlayerPrefs.Save();
    }

    //---------------------------------------------------------------------------------------------
    // 障がい者の方が操作する処理
    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Checkin処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator AsyncCheckin()
	{
        // ここでAPI通信する
        // ダミーで2秒待つ
        //yield return new WaitForSeconds(2);
        yield return StartCoroutine(ApiCallCheckIn(
            "135.000", "36.0000",
            (CheckInResponse response) => {
                Debug.Log("API finished! " + response.result);
                VersionText.text = response.result;
            })
        );

        // HelpForMeButtonを押せるようにする
        LoadingPanel.SetActive(false);
        HelpForMeButton.interactable = true;
    }

    /// <summary>
    /// Help For Meボタンクリック
    /// </summary>
    private void OnClickHelpForMeButton()
	{
        // 確認ダイアログ表示
        SelectYesCancelDialog.OpenDialog(
            2,
            "送信しますか？",
            () =>
            {
                // YESが押されたらAPI通信してみる
                if (SelectYesCancelDialog.SelectedButton == SelectDialog.EButtonType.Yes)
				{
                    //StartCoroutine(ApiCallTest(ApiFinishedCallback));
                    StartCoroutine(ApiCallHandicapRegister(
                        "たすけてー",
                        (HandicapRegisterResponse response) => {
                            Debug.Log("API finished! " + response._RawJson);
                            VersionText.text = response.result;

                            // api2
                            StartCoroutine(ApiCallGetHandicapList(
                                (GetHandicapListResponse response2) => {
                                    Debug.Log("API2 finished! " + response2._RawJson);
                                    VersionText.text = response2.result;
                                })
                            );
                            // end

                        })
                    );
                }
            }
        );
    }

    /// <summary>
    /// API通信が終わった後の処理
    /// ボランティアの方が向かっていますを表示する
    /// 到着しましたボタンがあります（クリックしたら評価入力画面へ）
    /// </summary>
    private void ApiFinishedCallback()
	{
        SelectYesCancelDialog.OpenDialog(1, "ボランティアの方が向かっています", DisplayEvaluationScreen);
    }

    /// <summary>
    /// 評価入力画面処理
    /// </summary>
    private void DisplayEvaluationScreen()
	{
        EvalutionScreenDialog.OpenScreen(()=> { });
    }

    /// 試しにAPI通信してみる
    /// </summary>
    /// <param name="callback">通信完了後のcallback</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator ApiCallTest(Action callback)
    {
        // 時間計測用
        var startTime = Time.time;

        LoadingPanel.SetActive(true);
        PanelMessage.text = "マッチング中です...";

        // API通信開始
        WebRequest.Init().CallApi(m_token);
        // 終了待ち
        while (true)
        {
            if (WebRequest.Finished == false) yield return null;
            else break;
        }

        Debug.Log("WebRequest.CurrentState:" + WebRequest.CurrentState);

        if (WebRequest.CurrentState != EState.Success)
        {
            Debug.LogError("通信エラー");
            yield return null;
        }

        // 受信した通信結果jsonをデコードしてみる
        var jsonDecode = new JsonDecodeTest();
        var decodeData = jsonDecode.Test(WebRequest.ResultJson);
        //Debug.Log("result:" + decodeData.token);
        //VersionText.text = decodeData.token;

        // マッチングが早く完了した場合、メッセージがすぐに消えてしまうので、最低でも2秒は待つ
        var elapsedTime = Time.time - startTime;
        if (elapsedTime < 2)
        {
            yield return new WaitForSeconds(2 - elapsedTime);
        }

        LoadingPanel.SetActive(false);

        callback?.Invoke();
    }

    private IEnumerator ApiCallCheckIn(string x_geometry, string y_geometry, Action<CheckInResponse> callback)
    {
        // 時間計測用
        var startTime = Time.time;

        LoadingPanel.SetActive(true);
        PanelMessage.text = "チェックイン中です...";

        // API通信開始
        var param = new CheckInRequest();
        param.token = m_token;
        param.x_geometry = x_geometry;
        param.y_geometry = y_geometry;
        WebRequest.Init().CallCheckInApi(param);
        // 終了待ち
        while (true)
        {
            if (WebRequest.Finished == false) yield return null;
            else break;
        }

        Debug.Log("WebRequest.CurrentState:" + WebRequest.CurrentState);

        if (WebRequest.CurrentState != EState.Success)
        {
            Debug.LogError("通信エラー");
            VersionText.text = "Error";
            yield return null;
        }

        // wait
        var elapsedTime = Time.time - startTime;
        if (elapsedTime < 1)
        {
            yield return new WaitForSeconds(1 - elapsedTime);
        }
        LoadingPanel.SetActive(false);

        callback?.Invoke((CheckInResponse)WebRequest.ResultObject);
    }

    private IEnumerator ApiCallHandicapRegister(string comment, Action<HandicapRegisterResponse> callback)
    {
        // 時間計測用
        var startTime = Time.time;

        LoadingPanel.SetActive(true);
        PanelMessage.text = "ヘルプ中です...";

        // API通信開始
        var param = new HandicapRegisterRequest();
        param.token = m_token;
        param.handicap_level = 1;
        param.handicap_level = 1;
        param.reliability_th = 1;
        param.severity = 4;
        param.comment = comment;
        WebRequest.Init().CallHandicapRegisterApi(param);
        // 終了待ち
        while (true)
        {
            if (WebRequest.Finished == false) yield return null;
            else break;
        }

        Debug.Log("WebRequest.CurrentState:" + WebRequest.CurrentState);

        if (WebRequest.CurrentState != EState.Success)
        {
            Debug.LogError("通信エラー");
            VersionText.text = "Error";
            yield return null;
        }

        // wait
        var elapsedTime = Time.time - startTime;
        if (elapsedTime < 1)
        {
            yield return new WaitForSeconds(1 - elapsedTime);
        }
        LoadingPanel.SetActive(false);

        callback?.Invoke((HandicapRegisterResponse)WebRequest.ResultObject);
    }


    private IEnumerator ApiCallGetHandicapList(Action<GetHandicapListResponse> callback)
    {
        // 時間計測用
        var startTime = Time.time;

        LoadingPanel.SetActive(true);
        PanelMessage.text = "ヘルプ中です...";

        // API通信開始
        var param = new GetHandicapListRequest();
        param.token = m_token;
        WebRequest.Init().CallGetHandicapApi(param);
        // 終了待ち
        while (true)
        {
            if (WebRequest.Finished == false) yield return null;
            else break;
        }

        Debug.Log("WebRequest.CurrentState:" + WebRequest.CurrentState);

        if (WebRequest.CurrentState != EState.Success)
        {
            Debug.LogError("通信エラー");
            VersionText.text = "Error";
            yield return null;
        }

        // wait
        var elapsedTime = Time.time - startTime;
        if (elapsedTime < 1)
        {
            yield return new WaitForSeconds(1 - elapsedTime);
        }
        LoadingPanel.SetActive(false);

        callback?.Invoke((GetHandicapListResponse)WebRequest.ResultObject);
    }
    //---------------------------------------------------------------------------------------------
    // ボランティアの方が操作する処理（まだ全然未実装）
    //---------------------------------------------------------------------------------------------




    #endregion
}
