using System;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RPBonusStats : MonoBehaviour, IPersistentUI
{
	private const float BONUS_ANIMATION_LENGTH = 1f;

	private const float TapAnimTime = 0.2f;

	public GameObject Offset;

	public TextMeshProUGUI BonusText;

	public Animation BonusAnimation;

	public Image BonusGlow;

	public bool ButtonEnabled = true;

	[HideInInspector]
	public float BonusGlowAlpha;

	private float targetBonusValue;

	private float lastKnownBonus;

	private float animationCurrent;

	private float animationTargetTime;

	private bool HasTapShrunk;

	private float TapAnimTimer;

	private void Awake()
	{
		Vector3 localPosition = this.Offset.transform.localPosition;
        //localPosition.x = GUICamera.Instance.ScreenWidth / 4f + 0.3f;
		this.Offset.transform.localPosition = localPosition;
		NavBarAnimationManager.Instance.Subscribe(this.Offset);
		this.lastKnownBonus = 0f;
		this.targetBonusValue = 0f;
		this.animationCurrent = 0f;
		this.animationTargetTime = 0f;
	}

	private void Update()
	{
		this.HandleGlowAnim();
		this.HandleTextAnim();
		if (this.HasTapShrunk)
		{
			this.HandleTapAnim();
		}
	}

	private void UpdatePosition()
	{
        //CSRScreenManager.Instance.RescaleUI();
	}

	public void Show(bool zShow)
	{
		if (zShow)
		{
			this.UpdatePosition();
		}
		base.gameObject.SetActive(zShow);
	}

	public void OnScreenChanged(ScreenID newScreen)
	{
		this.SetRPMultiplier(RPBonusManager.NavBarValue(), false);
	}

	public void SetRPMultiplier(float multiplier, bool animate = true)
	{
		if (multiplier == this.targetBonusValue)
		{
			return;
		}
		if (animate && multiplier > this.lastKnownBonus && base.gameObject.activeInHierarchy)
		{
			AnimationUtils.PlayFirstFrame(this.BonusAnimation);
			AnimationUtils.PlayAnim(this.BonusAnimation);
			this.animationCurrent = 0f;
			this.animationTargetTime = 1f;
			this.targetBonusValue = multiplier;
		}
		else
		{
			this.targetBonusValue = multiplier;
			this.lastKnownBonus = this.targetBonusValue;
			this.SetRPMultiplierText(this.targetBonusValue);
		}
	}

	private void OnRPBonusTap()
	{
		if (!this.ButtonEnabled)
		{
			return;
		}
		PlayerProfileManager.Instance.ActiveProfile.HasSeenRPBonusPopup = true;
		this.Offset.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
		this.HasTapShrunk = true;
		this.TapAnimTimer = 0f;
		if (!ScreenManager.Instance.IsScreenOnStack(ScreenID.RPBoost))
		{
            ScreenManager.Instance.PushScreen(ScreenID.RPBoost);
		}
		else
		{
            ScreenManager.Instance.PopToScreen(ScreenID.RPBoost);
		}
	}

	private void SetRPMultiplierText(float multiplier)
	{
		string text = "+" + multiplier.ToString("##0.") + "%";
		this.BonusText.text = text;
	}

	private void HandleGlowAnim()
	{
		if (!this.BonusAnimation.isPlaying)
		{
			this.BonusGlowAlpha = 0f;
		}
		Color color = this.BonusGlow.color;
		if (color.a != this.BonusGlowAlpha)
		{
			color.a = this.BonusGlowAlpha;
			this.BonusGlow.color = (color);
		}
	}

	private void HandleTextAnim()
	{
		if (this.targetBonusValue == this.lastKnownBonus)
		{
			return;
		}
		this.animationCurrent += Time.deltaTime;
		if (this.animationCurrent >= this.animationTargetTime)
		{
			this.lastKnownBonus = this.targetBonusValue;
			this.SetRPMultiplierText(this.targetBonusValue);
		}
		else
		{
			float t = this.animationCurrent / this.animationTargetTime;
			float rPMultiplierText = Mathf.Lerp(this.lastKnownBonus, this.targetBonusValue, t);
			this.SetRPMultiplierText(rPMultiplierText);
		}
	}

	private void HandleTapAnim()
	{
		this.TapAnimTimer += Time.deltaTime;
		if (this.TapAnimTimer >= 0.2f)
		{
			this.HasTapShrunk = false;
			this.Offset.transform.localScale = Vector3.one;
		}
	}
}
