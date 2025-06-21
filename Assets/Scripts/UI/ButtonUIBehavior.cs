using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonUIBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
	public Color baseTextColor = Color.black;
	public Color hoverTextColor = Color.white;
	public Color pressedTextColor = Color.black;
	public SoundFxKey clickSound = SoundFxKey.Click;

	private TextMeshProUGUI _buttonLabel;

	public void OnPointerClick (PointerEventData eventData)
	{
		PersistentManager.Instance.soundManager.PlaySoundFX(clickSound);
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (_buttonLabel != null)
			_buttonLabel.color = hoverTextColor;
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (_buttonLabel != null)
			_buttonLabel.color = baseTextColor;
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (_buttonLabel != null)
			_buttonLabel.color = pressedTextColor;
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		if (_buttonLabel != null)
			_buttonLabel.color = baseTextColor;
	}

	private void Start ()
	{
		_buttonLabel = GetComponentInChildren<TextMeshProUGUI>();
	}

	private void OnDisable ()
	{
		if (_buttonLabel != null)
			_buttonLabel.color = baseTextColor;
	}
}
