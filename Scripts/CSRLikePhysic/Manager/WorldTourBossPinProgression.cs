using DataSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldTourBossPinProgression : MonoBehaviour
{
	public Animation animationEvents;

	private string preIntroClip = "PizzaPreIntro";

	private string introClip = "PizzaIntro";

	private string outroClip = "PizzaOutro";

	public BossPinEffects pinEfffects;

	public WorldTourBossPinPiece[] pinPieces;

	public GameObject CentrePiece;

	private UnityEngine.Vector3 progressPinPosition = UnityEngine.Vector3.zero;

	private GUICameraShake cameraShake;

	public AnimationCurve FirstShakeCurve;

	public float FirstShakeDuration;

	public AnimationCurve SecondShakeCurve;

	public float SecondShakeDuration;

	private Dictionary<int, WorldTourAnimationClips> pieceAnimationClips = new Dictionary<int, WorldTourAnimationClips>
	{
		{
			0,
			new WorldTourAnimationClips("PreIntroTop", "PlayIntroTop", new UnityEngine.Vector3(0f, -0.17f, -0.2f))
		},
		{
			1,
			new WorldTourAnimationClips("PreIntroBot", "IntroBottom", new UnityEngine.Vector3(0f, 0.17f, -0.2f))
		},
		{
			2,
			new WorldTourAnimationClips("PreIntroLeft", "IntroLeft", new UnityEngine.Vector3(0f, 0f, -0.2f))
		},
		{
			3,
			new WorldTourAnimationClips("PreIntroRight", "IntroRight", new UnityEngine.Vector3(0.04f, -0.03f, -0.2f))
		}
	};

	private UnityEngine.Vector3 offset = new UnityEngine.Vector3(2.43f, -1.82f, -1f);

	public void SetPinPiecesUp()
	{
		base.gameObject.SetActive(true);
		this.Setup();
	}

	public void DisablePinPieces()
	{
		base.gameObject.SetActive(false);
	}

	public void SetUpProgressPinPosition(UnityEngine.Vector3 pos)
	{
		//this.progressPinPosition = pos;
		//base.gameObject.transform.position = this.progressPinPosition - this.offset;
	}

	public void SetEventPinForPiece(EventPin eventPin, string SequenceID)
	{
		WorldTourBossPinDetais bossPinDetails = TierXManager.Instance.ThemeDescriptor.BossPinDetails;
		for (int i = 0; i < this.pinPieces.Length; i++)
		{
			if (bossPinDetails.PinDetails[i].SequenceID == SequenceID)
			{
				bossPinDetails.PinDetails[i].EventPin = eventPin;
				return;
			}
		}
	}

	public void SetupForIntro()
	{
		for (int i = 0; i < this.pinPieces.Length; i++)
		{
			this.pinPieces[i].SetupForIntroAnim();
		}
	}

	public void SetupForPreIntro()
	{
		base.gameObject.SetActive(true);
		for (int i = 0; i < this.pinPieces.Length; i++)
		{
			this.pinPieces[i].SetupForPreIntroAnim();
		}
	}

	public void DisablePizzaPieces()
	{
		for (int i = 0; i < this.pinPieces.Length; i++)
		{
			this.pinPieces[i].gameObject.SetActive(false);
		}
	}

	public void SelectAndPlayPizzaIntroAnimation()
	{
		IGameState gameState = new GameStateFacade();
		ThemeCompletionLevel worldTourThemeCompletionLevel = gameState.GetWorldTourThemeCompletionLevel(TierXManager.Instance.CurrentThemeName);
		if (worldTourThemeCompletionLevel == ThemeCompletionLevel.LEVEL_2)
		{
			this.Setup();
			this.SetupForPreIntro();
			this.PlayPizzaPreIntro();
		}
		else if (worldTourThemeCompletionLevel == ThemeCompletionLevel.LEVEL_3)
		{
			this.SetupForIntro();
			this.PlayPizzaIntro();
		}
	}

	private void PlayPizzaPreIntro()
	{
		//TouchManager.DisableGesturesFor(this.animationEvents[this.preIntroClip].length + 0.5f);
		//TouchManager.DisableButtonsFor(this.animationEvents[this.preIntroClip].length + 0.5f);
		//AnimationUtils.PlayAnim(this.animationEvents, this.preIntroClip);
		AudioManager.Instance.PlaySound("WorldTourIntro", null);

        //Add by me to test without animation
        OnPreIntroAnimationFinished();
	}

	private void PlayPizzaIntro()
	{
		//TouchManager.DisableGesturesFor(this.animationEvents[this.introClip].length);
		//TouchManager.DisableButtonsFor(this.animationEvents[this.introClip].length);
		//AnimationUtils.PlayAnim(this.animationEvents, this.introClip);

	    //Add by me to test without animation
        OnIntroAnimationFinished();
    }

    public void PlayOutroAnimation()
	{
		WorldTourBossPinPiece[] array = this.pinPieces;
		//for (int i = 0; i < array.Length; i++)
		//{
		//	WorldTourBossPinPiece worldTourBossPinPiece = array[i];
		//	worldTourBossPinPiece.GetComponent<Renderer>().enabled = false;
		//}
		float num = 1.5f;
		base.gameObject.SetActive(true);
		//TouchManager.DisableGesturesFor(this.animationEvents[this.outroClip].length + num);
		//TouchManager.DisableButtonsFor(this.animationEvents[this.outroClip].length + num);
		//AnimationUtils.PlayAnim(this.animationEvents, this.outroClip);
		AudioManager.Instance.PlaySound("world_tour_outro", null);
	    TierXManager.Instance.Invoke("AwardWorldTourPrizeCar",1);//this.animationEvents.GetClip(this.outroClip).length + num);

        //This is called here because we have not animation in our UI
	    OnOutroFinshed();

	}

	public void PlayIntroForPiece(int index)
	{
		WorldTourAnimationClips worldTourAnimationClips = this.pieceAnimationClips[index];
		this.pinPieces[index].TriggerIntroAnim(worldTourAnimationClips.IntroClip, worldTourAnimationClips.IntroOffset);
	}

	public void PlayPreIntroForPiece(int index)
	{
		this.pinPieces[index].TriggerPreIntroAnim(this.pieceAnimationClips[index].PreIntroClip);
	}

	private void Setup()
	{
		WorldTourBossPinDetais bossPinDetails = TierXManager.Instance.ThemeDescriptor.BossPinDetails;
		UnityEngine.Vector3 a = this.progressPinPosition;
		//base.gameObject.transform.position = a - this.offset;
		IGameState gameState = new GameStateFacade();
		if (TierXManager.Instance.PinTextures.ContainsKey(bossPinDetails.PieceTexture))
		{
			Texture2D texture = TierXManager.Instance.PinTextures[bossPinDetails.PieceTexture];
			for (int i = 0; i < Mathf.Max(bossPinDetails.PinDetails.Length, i); i++)
			{
				this.pinPieces[i].Setup(bossPinDetails.PinDetails[i], gameState, texture);
			}
		}
		this.CentrePiece.transform.localPosition = bossPinDetails.PinDetails[0].EndPosition.AsUnityVector3();
		//this.pinEfffects.SetGlowColourForAnimation(TierXManager.Instance.ThemeDescriptor.GlowColour.AsUnityColor());
		//this.pinEfffects.SetCrossTextures(TierXManager.Instance.ThemeDescriptorPrefab.CrossTexture, TierXManager.Instance.ThemeDescriptor.GlowColour.AsUnityColor());
	}

	public void DoFirstCameraShake()
	{
		this.DoScreenShake(this.FirstShakeCurve, this.FirstShakeDuration);
	}

	public void DoSecondCameraShake()
	{
		this.DoScreenShake(this.SecondShakeCurve, this.SecondShakeDuration);
	}

	private void DoScreenShake(AnimationCurve curve, float duration)
	{
		//if (this.cameraShake == null)
		//{
		//	this.cameraShake = GUICamera.Instance.gameObject.AddComponent<GUICameraShake>();
		//}
		//this.cameraShake.SetCurve(curve);
		//this.cameraShake.ShakeTime = Time.time;
		//this.cameraShake.ShakingDuration = duration;
	}

	public void OnPreIntroAnimationFinished()
	{
		this.IncrementWorldTourThemeCompletionLevel(ThemeCompletionLevel.LEVEL_2);
		TierXManager.Instance.RefreshThemeMap();
	}

	public void OnIntroAnimationFinished()
	{
		this.IncrementWorldTourThemeCompletionLevel(ThemeCompletionLevel.LEVEL_3);
		TierXManager.Instance.RefreshThemeMap();
	}

	public void OnOutroFinshed()
	{
		PlayerProfileManager.Instance.ActiveProfile.SetAnimationCompletedForWorldTourEventID(TierXManager.Instance.ThemeDescriptor.ID, TierXManager.Instance.ThemeDescriptor.OutroAnimFlag);
	}

	private void IncrementWorldTourThemeCompletionLevel(ThemeCompletionLevel level)
	{
		string currentThemeName = TierXManager.Instance.CurrentThemeName;
		IGameState gameState = new GameStateFacade();
		if (gameState.GetWorldTourThemeCompletionLevel(currentThemeName) == level)
		{
			gameState.IncrementWorldTourThemeCompletionLevel(currentThemeName);
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}
	}
}
