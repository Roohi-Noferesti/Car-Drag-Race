using UnityEngine;
using System.Collections;

public class TimeScaleManager : MonoSingleton<TimeScaleManager>
{
    [Range(0,1)]
    public float TimeScale = 1;
    void Update()
    {
        Time.timeScale = TimeScale;
    }
}
