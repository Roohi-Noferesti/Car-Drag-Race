using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpotPrizeBar : MonoBehaviour
{
	public Image[] ProgressBarBackParts;

    public Image[] ProgressBarParts;

    public Image[] ProgressBarGlowParts;

    public Image ProgressBarMask;

	public TextMeshProUGUI Description;

	public GameObject SpotPrizesParent;

	public GameObject SpotPrizePrefab;

	public SpotPrizeBarBox SpotPrizeBox;

	public float BarTransitionTime;

	public float PinIntroAnimationsDelay;

	[Range(0f, 1f)]
	public float BarWidth = 0.8f;

	[Range(0f, 1f)]
	public float ProgressInterp;

	private MultiplayerEventData multiplayerEvent;

	private float lastSeenProgression;

	private float currentProgression;

	private float maxMilestone;

	private List<SpotPrizePin> spotPrizePins = new List<SpotPrizePin>();

	private Queue<SpotPrizeData> spotPrizeAwardQueue = new Queue<SpotPrizeData>();

	private Image ProgressBarLeft
	{
		get
		{
			return this.ProgressBarParts[0];
		}
	}

	private Image ProgressBarMiddle
	{
		get
		{
			return this.ProgressBarParts[1];
		}
	}

	private Image ProgressBarRight
	{
		get
		{
			return this.ProgressBarParts[2];
		}
	}

	private Image ProgressBarBackLeft
	{
		get
		{
			return this.ProgressBarBackParts[0];
		}
	}

	private Image ProgressBarBackMiddle
	{
		get
		{
			return this.ProgressBarBackParts[1];
		}
	}

	private Image ProgressBarBackRight
	{
		get
		{
			return this.ProgressBarBackParts[2];
		}
	}

	private Image ProgressBarGlowLeft
	{
		get
		{
			return this.ProgressBarGlowParts[0];
		}
	}

	private Image ProgressBarGlowMiddle
	{
		get
		{
			return this.ProgressBarGlowParts[1];
		}
	}

	private Image ProgressBarGlowRight
	{
		get
		{
			return this.ProgressBarGlowParts[2];
		}
	}

	private float ProgressValue
	{
		get
		{
			return Mathf.Lerp(this.lastSeenProgression, this.currentProgression, this.ProgressInterp);
		}
	}

	private float ProgressFrac
	{
		get
		{
			return this.ProgressValue / this.maxMilestone;
		}
	}

	public void Setup(MultiplayerEventData currentMultiplayerEvent, float maxWidth)
	{
		this.multiplayerEvent = currentMultiplayerEvent;
		this.maxMilestone = this.multiplayerEvent.GetMaxMilestone();
		this.currentProgression = MultiplayerEvent.Saved.GetProgression();
		this.lastSeenProgression = MultiplayerEvent.Saved.GetLastSeenProgression();
		this.SetBarWidth(maxWidth);
		this.Description.text = this.multiplayerEvent.PrizeProgression.GetDescription();
		//ColourSwatch swatch = this.multiplayerEvent.Theme.GetSwatch();
		//Image[] progressBarParts = this.ProgressBarParts;
		//for (int i = 0; i < progressBarParts.Length; i++)
		//{
		//	Image spriteBase = progressBarParts[i];
  //          //spriteBase.renderer.material.SetColor("_Color1", swatch.BarColor1);
  //          //spriteBase.renderer.material.SetColor("_Color2", swatch.BarColor2);
		//}
		this.UpdateProgress();
	    float num = 0;//this.ProgressBarParts.Sum((Image p) => p.width);
		float num2 = this.ProgressBarMiddle.transform.localPosition.x - 0.5f * num;
		AnimationClip animationClip = new AnimationClip();
		AnimationClip animationClip2 = new AnimationClip();
		foreach (SpotPrizeData current in this.multiplayerEvent.SpotPrizes)
		{
		    SpotPrizePin spotPrizePin = null;//GameObjectHelper.InstantiatePrefab<SpotPrizePin>(this.SpotPrizePrefab, this.SpotPrizesParent);
			if (current.Milestone > this.lastSeenProgression)
			{
				spotPrizePin.Setup(current, this.multiplayerEvent, SpotPrizePinState.Default);
				if (current.Milestone <= this.currentProgression)
				{
					this.spotPrizeAwardQueue.Enqueue(current);
					animationClip.AddEvent(new AnimationEvent
					{
						functionName = "AnimateSpotPrizeUnlock",
						intParameter = this.spotPrizePins.Count,
						time = this.BarTransitionTime * (current.Milestone - this.lastSeenProgression) / (this.currentProgression - this.lastSeenProgression)
					});
				}
			}
			else
			{
				spotPrizePin.Setup(current, this.multiplayerEvent, SpotPrizePinState.Unlocked);
			}
			float x = num2 + current.Milestone / this.maxMilestone * num;
			spotPrizePin.transform.localPosition = new Vector3(x, 0f, 0f);
			if (!MultiplayerEvent.Saved.HasBeenEntered())
			{
				animationClip2.AddEvent(new AnimationEvent
				{
					functionName = "AnimatePinsIntro",
					intParameter = this.spotPrizePins.Count,
					time = this.PinIntroAnimationsDelay * (float)(this.spotPrizePins.Count + 1)
				});
				spotPrizePin.PrewarmIntroAnimation();
			}
			this.spotPrizePins.Add(spotPrizePin);
		}
        //base.animation.AddClip(animationClip2, "SpotPrizeIntro");
		if (this.spotPrizePins.Count > 0)
		{
			AnimationEvent animationEvent = new AnimationEvent();
		    float a = 0;//this.BarTransitionTime + this.spotPrizePins[0].animation["SpotPrizeUnlock"].clip.length;
		    float length = 0;//base.animation["UpdateShort"].clip.length;
			animationEvent.functionName = "AwardNextSpotPrize";
			animationEvent.time = Mathf.Max(a, length);
			animationClip.AddEvent(animationEvent);
		}
        //base.animation.AddClip(animationClip, "SpotPrizeUnlocks");
        //base.animation["SpotPrizeUnlocks"].layer = 1;
		this.QueueMissingSpotPrizes();
		if (this.currentProgression != this.lastSeenProgression)
		{
			this.AnimateBarUpdate();
		}
		else if (this.spotPrizeAwardQueue.Count > 0)
		{
			this.AwardNextSpotPrize();
		}
		MultiplayerEvent.Saved.SetSeenProgression();
	}

	public void StartIntroAnimation()
	{
        //AnimationUtils.PlayAnim(base.animation, "SpotPrizeIntro");
	}

	private void AnimateBarUpdate()
	{
        //AnimationUtils.PlayAnim(base.animation, "UpdateShort");
        //AnimationUtils.PlayAnim(base.animation, "SpotPrizeUnlocks");
	}

	private void UpdateProgress()
	{
	    float num = 0;//this.ProgressBarParts.Sum((Image p) => p.width);
        //this.ProgressBarMask.SetSize((1f - this.ProgressFrac) * num, this.ProgressBarMask.height);
		string text = this.multiplayerEvent.PrizeProgression.FormatQuantity(this.ProgressValue);
		this.SpotPrizeBox.SetText(text);
		this.SpotPrizeBox.transform.localPosition = new Vector3((this.ProgressFrac - 0.5f) * num, 0f, 0f);
	}

	private void SetBarWidth(float maxWidth)
	{
		float num = this.BarWidth * maxWidth / base.transform.localScale.x;
        //this.ProgressBarMiddle.SetSize(num, this.ProgressBarMiddle.height);
        //this.ProgressBarBackMiddle.SetSize(num, this.ProgressBarBackMiddle.height);
        //this.ProgressBarGlowMiddle.SetSize(num, this.ProgressBarGlowMiddle.height);
		float num2 = 0.5f * num;
		Vector3 localPosition = new Vector3(-num2, 0f, 0f);
		Vector3 localPosition2 = new Vector3(num2, 0f, 0f);
		this.ProgressBarLeft.transform.localPosition = localPosition;
		this.ProgressBarRight.transform.localPosition = localPosition2;
		this.ProgressBarBackLeft.transform.localPosition = localPosition;
		this.ProgressBarBackRight.transform.localPosition = localPosition2;
		this.ProgressBarGlowLeft.transform.localPosition = localPosition;
		this.ProgressBarGlowRight.transform.localPosition = localPosition2;
        //this.ProgressBarMask.transform.localPosition = new Vector3(num2 + this.ProgressBarRight.width, 0f, 0f);
	}

	private void Update()
	{
		this.UpdateProgress();
	}

	private void AnimateSpotPrizeUnlock(int index)
	{
        //AnimationUtils.PlayAnim(this.spotPrizePins[index].animation, "SpotPrizeUnlock");
	}

	private void AwardNextSpotPrize()
	{
		this.multiplayerEvent.AwardNextSpotPrize(this.spotPrizeAwardQueue);
	}

	public void AnimatePinsIntro(int index)
	{
		this.spotPrizePins[index].PlayIntroAnimation();
	}

	private void QueueMissingSpotPrizes()
	{
		MultiplayerEvent.Saved.GetMissingSpotPrizes(new Func<float>(MultiplayerEvent.Saved.GetLastSeenProgression)).ForEachWithIndex(delegate(SpotPrizeData e, int idx)
		{
			this.spotPrizeAwardQueue.Enqueue(e);
		});
	}
}
