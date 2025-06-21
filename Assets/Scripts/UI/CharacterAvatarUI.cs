using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAvatarUI : MonoBehaviour
{
	public static readonly string FOLDER_SPRITES = "AvatarSprites/";

	public Image avatarSprite;
	public Localize avatarNameLocalize;

	private Sprite _sprite;

	public void SetAvatarSprite (string fileName)
	{
		Sprite sprite = Resources.Load<Sprite>($"{UIManager.PATH_SPRITES}{FOLDER_SPRITES}{fileName}");

		if (sprite == null)
		{
			Debug.LogWarning($"No sprite found for {fileName}");
			return;
		}

		_sprite = sprite;
		avatarSprite.sprite = _sprite;
	}

	public void SetAvatarName (string name)
	{
		avatarNameLocalize.SetTerm($"{LocCat.CharactersNames}/{name}");
	}

	private void OnDisable ()
	{
		if (_sprite != null)
		{
			Resources.UnloadAsset(_sprite);
			_sprite = null;
		}
	}

}
