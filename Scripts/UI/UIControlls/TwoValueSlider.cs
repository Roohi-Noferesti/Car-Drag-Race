using UnityEngine;
using UnityEngine.UI;

public class TwoValueSlider : Slider
{
    [SerializeField] private Image m_fill2;
    [SerializeField] private float m_value2;
    public Color color2;

    public override float value
    {
        get { return base.value; }
        set
        {
            base.value = value;
            value2 = value;
        }
    }

    public float value2
    {
        get { return m_value2; }

        set
        {
            var newValue = value;//Mathf.Clamp(value, minValue, maxValue);

            if (newValue != m_value2)
            {
                m_value2 = newValue;
                UpdateValue2();
            }
        }
    }

    //protected override void OnValidate()
    //{
    //    base.OnValidate();
    //    UpdateValue2();
    //}

    protected virtual void UpdateValue2()
    {
        if (m_fill2 == null) return;

        var diff = m_value2 - value;
        m_fill2.rectTransform.sizeDelta = fillRect.sizeDelta;
        m_fill2.rectTransform.pivot = fillRect.pivot;

        var parentRect = (RectTransform)fillRect.parent;

        var parentSize = parentRect.anchorMax.x - parentRect.anchorMin.x;

        var newSize = (diff / (maxValue - minValue)) * parentSize;

        if (diff > 0)
        {
            var max = fillRect.anchorMax;
            var min = fillRect.anchorMin;
            min.x = max.x;

            m_fill2.rectTransform.anchorMin = min;
            max.x += newSize;
            m_fill2.rectTransform.anchorMax = max;
        }
        else
        {
            var max = fillRect.anchorMax;
            var min = fillRect.anchorMin;
            min.x = max.x;

            m_fill2.rectTransform.anchorMax = max;
            min.x += newSize;
            m_fill2.rectTransform.anchorMin = min;
        }
        m_fill2.color = color2;
    }
}
