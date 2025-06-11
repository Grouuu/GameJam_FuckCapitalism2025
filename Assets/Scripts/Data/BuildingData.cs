
public class BuildingData
{
    public string id;
    public string name;
    public string displayName;
    public string description;
    public int constructionTime;
    public int buildLimit;
    public ResultVarChange[] costs;
    public ResultData result;
    public ResultVarChange[] production;
    public ProductionMultiplierData[] productionMultipliers;

    public bool isBuilt = false;

}
