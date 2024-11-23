using UnityEngine;
using System.Collections;

public class AB_Device_Screen_Res : MonoBehaviour
{

    public void Res(float Q)
    {
#if UNITY_ANDROID
        Screen.SetResolution((int)(AndroidDevice.OriginalScreenWidth * Q), (int)(AndroidDevice.OriginalScreenHeight* Q), true);
#elif UNITY_IPHONE
        //Screen.SetResolution((int)(AndroidDevice.OriginalScreenWidth * Q), (int)(AndroidDevice.OriginalScreenHeight* Q), true);
        #endif
    }
}
