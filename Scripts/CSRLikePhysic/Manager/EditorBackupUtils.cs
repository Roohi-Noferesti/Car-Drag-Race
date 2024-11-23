using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EditorBackupUtils : BaseBackupUtils
{
    // public override string GetExternalFolder()
    // {
    //     return destinationPath.Replace("/GT-Club", String.Empty);
    // }

    // public override bool SecretFileExists()
    // {
    //     return File.Exists(GetSecretFilename());
    // }

    // public override void CreateGTFolder()
    // {
    //     string path = this.GetExternalFolder();
    //     if (!Directory.Exists(path))
    //     {
    //         Directory.CreateDirectory(path);
    //     }
    // }
    //
    // public override void CreateSecretFile()
    // {
    //     this.CreateGTFolder();
    //     var secretFilename = GetSecretFilename();
    //     File.Create(secretFilename);
    // }
}
