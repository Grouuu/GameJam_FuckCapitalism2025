using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAvatarUI : MonoBehaviour
{
	public Image avatarSprite;
	public TextMeshProUGUI avatarName;

	private Sprite _sprite;

	public void SetAvatarSprite (string fileName)
	{
		Sprite sprite = Resources.Load<Sprite>($"AvatarSprites/{fileName}");

		if (sprite == null)
		{
			Debug.Log($"No sprite found for {fileName}");
			return;
		}

		_sprite = sprite;
		avatarSprite.sprite = _sprite;
	}

	public void SetAvatarName (string name)
	{
		avatarName.text = name;
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
