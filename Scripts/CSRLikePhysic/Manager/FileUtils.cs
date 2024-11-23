using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileUtils
{
	private static string m_TemporaryCachePath;
	private static string m_PersistentDataPath;

	private static Dictionary<GTPlatforms, string> editorRoots;

	private static Dictionary<GTPlatforms, string> playerRoots;

	public static string temporaryCachePath
	{
		get
		{
			return FileUtils.m_TemporaryCachePath;
		}
	}

    public static void SetPersistentPath(string persistentDataPath)
    {
        m_PersistentDataPath = persistentDataPath;

    }

    static FileUtils()
	{
#if UNITY_EDITOR
        m_PersistentDataPath = Application.persistentDataPath;
#endif
        FileUtils.m_TemporaryCachePath = Application.temporaryCachePath;
		FileUtils.editorRoots = new Dictionary<GTPlatforms, string>
		{
			{
			    GTPlatforms.OSX,
			    Application.streamingAssetsPath + "/AppDataRoot/"
//				Application.dataPath + "/../BuiltBundles/Android/AppDataRoot/"
			},
			{
			    GTPlatforms.WINDOWS,
			    Application.streamingAssetsPath + "/AppDataRoot/"
//				Application.dataPath + "/../BuiltBundles/Android/AppDataRoot/"
			}
		};
	    FileUtils.playerRoots = new Dictionary<GTPlatforms, string>
	    {
	        {
	            GTPlatforms.iOS,
	            Application.dataPath + "/../Data/Raw/AppDataRoot/"
	        },
	        {
	            GTPlatforms.ANDROID,
	            Application.persistentDataPath + "/AppDataRoot/"
	        },
	        {
	            GTPlatforms.METRO,
	            Application.dataPath + "/AppDataRoot/"
	        }
	    };
		FileUtils.WriteLocalStorage(".nomedia", new byte[0], false);
		string text = Application.persistentDataPath + "/.nomedia";
		FileUtils.EnsureDirectoryExists(text);
		using (FileStream fileStream = new FileStream(text, FileMode.Create))
		{
			using (StreamWriter streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(new byte[0]);
				streamWriter.Close();
			}
		}
	}

	public static void AppendFile(string zLocalPath, string zContent)
	{
		string localStorageFilePath = FileUtils.GetLocalStorageFilePath(zLocalPath);
		using (FileStream fileStream = new FileStream(localStorageFilePath, FileMode.Append))
		{
			using (StreamWriter streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.WriteLine(zContent);
			}
		}
	}

    public static string EnsureChecksum(string content)
	{
		int num = content.IndexOf('\n');
		if (num == -1)
		{
			return null;
		}
		string a = content.Substring(0, num);
		content = content.Substring(num + 1);
		string b = BasePlatform.ActivePlatform.HMACSHA1_Hash(content, BasePlatform.eSigningType.Client_Everything);
		if (a == b)
		{
			return content;
		}
		return null;
	}

    public static string ApplyChecksum(string content)
	{
		return BasePlatform.ActivePlatform.HMACSHA1_Hash(content, BasePlatform.eSigningType.Client_Everything) + "\n" + content;
	}

	public static bool Exists(string path)
	{
		string localStorageFilePath = FileUtils.GetLocalStorageFilePath(path);
		return File.Exists(localStorageFilePath);
	}

	public static string WriteLocalStorage(string zLocalPath, List<string> zStringList, bool addChecksum, bool useDocumentPath = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string current in zStringList)
		{
			stringBuilder.AppendLine(current);
		}
		return FileUtils.WriteLocalStorage(zLocalPath, stringBuilder.ToString(), addChecksum, useDocumentPath);
	}

	private static string EscapePath(string path)
	{
		return path.Replace(" ", "\\ ");
	}

	public static void CompressToLocalStorage(string zLocalPath, string zContent, bool addChecksum, bool useDocumentPath = false)
	{
		if (addChecksum)
		{
			zContent = FileUtils.ApplyChecksum(zContent);
		}
		string text = (!useDocumentPath) ? FileUtils.GetLocalStorageFilePath(zLocalPath) : FileUtils.GetDocumentsFilePath(zLocalPath);
		FileUtils.EnsureDirectoryExists(text);
		if (BasePlatform.ActivePlatform.HasZLIB())
		{
			BasePlatform.ActivePlatform.ZLIBCompressTextToFileX(text, zContent);
		}
		else
		{
			FileUtils.WriteLocalStorage(zLocalPath, Encoding.UTF8.GetBytes(zContent), useDocumentPath);
		}
	}

	public static string DecompressFromLocalStorage(string zLocalPath, bool hasChecksum, bool useDocumentPath = false)
	{
		string path = (!useDocumentPath) ? FileUtils.GetLocalStorageFilePath(zLocalPath) : FileUtils.GetDocumentsFilePath(zLocalPath);
		string text = string.Empty;
		bool hasZlib = BasePlatform.ActivePlatform.HasZLIB();
		if (hasZlib)
		{
			text = BasePlatform.ActivePlatform.ZLIBDecompressTextFromFileX(path);
		}
		else
		{
			byte[] array = null;
			FileUtils.ReadLocalStorage(zLocalPath, ref array, useDocumentPath);
			if (array == null)
			{
				return null;
			}
			text = Encoding.UTF8.GetString(array, 0, array.Length);
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (hasChecksum)
		{
			return FileUtils.EnsureChecksum(text);
		}
		return text;
	}

	public static string WriteLocalStorage(string zLocalPath, string zContent, bool addChecksum, bool useDocumentPath = false)
	{
		string text = (!useDocumentPath) ? FileUtils.GetLocalStorageFilePath(zLocalPath) : FileUtils.GetDocumentsFilePath(zLocalPath);
		FileUtils.EnsureDirectoryExists(text);
		if (addChecksum)
		{
			zContent = FileUtils.ApplyChecksum(zContent);
		}
		using (StreamWriter streamWriter = File.CreateText(text))
		{
			streamWriter.Write(zContent);
			BasePlatform.ActivePlatform.AddSkipBackupAttributeToItem(text);
		}
		return text;
	}

	public static string WriteLocalStorage(string zLocalPath, byte[] zBytes, bool useDocumentPath = false)
	{
		string text = (!useDocumentPath) ? FileUtils.GetLocalStorageFilePath(zLocalPath) : FileUtils.GetDocumentsFilePath(zLocalPath);
		FileUtils.EnsureDirectoryExists(text);
		using (Stream stream = File.Create(text))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(stream))
			{
				binaryWriter.Write(zBytes);
				BasePlatform.ActivePlatform.AddSkipBackupAttributeToItem(text);
			}
		}
		return text;
	}

	public static bool ReadLocalStorage(string zLocalPath, ref string zContent, bool hasChecksum, bool useDocumentPath = false)
	{
		using (Stream stream = FileUtils.OpenFileFromLocalStorage(zLocalPath, true, useDocumentPath))
		{
			if (stream == null)
			{
				bool result = false;
				return result;
			}
			using (StreamReader streamReader = new StreamReader(stream))
			{
				zContent = streamReader.ReadToEnd();
			}
			if (hasChecksum)
			{
				zContent = FileUtils.EnsureChecksum(zContent);
			}
			if (string.IsNullOrEmpty(zContent))
			{
				bool result = false;
				return result;
			}
		}
		return true;
	}

	public static bool ReadLocalStorage(string zLocalPath, ref byte[] zContent, bool useDocumentPath = false)
	{
		using (Stream stream = FileUtils.OpenFileFromLocalStorage(zLocalPath, true, useDocumentPath))
		{
			if (stream == null)
			{
				return false;
			}
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				zContent = binaryReader.ReadBytes((int)stream.Length);
			}
		}
		return true;
	}

	public static void EnsureDirectoryExists(string zDirectory)
	{
		int num = zDirectory.LastIndexOf("/");
		if (num >= 1)
		{
			string path = zDirectory.Substring(0, num);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}

	public static string GetDefaultFilePath(string zFileName)
	{
		string defaultFilePathRootForPlatform = FileUtils.GetDefaultFilePathRootForPlatform();
		string text = defaultFilePathRootForPlatform + zFileName;
        text = text.Replace("\\", "/");
	    return text;
	}

	public static string GetDefaultFilePathRootForPlatform()
	{
		string path = string.Empty;
		if (GTPlatform.IsEditor)
		{
			path = FileUtils.editorRoots[GTPlatform.Runtime];
		}
		else
		{
			path = FileUtils.playerRoots[GTPlatform.Runtime];
		}
		return Path.GetFullPath(path);
	}

	public static string GetLocalStorageFilePath(string zFileName)
	{
		return FileUtils.m_TemporaryCachePath + "/" + zFileName;
	}

	public static string GetDocumentsFilePath(string zFileName)
	{
		return m_PersistentDataPath + "/" + zFileName;
	}

	public static Stream OpenFileFromLocalStorage(string zLocalPath, bool warnIfNotFound = false, bool useDocumentPath = false)
	{
		string text = null;
		if (useDocumentPath && File.Exists(FileUtils.GetDocumentsFilePath(zLocalPath)))
		{
			text = FileUtils.GetDocumentsFilePath(zLocalPath);
		}
		else if (File.Exists(FileUtils.GetLocalStorageFilePath(zLocalPath)))
		{
			text = FileUtils.GetLocalStorageFilePath(zLocalPath);
		}
		else if (File.Exists(FileUtils.GetDefaultFilePath(zLocalPath)))
		{
			text = FileUtils.GetDefaultFilePath(zLocalPath);
		}
		else
		{
#if UNITY_EDITOR

#elif UNITY_ANDROID
            if (AndroidSpecific.APKFileExists(zLocalPath))
            {
                byte[] buffer = AndroidSpecific.LoadAPKFile(zLocalPath);
                Debug.Log("File exists in APK. Length: " + buffer.Length);
                return new MemoryStream(buffer);
            }
            //This is checking patch obb file that we do not apply any more
            //if (AndroidSpecific.OBBFileExists("/assets/AppDataRoot/" + zLocalPath, true))
            //{
            //    byte[] buffer = AndroidSpecific.LoadOBBFile("/assets/AppDataRoot/" + zLocalPath, true);
            //    return new MemoryStream(buffer);
            //}

            //var path = AndroidSpecific.GetObbFullPath(true) +"/assets/AppDataRoot/" + zLocalPath;
            //Debug.Log($"Trying to load from path: {path}");
            if (AndroidSpecific.OBBFileExists("/assets/AppDataRoot/" + zLocalPath, false))
            {
	            byte[] buffer2 = AndroidSpecific.LoadOBBFile("/assets/AppDataRoot/" + zLocalPath, false);
	            return new MemoryStream(buffer2);
            }
#endif
        }
        if (text == null)
		{
			if (warnIfNotFound)
			{
			}
			return null;
		}
		return new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.Read);
	}

	public static void EraseLocalStorageFile(string filePath, bool useDocumentPath = false)
	{
		string path = (!useDocumentPath) ? FileUtils.GetLocalStorageFilePath(filePath) : FileUtils.GetDocumentsFilePath(filePath);
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	public static int EraseLocalStorage()
	{
		return FileUtils.RecursiveFileErase(FileUtils.temporaryCachePath);
	}

    public static int EraseDocumentStorage()
    {
        return FileUtils.RecursiveFileErase(Application.persistentDataPath);
    }

    public static int RecursiveFileErase(string zDir)
	{
		if (!Directory.Exists(zDir))
		{
			return 0;
		}
		List<string> list = new List<string>();
		try
		{
			list.AddRange(Directory.GetDirectories(zDir));
		}
		catch (UnauthorizedAccessException)// var_1_24)
		{
			return 0;
		}
		int num = 0;
		foreach (string current in list)
		{
			num += FileUtils.RecursiveFileErase(current);
		}
		string[] files = Directory.GetFiles(zDir);
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			string path = array[i];
			File.Delete(path);
			num++;
		}
		return num;
	}

	public static bool LocalStorageFileExists(string zLocalPath)
	{
		return File.Exists(FileUtils.GetLocalStorageFilePath(zLocalPath)) || File.Exists(FileUtils.GetDefaultFilePath(zLocalPath)) || File.Exists(FileUtils.GetDocumentsFilePath(zLocalPath));
	}

	public static string[] GetAllFilesInDirectory(string path)
	{
		if (!Directory.Exists(FileUtils.GetLocalStorageFilePath(path)))
		{
			return null;
		}
		return Directory.GetFiles(FileUtils.GetLocalStorageFilePath(path));
	}
}
