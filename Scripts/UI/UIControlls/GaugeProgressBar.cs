using UnityEngine;
using UnityEngine.UI;

public class GaugeProgressBar : Slider
{
    [SerializeField] private Image m_fillImage;
    [SerializeField]
    private float m_minNiddleDegree;
    [SerializeField]
    private float m_maxNiddleDegree;
    [SerializeField]
    private float m_minImageFillAmount;
    [SerializeField]
    private float m_maxImageFillAmount;
    [SerializeField]
    private Transform m_niddleTransform;

    protected override void Set(float input, bool sendCallback)
    {
        base.Set(input, sendCallback);

        var normalizeValue = value/(maxValue);

        if (m_fillImage != null)
            m_fillImage.fillAmount = Mathf.Lerp(m_minImageFillAmount, m_maxImageFillAmount, normalizeValue);
        if (m_niddleTransform != null)
            m_niddleTransform.localRotation = Quaternion.Euler(0, 0,
                Mathf.Lerp(m_minNiddleDegree, m_maxNiddleDegree, normalizeValue));
    }

    //protected override void OnValidate()
    //{
    //    base.OnValidate();

    //    if (m_fillImage != null)
    //        m_fillImage.fillAmount = Mathf.Lerp(m_minImageFillAmount, m_maxImageFillAmount, value);
    //    if (m_niddleTransform != null)
    //    m_niddleTransform.localRotation = Quaternion.Euler(0, 0,
    //        Mathf.Lerp(m_minNiddleDegree, m_maxNiddleDegree, value));
    //}
}
