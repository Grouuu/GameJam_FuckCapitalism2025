using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public int startAmountCharacters;
	public Resources startResources;

	[HideInInspector] public CharactersManager charactersManager;
	[HideInInspector] public ResourcesManager resourcesManager;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;

		charactersManager = GetComponentInChildren<CharactersManager>();
		resourcesManager = GetComponentInChildren<ResourcesManager>();
	}

	private void Start ()
	{
		InitResources();
		InitCharacters();

#if UNITY_EDITOR
		resourcesManager.Log();
		charactersManager.Log();
# endif
	}

	private void InitResources ()
	{
		// TODO get data from save if available

		resourcesManager.resources = startResources;
	}

	private void InitCharacters ()
	{
		// TODO get data from save if available

		charactersManager.UpdateAvailableCharacters();

		string[] charactersId = charactersManager
			.PickRandomAvailableCharacters(startAmountCharacters)
			.Select(character => character.id)
			.ToArray()
		;
		charactersManager.SetPickedCharacters(charactersId);
		charactersManager.FlagUsedCharacters(charactersId); // TODO flag previous characters too
	}

}
