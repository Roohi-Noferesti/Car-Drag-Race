using System;
using System.Collections.Generic;
using System.Text;

public class AndroidIdentity : BaseIdentity
{
	public override string GetDeprecatedUUID()
	{
		if (FileUtils.LocalStorageFileExists("UUID"))
		{
			return FileUtils.DecompressFromLocalStorage("UUID", true, false);
		}
		string text = Guid.NewGuid().ToString();
		FileUtils.CompressToLocalStorage("UUID", text, true, false);
		return text;
	}

	public override string GenerateUDID()
	{
		string deviceSerial = BasePlatform.ActivePlatform.GetDeviceSerial();
		byte[] array = new byte[]
		{
			2,
			149,
			150,
			151,
			152,
			18,
			220,
			83,
			137,
			254
		};
		string text = string.Empty;
		for (int i = 0; i < deviceSerial.Length; i++)
		{
			text += (deviceSerial[i] ^ (char)array[i % array.Length]);
		}
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		return Convert.ToBase64String(bytes);
	}

	public override string GenerateIdentity()
	{
		string value = this.GetDeprecatedUUID();
		if (string.IsNullOrEmpty(value))
		{
			value = Guid.NewGuid().ToString();
		}
		string value2 = this.GenerateUDID();
		return JsonConverter.SerializeObject(new Dictionary<string, string>
		{
			{
				"UDID",
				value2
			},
			{
				"UUID",
				value
			}
		}, true);
	}

	private string GetIdent()
	{
        Keychain.Create("uuid");
        string result = Keychain.ReadString(eKeychain.KEYCHAIN_VALUE_DATA);
        Keychain.Destroy();
        return result;
	}

    public override string GetUUID()
	{
		Dictionary<string, string> dictionary = JsonConverter.DeserializeObject<Dictionary<string, string>>(this.GetIdent());
		if (dictionary == null)
		{
			return string.Empty;
		}
		string result;
		if (!dictionary.TryGetValue("UUID", out result))
		{
			return string.Empty;
		}
		return result;
	}

	public override bool ConfirmIdentity(PlayerProfile zProfile)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		string ident = this.GetIdent();
		Dictionary<string, string> dictionary = JsonConverter.DeserializeObject<Dictionary<string, string>>(ident);
		if (!dictionary.TryGetValue("UDID", out empty))
		{
			return false;
		}
		if (!dictionary.TryGetValue("UUID", out empty2))
		{
			return false;
		}
		if (zProfile.UUID != empty2)
		{
			return false;
		}
		string a = this.GenerateUDID();
		return !(a != empty);
	}
}
