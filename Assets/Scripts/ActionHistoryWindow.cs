using UnityEngine;
using UnityEngine.UI;

public class ActionHistoryWindow : MonoBehaviour
{
	[SerializeField] private Button BackButton;

	private void Awake()
	{
		gameObject.SetActive(false);
		BackButton.onClick.AddListener(() =>
	    {
		   CloseWindow();
	    });
	}

	public void OpenWindow()
	{
		gameObject.SetActive(true);
	}

	private void CloseWindow()
	{
		gameObject.SetActive(false);
	}
}
