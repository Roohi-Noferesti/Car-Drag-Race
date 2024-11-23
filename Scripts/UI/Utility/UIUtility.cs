using System.Linq;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    public static Vector2 GetWolrdSize(this RectTransform rectTransform)
    {
        var canvas = rectTransform.GetComponentInParent<Canvas>();
        var height = rectTransform.rect.height * canvas.transform.localScale.y *
                     rectTransform.localScale.y;
        var width = rectTransform.rect.width * canvas.transform.localScale.x *
             rectTransform.localScale.x;

        return new Vector2(width, height);
    }

    public static Vector3 GetUpperPoint(this RectTransform rectTransform)
    {
        var height = rectTransform.GetWolrdSize().y;
        return rectTransform.position + Vector3.up*height/2;
    }

    public static Vector3 GetBottomPoint(this RectTransform rectTransform)
    {
        var height = rectTransform.GetWolrdSize().y;
        return rectTransform.position + Vector3.down * height / 2;
    }

    public static void SetText(this Button button,string text)
    {
        var comp = button.GetComponentInChildren<TextMeshProUGUI>();
        if (comp != null)
        {
            comp.text = text;
        }
    }

    public static ScreenID GetScreenID(this HUDScreen screen)
    {
        var zHudScreen = screen as ZHUDScreen;
        if (zHudScreen != null)
        {
            return zHudScreen.ID;
        }
        return ScreenID.Invalid;
    }
}
