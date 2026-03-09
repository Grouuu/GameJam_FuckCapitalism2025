using Grouuu.Enum;
using Grouuu.Utils;
using Newtonsoft.Json;

namespace Grouuu.Data
{
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
		public int currentValue
		{
			get => _currentValue;
			set
			{
				previousValue = _currentValue;
				_currentValue = value;
			}
		}

		private int _currentValue;

		public bool IsLow ()
		{
			if (lowThreshold == null)
				return false;

			int checkedValue = GameController.GameManagers.VarsManager.GetVarValue(lowThreshold.varId);

			return lowThreshold.IsValueOK(checkedValue);
		}

		public VarData Clone ()
		{
			var serialized = JsonConvert.SerializeObject(this);
			return JsonConvert.DeserializeObject<VarData>(serialized);
		}
	}
}