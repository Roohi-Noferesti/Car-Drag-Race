using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class LoadingHelper
{
    private readonly float m_initialvalue;
    [SerializeField] private float[] m_loadingSteps;
    [SerializeField] private float m_loadingSpeed;
    private float m_loadingMaxValue;
    private float m_loadingValue;
    [HideInInspector]
    public bool isLoading;

    public LoadingHelper(float value)
    {
        m_initialvalue = LoadingValue = value;
    }

    public float LoadingValue
    {
        get { return m_loadingValue; }
        set
        {
            m_loadingValue = value;
            var steps = m_loadingSteps.Where(s => s > value).ToArray();
            m_loadingMaxValue = steps.Length > 0 ? steps.Min() : 1;
        }
    }

    public void UpdateStep()
    {
        m_loadingValue += Time.deltaTime * m_loadingSpeed;
        m_loadingValue = Mathf.Clamp(m_loadingValue, 0, m_loadingMaxValue);
    }

    public void Reset()
    {
        LoadingValue = m_initialvalue;
    }
}
