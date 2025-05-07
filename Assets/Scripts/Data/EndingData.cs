using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EndingData", menuName = "Scriptable Objects/EndingData")]
public class EndingData : ScriptableObject
{
    [HideInInspector] public string id = Guid.NewGuid().ToString();

}
