
public class BuildingData
{
    public string id;
    public string name;
    public ResultData result;
    public ResultVarChange[] production;
    public ProductionMultiplierData[] productionMultipliers;

    // runtime values
    public int progress = 0;
    public bool isBuilt = false;

}
