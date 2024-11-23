using System.IO;
using UnityEngine;

public class KeychainFake
{
	private static string m_Ident;

    private static string m_persistentDataPath;

    public static void SetPersistentPath(string path)
    {
        m_persistentDataPath = path;
    }

	public static string GetShortPath()
	{
		ulong num = 14996168890223773720uL;
		if (BasePlatform.ActivePlatform.HasZLIB())
		{
			ulong num2 = 5017305976705350224uL;
			uint num3 = 428u;
			num2 = (num2 << 9) + (ulong)num3;
			return (num ^ num2).ToString();
		}
		ulong num4 = 5657884441222566167uL;
		uint num5 = 1u;
		num4 = (num4 << 9) + (ulong)num5;
		return (num ^ num4).ToString();
	}

	public static string GetPath()
	{
#if UNITY_EDITOR
        m_persistentDataPath = Application.persistentDataPath;
#endif
        //Debug.Log("returning persistent path for : "+ m_persistentDataPath);
        return Path.Combine(m_persistentDataPath, GetShortPath());
	}

	private static string GetFile()
	{
		return GetPath() + "/" + m_Ident;
	}

	public static void CreateWrapper(string ident)
	{
		m_Ident = BasePlatform.ActivePlatform.HMACSHA1_Hash(ident, GetSigningType());
	}

	public static void Log()
	{
	}

	public static void TrashAll()
	{
	}

	public static void TrashAllAndExit()
	{
	}

	public static string ReadString(eKeychain key)
	{
		if (m_Ident == null)
		{
            //Debug.Log("ident is null");
			return string.Empty;
		}
		if (key != eKeychain.KEYCHAIN_VALUE_DATA)
		{
			return string.Empty;
		}
		string file = GetFile();
		string path = file;
		if (!File.Exists(path))
		{
            //Debug.Log("file '"+path+"' not exists");
			return string.Empty;
		}
		string text = string.Empty;
		if (BasePlatform.ActivePlatform.HasZLIB())
		{
			text = BasePlatform.ActivePlatform.ZLIBDecompressTextFromFileX(file);
		}
		else
		{
			using (StreamReader streamReader = File.OpenText(path))
			{
				text = streamReader.ReadToEnd();
				streamReader.Dispose();
			}
		}
        //Debug.Log("read string : " + text);
		int num = text.IndexOf('\n');
		if (num == -1)
		{
            //Debug.Log("num is : " + num);
			return string.Empty;
		}
		string a = text.Substring(0, num);
		text = text.Substring(num + 1);
		string b = BasePlatform.ActivePlatform.HMACSHA1_Hash(text, GetSigningType());
		if (a == b)
		{
			return text;
		}
		return string.Empty;
	}

	private static BasePlatform.eSigningType GetSigningType()
	{
		return BasePlatform.eSigningType.Client_Everything;
	}

	public static void WriteString(eKeychain key, string contents)
	{
		if (m_Ident == null)
		{
			return;
		}
		contents = BasePlatform.ActivePlatform.HMACSHA1_Hash(contents, GetSigningType()) + "\n" + contents;
		if (key == eKeychain.KEYCHAIN_VALUE_DATA)
		{
			Directory.CreateDirectory(GetPath());
			string file = GetFile();
			if (BasePlatform.ActivePlatform.HasZLIB())
			{
				BasePlatform.ActivePlatform.ZLIBCompressTextToFileX(file, contents);
			}
			else
			{
				using (StreamWriter streamWriter = File.CreateText(file))
				{
					streamWriter.Write(contents);
				}
			}
		}
	}

	public static void DestroyWrapper()
	{
		m_Ident = null;
	}
}
