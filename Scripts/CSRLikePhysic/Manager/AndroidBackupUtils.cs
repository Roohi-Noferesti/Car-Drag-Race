using System.IO;

#if UNITY_ANDROID
public class AndroidBackupUtils : BaseBackupUtils
{
	// public override string GetExternalFolder()
	// {
	// 	string result;
	// 	if (AndroidSpecific.IsExternalStorageMounted())
	// 	{
	// 		result = AndroidSpecific.GetExternalStorageDirectory();
	// 	}
	// 	else
	// 	{
	// 		result = destinationPath;
	// 	}
	// 	return result;
	// }

	// public override void CreateGTFolder()
	// {
 //        string path = this.GetExternalFolder()+"/GT-Club/";
	// 	if (!Directory.Exists(path))
	// 	{
	// 		Directory.CreateDirectory(path);
	// 	}
	// }

	// public override void CreateSecretFile()
	// {
	// 	this.CreateGTFolder();
	// 	File.Create(GetSecretFilename());
	// }
	//
	// public override bool SecretFileExists()
	// {
	// 	return File.Exists(GetSecretFilename());
	// }
}
#endif
