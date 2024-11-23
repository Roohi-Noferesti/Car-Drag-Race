using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image)),ExecuteInEditMode]
public class GreenLightIndicator : MonoBehaviour
{
    private Image m_image;
    [SerializeField] private float m_minAngle;
    [SerializeField] private float m_maxAngle;
    [HideInInspector]
    public float MaxRevs;
    [HideInInspector]
    public float IdleRpm;
    public int MaxGaugeRpm = 8000;
    [SerializeField] private float m_offset;
    [SerializeField] private bool m_offsetForLaunch;
    [SerializeField] private float m_bandFractionMultiplier = 1;

    [SerializeField]
    private float minRpm = 4000;
    [SerializeField]
    private float maxRpm = 4500;


    void Awake()
    {
        m_image = GetComponent<Image>();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
            UpdateIndicator(minRpm, maxRpm, false);
    }
#endif

    public void UpdateIndicator(float startRpm, float endRpm,bool isForLaunch)
    {
        var t = startRpm / MaxGaugeRpm;//Mathf.InverseLerp(1000, MaxGaugeRpm, minNormalizedRev);
        var t2 = endRpm * m_bandFractionMultiplier / MaxGaugeRpm;//Mathf.InverseLerp(1000, MaxGaugeRpm, maxNormalizedRev);
        m_image.fillAmount = (t2 - t);
        var angle = t * (m_maxAngle - m_minAngle);
        var realoffset = isForLaunch && m_offsetForLaunch ? 0 : m_offset;
        m_image.transform.localRotation = Quaternion.Euler(0, 0, angle + realoffset);
        //Debug.Log(newMin + "   " + minNormalizedRev + "    " + MaxGaugeRpm + "     " + MaxRevs + "   " + angle);
    }

    public void Reset()
    {
        m_image.fillAmount = 0;
    }
}
