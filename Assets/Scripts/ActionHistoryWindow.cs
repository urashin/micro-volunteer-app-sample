using UnityEngine;
using UnityEngine.UI;

public class ActionHistoryWindow : MonoBehaviour
{
	[SerializeField] private Button BackButton;

	public void OpenWindow()
	{
		gameObject.SetActive(true);
		BackButton.onClick.AddListener(() =>
		{
			BackButton.onClick.RemoveAllListeners();
			Hide();
		});
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
