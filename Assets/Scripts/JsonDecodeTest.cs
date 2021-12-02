using System.Collections.Generic;
using Utf8Json;

public class TestJsonData
{
    public string token { get; set; }
}

public class JsonDecodeTest
{
    public TestJsonData Test(string jsonText)
    {
        return JsonSerializer.Deserialize<TestJsonData>(jsonText);
    }
}
