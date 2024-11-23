using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CarePackagesConfiguration))]
public class CarePackagesConfigurationInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        //if (GUILayout.Button("Load Tim From json"))
        //{
        //    var filePath = EditorUtility.OpenFilePanel("CarPackage Json", Application.persistentDataPath, "txt");

        //    if (!string.IsNullOrEmpty(filePath))
        //    {
        //        var json = File.ReadAllText(filePath);
        //        var loadedCarepackage = JsonConverter.DeserializeObject<CarePackagesConfiguration>(json);
        //        if (loadedCarepackage != null)
        //        {
        //            var carePackageConfigs = target as CarePackagesConfiguration;

        //            foreach (var package in carePackageConfigs.CarePackages)
        //            {
        //                var carePackage = loadedCarepackage.CarePackages.FirstOrDefault(c => c.ID == package.ID);
        //                if (carePackage != null)
        //                {
        //                    foreach (var carePackageLevel in package.CarePackageLevels)
        //                    {
        //                        var level =
        //                            carePackage.CarePackageLevels.FirstOrDefault(l => l.ID == carePackageLevel.ID);

        //                        if (level != null)
        //                        {
        //                            carePackageLevel.InactiveTime = level.InactiveTime;
        //                        }
        //                    }
        //                }
        //            }

        //            EditorUtility.SetDirty(target);
        //        }
        //    }
        //}

        if (GUILayout.Button("Load..."))
        {
            CopyConfigUtils.CopyFromFile<CarePackagesConfiguration>(target);
        }

        if (GUILayout.Button("Load From AssetBundle..."))
        {
            CopyConfigUtils.CopyFromAssetBundle<CarePackagesConfiguration>(target);
        }

        if (GUILayout.Button("Save..."))
        {
            CopyConfigUtils.SaveToFile((CarePackagesConfiguration)target);
        }


        if (GUILayout.Button("Fix character images"))
        {
            var carePackageConfigs = target as CarePackagesConfiguration;

            foreach (var package in carePackageConfigs.CarePackages)
            {
                foreach (var carePackageLevel in package.CarePackageLevels)
                {
                    var image = carePackageLevel.NotificationDetails.InGameNotification.Image;
                    image = image.Replace("Agents/Character_Layer_Lady", "Advisor_Agent.Agent");
                    image = image.Replace("Agents/Character_Layer_OldMan", "Advisor_Race_Official.Race Official");
                    image = image.Replace("Agents/Character_Layer_Boy", "Advisor_Mechanic.Mechanic");
                    if (image.Contains("CharacterCards"))
                        image = image.Replace("CharacterCards/", "");
                    carePackageLevel.NotificationDetails.InGameNotification.Image = image;
                }
            }

            EditorUtility.SetDirty(target);
        }


        if (GUILayout.Button("Debug character images"))
        {
            var carePackageConfigs = target as CarePackagesConfiguration;

            foreach (var package in carePackageConfigs.CarePackages)
            {
                foreach (var carePackageLevel in package.CarePackageLevels)
                {
                    var image = carePackageLevel.NotificationDetails.InGameNotification.Image;
                    Debug.Log(image);
                }
            }

            EditorUtility.SetDirty(target);
        }


        if (GUILayout.Button("Gold x 10"))
        {
            var carePackageConfigs = target as CarePackagesConfiguration;

            foreach (var package in carePackageConfigs.CarePackages)
            {
                foreach (var carePackageLevel in package.CarePackageLevels)
                {
                    foreach (var carePackageReward in carePackageLevel.Rewards)
                    {
                        carePackageReward.Details.Gold *= 10;
                        if (carePackageReward.Details.Gold > 0)
                        {
                            Debug.Log(carePackageLevel.ID + " reward changed to " + carePackageReward.Details.Gold);
                        }
                    }
                }
            }

            EditorUtility.SetDirty(target);
        }


        if (GUILayout.Button("Print Image String"))
        {
            var carePackageConfigs = target as CarePackagesConfiguration;

            foreach (var package in carePackageConfigs.CarePackages)
            {
                foreach (var carePackageLevel in package.CarePackageLevels)
                {
                    if (!string.IsNullOrEmpty(carePackageLevel.NotificationDetails.InGameNotification.Image))
                    {
                        Debug.Log(carePackageLevel.NotificationDetails.InGameNotification.Image);
                    }
                }
            }
        }
        base.OnInspectorGUI();
    }
}
