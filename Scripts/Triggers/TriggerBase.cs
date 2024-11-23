using System;
using UnityEngine;

public class TriggerBase : MonoBehaviour
{
#if(UNITY_EDITOR)
    [SerializeField] private bool m_debug = false;
#endif

    [SerializeField] private string[] _tags;
    public event Action<Collider> Enter;

    void OnTriggerEnter(Collider other)
    {
        foreach (var s in _tags)
        {
            if (other.CompareTag(s))
            {

#if(UNITY_EDITOR)
                if (m_debug)
                {
                    //var ditance = transform.position.z - LevelController.getPlayerPosition(0).z;
                    //Debug.Log(String.Format("{0} travel {1:0} meter at {2:0.00} seconds", other.name, ditance,
                    //    LevelController.raceTime));
                }
#endif

                OnEnter(other);
            }
        }
    }

    protected virtual void OnEnter(Collider other)
    {
        var handler = Enter;
        if (handler != null) handler(other);
    }
}
