using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

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
		Button0,
		Button1,
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
	private void Set1ButtonMode(List<string> buttonTexts)
	{
		Buttons[1].gameObject.SetActive(false);

		Buttons[0].onClick.AddListener(() =>
		{
			ButtonClicked(EButtonType.Button0);
		});
		Buttons[0].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = buttonTexts[0];
	}

	/// <summary>
	/// YES, Cancelボタンモード
	/// </summary>
	private void Set2ButtonMode(List<string> buttonTexts)
	{
		Buttons[1].gameObject.SetActive(true);

		Buttons[0].onClick.AddListener(() =>
		{
			ButtonClicked(EButtonType.Button0);
		});
		Buttons[1].onClick.AddListener(() =>
		{
			ButtonClicked(EButtonType.Button1);
		});

		Buttons[0].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = buttonTexts[0];
		Buttons[1].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = buttonTexts[1];
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
	public void OpenDialog(string message, List<string> buttonTexts, Action callback = null)
	{
		switch (buttonTexts.Count)
		{
			case 1:
				Set1ButtonMode(buttonTexts);
				break;
			case 2:
				Set2ButtonMode(buttonTexts);
				break;
			default:
				return;
		}

		gameObject.SetActive(true);
		Message.text = message;
		m_callback = callback;
	}

}
