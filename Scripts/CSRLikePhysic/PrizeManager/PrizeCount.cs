using System;
using TMPro;
using UnityEngine;

public class PrizeCount : MonoBehaviour
{
    public TextMeshProUGUI CountText;

    //public PackedSprite SpriteWon;

    //public PackedSprite SpriteInactive;

    //public global::Sprite SpriteFlare;

    //public PackedSprite SpriteBackgroundDark;

    //public PackedSprite SpriteBackground;

    public Color TextColor = new Color(1f, 1f, 1f, 1f);

    public float LitBarWidth;

    private float _prevBarWidth;

    //private Color _prevTextColor = new Color(1f, 1f, 1f, 1f);

    public void Update()
    {
        //if (this._prevBarWidth != this.LitBarWidth)
        //{
        //    this.SpriteBackground.SetSize(this.LitBarWidth * this.SpriteBackgroundDark.width, this.SpriteBackground.height);
        //    this._prevBarWidth = this.LitBarWidth;
        //}
        //if (this._prevTextColor != this.TextColor)
        //{
        //    this.CountText.SetColor(this.TextColor);
        //    this._prevTextColor = this.TextColor;
        //}
    }

    public void SetText(string text)
    {
        this.CountText.text = text;
    }

    public void SetWon(bool won)
    {
        //if (won)
        //{
        //    this.SpriteWon.Hide(false);
        //    this.SpriteInactive.Hide(true);
        //}
        //else
        //{
        //    this.SpriteWon.Hide(true);
        //    this.SpriteInactive.Hide(false);
        //}
    }

    public void SetFlareColor(Color color)
    {
        //this.SpriteFlare.SetColor(color);
    }

    public void SetBackgroundColor(Color color)
    {
    }
}
