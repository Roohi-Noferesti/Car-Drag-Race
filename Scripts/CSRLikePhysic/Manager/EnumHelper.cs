using System;

public static class EnumHelper
{
    public static T FromString<T>(string value)
    {
        return (T)((object)Enum.Parse(typeof(T), value));
    }

    public static bool TryParse<T>(string valueToParse, out T returnValue)
    {
        returnValue = default(T);
        if (valueToParse == null)
        {
            return false;
        }
        if (Enum.IsDefined(typeof(T), valueToParse))
        {
            returnValue = FromString<T>(valueToParse);
            return true;
        }
        return false;
    }

    public static int CountNames<T>()
    {
        return Enum.GetNames(typeof(T)).Length;
    }
}
