using UnityEngine;

public abstract class BaseBackupUtils
{
    public static readonly BaseBackupUtils ActivePlatform;

    protected static string destinationPath = Application.persistentDataPath;

    static BaseBackupUtils()
    {
#if UNITY_EDITOR
        ActivePlatform = new EditorBackupUtils();
#elif UNITY_ANDROID
        ActivePlatform = new AndroidBackupUtils();
#else
        ActivePlatform = new IOSBackupUtils();
#endif
    }

    // public static string GetBackupFilename()
    // {
    //     return BaseBackupUtils.ActivePlatform.GetExternalFolder() + "/GT-Club/GTClubBackup";
    // }
    //
    // public static string GetSecretFilename()
    // {
    //     return BaseBackupUtils.ActivePlatform.GetExternalFolder() + "/GT-Club/.GTClubBackupMeta";
    // }

    public virtual string GetExternalFolder()
    {
        return string.Empty;
    }

    public virtual void CreateGTFolder()
    {
    }

    public virtual void CreatePPFolder()
    {
    }

    public virtual void CreateKeychainFolder()
    {
    }

    public virtual void CreateSecretFile()
    {
    }

    public virtual bool SecretFileExists()
    {
        return true;
    }
}
