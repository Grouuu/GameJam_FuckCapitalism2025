using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    [HideInInspector] public string id = Guid.NewGuid().ToString();

}
