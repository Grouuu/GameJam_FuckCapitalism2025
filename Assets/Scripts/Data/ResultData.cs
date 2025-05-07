using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ResultData", menuName = "Scriptable Objects/ResultData")]
public class ResultData : ScriptableObject
{
    [HideInInspector] public string id = Guid.NewGuid().ToString();

}
