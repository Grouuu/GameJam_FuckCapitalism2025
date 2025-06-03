using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Color baseColor = Color.black;
	public Color hoverColor = Color.white;

	private TextMeshProUGUI _buttonLabel;

	public void OnPointerClick (PointerEventData eventData)
	{
		throw new System.NotImplementedException();
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (_buttonLabel != null)
			_buttonLabel.color = hoverColor;
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (_buttonLabel != null)
			_buttonLabel.color = baseColor;
	}

	private void Start ()
	{
		_buttonLabel = GetComponentInChildren<TextMeshProUGUI>();
	}
}
