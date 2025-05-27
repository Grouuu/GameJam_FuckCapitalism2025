using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DatabaseImporterConfig", menuName = "Editor/DatabaseImporterConfig")]
public class GSpreadSheetsToJsonConfig : ScriptableObject
{
	public string spreadSheetKey;
	public List<string> wantedSheetNames;
	public string outputDir;
}
