using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class JsonSorter
{
    public static string SortKeys(string json)
    {
        // Parse JSON into dictionary
        Dictionary<string, object> dict = ParseJson(json);

        // Sort dictionary by keys
        var sortedDict = dict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        // Build sorted JSON string
        StringBuilder sb = new StringBuilder("{");
        foreach (var kvp in sortedDict)
        {
            sb.Append($"\"{kvp.Key}\":{GetValueString(kvp.Value)},");
        }
        if (sortedDict.Count > 0)
        {
            sb.Remove(sb.Length - 1, 1); // Remove the trailing comma
        }
        sb.Append("}");

        return sb.ToString();
    }

    static Dictionary<string, object> ParseJson(string json)
    {
        Stack<Dictionary<string, object>> stack = new Stack<Dictionary<string, object>>();
        stack.Push(new Dictionary<string, object>());

        StringBuilder currentKey = new StringBuilder();
        StringBuilder currentValue = new StringBuilder();
        bool inString = false;
        bool isKey = false;

        foreach (char c in json)
        {
            if (c == '"' && !inString)
            {
                inString = true;
                isKey = !isKey;
                continue;
            }

            if (c == '"' && inString)
            {
                inString = false;
                if (isKey)
                {
                    currentKey.Clear();
                }
                else
                {
                    stack.Peek()[currentKey.ToString()] = currentValue.ToString();
                    currentValue.Clear();
                }
                continue;
            }

            if (inString)
            {
                if (isKey)
                {
                    currentKey.Append(c);
                }
                else
                {
                    currentValue.Append(c);
                }
            }
            else
            {
                if (c == '{')
                {
                    stack.Push(new Dictionary<string, object>());
                }
                else if (c == '}')
                {
                    var obj = stack.Pop();
                    stack.Peek()[currentKey.ToString()] = obj;
                    currentKey.Clear();
                }
            }
        }

        return stack.Pop();
    }

    static string GetValueString(object value)
    {
        if (value is Dictionary<string, object> dict)
        {
            StringBuilder sb = new StringBuilder("{");
            foreach (var kvp in dict)
            {
                sb.Append($"\"{kvp.Key}\":{GetValueString(kvp.Value)},");
            }
            if (dict.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1); // Remove the trailing comma
            }
            sb.Append("}");
            return sb.ToString();
        }
        else
        {
            return value.ToString();
        }
    }
}
