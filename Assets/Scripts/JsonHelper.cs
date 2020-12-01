using System.IO;
using UnityEngine;

public class JsonHelper
{
    public static T[] ReadFromJsonServer<T>(string json)
    {
        var newJson = string.Empty;
        var tempJson = "{ \"array\": " + json + "}";
        newJson = json.Contains("array") ? ParserStringForJSON(json) : 
                                           ParserStringForJSON(tempJson);
        
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static T[] ReadFromJsonFile<T>(string way)
    {
        using (StreamReader fs = new StreamReader(way))
        {
            var json = fs.ReadToEnd();
            json = ParserStringForJSON(json);
            return JsonUtility.FromJson<Wrapper<T>>(json).array;
        }
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T item)
    {
        return JsonUtility.ToJson(item);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    private static string ParserStringForJSON(string json)
    {
        return json.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
    }
}