using UnityEngine;

public class RequirementData
{
	public VarCompareValue[] varsChecks;

    public bool IsOK ()
	{
		return IsResourcesOK();
	}

	private bool IsResourcesOK ()
	{
		if (varsChecks == null)
			return true;

		foreach (VarCompareValue varCompare in varsChecks)
		{
			int valueToCheck = GameManager.Instance.varsManager.GetVarValue(varCompare.varId);

			if (!varCompare.IsValueOK(valueToCheck))
				return false;
		}

		return true;
	}

}
