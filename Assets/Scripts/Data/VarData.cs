using Newtonsoft.Json;
using System;

public class VarData
{
	public string id;
	public string name;
	public GameVarId varId;
	public GameVarType type;
	public string iconFileName;
	public int startValue;
	public int minValue;
	public int maxValue;
	public VarCompareValue lowThreshold;

	// runtime values
	public int previousValue;
	public int currentValue {
		get => _currentValue;
		set {
			previousValue = _currentValue;
			_currentValue = value;
		}
	}

	private int _currentValue;

	public bool IsLow ()
	{
		if (lowThreshold == null)
			return false;

		int checkedValue = GameManager.Instance.varsManager.GetVarValue(lowThreshold.varId);

		return lowThreshold.IsValueOK(checkedValue);
	}

	public VarData Clone ()
	{
		var serialized = JsonConvert.SerializeObject(this);
		return JsonConvert.DeserializeObject<VarData>(serialized);
	}
}

public class VarCompareValue
{
	public GameVarId varId = GameVarId.None;
	public CompareValueType checkType = CompareValueType.None;
	public int compareValue;

	public bool IsValueOK (int value)
	{
		return GetCompareFunc()(value);
	}

	private Func<int, bool> GetCompareFunc ()
	{
		return checkType switch
		{
			CompareValueType.Equal => valueToCheck => valueToCheck == compareValue,
			CompareValueType.Less => valueToCheck => valueToCheck < compareValue,
			CompareValueType.More => valueToCheck => valueToCheck > compareValue,
			CompareValueType.LessEqual => valueToCheck => valueToCheck <= compareValue,
			CompareValueType.MoreEqual => valueToCheck => valueToCheck >= compareValue,
			_ => valueToCheck => true,
		};
	}
}
