using System;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class MultiplayerHubScreen : BaseMultiplayerHubScreen
{
	public TextMeshProUGUI ModeTitle;

	public TextMeshProUGUI SeasonInfo;

	private ModeInfo modeInfo;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.MultiplayerHub;
		}
	}

	protected override void ApplyTheme()
	{
		base.ApplyTheme();
		Texture2D texture = Resources.Load(this.currentTheme.Icon) as Texture2D;
        //this.EventIcon.renderer.material.SetTexture("_MainTex", texture);
		base.SetClipPosition();
		this.ModeTitle.text = LocalizationManager.GetTranslation(this.modeInfo.Title);
		this.EventTitle.text = LocalizationManager.GetTranslation(this.currentSeasonEvent.EventTitle);
		this.EventDescription.text = LocalizationManager.GetTranslation(this.currentSeasonEvent.MultiplayerBlurb);
        //this.EventDescription.maxWidth = this.MainPane.Width * 0.8f;
		TexturePack.RequestTextureFromBundle(this.currentSeasonEvent.SeasonCarImageBundle + ".Prize Car", delegate(Texture2D tex)
		{
            //base.SetFadeInSpriteTextureAndResizeToFitAndScale(tex, this.FeatureImage, new Vector2(this.featureImageScale, this.featureImageScale));
		});
		this.SeasonInfo.text = LocalizationManager.GetTranslation(this.currentSeasonEvent.SeasonIntroText);
        //this.SeasonInfo.maxWidth = this.MainPane.Width * 0.95f;
	}

	private void CheckRestrictions()
	{
		if (!this.modeInfo.DoesMeetRestrictions(PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup))
		{
			this.RestrictionPanel.Setup(this.modeInfo);
			this.RestrictionPanel.RestrictionBubble.ShowRestrictionBubbleMessage();
			this.RestrictionPanel.AnimateRestriction();
			this.RaceButton.SetActive(false);
			this.TimeRemaining.gameObject.SetActive(false);
		}
		else
		{
			this.RestrictionPanel.gameObject.SetActive(false);
			this.RaceButton.SetActive(true);
		}
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		base.OnActivate(zAlreadyOnStack);
		MultiplayerMode selectedMultiplayerMode = MultiplayerUtils.SelectedMultiplayerMode;
		if (selectedMultiplayerMode != MultiplayerMode.RACE_THE_WORLD)
		{
			if (selectedMultiplayerMode != MultiplayerMode.PRO_CLUB)
			{
				return;
			}
			this.modeInfo = StreakManager.StreakData.EliteClubInfo;
		}
		else
		{
			this.modeInfo = StreakManager.StreakData.RaceTheWorldInfo;
		}
		int mostRecentActiveSeasonEventID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID();
		this.currentSeasonEvent = GameDatabase.Instance.SeasonEvents.GetEvent(mostRecentActiveSeasonEventID);
		this.currentTheme = this.modeInfo.Theme;
		this.ThemeTexturePackBundleName = this.currentTheme.ThemeTexturePack;
        //this.ApplyTheme();
		if (!base.CheckEntryAllowed())
		{
			return;
		}
		if (!PlayerProfileManager.Instance.ActiveProfile.HasSeenMultiplayerIntroScreen)
		{
			ScreenManager.Instance.PushScreen(ScreenID.MultiplayerUnlock);
            ScreenManager.Instance.UpdateImmediately();
			return;
		}
		this.CheckRestrictions();
		if (this.modeInfo.RPMultiplier > 1f)
		{
			this.RPBonusParent.SetActive(true);
			this.RPBonusMultiplier.text = string.Format("x{0:0}", this.modeInfo.RPMultiplier);
		}
	}

	protected override void Update()
	{
		base.Update();
		string remainingTimeString = SeasonCountdownManager.GetRemainingTimeString(this.currentSeasonEvent.ID, false);
		if (remainingTimeString != this.TimeRemaining.text)
		{
			this.TimeRemaining.text = remainingTimeString;
		}
	}
}
