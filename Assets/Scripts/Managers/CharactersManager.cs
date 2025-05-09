using System;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
	public CharacterData[] characters;

	private List<CharacterData> _charactersAvailable = new();
	private CharacterData _selectedCharacter;

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

	public CharacterData SelectRandomCharacter ()
	{
		int randomIndex = UnityEngine.Random.Range(0, _charactersAvailable.Count);
		CharacterData character = _charactersAvailable[randomIndex];
		_selectedCharacter = character;
		return _selectedCharacter;
	}

	public void Log ()
	{
		if (_selectedCharacter != null)
			Debug.Log($"selected character: {_selectedCharacter.characterName}");
	}

	private CharacterData FindCharacterById (string characterId)
	{
		return Array.Find(characters, character => character.id == characterId);
	}

}
