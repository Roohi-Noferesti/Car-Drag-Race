public static class Keychain
{
	public static bool IsEnabled()
	{
		return true;
	}

	public static void Log()
	{
		KeychainFake.Log();
	}

	public static void TrashAll()
	{
		KeychainFake.TrashAll();
	}

	public static void TrashAllAndExit()
	{
		KeychainFake.TrashAllAndExit();
	}

	public static void Create(string ident)
	{
		KeychainFake.CreateWrapper(ident);
	}

	public static string ReadString(eKeychain key)
	{
		return KeychainFake.ReadString(key);
	}

	public static void WriteString(eKeychain key, string value)
	{
		KeychainFake.WriteString(key, value);
	}

	public static void Destroy()
	{
		KeychainFake.DestroyWrapper();
	}

	public static string ReadAccount(string ident)
	{
		Create(ident);
		string result = ReadString(eKeychain.KEYCHAIN_VALUE_DATA);
		Destroy();
		return result;
	}

	public static void WriteAccount(string ident, string account)
	{
		Create(ident);
		WriteString(eKeychain.KEYCHAIN_VALUE_DATA, account);
		Destroy();
	}
}
