using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrizeEntry : MonoBehaviour
{
    public enum eState
    {
        WonAlready,
        ToWin,
        Winning,
        LosePrize,
        Inactive
    }

    public TextMeshProUGUI MainText;

    public TextMeshProUGUI TextPrizeRevealed;

    public PrizeCards Cards;

    public PrizeCount Count;

    public float Width;

    //public PackedSprite LeftSpriteOn;

    //public PackedSprite MidSpriteOn;

    //public PackedSprite RightSpriteOn;

    //public PackedSprite LeftSpriteOff;

    //public PackedSprite MidSpriteOff;

    //public PackedSprite RightSpriteOff;

    //public global::Sprite GlowSprite;

    public float BarOnAlpha = 1f;

    public Transform CardNode;

    public Color PrizeTextColor;

    //public FullScreenFlash flash;

    public GameObject BarOnCombined;

    public GameObject BarOffCombined;

    private PrizeEntry.eState state;

    private Color prevPrizeTextColor;

    //private float prevBarOnAlpha = -1f;

    public void Setup(PrizeEntry.eState state, int cash, int cards)
    {
        this.state = state;
        string cashString = CurrencyUtils.GetCashString(cash);
        this.MainText.text = cashString;
        this.TextPrizeRevealed.text = cashString;
        if (state == PrizeEntry.eState.Inactive)
        {
            cards = 0;
        }
        if (state == PrizeEntry.eState.LosePrize)
        {
            this.MainText.text = string.Empty;
        }
        this.Cards.SetNumCards(cards);
        this.Cards.SetState(state);
        //this.UpdateSize();
        //List<GameObject> zObjects = new List<GameObject>
        //{
        //    this.LeftSpriteOn.gameObject,
        //    this.MidSpriteOn.gameObject,
        //    this.RightSpriteOn.gameObject
        //};
        //List<GameObject> zObjects2 = new List<GameObject>
        //{
        //    this.LeftSpriteOff.gameObject,
        //    this.MidSpriteOff.gameObject,
        //    this.RightSpriteOff.gameObject
        //};
        //this.BarOnCombined = MeshCombiner.CombineThenDestroy(zObjects, this.LeftSpriteOn.renderer.sharedMaterial, "GUI", this.LeftSpriteOn.transform.parent);
        //this.BarOffCombined = MeshCombiner.CombineThenDestroy(zObjects2, this.LeftSpriteOff.renderer.sharedMaterial, "GUI", this.LeftSpriteOff.transform.parent);
    }

    public void Update()
    {
        //if (this.PrizeTextColor != this.prevPrizeTextColor)
        //{
        //    this.TextPrizeRevealed.SetColor(this.PrizeTextColor);
        //    this.prevPrizeTextColor = this.PrizeTextColor;
        //}
        //if (this.BarOnAlpha != this.prevBarOnAlpha)
        //{
        //    Color color = this.BarOnCombined.renderer.material.GetColor("_Tint");
        //    color.a = this.BarOnAlpha;
        //    this.BarOnCombined.renderer.material.SetColor("_Tint", color);
        //    this.prevBarOnAlpha = this.BarOnAlpha;
        //}
    }

    public void UpdateSize()
    {
        //float num = this.Width / 2f;
        //if (this.LeftSpriteOn != null)
        //{
        //    this.LeftSpriteOn.transform.localPosition = new Vector3(-num, 0f, 0f);
        //}
        //if (this.LeftSpriteOff != null)
        //{
        //    this.LeftSpriteOff.transform.localPosition = new Vector3(-num, 0f, 0f);
        //}
        //if (this.RightSpriteOn != null)
        //{
        //    this.RightSpriteOn.transform.localPosition = new Vector3(num, 0f, 0f);
        //}
        //if (this.RightSpriteOff != null)
        //{
        //    this.RightSpriteOff.transform.localPosition = new Vector3(num, 0f, 0f);
        //}
        //if (this.MidSpriteOn != null)
        //{
        //    this.MidSpriteOn.SetSize(this.Width, this.MidSpriteOn.height);
        //}
        //if (this.MidSpriteOff != null)
        //{
        //    this.MidSpriteOff.SetSize(this.Width, this.MidSpriteOff.height);
        //}
        //if (this.CardNode != null)
        //{
        //    this.CardNode.localPosition = new Vector3(num, this.CardNode.localPosition.y, this.CardNode.localPosition.z);
        //}
    }

    public void SetRGBTint(Color c)
    {
        //Color color = c;
        //color.a = this.BarOnCombined.renderer.material.GetColor("_Tint").a;
        //this.BarOnCombined.renderer.material.SetColor("_Tint", color);
    }

    public void SetRGBGlowTint(Color c)
    {
        //Color color = c;
        //color.a = this.GlowSprite.renderer.material.GetColor("_Tint").a;
        //this.GlowSprite.renderer.material.SetColor("_Tint", color);
    }

    public void Anim_Initialize()
    {
        //this.Cards.Anim_Initialize();
        //if (this.state == PrizeEntry.eState.Inactive)
        //{
        //    AnimationUtils.PlayFirstFrame(base.animation, "RespectRankingScreen_Prize_ToWin");
        //}
        //else
        //{
        //    AnimationUtils.PlayFirstFrame(base.animation, this.GetAnimation());
        //}
        //base.animation.Sample();
        //this.Update();
    }

    public void Anim_Play()
    {
        //if (this.state == PrizeEntry.eState.Inactive)
        //{
        //    return;
        //}
        //AnimationUtils.PlayAnim(base.animation, this.GetAnimation());
    }

    public void Anim_Finish()
    {
        //if (this.state == PrizeEntry.eState.Inactive)
        //{
        //    return;
        //}
        //this.Cards.Anim_Finish();
        //AnimationUtils.PlayLastFrame(base.animation, this.GetAnimation());
    }

    private void Anim_Cards()
    {
        //this.Cards.Anim_Play();
    }

    private string GetAnimation()
    {
        if (this.state == PrizeEntry.eState.ToWin)
        {
            return "RespectRankingScreen_Prize_ToWin";
        }
        if (this.state == PrizeEntry.eState.Winning)
        {
            return "RespectRankingScreen_Prize_Win";
        }
        if (this.state == PrizeEntry.eState.WonAlready)
        {
            return "RespectRankingScreen_Prize_Won";
        }
        if (this.state == PrizeEntry.eState.LosePrize)
        {
            return "RespectRankingScreen_Prize_Lose";
        }
        return string.Empty;
    }

    private void Anim_FullScreenFlash()
    {
        //this.flash.StartFlashAnimation();
    }
}
