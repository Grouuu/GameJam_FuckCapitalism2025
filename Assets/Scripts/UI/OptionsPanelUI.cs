using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsPanelUI : MonoBehaviour
{
	public GameObject parent;
	public Button optionsButton;
	public Button closeButton;
	public Button mainMenuButton;
	public VolumeControlsUI volumeControls;

	/**
	 * Linked in the editor
	 */
	public void OnOptionsClick ()
	{
		parent.SetActive(true);
	}

	/**
	 * Linked in the editor
	 */
	public void OnCloseClick ()
	{
		parent.SetActive(false);
	}

	/**
	 * Linked in the editor
	 */
	public void OnMainMenuClick ()
	{
		SceneManager.LoadScene(SceneList.MAIN);
	}

	private void Awake ()
	{
		parent.SetActive(false);
	}

}
