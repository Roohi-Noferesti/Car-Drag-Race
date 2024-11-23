using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionLoadingScreen : HUDScreen 
{
    [SerializeField]
    private Image m_loadingImage;
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

    public float LoadingProgress
    {
        set
        {
            m_loadingImage.fillAmount = Mathf.Lerp(m_minImageFillAmount, m_maxImageFillAmount, value);
            m_niddleTransform.localRotation = Quaternion.Euler(0, 0,
                Mathf.Lerp(m_minNiddleDegree, m_maxNiddleDegree, value));
        }
    }
}
