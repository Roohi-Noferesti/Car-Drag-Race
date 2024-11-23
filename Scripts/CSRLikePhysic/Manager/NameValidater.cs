public static class NameValidater
{
	public static bool GetIsNameValid(string name)
	{
	    //TextAsset defaultFont = UIManager.instance.defaultFont;
        //SpriteFont font = FontStore.GetFont(defaultFont);
        //return !font.ContainsUnsupportedCharacters(name) && !name.Contains("?");
	    return true;
	}

    public static string GetDisplayableName(string initialName, string uid)
	{
		if (GetIsNameValid(initialName))
		{
			return initialName;
		}
		return uid;
	}

	public static string CreateIdUsername(int id)
	{
		return "user" + id;
	}

	public static string ReplaceUnsupportedCharacters(string name)
	{
	    //TextAsset defaultFont = UIManager.instance.defaultFont;
        //SpriteFont font = FontStore.GetFont(defaultFont);
        //return font.ReplaceUnsupportedCharacters(name, '?');
        return name;
	}

    public static bool CanNameBeDisplayedInCurrentLanguage(string name)
    {
        //TextAsset defaultFont = UIManager.instance.defaultFont;
        //SpriteFont font = FontStore.GetFont(defaultFont);
        //bool result = true;
        //for (int i = 0; i < name.Length; i++)
        //{
        //    char c = name[i];
        //    int num = (int)c;
        //    if (num > 12288 && !font.IsSupportedCharacter(c))
        //    {
        //        result = false;
        //        break;
        //    }
        //}
        //return result;
        return false;
    }
}
