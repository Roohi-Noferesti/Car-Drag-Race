
using UnityEngine;

public class RaceRoadTrigger:MonoBehaviour
{
    public RaceRoadTriggerType Type;
    public bool Reverb;

    public float Position
    {
        get { return transform.position.z; }
    }
}