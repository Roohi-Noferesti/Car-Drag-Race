using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using KingKodeStudio.IAB;
using UnityEngine;
using UnityEngine.Android;

public class PermissionManager : MonoBehaviour
{
    //private static bool m_waitingForPermission;
    //private static bool m_waitingForPopup;
    public PermissionDescriptions[] PermissionDescriptionses;
    private static bool m_waiting;
    private static bool m_waitingForDialog;

    public static PermissionManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

#if UNITY_ANDROID
    public static IEnumerator CheckRequiredPermission()
    {
        //var kinIABSetting = Resources.Load< IABConfig>("IABSetting");

        var systemLanguage = Application.systemLanguage;
        foreach (var permissionDescriptiones in Instance.PermissionDescriptionses)
        {
            if (permissionDescriptiones.Enable &&
                !Permission.HasUserAuthorizedPermission(permissionDescriptiones.Permission))
            {
                yield return new WaitForEndOfFrame();

                GTDebug.Log(GTLogChannel.Android,"permission " + permissionDescriptiones.Permission + " not granted,try to grant");
                m_waitingForDialog = true;
                var title = systemLanguage == SystemLanguage.Unknown
                    ? permissionDescriptiones.Title
                    : permissionDescriptiones.TitleEN;

                var desc = systemLanguage == SystemLanguage.Unknown
                    ? permissionDescriptiones.Description
                    : permissionDescriptiones.DescriptionEN;

                AndroidSpecific.ShowPermissionDialog(permissionDescriptiones.Permission,
                    title, desc);
                while (m_waitingForDialog)
                {
                    yield return new WaitForEndOfFrame();
                }

                //Code will be blocked here
                m_waiting = true;
                Permission.RequestUserPermission(permissionDescriptiones.Permission);

                //while (m_waiting)
                //{
                //    yield return new WaitForEndOfFrame();
                //}
            }
        }
    }
#else
    public static IEnumerator CheckRequiredPermission()
    {
        yield break;
    }
#endif

    public void OnPopupResponse(string permission)
    {
        var permissionDetails = Instance.PermissionDescriptionses.FirstOrDefault(p => p.Permission == permission);
        if (permissionDetails != null)
        {
            permissionDetails.Approved = false;
        }
        m_waitingForDialog = false;
    }

    public void OnPermissionAccepted(string permission)
    {
        var permissionDetails = Instance.PermissionDescriptionses.FirstOrDefault(p => p.Permission == permission);
        if (permissionDetails != null)
        {
            permissionDetails.Approved = true;
        }
        m_waiting = false;
    }

    public void OnPermissionRejected(string permission)
    {
        var permissionDetails = Instance.PermissionDescriptionses.FirstOrDefault(p => p.Permission == permission);
        if (permissionDetails != null)
        {
            permissionDetails.Approved = !permissionDetails.Mandatory;
        }
        m_waiting = false;
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause && m_waitingForDialog)
        {
            m_waitingForDialog = false;
        }
    }
}

[System.Serializable]
public class PermissionDescriptions
{
    public bool Enable;
    public string Permission;
    public string Title;
    public string TitleEN;
    public string Description;
    public string DescriptionEN;
    public bool Mandatory;
    //public bool EnabledWhenInIran;

    public bool Approved { get; set; }
}
