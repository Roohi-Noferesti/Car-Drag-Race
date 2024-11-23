using System;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

public class BodyShaderScrollerController : CarPropertyScrollerController
{
    [SerializeField] private BodyShaderColor[] m_colorD1;
    [SerializeField] private BodyShaderColor[] m_colorD2;
    [SerializeField] private BodyShaderColor[] m_colorD3;

    private int m_divisionIndex;

    public void SetDivision(int divisionIndex)
    {
        m_divisionIndex = divisionIndex;
        Reload();
        //m_scroller.ReloadData();
    }

    public override void UpdateCellData(EnhancedScrollerCellView cellView, int dataIndex, int cellIndex)
    {
        var imageCellView = cellView as BodyShaderCellView;
        imageCellView.name = IDs[dataIndex];
        char[] separator = {Convert.ToChar("_")};
        if (Colors.Length > 0 && dataIndex<Colors.Length)
        {
            String[] strlist = IDs[dataIndex].Split(separator, 3);
            int index = Convert.ToInt32(strlist[2]);
            if (strlist[1] != "Custom")
            {
                index = index - 1;
            }
            imageCellView.Has2Tone = Colors[index].Is2Tone;
            imageCellView.HasBlurReflection = Colors[index].IsReflectionBlur;
            imageCellView.PrimaryColor = Colors[index].PrimaryColor;
            imageCellView.SecondaryColor = Colors[index].SecondaryColor;
            imageCellView.ReflcetionColor = Colors[index].ReflectionColor;
        }
    }

    private BodyShaderColor[] Colors
    {
        get
        {
            //Debug.Log("DivisionIndex : " + m_divisionIndex);
            switch (m_divisionIndex)
            {
                case 1:
                    return m_colorD2;
                case 2:
                    return m_colorD3;
                default:
                    return m_colorD1;
            }
        }
    }
}


[Serializable]
public class BodyShaderColor
{
    public bool Is2Tone;
    public bool IsReflectionBlur;
    public Color PrimaryColor = new Color(1, 1, 1, 1);
    public Color SecondaryColor = new Color(1, 1, 1, 1);
    public Color ReflectionColor = new Color(1, 1, 1, 1);
}
