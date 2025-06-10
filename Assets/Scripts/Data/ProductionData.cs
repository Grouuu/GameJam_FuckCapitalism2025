using UnityEngine;

public class ProductionData
{
    private readonly int MAX = 100;

    public string id;
    public GameVarId varId;
    public int randomWeight;

    public bool CheckRandom () => Random.Range(0, MAX) <= randomWeight;

}
