using System.Collections.Generic;

public static class PlayerProfileMigration
{
	public static void RunProfileMigrationProcess()
	{
        MigrateIdent();
        List<Account> allAccounts = UserManager.Instance.deviceAccounts.GetAllAccounts();
        allAccounts.ForEach(delegate(Account q)
        {
            MigrateProfile(q.Username);
        });
	    var profile = new PlayerProfile("defaultName");
        profile.CreateDefault();
	    MigrateProfile(profile);
	}

	private static void MigrateIdent()
	{
        string zContents = BaseIdentity.ActivePlatform.GenerateIdentity();
		BaseIdentity.ActivePlatform.SaveUUID(zContents);
		DeleteOKNamedUUIDFile();
	}

	public static void MigrateProfile(string zUsername)
	{
		PlayerProfile playerProfile = new PlayerProfile(zUsername);
        if (!playerProfile.Load(EProfileFileType.account))
		{
			return;
		}
		MigrateProfile(playerProfile);
	}

	public static void MigrateProfile(PlayerProfile zProfile)
	{
        zProfile.UDID = BaseIdentity.ActivePlatform.GenerateUDID();
        zProfile.UUID = BaseIdentity.ActivePlatform.GetUUID();
		zProfile.Save();
		PlayerProfileManager.Instance.SendMetricsForPlayerMigrated(zProfile.Name, zProfile.UDID);
	}

	private static void DeleteOKNamedUUIDFile()
	{
		string text = "UUID";
		if (!FileUtils.LocalStorageFileExists(text))
		{
			return;
		}
		FileUtils.EraseLocalStorageFile(text, false);
	}
}
