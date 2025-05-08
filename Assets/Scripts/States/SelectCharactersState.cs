
public class SelectCharactersState : StateCommand
{
	public override void StartCommand ()
	{
		GameManager gm = GameManager.Instance;
		CharacterData[] characters = gm.charactersManager.PickRandomAvailableCharacters(gm.startAmountCharacters);
		gm.uiManager.SetCharacters(characters);

		EndCommand();
	}

	protected override void Reset () {}

	private void OnEnable () =>  state = GameState.SelectCharacters;

}
