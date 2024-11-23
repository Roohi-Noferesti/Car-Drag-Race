using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEchoController : MonoBehaviour 
{
    private RaceRoadTrigger currentRoadTrigger;
    private AudioReverbFilter m_reverbFilter;
    private int m_roadTriggerIndex;

    void Start()
    {
        m_reverbFilter = GetComponent<AudioReverbFilter>();
        currentRoadTrigger = RaceEnvironmentSettings.Instance.RoadTriggers[m_roadTriggerIndex];
    }
    void Update()
    {
        ApplyRoadBodyLight();
    }
    private void ApplyRoadBodyLight()
    {
        if (currentRoadTrigger != null)
        {
            var targetValue = currentRoadTrigger.Reverb;

            if (m_reverbFilter.enabled != targetValue)
                m_reverbFilter.enabled = targetValue;
        }

        if (RaceController.Instance!=null && RaceController.Instance.Machine.currentId == RaceStateEnum.race && RaceEnvironmentSettings.Instance != null && RaceEnvironmentSettings.Instance.RoadTriggers.Length > m_roadTriggerIndex)
        {
            currentRoadTrigger = RaceEnvironmentSettings.Instance.RoadTriggers[m_roadTriggerIndex];
            if (RaceEnvironmentSettings.Instance.RoadTriggers.Length > m_roadTriggerIndex + 1)
            {
                var nextRoadTrigger = RaceEnvironmentSettings.Instance.RoadTriggers[m_roadTriggerIndex + 1];
                if (transform.position.z > nextRoadTrigger.Position)
                {
                    m_roadTriggerIndex++;
                }
            }
        }
    }
}
