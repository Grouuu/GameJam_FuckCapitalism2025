using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAvatarUI : MonoBehaviour
{
	public Image avatarSprite;
	public TextMeshProUGUI avatarName;

	public void SetAvatarSprite (Sprite sprite)
	{
		avatarSprite.sprite = sprite;
	}

	public void SetAvatarName (string name)
	{
		avatarName.text = name;
	}

}
