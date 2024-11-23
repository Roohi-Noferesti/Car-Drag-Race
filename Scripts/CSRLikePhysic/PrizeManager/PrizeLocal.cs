using System;
using TMPro;
using UnityEngine;

public class PrizeLocal : MonoBehaviour
{
    //public PackedSprite LeftBarEnd;

    //public PackedSprite RightBarEnd;

    //public PackedSprite MidBar;

    //public global::Sprite Glow;

    public ParticleSystem Particles;

    public ParticleSystem BankParticles;

    public TextMeshProUGUI PrizeText;

    public float MasterAlpha;

    public float GlowAlpha;

    public float AdditionalWidth;

    //public FlareManager PrizeFlare;

    public float LensFlarePosition;

    public float LensFlareStartX;

    public float LensFlareEndX;

    public float LensFlareAlpha;

    public float LensFlareLerp;

    public AnimationCurve CameraShakeCurve;

    private GUICameraShake cameraShake;

    //private float _prevAlpha = -1f;

    //private float _prevGlowAlpha = -1f;

    private float _prevFlareAlpha;

    private float _prevFlareLerp;

    public void Start()
    {
        //this.PrizeFlare.FadeColorsMultiply(0f);
        //this.PrizeFlare.UpdateColors();
    }

    public void SetPrize(int cash)
    {
        this.PrizeText.text = CurrencyUtils.GetCashString(cash);
    }

    public void SetWidth(float Width)
    {
        //float num = Width + this.AdditionalWidth;
        //float zScaler = num / 2f;
        //float num2 = GameObjectHelper.MakeLocalPositionPixelPerfect(zScaler);
        //this.LeftBarEnd.transform.localPosition = new Vector3(-num2, 0f, 0f);
        //this.RightBarEnd.transform.localPosition = new Vector3(num2, 0f, 0f);
        //this.MidBar.SetSize(num2 * 2f, this.MidBar.height);
    }

    protected void Update()
    {
        //if (this._prevAlpha != this.MasterAlpha || this._prevGlowAlpha != this.GlowAlpha)
        //{
        //    this.Glow.renderer.material.SetColor("_Tint", new Color(1f, 1f, 1f, this.MasterAlpha * this.GlowAlpha));
        //}
        //if (this._prevAlpha != this.MasterAlpha)
        //{
        //    Color color = new Color(1f, 1f, 1f, this.MasterAlpha);
        //    this.LeftBarEnd.SetColor(color);
        //    this.RightBarEnd.SetColor(color);
        //    this.MidBar.SetColor(color);
        //    this.PrizeText.SetColor(new Color(0f, 0f, 0f, this.MasterAlpha));
        //}
        //if (this.LensFlareLerp != this._prevFlareLerp)
        //{
        //    this.PrizeFlare.UpdateAnimation(this.LensFlareStartX + this.LensFlareLerp * (this.LensFlareEndX - this.LensFlareStartX));
        //    this._prevFlareLerp = this.LensFlareLerp;
        //}
        //if (this.LensFlareAlpha != this._prevFlareAlpha)
        //{
        //    this.PrizeFlare.FadeColorsMultiply(this.LensFlareAlpha);
        //    this.PrizeFlare.UpdateColors();
        //    this._prevFlareAlpha = this.LensFlareAlpha;
        //}
    }

    public void Anim_Initialize()
    {
        //AnimationUtils.PlayFirstFrame(base.animation, "RespectRankingScreen_RevealPrizeBar");
        //base.animation.Sample();
        //this.Update();
    }

    private void Anim_Particles()
    {
        this.Particles.Play();
    }

    private void Anim_BankParticles()
    {
        if (this.BankParticles != null)
        {
            this.BankParticles.Play();
        }
    }

    private void Anim_EnableLensFlare()
    {
        //this.PrizeFlare.Show();
    }

    private void Anim_DisableLensFlare()
    {
        //this.PrizeFlare.Hide();
    }

    private void Anim_DoCameraShake()
    {
        //if (this.cameraShake == null)
        //{
        //    this.cameraShake = GUICamera.Instance.gameObject.AddComponent<GUICameraShake>();
        //}
        //this.cameraShake.SetCurve(this.CameraShakeCurve);
        //this.cameraShake.ShakeTime = Time.time;
    }

    public void Anim_Finish()
    {
        this.Anim_DisableLensFlare();
        this.StopCameraShake();
    }

    private void StopCameraShake()
    {
        if (this.cameraShake)
        {
            this.cameraShake.ShakeOver();
        }
    }
}
