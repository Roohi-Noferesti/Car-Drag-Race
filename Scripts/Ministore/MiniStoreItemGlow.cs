using UnityEngine;
using UnityEngine.UI;

public class MiniStoreItemGlow : MonoBehaviour
{
	private enum AnimState
	{
		APPEAR,
		HIDE,
		NONE
	}

	public Image GlowCenter;

    public Image GlowLeft;

    public Image GlowRight;

	public Material CenterGlowMat;

	public Material SideGlowMat;

	public Color GlowStartColor;

	public Color GlowEndColor;

	public AnimationCurve GlowInAnimCurve;

	public AnimationCurve GlowOutAnimCurve;

	public float GlowInAnimDuration;

	public float GlowOutAnimDuration;

	public bool ShouldGlowOut;

	private float AnimTimePos;

	private AnimState CurrentState;

	private float GlowAnimDuration;

	public void Create()
	{
		this.GlowCenter.gameObject.SetActive(true);
		this.GlowLeft.gameObject.SetActive(true);
		this.GlowRight.gameObject.SetActive(true);
        //this.GlowCenter.renderer.material = this.CenterGlowMat;
        //this.GlowLeft.renderer.material = this.SideGlowMat;
        //this.GlowRight.renderer.material = this.SideGlowMat;
		this.SetAnimation(AnimState.APPEAR, this.GlowInAnimDuration);
	}

	private void UpdateGlowTint(float GlowInterp, Color StartColor, Color EndColor)
	{
		Color color = StartColor * (1f - GlowInterp) + EndColor * GlowInterp;
		color.a *= GlowInterp;
        ////this.GlowCenter.renderer.material.SetColor("_Tint", color);
        ////this.GlowLeft.renderer.material.SetColor("_Tint", color);
        ////this.GlowRight.renderer.material.SetColor("_Tint", color);
	}

	private void SetAnimation(AnimState State, float Duration)
	{
		this.AnimTimePos = 0f;
		this.CurrentState = State;
		this.GlowAnimDuration = Duration;
	}

	private void Update()
	{
		AnimState currentState = this.CurrentState;
		if (currentState != AnimState.APPEAR)
		{
			if (currentState == AnimState.HIDE)
			{
				if (this.AnimTimePos < this.GlowAnimDuration)
				{
					this.AnimTimePos += ((Time.deltaTime <= 0f) ? Time.fixedDeltaTime : Time.deltaTime);
					float num = this.AnimTimePos / this.GlowAnimDuration;
					float glowInterp = this.GlowOutAnimCurve.Evaluate(1f - Mathf.Min(num, 1f));
					if (num >= 1f)
					{
						this.SetAnimation(AnimState.NONE, 0f);
					}
					this.UpdateGlowTint(glowInterp, this.GlowStartColor, this.GlowEndColor);
				}
			}
		}
		else if (this.AnimTimePos < this.GlowAnimDuration)
		{
			this.AnimTimePos += ((Time.deltaTime <= 0f) ? Time.fixedDeltaTime : Time.deltaTime);
			float num2 = this.AnimTimePos / this.GlowAnimDuration;
			float glowInterp2 = this.GlowInAnimCurve.Evaluate(Mathf.Min(num2, 1f));
			if (num2 >= 1f)
			{
				if (this.ShouldGlowOut)
				{
					this.SetAnimation(AnimState.HIDE, this.GlowOutAnimDuration);
				}
				else
				{
					this.SetAnimation(AnimState.NONE, 0f);
				}
			}
			this.UpdateGlowTint(glowInterp2, this.GlowStartColor, this.GlowEndColor);
		}
	}
}
