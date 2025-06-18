using System.Collections.Generic;
using UnityEngine;

public class SegmentedProgressBarUI : MonoBehaviour
{
	public GameObject prefabSegmentFull;
	public Transform parentFull;
	public GameObject prefabSegmentEmpty;
	public Transform parentEmtpy;
	public int ratio1PerSegment = 1;

	public int NumberSegments => Mathf.CeilToInt(_max / ratio1PerSegment);

	private int _min = 0;
	private int _max = 0;
	private int _value = 0;

	private List<GameObject> _segmentsFull = new();
	private List<GameObject> _segmentsEmpty = new();

	public void SetValue (int value)
	{
		_value = value;
		UpdateBar();
	}

	public void SetMin (int min) => _min = min;

	public void SetMax (int max)
	{
		_max = max;
		UpdateBar();
	}

	private void UpdateBar ()
	{
		if (_segmentsFull.Count < NumberSegments)
			FillBar();

		int indexLastFullSegment = Mathf.RoundToInt(MathUtils.Remap(_min, _max, 0, NumberSegments, _value)) - 1;

		for (int i = 0; i < NumberSegments; i++)
		{
			_segmentsFull[i].SetActive(i <= indexLastFullSegment);
			_segmentsEmpty[i].SetActive(i > indexLastFullSegment);
		}
	}

	private void FillBar ()
	{
		while (_segmentsFull.Count < NumberSegments)
		{
			GameObject segmentFull = Instantiate(prefabSegmentFull, parentFull);
			GameObject segmentEmpty = Instantiate(prefabSegmentEmpty, parentEmtpy);

			segmentFull.SetActive(false);

			_segmentsFull.Add(segmentFull);
			_segmentsEmpty.Add(segmentEmpty);
		}
	}

	private void OnEnable ()
	{
		foreach (Transform child in parentFull)
			Destroy(child.gameObject);
		foreach (Transform child in parentEmtpy)
			Destroy(child.gameObject);

		UpdateBar();
	}

}
