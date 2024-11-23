using DataSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldTourBossPinPiece : MonoBehaviour
{
	public RawImage pinPieceRenderer;

	public Animation completedAnimation;

	public Animation lensFLareAnimation;

	public Animation particlesAnimation;

	public Animation glowAnimation;

	public ParticleSystem particles;

	private EventPin eventPin;

	private GUICameraShake cameraShake;

	public AnimationCurve CameraShakeCurve;

	public float CameraShakeDuration;

	public FullScreenFlash Flash;

	private UnityEngine.Vector3 finalPosition;

	//public List<SimpleGlow> childGlows = new List<SimpleGlow>();

	private string eventIDForPin = string.Empty;

	public void Setup(WorldTourBossPinPieceDetails details, IGameState gameState, Texture2D texture)
	{
		this.finalPosition = details.EndPosition.AsUnityVector3();
		//this.pinPieceRenderer.renderer.enabled = true;
		this.pinPieceRenderer.texture = texture;//.material.SetTexture("_MainTex", texture);
		float num = TierXManager.Instance.PinSchedule.GetProgression(gameState, details.SequenceID).ToPercent();
		if (num >= 1f)
		{
			this.OnPinComplete(this.finalPosition, details.SequenceID);
		}
		else
		{
			base.gameObject.transform.localPosition = details.StartPosition.AsUnityVector3();
		}
		this.eventPin = (details.EventPin as EventPin);
	}

	private void OnPinComplete(UnityEngine.Vector3 endPosition, string eventID)
	{
		this.eventIDForPin = eventID;
		if (PlayerProfileManager.Instance.ActiveProfile.IsAnimationCompletedForWorldTourEventID(TierXManager.Instance.ThemeDescriptor.ID, eventID))
		{
			base.gameObject.transform.localPosition = endPosition;
			return;
		}
		this.PlayCrewMemberDefeatedAnimation();
	}

	public void PlayCrewMemberDefeatedAnimation()
	{
		//TouchManager.DisableButtonsFor(this.completedAnimation.clip.length);
		//TouchManager.DisableGesturesFor(this.completedAnimation.clip.length);
		AudioManager.Instance.PlaySound("WorldTourCrewDefeat", null);

        //This is called here because lack of animation in our UI
	    CheckForPlayOutro();
	    //this.completedAnimation.Play();
	}

	public void DoScreenShake()
	{
		if (this.cameraShake == null)
		{
			//this.cameraShake = GUICamera.Instance.gameObject.AddComponent<GUICameraShake>();
		}
		//this.cameraShake.SetCurve(this.CameraShakeCurve);
		//this.cameraShake.ShakeTime = Time.time;
		//this.cameraShake.ShakingDuration = this.CameraShakeDuration;
	}

	public void PlayParticles(string clipName)
	{
		AnimationUtils.PlayAnim(this.particlesAnimation, clipName);
	}

	public void TriggerBossPinFadeOut()
	{
		this.eventPin.GreyOutBossSprite();
		this.eventPin.CrossAnimation();
	}

	public void PlayLensFlare()
	{
		AnimationUtils.PlayAnim(this.lensFLareAnimation);
	}

	public void PlayFullScreenFlash()
	{
		this.Flash.StartFlashAnimation();
	}

	public void PlayIntroParticle()
	{
		if (!this.particles.gameObject.activeInHierarchy)
		{
			this.particles.gameObject.SetActive(true);
		}
		this.particles.Play();
	}

	public void SetToPosition(UnityEngine.Vector3 pos)
	{
		base.gameObject.transform.localPosition = pos;
	}

	public void DoGlowAnimation(string clipName)
	{
		AnimationUtils.PlayAnim(this.glowAnimation, clipName);
	}

	public void SetupForIntroAnim()
	{
		if (this.eventPin != null)
		{
			this.eventPin.gameObject.SetActive(false);
		}
		base.gameObject.SetActive(true);
		base.gameObject.transform.localPosition = this.finalPosition;
		base.gameObject.transform.localScale = UnityEngine.Vector3.one;
	}

	public void SetupForPreIntroAnim()
	{
		if (this.eventPin != null)
		{
			this.eventPin.gameObject.SetActive(false);
		}
		base.gameObject.transform.localPosition = this.finalPosition;
		base.gameObject.transform.localScale = UnityEngine.Vector3.zero;
	}

	public void TriggerPreIntroAnim(string name)
	{
		TouchManager.DisableButtonsFor(this.completedAnimation[name].length);
		TouchManager.DisableGesturesFor(this.completedAnimation[name].length);
		AnimationUtils.PlayAnim(this.completedAnimation, name);
	}

	public void TriggerIntroAnim(string pinPieceAnimClip, UnityEngine.Vector3 offset)
	{
		this.eventPin.gameObject.SetActive(true);
		base.gameObject.SetActive(true);
		this.eventPin.gameObject.transform.parent = base.gameObject.transform;
		this.eventPin.gameObject.transform.localPosition = offset;
		TouchManager.DisableButtonsFor(this.completedAnimation[pinPieceAnimClip].length);
		TouchManager.DisableGesturesFor(this.completedAnimation[pinPieceAnimClip].length);
		AnimationUtils.PlayAnim(this.completedAnimation, pinPieceAnimClip);
	}

	public void CheckForPlayOutro()
	{
		PlayerProfileManager.Instance.ActiveProfile.SetAnimationCompletedForWorldTourEventID(TierXManager.Instance.ThemeDescriptor.ID, this.eventIDForPin);
		TierXManager.Instance.RefreshThemeMap();
	}

	public void TriggerChildGlow(int index)
	{
		//this.childGlows[index].gameObject.SetActive(true);
		//this.childGlows[index].StartGlow();
	}
}
