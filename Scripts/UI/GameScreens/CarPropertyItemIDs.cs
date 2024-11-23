using System;
using System.Collections.Generic;
using Z2HSharedLibrary.DatabaseEntity;

public static class CarPropertyItemIDs
{
    private static List<string> m_bodyShaderIDs;
    private static string[][] m_bodyShaderDivisionIDs;

    private static string[] m_ringShaderIDs;
    private static string[] m_headlightShaderIDs;
    private static string[] m_stickerIDs;
    private static string[] m_spoilerIDs;

    static CarPropertyItemIDs()
    {
        //var carVisuals = SceneManagerGarage.Instance.currentCarVisuals;
        m_bodyShaderIDs = new List<string>();
        m_bodyShaderDivisionIDs = new string[3][];
        m_bodyShaderDivisionIDs[0] = new string[21];
        for (int i = 0; i < 21; i++)
        {
            //var postFix = (i + 1) == 23 ? "_Blur" : String.Empty;
            m_bodyShaderDivisionIDs[0][i] = String.Format("CarBody_Simple_{0}", i + 1);
        }
        
        // for (int i = 0; i < carVisuals.simpleColorList.Count; i++)
        // {
        //     m_bodyShaderDivisionIDs[0][i] = String.Format("CarBody_Simple_{0}", carVisuals.simpleColorList[i]);
        // }

        m_bodyShaderDivisionIDs[1] = new string[22];
        for (int i = 0; i < 22; i++)
        {
            m_bodyShaderDivisionIDs[1][i] = String.Format("CarBody_Matte_{0}", i + 1);
        }
        
        // for (int i = 0; i < carVisuals.matteColorList.Count; i++)
        // {
        //     m_bodyShaderDivisionIDs[1][i] = String.Format("CarBody_Matte_{0}", carVisuals.matteColorList[i]);
        // }

        m_bodyShaderDivisionIDs[2] = new string[18];
        for (int i = 0; i < 18; i++)
        {
            m_bodyShaderDivisionIDs[2][i] = String.Format("CarBody_Custom_{0}", i);
        }
        
        // for (int i = 0; i < carVisuals.customColorList.Count; i++)
        // {
        //     m_bodyShaderDivisionIDs[2][i] = String.Format("CarBody_Custom_{0}", carVisuals.customColorList[i]);
        // }

        foreach (var bodyShaderDivisionID in m_bodyShaderDivisionIDs)
        {
            m_bodyShaderIDs.AddRange(bodyShaderDivisionID);
        }

        m_stickerIDs = new string[60];
        m_stickerIDs[0] = "Sticker_no";
        for (int i = 1; i < 60; i++)
        {
            m_stickerIDs[i] = "Sticker_" + (i <= 19 ? (i - 1) : i);
        }

        m_ringShaderIDs = new string[10];
        for (int i = 0; i < m_ringShaderIDs.Length; i++)
        {
            m_ringShaderIDs[i] = "Ring_" + (i+1);
        }

        m_headlightShaderIDs = new string[8];
        for (int i = 0; i < m_headlightShaderIDs.Length; i++)
        {
            m_headlightShaderIDs[i] = "HeadLight_" + (i+1);
        }

        m_spoilerIDs = new string[8];
        for (int i = 0; i < m_spoilerIDs.Length; i++)
        {
            if (i == 0)
            {
                m_spoilerIDs[i] = "Spoiler_no";
            }
            else
            {
                m_spoilerIDs[i] = "Spoiler_" + i;
            }
        }
    }

    public static string[] BodyShaderIDs
    {
        get
        {
            return m_bodyShaderIDs.ToArray();
        }
    }

    public static string[] GetBodyShaderIDsByDivision(int index)
    {
        return m_bodyShaderDivisionIDs[index];
    }
    
    public static string[] GetSelectedBodyShaderIDsByDivision(int index)
    {
        var carVisuals = SceneManagerGarage.Instance.currentCarVisuals;
        var bodyColor =  m_bodyShaderDivisionIDs[index];
        int colorArrayCount = 0;
        List<string> colorType = new List<string>();
        switch (index)
        {
            case 0:
                colorArrayCount = carVisuals.simpleColorList.Count;
                colorType = carVisuals.simpleColorList;
                break;
            case 1:
                colorArrayCount = carVisuals.matteColorList.Count;
                colorType = carVisuals.matteColorList;
                break;
            case 2:
                colorArrayCount = carVisuals.customColorList.Count;
                colorType = carVisuals.customColorList;
                break;
        }
        string[] newColors = new string[colorArrayCount];
        int i = 0;
        foreach (var color in colorType)
        {
            if (bodyColor.Contains(color))
            {
                newColors[i] = color;
                i++;

            }

        }

        return newColors;
    }

    public static string[] RingShaderIDs
    {
        get
        {
            return m_ringShaderIDs;
        }
    }
    
    public static string[] RingSelectedShaderIDs
    {
        get
        {
            var carVisuals = SceneManagerGarage.Instance.currentCarVisuals;
            string[] newColors = new string[carVisuals.ringShaderList.Count];
            int i = 0;
            foreach (var color in carVisuals.ringShaderList)
            {
                if (m_ringShaderIDs.Contains(color))
                {
                    newColors[i] = color;
                    i++;

                }

            }
            return newColors;
        }
    }

    public static string[] HeadlightShaderIDs
    {
        get
        {
            return m_headlightShaderIDs;
        }
    }
    
    public static string[] HeadlightSelectedShaderIDs
    {
        get
        {
            var carVisuals = SceneManagerGarage.Instance.currentCarVisuals;
            string[] newColors = new string[carVisuals.lightShaderList.Count];
            int i = 0;
            foreach (var color in carVisuals.lightShaderList)
            {
                if (m_headlightShaderIDs.Contains(color))
                {
                    newColors[i] = color;
                    i++;

                }

            }
            return newColors;
        }
    }

    public static string[] StickerIDs
    {
        get
        {
            return m_stickerIDs;
        }
    }
    
    public static string[] StickerSelectedIDs
    {
        get
        {
            var carVisuals = SceneManagerGarage.Instance.currentCarVisuals;
            string[] newStickers = new string[carVisuals.stickerShaderList.Count];
            int i = 0;
            foreach (var sticker in carVisuals.stickerShaderList)
            {
                if (m_stickerIDs.Contains(sticker))
                {
                    newStickers[i] = sticker;
                    i++;

                }

            }
            return newStickers;
        }
    }

    public static string[] SpoilerIDs
    {
        get
        {
            return m_spoilerIDs;
        }
    }
    
    public static string[] SpoilerSelectedIDs
    {
        get
        {
            var carVisuals = SceneManagerGarage.Instance.currentCarVisuals;
            string[] newColors = new string[carVisuals.spoilerShaderList.Count];
            int i = 0;
            foreach (var color in carVisuals.spoilerShaderList)
            {
                if (m_spoilerIDs.Contains(color))
                {
                    newColors[i] = color;
                    i++;

                }

            }
            return newColors;
        }
    }

    public static string[] GetItemsByType(VirtualItemType itemType)
    {
        switch (itemType)
        {
                case VirtualItemType.BodyShader:
                return BodyShaderIDs;
                case VirtualItemType.CarSpoiler:
                return SpoilerIDs;
                case VirtualItemType.CarSticker:
                return StickerIDs;
                case VirtualItemType.HeadLighShader:
                return HeadlightShaderIDs;
                case VirtualItemType.RingShader:
                return RingShaderIDs;
        }
        return null;
    }
}
