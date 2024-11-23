using System;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class NumberPlate
{
    public string Text = string.Empty;

    public Color BackgroundColor = new Color(0.96f, 0.96f, 0.96f);

    public Color TextColor = new Color(0.1f, 0.1f, 0.1f);

    public Color BorderColor = new Color(0.1f, 0.1f, 0.1f);

    public void SetToInactiveColours()
    {
        this.BackgroundColor = new Color(0.96f, 0.96f, 0.96f);
        this.TextColor = new Color(0.1f, 0.1f, 0.1f);
        this.BorderColor = new Color(0.1f, 0.1f, 0.1f);
    }

    public void SetToDefaultColours()
    {
        this.BackgroundColor = new Color(0.96f, 0.96f, 0.96f);
        this.TextColor = new Color(0.1f, 0.1f, 0.1f);
        this.BorderColor = new Color(0.1f, 0.1f, 0.1f);
    }

    public static string SafeString(string val)
    {
        string text = val.ToUpper();
        text = Regex.Replace(text, "[^A-Z0-9 \\-]+", string.Empty);
        if (text.Length > 7)
        {
            text = text.Substring(0, 7);
        }
        return text;
    }
}
