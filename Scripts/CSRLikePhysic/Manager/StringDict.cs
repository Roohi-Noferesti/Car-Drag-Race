using System.Collections.Generic;

public class StringDict : Dictionary<string, string>
{
    public string Get(string field, bool convert_nulls = false)
    {
        string text = (!this.ContainsKey(field)) ? null : this[field];
        if (text == null && convert_nulls)
        {
            return string.Empty;
        }
        return text;
    }

    public int GetInt(string field, int bad = 0)
    {
        return this.Get(field, true).ToIntOrDefault(bad);
    }

    public bool GetBool(string field)
    {
        int num = this.Get(field, true).ToIntOrDefault(0);
        return num != 0;
    }
}
