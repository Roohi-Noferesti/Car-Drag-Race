using System;
using System.Collections.Generic;
using UnityEngine;

public class BossPinEffects : MonoBehaviour
{
	public FullScreenFlash Flash;

	public ParticleSystem IntroCentreParticleSystem;

	public ParticleSystem OutroCentreParticleSystem;

	public Animation LensFLareAnimation;

	//public List<SimpleGlow> simpleGlows;

	public Renderer leftCrossEffect;

	public Renderer rightCrossEffect;

	public void SetGlowColourForAnimation(Color glowColor)
	{
		//for (int i = 0; i < this.simpleGlows.Count; i++)
		//{
		//	this.simpleGlows[i].glowColor = glowColor;
		//}
	}

	public void SetCrossTextures(Texture2D crossTexture, Color glowColor)
	{
		if (crossTexture == null)
		{
			return;
		}
		this.leftCrossEffect.material.SetTexture("_MainTex", crossTexture);
		this.rightCrossEffect.material.SetTexture("_MainTex", crossTexture);
		this.leftCrossEffect.material.SetColor("_Tint", glowColor);
		this.rightCrossEffect.material.SetColor("_Tint", glowColor);
	}

	public void PlayIntroCentreParticleSystem()
	{
		this.IntroCentreParticleSystem.gameObject.SetActive(true);
		this.IntroCentreParticleSystem.Play();
	}

	public void PlayOutroCentreParticleSystem()
	{
		this.OutroCentreParticleSystem.gameObject.SetActive(true);
		this.OutroCentreParticleSystem.Play();
	}

	public void PlayFullScreenFlash()
	{
		this.Flash.StartFlashAnimation();
	}

	public void PlayLensFlare()
	{
		AnimationUtils.PlayAnim(this.LensFLareAnimation);
	}
}
