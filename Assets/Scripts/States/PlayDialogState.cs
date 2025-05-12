using UnityEngine;

public class PlayDialogState : StateCommand
{
	public override void StartCommand ()
	{
		PickCharacter();
	}

	protected override void Reset () {}

	private void OnEnable () =>  state = GameState.PlayDialog;

	private void PickCharacter ()
	{
		GameManager gm = GameManager.Instance;
		gm.charactersManager.UpdateAvailableCharacters();
		CharacterData character = gm.charactersManager.SelectRandomCharacter();

		gm.charactersManager.Log();

		EndCommand(); // DEBUG
	}

}
