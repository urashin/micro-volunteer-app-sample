using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnsRegisterScreen : MonoBehaviour
{
    #region Inspector
    [SerializeField] Button RegisterButton;
    [SerializeField] TMP_InputField EmailInputField;
    [SerializeField] TMP_InputField PasswordInputField;
    #endregion

    public void OpenScreen(Action<string, string> callback)
    {
        RegisterButton.onClick.AddListener(() =>
        {
            RegisterButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
            callback?.Invoke(EmailInputField.text, PasswordInputField.text);
        });

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
