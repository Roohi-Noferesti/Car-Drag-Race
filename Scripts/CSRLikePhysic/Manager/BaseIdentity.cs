public abstract class BaseIdentity
{
    public static readonly BaseIdentity ActivePlatform=new AndroidIdentity();

	public virtual string GetUUID()
	{
        Keychain.Create("uuid");
        string result = Keychain.ReadString(eKeychain.KEYCHAIN_VALUE_DATA);
        Keychain.Destroy();
        return result;
	}

    public virtual string GenerateUDID()
	{
		return "NOT_IMPLEMENTED";
	}

	public virtual string GenerateIdentity()
	{
		return string.Empty;
	}

	public virtual bool ConfirmIdentity(PlayerProfile zProfile)
	{
		if (!string.IsNullOrEmpty(zProfile.UUID))
		{
			if (zProfile.UUID != this.GetUUID())
			{
				return false;
			}
		}
		if (!string.IsNullOrEmpty(zProfile.UDID))
		{
			string b = this.GenerateUDID();
			if (zProfile.UDID != b)
			{
				return false;
			}
		}
		return true;
	}

	public string SaveUUID(string zContents)
	{
        Keychain.Create("uuid");
        Keychain.WriteString(eKeychain.KEYCHAIN_VALUE_DATA, zContents);
        Keychain.Destroy();
		return zContents;
	}

	public virtual bool DoesUUIDExist()
	{
        Keychain.Create("uuid");
        string value = Keychain.ReadString(eKeychain.KEYCHAIN_VALUE_DATA);
        Keychain.Destroy();
        return !string.IsNullOrEmpty(value);
	}

	public virtual string GetDeprecatedUUID()
	{
		return this.GetUUID();
	}
}
