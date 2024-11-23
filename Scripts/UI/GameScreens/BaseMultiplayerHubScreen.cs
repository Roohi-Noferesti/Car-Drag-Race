using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseMultiplayerHubScreen : ZHUDScreen
{
	public Image EventPaneBackground;

    public Image EventPaneGlow;

    //public AutoPositionStretcher MainPane;

	public GameObject LeaderboardParent;

	public GameObject LeaderboardPrefab;

	public TextMeshProUGUI TimeRemaining;

	public TextMeshProUGUI EventDescription;

	public TextMeshProUGUI EventTitle;

	public Image EventGlow;

    public Image BackgroundSprite;

    public Image BackgroundFloor;

	public Image BackgroundBottom;

    public Image BackgroundGlow;

	public Material AnimatedBackgroundMaterial;

    public Image FeatureImage;

	public GameObject RPBonusParent;

	public TextMeshProUGUI RPBonusMultiplier;

	public Transform PrizeBracketParent;

	public GameObject PrizeBracketsPrefab;

	public GameObject RaceButton;

	public GameObject CarPiecesButton;

    public Image EventIcon;

	public float IconYClipPosition;

	public EventPaneRestrictionPanel RestrictionPanel;

	protected SeasonEventMetadata currentSeasonEvent;

	protected MultiplayerModeTheme currentTheme;

	protected string ThemeTexturePackBundleName = string.Empty;

	public float featureImageScale;

	protected List<Texture> LoadedTextures = new List<Texture>();

	private void OnPrizeList()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (!activeProfile.MultiplayerTutorial_FirstCarPartCompleted)
		{
			List<MultiplayerCarPrize> partiallyWonMultiplayerCars = activeProfile.GetPartiallyWonMultiplayerCars();
			MultiplayerCarPrize multiplayerCarPrize;
			if (partiallyWonMultiplayerCars.Count > 0)
			{
				int maxPieces = partiallyWonMultiplayerCars.Max((MultiplayerCarPrize p) => p.PiecesWon.Count);
				multiplayerCarPrize = partiallyWonMultiplayerCars.First((MultiplayerCarPrize c) => c.PiecesWon.Count == maxPieces);
			}
			else
			{
				multiplayerCarPrize = activeProfile.GetFullyWonMultiplayerCars().First<MultiplayerCarPrize>();
			}
			PrizePieceGiveScreen.OnLoadPrizeGained = multiplayerCarPrize.CarDBKey;
			PrizePieceGiveScreen.PiecesToAward = multiplayerCarPrize.PiecesWon.Count;
			ScreenManager.Instance.PushScreen(ScreenID.PrizePieceGive);
		}
		else
		{
			ScreenManager.Instance.PushScreen(ScreenID.PrizeList);
		}
	}

	private void OnLeaderboard()
	{
		ScreenManager.Instance.PushScreen(ScreenID.Leaderboards);
	}

    public virtual void OnRace()
	{
		StreakManager.ResetOpponentsList();
		ScreenManager.Instance.PushScreen(ScreenID.PlayerList);
	}

	protected virtual void ApplyTheme()
	{
		if (this.currentTheme == null)
		{
			return;
		}
		ColourSwatch swatch = this.currentTheme.GetSwatch();
		this.BackgroundSprite.gameObject.SetActive(false);
		Texture2D texture2D = (Texture2D)Resources.Load("MultiplayerEvents/Textures/top-dots");
		this.LoadedTextures.Add(texture2D);
		if (texture2D != null)
		{
            //this.BackgroundSprite.SetMaterial(this.AnimatedBackgroundMaterial);
			this.SetStaticBackgroundTexture(texture2D, this.BackgroundSprite);
			this.BackgroundSprite.GetComponent<Renderer>().material.SetColor("_Tint", swatch.Primary);
            //this.BackgroundSprite.Setup(GUICamera.Instance.ScreenWidth, GUICamera.Instance.ScreenHeight / 3.8f + CommonUI.Instance.NavBar.GetHeight() / 2f, this.BackgroundSprite.lowerLeftPixel, new Vector2((float)texture2D.width * (GUICamera.Instance.ScreenWidth / ((float)texture2D.width / 200f)), (float)texture2D.height));
		}
        //this.BackgroundSprite.transform.Translate(0f, -CommonUI.Instance.NavBar.GetHeight() - GUICamera.Instance.ScreenHeight / 3.8f, 0f);
		Texture2D texture2D2 = (Texture2D)Resources.Load("MultiplayerEvents/Textures/floor");
		this.LoadedTextures.Add(texture2D2);
		if (texture2D2 != null)
		{
			this.SetImageTexture(texture2D2, this.BackgroundFloor);
            //this.BackgroundFloor.Color = swatch.Primary;
			this.BackgroundFloor.transform.position = this.BackgroundSprite.transform.position;
			this.BackgroundFloor.transform.Translate(0f, 0.01f, 0.1f);
            //this.BackgroundFloor.Setup(GUICamera.Instance.ScreenWidth, GUICamera.Instance.ScreenHeight * 0.4f, this.BackgroundFloor.lowerLeftPixel, this.BackgroundFloor.pixelDimensions);
		}
		Texture2D texture2D3 = (Texture2D)Resources.Load("MultiplayerEvents/Textures/horizon");
		this.LoadedTextures.Add(texture2D3);
		if (texture2D3 != null)
		{
			this.SetImageTexture(texture2D3, this.BackgroundGlow);
            //this.BackgroundGlow.Setup(GUICamera.Instance.ScreenWidth, GUICamera.Instance.ScreenWidth / this.GetAspectRatio(this.BackgroundGlow) * 0.5f, this.BackgroundGlow.lowerLeftPixel, this.BackgroundGlow.pixelDimensions);
			this.BackgroundGlow.transform.position = this.BackgroundSprite.transform.position;
		}
        //this.BackgroundBottom.width = GUICamera.Instance.ScreenWidth;
        //this.BackgroundBottom.height = GUICamera.Instance.ScreenHeight / 4f;
        //this.BackgroundBottom.transform.Translate(0f, -GUICamera.Instance.ScreenHeight, 0f);
		this.EventPaneGlow.GetComponent<Renderer>().material.SetColor("_Tint", swatch.Primary);
        //this.EventGlow.Color = ((!this.currentTheme.Premium) ? swatch.Primary : new Color(0.871f, 0.678f, 0f));
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		this.ShowCarName = true;
		RaceEventInfo.Instance.PopulateFromRaceEvent(MultiplayerUtils.GetEventData(MultiplayerUtils.SelectedMultiplayerMode));
        //this.EventPaneBackground.height = GUICamera.Instance.ScreenHeight;
        //this.EventPaneGlow.height = this.EventPaneBackground.height;
        //this.MainPane.Width = GUICamera.Instance.ScreenWidth - this.EventPaneBackground.width;
        //this.MainPane.Height = GUICamera.Instance.ScreenHeight - CommonUI.Instance.NavBar.GetHeight();
		if (this.LeaderboardParent != null && this.LeaderboardPrefab != null)
		{
			GameObject leaderboard = Instantiate(this.LeaderboardPrefab) as GameObject;
            leaderboard.transform.SetParent(this.LeaderboardParent.transform);
            leaderboard.transform.localPosition = Vector3.zero;
            MapMultiplayerLeaderboard mapMultiplayer = leaderboard.GetComponent<MapMultiplayerLeaderboard>();
            this.AlignCarPiecesButton(mapMultiplayer);
		}
		if (this.PrizeBracketsPrefab != null)
		{
            GameObject prizeBracketsInst = Instantiate(this.PrizeBracketsPrefab) as GameObject;
            prizeBracketsInst.transform.parent = this.PrizeBracketParent;
            prizeBracketsInst.transform.localPosition = Vector3.zero;
            PrizeBrackets prizeBrackets = prizeBracketsInst.GetComponent<PrizeBrackets>();
            prizeBrackets.Setup();
		}
		if (!PlayerProfileManager.Instance.ActiveProfile.HasWonAnyMultiplayerCarPieces())
		{
			this.CarPiecesButton.gameObject.SetActive(false);
		}
	}

	protected void SetStaticBackgroundTexture(Texture2D texture, Image sprite)
	{
        //sprite.renderer.material.SetTexture("_StaticGlowTex", texture);
        //sprite.SetPixelToUV(texture);
        //sprite.StartFade();
		sprite.gameObject.SetActive(true);
	}

	protected void SetImageTexture(Texture2D texture, Image sprite)
	{
		if (sprite != null && texture != null)
		{
            //sprite.SetTexture(texture);
            //sprite.StartFade();
			sprite.gameObject.SetActive(true);
		}
	}

	protected void SetImageTextureAndResizeToFit(Texture2D texture, Image sprite)
	{
		if (sprite != null && texture != null)
		{
			Vector2 lowerleftPixel = new Vector2(0f, (float)texture.height - 1f);
			Vector2 pixeldimensions = new Vector2((float)texture.width, (float)texture.height);
			float w = (float)texture.width / 200f;
			float h = (float)texture.height / 200f;
			sprite.gameObject.SetActive(true);
            //sprite.SetTexture(texture);
            //sprite.Setup(w, h, lowerleftPixel, pixeldimensions);
            //sprite.StartFade();
		}
	}

	protected void SetImageTextureAndResizeToFitAndScale(Texture2D texture, Image sprite, Vector2 scale)
	{
		if (sprite != null && texture != null)
		{
			Vector2 lowerleftPixel = new Vector2(0f, (float)texture.height - 1f);
			Vector2 pixeldimensions = new Vector2((float)texture.width, (float)texture.height);
			float w = (float)texture.width / 200f * scale.x;
			float h = (float)texture.height / 200f * scale.y;
			sprite.gameObject.SetActive(true);
            //sprite.SetTexture(texture);
            //sprite.Setup(w, h, lowerleftPixel, pixeldimensions);
            //sprite.StartFade();
		}
	}

	protected float GetAspectRatio(Image s)
	{
	    //float height = s.height;
        //float width = s.width;
        //return width / height;
	    return 0;
	}

    public void OnRestrictionPress()
	{
		this.RestrictionPanel.NextPressed();
	}

	private void AlignCarPiecesButton(MapMultiplayerLeaderboard leaderboardScript)
	{
		Vector3 position = this.CarPiecesButton.transform.position;
		position.y = leaderboardScript.LeaderboardButton.transform.position.y;
		this.CarPiecesButton.transform.position = position;
	}

	protected void SetClipPosition()
	{
        //float num = (this.IconYClipPosition - CommonUI.Instance.NavBar.GetHeight()) / GUICamera.Instance.ScreenHeight;
        //num = num * 2f + 1f;
        //this.EventIcon.gameObject.renderer.material.SetFloat("_ClipPos", num);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.LoadedTextures.ForEach(delegate(Texture t)
		{
			if (t != null)
			{
				Resources.UnloadAsset(t);
			}
		});
	}

	protected override void Update()
	{
		base.Update();
		int mostRecentActiveSeasonEventID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID();
		if (this.currentSeasonEvent != null && mostRecentActiveSeasonEventID != this.currentSeasonEvent.ID)
		{
			SeasonUtilities.ShowSeasonEndedPopUp(delegate
			{
				ScreenManager.Instance.PopScreen();
				ScreenManager.Instance.UpdateImmediately();
			});
		}
	}

	protected bool CheckEntryAllowed()
	{
		if (UserManager.Instance.currentAccount.IsBanned || PlayerProfileManager.Instance.ActiveProfile.PlayerCarInvalidRTW(string.Empty))
		{
			PopUp popUp = new PopUp();
			popUp.Title = "TEXT_POPUPS_BANNED_TITLE";
			popUp.BodyText = "TEXT_POPUPS_BANNED_TEXT";
			popUp.IsBig = true;
			popUp.CancelAction = delegate
			{
				ScreenManager.Instance.PopScreen();
				Application.OpenURL(GTPlatform.GetPlatformProblemURL());
			};
			popUp.ConfirmAction = new PopUpButtonAction(ScreenManager.Instance.PopScreen);
			popUp.CancelText = "TEXT_BUTTON_REPORT_A_PROBLEM";
			popUp.ConfirmText = "TEXT_BUTTON_OK";
			PopUp popup = popUp;
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
			return false;
		}
		if (MultiplayerUtils.IsAppOutOfDate())
		{
			PopUp popUp = new PopUp();
			popUp.Title = "TEXT_POPUPS_NEEDUPDATES_TITLE";
			popUp.BodyText = LocalizationManager.GetTranslation("TEXT_POPUPS_NEEDUPDATES_BODY");
			popUp.IsBig = true;
			popUp.CancelAction = new PopUpButtonAction(ScreenManager.Instance.PopScreen);
			popUp.ConfirmAction = delegate
			{
				Application.OpenURL(GTPlatform.GetPlatformUpdateURL());
				ScreenManager.Instance.PopScreen();
			};
			popUp.CancelText = "TEXT_BUTTON_IGNORE";
            popUp.ConfirmText = LocalizationManager.GetTranslation("TEXT_BUTTON_GOTO_APP_STORE");
			PopUp popup2 = popUp;
			PopUpManager.Instance.TryShowPopUp(popup2, PopUpManager.ePriority.System, null);
			return false;
		}
		if (!RTWStatusManager.NetworkStateValidToEnterRTW())
		{
			PopUp popup3 = new PopUp
			{
				Title = "TEXT_WEB_REQUEST_STATUS_CODE_0",
				BodyText = "TEXT_CONNECT_SCREEN_INFO_ERROR_CONNECTING_TO_SERVICE",
				IsBig = true,
				ConfirmAction = new PopUpButtonAction(ScreenManager.Instance.PopScreen),
				ConfirmText = "TEXT_BUTTON_OK",
                ImageCaption = "TEXT_NAME_AGENT",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab
            };
			PopUpManager.Instance.TryShowPopUp(popup3, PopUpManager.ePriority.System, null);
		}
		else
		{
			if (!MultiplayerUtils.IsMultiplayerUnlocked())
			{
				PopUpDatabase.Common.ShowMultiplayerLockedPopup(delegate
				{
					ScreenManager.Instance.PopToOrPushScreen(ScreenID.Workshop);
				});
				return false;
			}
			if (SeasonServerDatabase.Instance.GetMostRecentActiveSeasonEventID() == -1)
			{
				SeasonUtilities.ShowSeasonEndedPopUp(new PopUpButtonAction(ScreenManager.Instance.PopScreen));
				return false;
			}
		}
		return true;
	}
}
