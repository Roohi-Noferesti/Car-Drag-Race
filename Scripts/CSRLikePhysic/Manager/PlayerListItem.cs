using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour, IBundleOwner
{
	public enum ActionType
	{
		Invalid,
		Invite,
		AcceptChallenge,
		Race,
		Avatar
	}

	private PlayerListItem.ActionType _lastUsedActionButtonType;

	public RawImage TopBackground;

    public RawImage BottomBackground;

    public RawImage BackgroundHorizon;

	public TextMeshProUGUI UserName;

	public GameObject GoRaceButtonObj;

	public GameObject YouWonText;

	public GameObject DefeatStrike;

	public GameObject BeatenOverlay;

	public GameObject InfoButton;

	public AnimationCurve ScreenShakeCurve;

	private GUICameraShake CameraShake;

	public GameObject EliteOverlayNode;

	public AvatarPicture Avatar;

	public PrefabPlaceholder CarStatsElemHolder;

	public TextMeshProUGUI CarName;

	public CarSnapshotGeneric Snapshot;

	public Image SnapshotSprite;

	public TextMeshProUGUI RPValue;

	public Renderer[] tintableElements;

	public Renderer[] GameObjectsToTintOnButtonPress;

	public Color PressedColourIntensity;

	public Color RPDeltaIncreaseColour;

	public Color RPDeltaDecreaseColour;

	public RuntimeButton GoRaceButton;

	private GameObject eliteGameObject;

	private Color tierTintColour;

	private CachedOpponentInfo _playerReplay;

	private string introAnimationName = string.Empty;

	private List<Texture> LoadedTextures = new List<Texture>();

	private BaseRuntimeControl.State m_currentState;

    public event OnItemTap OnTap;

    public event OnSlideInAnimEvent OnSlideInAnimHalfWay;

	public PlayerListItem.ActionType LastUsedActionType
	{
		get
		{
			return this._lastUsedActionButtonType;
		}
	}

	public CarStatsElem CarStatsElem
	{
		get
		{
			return this.CarStatsElemHolder.GetBehaviourOnPrefab<CarStatsElem>();
		}
	}

	public PlayerInfo Player
	{
		get
		{
			return this._playerReplay.PlayerReplay.playerInfo;
		}
	}

	public PlayerReplay PlayerReplay
	{
		get
		{
			return this._playerReplay.PlayerReplay;
		}
	}

	public CachedOpponentInfo CachedReplay
	{
		get
		{
			return this._playerReplay;
		}
	}

	public bool SnapshotIsLoaded
	{
		get
		{
			return this.Snapshot.SnapshotIsLoaded;
		}
	}

	private BaseRuntimeControl.State CurrentState
	{
		get
		{
			return this.m_currentState;
		}
		set
		{
			this.m_currentState = value;
			this.ApplyButtonStateAnimation(this.m_currentState == BaseRuntimeControl.State.Pressed);
			this.ApplyButtonStateTint(this.m_currentState == BaseRuntimeControl.State.Pressed);
		}
	}

	public void CreateFromReplay(CachedOpponentInfo zPlayerReplay, bool showCarPP)
	{
		this.Create(zPlayerReplay, showCarPP);
		this.VisualsBasedOnDefeat();
	}

	private void VisualsBasedOnDefeat()
	{
		bool defeated = this._playerReplay.Defeated;
		bool animated = this._playerReplay.Animated;
		this.GoRaceButtonObj.SetActive(!defeated);
		this.YouWonText.SetActive(defeated);
		this.BeatenOverlay.SetActive(defeated);
		if (defeated)
		{
			if (!animated)
			{
				AnimationUtils.PlayAnim(base.GetComponent<Animation>(), "PlayerListItemDefeat");
			}
			else
			{
				this.DefeatStrike.SetActive(true);
				this.DefeatStrike.GetComponent<Renderer>().material.SetFloat("_Greyness", 1f);
				this.DefeatStrike.GetComponent<Renderer>().material.SetColor("_Tint", this.PressedColourIntensity);
			}
		}
		this.InfoButton.SetActive(!defeated);
		if (defeated && !animated)
		{
			StreakManager.SetAnimated(this._playerReplay.EntryID, true);
		}
	}

	public void Create(CachedOpponentInfo zPlayerReplay, bool showCarPP)
	{
		this._playerReplay = zPlayerReplay;
		this.GoRaceButton = this.GoRaceButtonObj.GetComponent<RuntimeButton>();
		this.SetupBackground();
		this.SetUpPlayerInfo();
		this.SetUpCarInfo(showCarPP);
		this.SetupTint();
	}

	private void SetupBackground()
	{
		Texture2D texture2D = (Texture2D)Resources.Load("MultiplayerEvents/Textures/top-dots");
		if (texture2D != null)
		{
			this.TopBackground.texture = (texture2D);
            //this.TopBackground.Setup(this.TopBackground.width, this.TopBackground.height, new Vector2(0f, (float)texture2D.height - 1f), new Vector2((float)texture2D.width, (float)texture2D.height));
			this.LoadedTextures.Add(texture2D);
		}
		Texture2D texture2D2 = (Texture2D)Resources.Load("MultiplayerEvents/Textures/floor");
		if (texture2D2 != null)
		{
            this.BottomBackground.texture = (texture2D2);
            //this.BottomBackground.Setup(this.BottomBackground.width, this.BottomBackground.height, new Vector2(0f, (float)texture2D2.height - 1f), new Vector2((float)texture2D2.width, (float)texture2D2.height));
			this.LoadedTextures.Add(texture2D2);
		}
		Texture2D texture2D3 = (Texture2D)Resources.Load("MultiplayerEvents/Textures/horizon");
		if (texture2D3 != null)
		{
            this.BackgroundHorizon.texture = (texture2D3);
            //this.BackgroundHorizon.Setup(this.BackgroundHorizon.width, this.BackgroundHorizon.height, new Vector2(0f, (float)texture2D3.height - 1f), new Vector2((float)texture2D3.width, (float)texture2D3.height));
			this.LoadedTextures.Add(texture2D3);
		}
	}

	private void SetUpPlayerInfo()
	{
		PlayerReplay playerReplay = this._playerReplay.PlayerReplay;
		this.UserName.text = playerReplay.playerInfo.DisplayName;
		playerReplay.playerInfo.Persona.SetupAvatarPicture(this.Avatar);
		RTWPlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RTWPlayerInfoComponent>();
        this.RPValue.text = CurrencyUtils.GetRankPointsString(component.RankPoints, true, false);
	}

	public void SetUpCarInfo(bool HaveRevealedCarPP)
	{
		PlayerReplay playerReplay = this._playerReplay.PlayerReplay;
		RacePlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
		CarInfo car = CarDatabase.Instance.GetCar(component.CarDBKey);
		int pPIndex = component.PPIndex;
		eCarTier baseCarTier = car.BaseCarTier;
		if (HaveRevealedCarPP)
		{
			this.CarStatsElem.Set(baseCarTier, pPIndex);
			this.CarStatsElem.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
		}
		else
		{
			this.CarStatsElem.Set(baseCarTier, string.Empty);
		}
		string text = LocalizationManager.GetTranslation(car.MediumName);
        this.CarName.text = text;
		if (this.eliteGameObject != null)
		{
			UnityEngine.Object.Destroy(this.eliteGameObject);
			this.eliteGameObject = null;
		}
		if (component.IsEliteCar && HaveRevealedCarPP)
		{
			EliteCarOverlay eliteCarOverlay = EliteCarOverlay.Create(false);
			this.eliteGameObject = eliteCarOverlay.gameObject;
			eliteCarOverlay.Setup(new Vector3(0.21f, -0.11f, -0.05f), this.EliteOverlayNode.transform, 0.3f, 0.32f);
			eliteCarOverlay.transform.localScale = Vector3.one * 0.75f;
		}
		this.Snapshot.Setup(component.CarDBKey, component.AppliedColourIndex, component.AppliedLivery,null);// new Action(this.SnapshotSprite.StartFade));
	}

	private void SetupTint()
	{
		PlayerReplay playerReplay = this._playerReplay.PlayerReplay;
		RacePlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
		CarInfo car = CarDatabase.Instance.GetCar(component.CarDBKey);
		this.tierTintColour = GameDatabase.Instance.Colours.GetTierColour(car.BaseCarTier);
		Renderer[] array = this.tintableElements;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			renderer.material.SetColor("_Tint", this.tierTintColour);
		}
	}

	private void Update()
	{
		if (this.CurrentState == BaseRuntimeControl.State.Active)
		{
			if (this.GoRaceButton.CurrentState == BaseRuntimeControl.State.Active)
			{
				this.CurrentState = BaseRuntimeControl.State.Pressed;
			}
		}
        else if (this.CurrentState == BaseRuntimeControl.State.Pressed && this.GoRaceButton.CurrentState != BaseRuntimeControl.State.Active)
		{
			this.CurrentState = BaseRuntimeControl.State.Active;
		}
	}

	private void ApplyButtonStateAnimation(bool isPressed)
	{
		if (isPressed)
		{
			this.PlayOnTap();
		}
		else
		{
			this.PlayOnRelease();
		}
	}

	private void ApplyButtonStateTint(bool isPressed)
	{
		Color color = (!isPressed) ? this.tierTintColour : (this.tierTintColour * this.PressedColourIntensity);
		Renderer[] gameObjectsToTintOnButtonPress = this.GameObjectsToTintOnButtonPress;
		for (int i = 0; i < gameObjectsToTintOnButtonPress.Length; i++)
		{
			Renderer renderer = gameObjectsToTintOnButtonPress[i];
			renderer.material.SetColor("_Tint", color);
		}
	}

	private void OnRace()
	{
		if (!TouchManager.AttemptToUseButton("OnRaceButton"))
		{
			return;
		}
		this._lastUsedActionButtonType = PlayerListItem.ActionType.Race;
		MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
		if (this.OnTap != null)
		{
			this.OnTap(this);
		}
	}

	private void OnAvatarTap()
	{
		if (!TouchManager.AttemptToUseButton("OnAvatarTapButton"))
		{
			return;
		}
		MultiplayerProfileScreen.ReplayData = this._playerReplay;
		MultiplayerProfileScreen.isOwnProfile = false;
		MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
		AnimationUtils.PlayAnim(base.GetComponent<Animation>(), "PlayerListItemAvatarTap");
		ScreenManager.Instance.PushScreen(ScreenID.MultiplayerProfile);
	}

	public void AssignIntroAnimation(string introAnimation)
	{
		this.introAnimationName = introAnimation;
		if (!string.IsNullOrEmpty(this.introAnimationName))
		{
			this.PrewarmAnimation();
		}
	}

	private void PrewarmAnimation()
	{
		AnimationUtils.PlayFirstFrame(base.GetComponent<Animation>(), this.introAnimationName);
	}

	public void PlayAssignedAnimation()
	{
		AnimationUtils.PlayAnim(base.GetComponent<Animation>(), this.introAnimationName);
	}

	private void PlayOnTap()
	{
		AnimationUtils.PlayAnim(base.GetComponent<Animation>(), "PlayerListItemOnTap");
	}

	private void PlayOnRelease()
	{
		AnimationUtils.PlayAnim(base.GetComponent<Animation>(), "PlayerListItemOnRelease");
	}

	private void AnimEvent_SlideAnimHalfWay()
	{
		if (this.OnSlideInAnimHalfWay != null)
		{
			this.OnSlideInAnimHalfWay(this);
		}
	}

	private void AnimEvent_PlayDefeatedAudio()
	{
		AudioManager.Instance.PlaySound("CrewDefeat", null);
	}

	private void OnDestroy()
	{
		this.LoadedTextures.ForEach(delegate(Texture t)
		{
			if (t != null)
			{
				Resources.UnloadAsset(t);
			}
		});
	}
}
