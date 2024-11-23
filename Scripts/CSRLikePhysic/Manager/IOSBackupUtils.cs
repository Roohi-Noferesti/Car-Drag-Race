using System.IO;
using UnityEngine;

#if UNITY_IPHONE
public class IOSBackupUtils : BaseBackupUtils
{

	public override string GetExternalFolder()
	{
		return destinationPath;
	}
	public override void CreateGTFolder()
	{
        string path = this.GetExternalFolder()+"/GT-Club/";
        Debug.Log("gt folder is : "+path);
		if (!Directory.Exists(path))
		{
			Debug.Log("gt folder is not exists , creating...");
			Directory.CreateDirectory(path);
		}
	}

	public override void CreateSecretFile()
	{
		this.CreateGTFolder();
		var secretFilename = GetSecretFilename();
		Debug.Log("Creating secret file name at : "+secretFilename);
		File.Create(secretFilename);
	}

	public override bool SecretFileExists()
	{
		return File.Exists(GetSecretFilename());
	}
}
#endif
