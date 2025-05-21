using Newtonsoft.Json;
using System;

public class TestImportDatabase
{
	public TestImportData[] entries;
}

public class TestImportData
{
	public int id { get; set; }
	public string name { get; set; }
}

public class TestImportParser : DatabaseParser
{
	private TestImportDatabase _data;

	public override Type GetDataType () => typeof(TestImportDatabase);

	public override TestImportDatabase GetData<TestImportDatabase> () {
		return (TestImportDatabase) (object) _data;
	}

	public override void ParseData (string json)
	{
		_data = new();
		_data.entries = JsonConvert.DeserializeObject<TestImportData[]>(json);
	}

}
