using System;
using UnityEngine;
using UnityEngine.UI;

public class PrizeCards : MonoBehaviour
{
    public Image[] CardSprites;

    //private int cards;

    private PrizeEntry.eState state;

    public Color Card1VertexColor = Color.white;

    public Color Card2VertexColor = Color.white;

    public Color Card3VertexColor = Color.white;

    //private Color _prevCard1Color = Color.white;

    //private Color _prevCard2Color = Color.white;

    //private Color _prevCard3Color = Color.white;

    public void Update()
    {
        //if (this.Card1VertexColor != this._prevCard1Color)
        //{
        //    this.CardSprites[0].SetColor(this.Card1VertexColor);
        //    this._prevCard1Color = this.Card1VertexColor;
        //}
        //if (this.Card2VertexColor != this._prevCard2Color)
        //{
        //    this.CardSprites[1].SetColor(this.Card2VertexColor);
        //    this._prevCard2Color = this.Card2VertexColor;
        //}
        //if (this.Card3VertexColor != this._prevCard3Color)
        //{
        //    this.CardSprites[2].SetColor(this.Card3VertexColor);
        //    this._prevCard3Color = this.Card3VertexColor;
        //}
    }

    public void SetState(PrizeEntry.eState state)
    {
        this.state = state;
        if (state == PrizeEntry.eState.WonAlready)
        {
            this.SetNumCards(0);
        }
    }

    public void SetNumCards(int cards)
    {
        //this.cards = cards;
        for (int i = 0; i < this.CardSprites.Length; i++)
        {
            if (i >= cards)
            {
                //this.CardSprites[i].Hide(true);
                this.CardSprites[i].gameObject.SetActive(false);
            }
        }
    }

    private string GetAnimationName()
    {
        if (this.state == PrizeEntry.eState.ToWin)
        {
            return "RespectRankingScreen_CardToWin";
        }
        return "RespectRankingScreen_WinCard";
    }

    public void Anim_Initialize()
    {
        //if (this.cards != 0 && this.state != PrizeEntry.eState.WonAlready)
        //{
        //    AnimationUtils.PlayFirstFrame(base.animation, this.GetAnimationName());
        //}
        //base.animation.Sample();
        //this.Update();
    }

    public void Anim_Play()
    {
        //if (this.cards != 0 && this.state != PrizeEntry.eState.WonAlready)
        //{
        //    AnimationUtils.PlayAnim(base.animation, this.GetAnimationName());
        //}
    }

    public void Anim_Finish()
    {
        //if (this.cards != 0 && this.state != PrizeEntry.eState.WonAlready)
        //{
        //    AnimationUtils.PlayLastFrame(base.animation, this.GetAnimationName());
        //}
    }
}
