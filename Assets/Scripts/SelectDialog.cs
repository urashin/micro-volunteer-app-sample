using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// ダイアログ制御をざっくり実装
/// ボタンが1つの時（到着しましたボタン表示）
/// ボタンが2つの時（YES, Cancelボタン表示）
/// </summary>
public class SelectDialog : MonoBehaviour
{
	#region Inspector

	[SerializeField] TextMeshProUGUI Message;
	[SerializeField] private Button[] Buttons;

	#endregion

	/// <summary>
	/// ボタン種別
	/// </summary>
	public enum EButtonType
	{
		Yes,
		Cancel,
		Arrived
	}

	/// <summary>
	/// どのボタンが押されたのか
	/// </summary>
	public EButtonType SelectedButton { private set; get; }

	/// <summary>
	/// ダイアログを閉じた時のCallback
	/// </summary>
	private Action m_callback;

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	/// <summary>
	/// ボタン1つモード（仮で、到着しましたボタンのみを表示）
	/// </summary>
	private void Set1ButtonMode()
	{
		Buttons[1].gameObject.SetActive(false);

		Buttons[0].onClick.AddListener(() =>
		{
			ButtonClicked(EButtonType.Arrived);
		});
		Buttons[0].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "到着しました";
	}

	/// <summary>
	/// YES, Cancelボタンモード
	/// </summary>
	private void Set2ButtonMode()
	{
		Buttons[1].gameObject.SetActive(true);

		Buttons[0].onClick.AddListener(() =>
		{
			ButtonClicked(EButtonType.Yes);
		});
		Buttons[1].onClick.AddListener(() =>
		{
			ButtonClicked(EButtonType.Cancel);
		});

		Buttons[0].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "はい";
		Buttons[1].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "キャンセル";
	}

	/// <summary>
	/// ボタンクリックの共通処理
	/// </summary>
	/// <param name="buttonType"></param>
	private void ButtonClicked(EButtonType buttonType)
	{
		SelectedButton = buttonType;
		gameObject.SetActive(false);

		foreach (var item in Buttons)
		{
			item.onClick.RemoveAllListeners();
		}

		m_callback?.Invoke();
	}

	/// <summary>
	/// ダイアログ開く
	/// </summary>
	/// <param name="message">表示するメッセージ</param>
	/// <param name="callback">閉じた時のCallback</param>
	public void OpenDialog(int buttonNum, string message, Action callback)
	{
		switch (buttonNum)
		{
			case 1:
				Set1ButtonMode();
				break;
			case 2:
				Set2ButtonMode();
				break;
			default:
				return;
		}

		gameObject.SetActive(true);
		Message.text = message;
		m_callback = callback;
	}

}
