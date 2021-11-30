using System.Collections.Generic;
using Utf8Json;

public class Todos
{
    public List<TestJsonData> TestJsonDataList;
}

public class TestJsonData
{
    public int userId { get; set; }
    public int id { get; set; }
    public string title { get; set; }
    public bool completed { get; set; }
    
    //public Nest nest { get; set; }
}

public class Nest
{
    public bool foobar { get; set; }
}

public class JsonDecodeTest
{
    public List<TestJsonData> Test(string jsonText)
    {
        return JsonSerializer.Deserialize<List<TestJsonData>>(jsonText);
    }
}
