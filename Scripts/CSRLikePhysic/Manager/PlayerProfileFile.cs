using System.IO;

public static class PlayerProfileFile
{
	private static string _directory = "pp/";

    public static void SetToDefaultDirectory()
	{
		_directory = "pp/";
	}

	public static void SetToRootDirectory()
	{
		_directory = string.Empty;
	}

	public static void WriteFile(string username, string json, EProfileFileType fileType)
	{
		string zLocalPath = _directory + username;
		FileUtils.CompressToLocalStorage(zLocalPath, json, true, true);
	}

	public static void WriteBinaryFile(string username, byte[] bytes)
	{
		FileUtils.WriteLocalStorage(_directory + username, bytes, true);
	}

	public static bool ProfileFileExists(string username)
	{
		return FileUtils.LocalStorageFileExists(_directory + username);
	}

	public static bool ReadBinaryFile(string username, ref byte[] bytes)
	{
		bool flag = FileUtils.ReadLocalStorage(_directory + username, ref bytes, true);
		if (!flag)
		{
			flag = FileUtils.ReadLocalStorage(_directory + username, ref bytes, false);
		}
		return flag;
	}

	public static bool ReadFile(string username, ref string json, EProfileFileType fileType)
	{
		string zLocalPath = _directory + username;
		json = FileUtils.DecompressFromLocalStorage(zLocalPath, true, true);
		if (json == null)
		{
			json = FileUtils.DecompressFromLocalStorage(zLocalPath, true, false);
		}
		return json != null;
	}

    public static void WriteActiveProfileFile(string json, EProfileFileType fileType)
    {
        WriteFile(_directory, json, fileType);
    }

    public static void EraseTransactionFile()
    {
        string filename = PlayerProfileFile.GetFilename(PlayerProfileFile._directory, EProfileFileType.transaction);
        if (string.IsNullOrEmpty(filename))
        {
            return;
        }
        FileUtils.EraseLocalStorageFile(filename);
    }

    private static string GetFilename(string profileDirectory, EProfileFileType fileType)
    {
        var userID = UserManager.Instance.currentAccount.UserID;
        if (userID==0)
        {
            return null;
        }
        string text = Path.Combine(profileDirectory, userID.ToString());
        switch (fileType)
        {
            case EProfileFileType.nonSecure:
                text = Path.Combine(text, "nsb");
                break;
            case EProfileFileType.secure:
                text = Path.Combine(text, "scb");
                break;
            case EProfileFileType.transaction:
                text = Path.Combine(text, "trb");
                break;
            case EProfileFileType.profileNetworkInfo:
                text = Path.Combine(text, "pni");
                break;
            case EProfileFileType.account:
                text = Path.Combine(text, "acb");
                break;
        }
        return text;
    }

    public static bool ReadActiveProfileFile(ref string json, EProfileFileType fileType)
    {
        return PlayerProfileFile.ReadFile(PlayerProfileFile._directory, ref json, fileType);
    }
}
