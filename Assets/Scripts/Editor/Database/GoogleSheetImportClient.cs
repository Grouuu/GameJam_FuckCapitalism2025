using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GoogleSheetImportClient : GSpreadSheetsToJson
{
	private delegate object ParseFunc (string cellValue, string propertyName, string fileName);
	private readonly Dictionary<string, ParseFunc> supportedDataTypes = new()
	{
		{ "string",			ParseString },
		{ "int",			ParseInt },
		{ "float",			ParseFloat },
		{ "bool",			ParseBool },
		{ "string[]",		ParseStringArray },
		{ "int[]",			ParseIntArray },
		{ "float[]",		ParseFloatArray },
		{ "bool[]",			ParseBoolArray },
	};
	private readonly string configPath = "Assets/Scripts/Database/DatabaseImporterConfig.asset";

	[MenuItem("Database/GoogleSheets to JSON")]
	private static void ShowWindow () => _ = GetWindow(typeof(GoogleSheetImportClient)) as GoogleSheetImportClient;

	protected override void CreateJsonFile (string fileName, string outputDirectory, ValueRange valueRange)
	{
		//Get properties's name, data type and sheet data
		IDictionary<int, string> propertyNames = new Dictionary<int, string>(); //Dictionary of (column index, property name of that column)
		IDictionary<int, string> dataTypes = new Dictionary<int, string>();     //Dictionary of (column index, data type of that column)
		IDictionary<int, Dictionary<int, string>> values = new Dictionary<int, Dictionary<int, string>>();  //Dictionary of (row index, dictionary of (column index, value in cell))

		int rowIndex = 0;

		foreach (IList<object> row in valueRange.Values)
		{
			int columnIndex = 0;

			foreach (string cellValue in row)
			{
				string value = cellValue;

				if (rowIndex == 0)
				{
					//This row is properties's name row
					propertyNames.Add(columnIndex, value);
				}
				else if (rowIndex == 1)
				{
					//This row is properties's data type row
					dataTypes.Add(columnIndex, value);
				}
				else
				{
					//Data rows
					//Because first row is name row and second row is data type row, so we will minus 2 from rowIndex to make data index start from 0
					if (!values.ContainsKey(rowIndex - 2))
						values.Add(rowIndex - 2, new Dictionary<int, string>());

					values[rowIndex - 2].Add(columnIndex, value);
				}

				columnIndex++;
			}

			rowIndex++;
		}

		//Create list of Dictionaries (property name, value). Each dictionary represent for a object in a row of sheet.
		List<object> datas = new List<object>();
		foreach (int rowId in values.Keys)
		{
			bool thisRowHasError = false;
			Dictionary<string, object> data = new();

			foreach (int columnId in propertyNames.Keys)
			{
				//Read through all columns in sheet, with each column, create a pair of property(string) and value(type depend on dataType[columnId])
				if (thisRowHasError)
					break;

				if ((!dataTypes.ContainsKey(columnId)) || (!supportedDataTypes.ContainsKey(dataTypes[columnId])))
					continue;   //There is not any data type or this data type is strange. May be this column is used for comments. Skip this column.

				if (!values[rowId].ContainsKey(columnId))
					values[rowId].Add(columnId, "");

				string strVal = values[rowId][columnId];
				string type = dataTypes[columnId];

				if (type != "ignore")
				{
					string propertyName = propertyNames[columnId];
					object parsedValue = ParseEntry(type, strVal, propertyName, fileName);

					if (parsedValue != null)
						data.Add(propertyNames[columnId], parsedValue);
					else
						thisRowHasError = true;
				}
			}

			if (!thisRowHasError)
				datas.Add(data);
			else
				Debug.LogError("There's error!");
		}

		//Create json text
		string jsonText = JsonConvert.SerializeObject((object) datas);

		//Create directory to store the json file
		if (!outputDirectory.EndsWith("/"))
			outputDirectory += "/";

		Directory.CreateDirectory(outputDirectory);
		StreamWriter strmWriter = new StreamWriter(outputDirectory + fileName + ".json", false, System.Text.Encoding.UTF8);
		strmWriter.Write(jsonText);
		strmWriter.Close();

		Debug.Log("Created: " + fileName + ".json");
	}

	private void OnEnable ()
	{
		var config = AssetDatabase.LoadAssetAtPath<GSpreadSheetsToJsonConfig>(configPath);

		if (config != null)
		{
			spreadSheetKey = config.spreadSheetKey;
			wantedSheetNames = config.wantedSheetNames;
			outputDir = config.outputDir;
		}
		else
			Debug.LogError($"No config file found at path: {configPath}");
	}

	private object ParseEntry (string type, string cellValue, string propertyName, string fileName)
	{
		if (supportedDataTypes.TryGetValue(type, out ParseFunc parser))
			return parser(cellValue, propertyName, fileName);

		return null;
	}

	private static object ParseString (string cellValue, string propertyName, string fileName)
	{
		return cellValue;
	}

	private static object ParseInt (string cellValue, string propertyName, string fileName)
	{
		int value = 0;

		if (!string.IsNullOrEmpty(cellValue))
		{
			try
			{
				value = int.Parse(cellValue);
			}
			catch (Exception e)
			{
				Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyName, fileName, e.ToString()));
				return null;
			}
		}

		return value;
	}

	private static object ParseFloat (string cellValue, string propertyName, string fileName)
	{
		float value = 0f;

		if (!string.IsNullOrEmpty(cellValue))
		{
			try
			{
				value = float.Parse(cellValue);
			}
			catch (Exception e)
			{
				Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyName, fileName, e.ToString()));
				return null;
			}
		}

		return value;
	}

	private static object ParseBool (string cellValue, string propertyName, string fileName)
	{
		bool value = false;

		if (!string.IsNullOrEmpty(cellValue))
		{
			try
			{
				value = bool.Parse(cellValue);
			}
			catch (Exception e)
			{
				Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyName, fileName, e.ToString()));
				return null;
			}
		}

		return value;
	}

	private static object ParseStringArray (string cellValue, string propertyName, string fileName)
	{
		return cellValue.Split(new char[] { ',' });
	}

	private static object ParseIntArray (string cellValue, string propertyName, string fileName)
	{
		string[] stringArray = cellValue.Split(new char[] { ',' });
		int[] valueArray = new int[stringArray.Length];

		if (string.IsNullOrEmpty(cellValue.Trim()))
			valueArray = new int[0];

		for (int i = 0; i < valueArray.Length; i++)
		{
			int value = 0;

			if (!string.IsNullOrEmpty(stringArray[i]))
			{
				try
				{
					value = int.Parse(stringArray[i]);
				}
				catch (Exception e)
				{
					Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyName, fileName, e.ToString()));
					return null;
				}
			}

			valueArray[i] = value;
		}

		return valueArray;
	}

	private static object ParseFloatArray (string cellValue, string propertyName, string fileName)
	{
		string[] stringArray = cellValue.Split(new char[] { ',' });
		float[] valueArray = new float[stringArray.Length];

		if (string.IsNullOrEmpty(cellValue.Trim()))
			valueArray = new float[0];

		for (int i = 0; i < valueArray.Length; i++)
		{
			float value = 0;

			if (!string.IsNullOrEmpty(stringArray[i]))
			{
				try
				{
					value = float.Parse(stringArray[i]);
				}
				catch (Exception e)
				{
					Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyName, fileName, e.ToString()));
					return null;
				}
			}

			valueArray[i] = value;
		}

		return valueArray;
	}

	private static object ParseBoolArray (string cellValue, string propertyName, string fileName)
	{
		string[] stringArray = cellValue.Split(new char[] { ',' });
		bool[] valueArray = new bool[stringArray.Length];

		if (string.IsNullOrEmpty(cellValue.Trim()))
			valueArray = new bool[0];

		for (int i = 0; i < valueArray.Length; i++)
		{
			bool value = false;

			if (!string.IsNullOrEmpty(stringArray[i]))
			{
				try
				{
					value = bool.Parse(stringArray[i]);
				}
				catch (Exception e)
				{
					Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyName, fileName, e.ToString()));
					return null;
				}
			}

			valueArray[i] = value;
		}
		return valueArray;
	}

}
