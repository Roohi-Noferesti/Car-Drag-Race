using UnityEngine;
using System.Collections;

public class RoadBlur : MonoBehaviour
{
    private Material m_material;
    private static float m_maxSpeed = 160;
    void Awake()
    {
        m_material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (CompetitorManager.Instance.LocalCompetitor != null
            && CompetitorManager.Instance.LocalCompetitor.CarPhysics!=null)
        {
            var speed = CompetitorManager.Instance.LocalCompetitor.CarPhysics.SpeedKPH;
            m_material.SetFloat("_mix", Mathf.Min(speed/m_maxSpeed,1));
        }
    }
}
