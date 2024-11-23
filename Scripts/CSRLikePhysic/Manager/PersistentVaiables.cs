using UnityEngine;

public class PersistentVaiables
{
    public static string Gmail
    {
        get { return PlayerPrefs.GetString("gmil"); }
        set { PlayerPrefs.SetString("gmil", value); }
    }

    public static bool LastSyncUseGmail
    {
        get { return PlayerPrefs.GetInt("lsug") == 1; }
        set { PlayerPrefs.SetInt("lsug", value ? 1 : 0); }
    }

    public static string ImageUrl
    {
        get { return PlayerPrefs.GetString("imur"); }
        set { PlayerPrefs.SetString("imur", value); }
    }

    public static void Clear()
    {
        Gmail = null;
        LastSyncUseGmail = false;
        ImageUrl = null;
    }
}