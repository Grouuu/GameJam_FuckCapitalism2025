using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsPanelUI : MonoBehaviour
{
	public GameObject parent;
	public Button optionsButton;
	public Button closeButton;
	public Button mainMenuButton;

	public void OnOptionsClick ()
	{
		parent.SetActive(true);
	}

	public void OnCloseClick ()
	{
		parent.SetActive(false);
	}

	public void onMainMenuClick ()
	{
		SceneManager.LoadScene(SceneList.MAIN);
	}

	private void Awake ()
	{
		parent.SetActive(false);
	}

}
