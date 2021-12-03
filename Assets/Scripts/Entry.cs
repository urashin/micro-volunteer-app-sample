using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// アプリ本体
/// </summary>
public class Entry : MonoBehaviour
{
	#region Inspector

	[SerializeField] TextMeshProUGUI VersionText;
    [SerializeField] WebRequestTest WebRequest;

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

    #region Method

    // Start is called before the first frame update
    void Start()
    {
        LoadingPanel.SetActive(false);

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
        PanelMessage.text = "Checking in...";
        StartCoroutine(AsyncCheckin());
    }

    private void OnclickActionHistoryButton()
	{
        ActionHistoryWindow.OpenWindow();
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
        yield return new WaitForSeconds(2);

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
            "Do you want to send?",
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
        SelectYesCancelDialog.OpenDialog(1, "Volunteers are heading.", DisplayEvaluationScreen);
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
        PanelMessage.text = "Matching...";

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

        // マッチングが早く完了した場合、メッセージがすぐに消えてしまうので、最低でも2秒は待つ
        var elapsedTime = Time.time - startTime;
        if (elapsedTime < 2)
        {
            yield return new WaitForSeconds(2 - elapsedTime);
        }

        LoadingPanel.SetActive(false);

        callback?.Invoke();
    }

    //---------------------------------------------------------------------------------------------
    // ボランティアの方が操作する処理（まだ全然未実装）
    //---------------------------------------------------------------------------------------------




    #endregion
}
