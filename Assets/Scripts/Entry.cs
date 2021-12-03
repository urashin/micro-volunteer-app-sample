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

        var deeplink = processDeepLinkMngr.deeplinkURL;
        deeplink = "http://example.com/sns-register?sns_id=Ufd9b51a10f5ac66057fb93a1d75f79e5&token=1e57a9d5-9c04-47e0-bc27-005c88ec0ad7";

        if (deeplink != "")
        {
            var res = Utility.ParseDeepLink(deeplink);
            VersionText.text = res.Endpoint;
            SaveToken(res.QueryDictionary["token"]);
        }

        // 端末にtokenが保存されているかを調べる
        var token = PlayerPrefs.GetString("token", "");
        if (token == "")
		{
            Debug.Log("tokenが端末に保存されていないので未登録ユーザ");
		}
        else
        {
            Debug.Log("token保存済み、登録ユーザ");
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
        yield return StartCoroutine(ApiCallCheckIn("135.000", "36.0000",
            (WebRequest.CheckInResponse response) => {
                Debug.Log("API finished! " + response.result);
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
                    StartCoroutine(ApiCallTest(ApiFinishedCallback));
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

    private IEnumerator ApiCallCheckIn(string x_geometry, string y_geometry, Action<WebRequest.CheckInResponse> callback)
    {
        // 時間計測用
        var startTime = Time.time;

        LoadingPanel.SetActive(true);
        PanelMessage.text = "チェックイン中です...";

        // API通信開始
        WebRequest.Init().CallCheckInApi(m_token, x_geometry, y_geometry);
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

        // wait
        var elapsedTime = Time.time - startTime;
        if (elapsedTime < 1)
        {
            yield return new WaitForSeconds(1 - elapsedTime);
        }
        LoadingPanel.SetActive(false);

        callback?.Invoke((WebRequest.CheckInResponse)WebRequest.ResultObject);
    }

    //---------------------------------------------------------------------------------------------
    // ボランティアの方が操作する処理（まだ全然未実装）
    //---------------------------------------------------------------------------------------------




    #endregion
}
