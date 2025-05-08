using UnityEngine;

public class UIManager : MonoBehaviour
{
	public ResourceValueUI scrapsValue;
	public ResourceValueUI energyValue;
	public ResourceValueUI trustValue;
	public ResourceValueUI populationValue;
	public ResourceValueUI scienceValue;
	public CharacterAvatarUI[] characterAvatars;

	public void SetValues (GameResources resources)
	{
		SetScrapsValue(resources.scraps);
		SetEnergyValue(resources.energy);
		SetTrustValue(resources.trust);
		SetPopulationValue(resources.population);
		SetScienceValue(resources.science);
	}

	public void SetScrapsValue (int value)
	{
		scrapsValue.SetValue(value);
	}

	public void SetEnergyValue (int value)
	{
		energyValue.SetValue(value);
	}

	public void SetTrustValue (int value)
	{
		trustValue.SetValue(value);
	}

	public void SetPopulationValue (int value)
	{
		populationValue.SetValue(value);
	}

	public void SetScienceValue (int value)
	{
		scienceValue.SetValue(value);
	}

	public void SetCharacters (CharacterData[] characters)
	{
		for (int i = 0; i < characterAvatars.Length; i++)
		{
			CharacterAvatarUI avatar = characterAvatars[i];

			if (i >= characters.Length)
			{
				avatar.gameObject.SetActive(false);
				continue;
			}

			CharacterData data = characters[i];

			avatar.gameObject.SetActive(true);
			avatar.SetAvatarSprite(data.characterAvatar);
		}
	}

}
