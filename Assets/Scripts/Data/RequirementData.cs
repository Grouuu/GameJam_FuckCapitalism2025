using System;

public enum Check
{
	None,
	Equal,
	Less,
	More,
	LessEqual,
	MoreEqual
}

[Serializable]
public class ResourceCompareValues
{
	public ResourceId resourceType = ResourceId.None;
	public Check checkType = Check.None;
	public int value;

	public bool IsValueOK (int value)
	{
		return GetCompareFunc()(value);
	}

	private Func<int, bool> GetCompareFunc ()
	{
		switch (checkType)
		{
			case Check.Equal: return valueToCheck => valueToCheck == value;
			case Check.Less: return valueToCheck => valueToCheck < value;
			case Check.More: return valueToCheck => valueToCheck > value;
			case Check.LessEqual: return valueToCheck => valueToCheck <= value;
			case Check.MoreEqual: return valueToCheck => valueToCheck >= value;
		}

		return valueToCheck => true;
	}
}

[Serializable]
public class RequirementData
{
	public EventData[] usedEvents;
	public DialogData[] usedDialogs;
	public ResourceCompareValues[] resourceChecks;

    public bool IsOK ()
	{
		return IsEventsOK() && IsDialogsOK() && IsResourcesOK();
	}

	private bool IsEventsOK ()
	{
		foreach (EventData eventData in usedEvents)
		{
			if (!eventData.isUsed)
				return false;
		}

		return true;
	}

	private bool IsDialogsOK ()
	{
		foreach (DialogData dialogData in usedDialogs)
		{
			if (!dialogData.isUsed)
				return false;
		}

		return true;
	}

	private bool IsResourcesOK ()
	{
		ResourcesManager rm = GameManager.Instance.resourcesManager;

		foreach (ResourceCompareValues resourceCompare in resourceChecks)
		{
			int valueToCheck = rm.GetResourceValue(resourceCompare.resourceType);

			if (!resourceCompare.IsValueOK(valueToCheck))
				return false;
		}

		return true;
	}

}
