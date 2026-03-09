using Grouuu.Enum;
using System;

namespace Grouuu.Utils
{
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
}