using UnityEngine;
using UnityEngine.UI;

public class CharacterAvatarUI : MonoBehaviour
{
	public Image avatar;

	public void SetAvatarSprite (Sprite sprite)
	{
		avatar.sprite = sprite;
	}

}
