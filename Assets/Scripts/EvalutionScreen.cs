using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 評価入力画面
/// </summary>
public class EvalutionScreen : MonoBehaviour
{
    [SerializeField] Button SendButton;

    public void OpenScreen(Action callback)
	{
        SendButton.onClick.AddListener(() =>
        {
            SendButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
            callback?.Invoke();
        });

        gameObject.SetActive(true);
    }

    public void Hide()
	{
        gameObject.SetActive(false);
    }
}
