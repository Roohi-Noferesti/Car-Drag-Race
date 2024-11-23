using UnityEngine;
using UnityEngine.UI;

public class BodyShaderCellView : ImageButtonCellView
{
    [SerializeField] private Image m_2ToneCenter;
    [SerializeField] private Image m_2ToneEdge;
    [SerializeField] private Image m_reflectionSharpe;
    [SerializeField] private Image m_reflectionBlur;

    public Color PrimaryColor
    {
        set
        {
            SetImageColor(m_iconImage, value);
            SetImageColor(m_2ToneCenter, value);
        }
    }

    public bool Has2Tone
    {
        set
        {
            SetActiveImage(m_2ToneCenter, value);
            SetActiveImage(m_2ToneEdge, value);
            SetActiveImage(m_iconImage, !value); 
        }
    }

    public Color SecondaryColor
    {
        set { SetImageColor(m_2ToneEdge,value); }
    }

    public bool HasBlurReflection
    {
        set
        {
            SetActiveImage(m_reflectionSharpe, !value);
            SetActiveImage(m_reflectionBlur, value);
        }
    }

    public Color ReflcetionColor
    {
        set
        {
            m_reflectionSharpe.color = value;
            m_reflectionBlur.color = value;
        }
    }

    private void SetActiveImage(Image image,bool value)
    {
        if (image != null)
        {
            image.gameObject.SetActive(value);
        }
    }

    private void SetImageColor(Image image, Color value)
    {
        if (image != null)
        {
            image.color = value;
        }
    }
}
