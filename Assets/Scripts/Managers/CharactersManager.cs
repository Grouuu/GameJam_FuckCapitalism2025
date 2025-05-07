using System;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
	public CharacterData[] characters;

	private List<CharacterData> _charactersAvailable = new();
	private List<CharacterData> _charactersSelected = new();

	/**
	 * Update the selected characters
	 */
	public void SetPickedCharacters (string[] charactersId)
	{
		_charactersSelected = new();

		foreach (string characterId in charactersId)
		{
			CharacterData characterData = FindCharacterById(characterId);

			if (characterData != null)
				_charactersSelected.Add(characterData);
			else
				Debug.LogWarning($"Character not found (id: {charactersId})");
		}
	}

	public void FlagUsedCharacters (string[] charactersId)
	{
		foreach (string characterId in charactersId)
		{
			CharacterData characterData = FindCharacterById(characterId);

			if (characterData != null)
				characterData.isUsed = true;
			else
				Debug.LogWarning($"Character not found (id: {charactersId})");
		}
	}

	/**
	 * Update the available characters, and only keep ones with available dialogs
	 */
	public CharacterData[] UpdateAvailableCharacters ()
	{
		_charactersAvailable = new List<CharacterData>();

		if (characters == null)
			return _charactersAvailable.ToArray();

		foreach(CharacterData character in characters)
		{
			if (character.isAvailable())
				_charactersAvailable.Add(character);
		}

		return _charactersAvailable.ToArray();
	}

	/**
	 * Pick a certain amount of available characters
	 */
	public CharacterData[] PickRandomAvailableCharacters (int amountOfCharacters)
	{
		if (_charactersAvailable.Count <= amountOfCharacters)
			return _charactersAvailable.ToArray();

		List<CharacterData> pickedCharacters = new();
		HashSet<int> selectedCharactersIndex = new();

		while (selectedCharactersIndex.Count < amountOfCharacters)
		{
			// TODO use a seed to prevent a different selection after a reload
			int randomIndex = UnityEngine.Random.Range(0, _charactersAvailable.Count);

			if (selectedCharactersIndex.Add(randomIndex))
				pickedCharacters.Add(_charactersAvailable[randomIndex]);
		}

		return pickedCharacters.ToArray();
	}

	public void Log ()
	{
		if (_charactersSelected == null || _charactersSelected.Count == 0)
		{
			Debug.Log("No selected character");
			return;
		}

		foreach (CharacterData character in _charactersSelected)
		{
			Debug.Log($"selected character: {character.characterName}");
		}
	}

	private CharacterData FindCharacterById (string characterId)
	{
		return Array.Find(characters, character => character.id == characterId);
	}

}
