using DataSerialization;
using EventPaneRestriction;
using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldTourChoiceItem : MonoBehaviour
{
	public RuntimeTextButton GetCarButton;

	public TextMeshProUGUI ChoiceText;

	public TextMeshProUGUI ValidText;

	public RuntimeButton GoRaceButton;

	//public DataDrivenPortrait Portrait;

	public Image PortraitBackgroundLeft;

	public Image PortraitBackgroundRight;

	public Image CarSnapshotSprite;

	public CarSnapshotRaceOpponent CarSnapshot;

	public TextMeshProUGUI DifficultySpriteT;

	public Image DifficultyGraphic;

	public Image DifficultyShadowGraphic;

	private ScheduledPin m_pin;

	private bool isWorldTourBossCar;

	public GameObject[] GameObjectsToTint;

	private UnityEngine.Color tint = UnityEngine.Color.white;

	public UnityEngine.Color Tint
	{
		get
		{
			return this.tint;
		}
		set
		{
			if (this.tint != value)
			{
				this.tint = value;
				GameObject[] gameObjectsToTint = this.GameObjectsToTint;
				for (int i = 0; i < gameObjectsToTint.Length; i++)
				{
					//GameObject gameObject = gameObjectsToTint[i];
					//gameObject.renderer.material.SetColor("_Tint", this.tint);
				}
			}
		}
	}

	private RaceEventData GetEvent()
	{
		PinDetail pin = TierXManager.Instance.GetPin(this.m_pin);
		return GameDatabase.Instance.Career.GetEventByEventIndex(pin.EventID);
	}

	private static IRestriction GetRestriction(RaceEventData raceEvent)
	{
		IRestriction activeRestriction = RestrictionHelper.GetActiveRestriction(raceEvent);
		if (activeRestriction is SpecificCarsRequired)
		{
			IEnumerable<CarGarageInstance> compatibleCars = raceEvent.GetCompatibleCars();
			if (compatibleCars != null && compatibleCars.Any<CarGarageInstance>())
			{
				return null;
			}
		}
		return activeRestriction;
	}

	public static bool IsRestrictedChoice(RaceEventData raceEvent)
	{
		IRestriction restriction = WorldTourChoiceItem.GetRestriction(raceEvent);
		return restriction != null && !(restriction is GasRequired);
	}

	private void SetUpCarInfo(bool alreadyChosen)
	{
		RaceEventData @event = this.GetEvent();
		CarInfo car = CarDatabase.Instance.GetCar(@event.AICar);
		this.ValidText.text = LocalizationManager.GetTranslation(car.MediumName).ToUpper();
		this.isWorldTourBossCar = this.IsWorldTourBossCar();
		if (WorldTourChoiceItem.IsRestrictedChoice(this.GetEvent()))
		{
			if (this.isWorldTourBossCar)
			{
				this.GetCarButton.SetText("TEXT_BUTTON_" + @event.AIDriverCrew, false, true);
				this.DifficultySpriteT.text = "\n" + LocalizationManager.GetTranslation("TEXT_WIN_IN_WT");
			}
			else
			{
				this.GetCarButton.SetText("TEXT_BUTTON_GET_CAR", false, true);
				this.DifficultySpriteT.text = LocalizationManager.GetTranslation("TEXT_GET_CAR_DESCRIPTION");
				//if (LocalisationManager.GetSystemLanguage() == LocalisationManager.ISO6391.FR)
				//{
				//	this.GetCarButton.GetTextSprite().SetCharacterSize(this.GetCarButton.GetTextSprite().characterSize - 0.02f);
				//	this.GetCarButton.ForceAwake();
				//}
			}
			this.DifficultySpriteT.gameObject.SetActive(true);
			this.DifficultyGraphic.gameObject.SetActive(false);
			this.DifficultyShadowGraphic.gameObject.SetActive(false);
		}
		else if (!alreadyChosen)
		{
			this.SetupDifficultyRating();
			this.GetCarButton.gameObject.SetActive(false);
			this.GoRaceButton.gameObject.SetActive(true);
		}
	}

	private bool IsWorldTourBossCar()
	{
		CarInfo car = CarDatabase.Instance.GetCar(this.GetEvent().AICar);
		if (!car.IsAvailableToBuyInShowroom() && !string.IsNullOrEmpty(car.PrizeInfoText) && !string.IsNullOrEmpty(car.PrizeInfoButton))
		{
			string[] array = car.PrizeInfoButton.Split(new char[]
			{
				','
			});
			if (array[1].Contains("OnGoWorldTourHub"))
			{
				return true;
			}
		}
		return false;
	}

	public void Setup(ScheduledPin pin)
	{
		this.m_pin = pin;
		//this.GetCarButton.ForceAwake();
		this.GetCarButton.Show(true);
		this.CarSnapshot.Setup(this.GetEvent(), delegate
		{
			//this.CarSnapshotSprite.StartFade();
		});
		IGameState gameState = new GameStateFacade();
		bool flag = gameState.GetWorldTourRaceResultCount(TierXManager.Instance.CurrentThemeName, pin.ParentSequence.ID, pin.ID, true) > 0;
		if (flag)
		{
			this.GetCarButton.CurrentState = BaseRuntimeControl.State.Disabled;
			this.GetCarButton.SetText("TEXT_BUTTON_DEFEATED", false, true);
			//this.ValidText.SetColor(UnityEngine.Color.gray);
		}
		this.ChoiceText.text = LocalizationManager.GetTranslation(this.GetEvent().EventName);
		this.SetUpCarInfo(flag);
		if (this.GetEvent().IsAIDriverAvatarAvailable())
		{
			string text = this.GetEvent().AIDriverCrew;
			if (text.EndsWith("_Crew") && this.Tint == UnityEngine.Color.white)
			{
				text = text.Substring(0, text.LastIndexOf("_Crew"));
				this.Tint = GameDatabase.Instance.Colours.GetTierColour(text);
			}
			//this.Portrait.Init(this.GetEvent().AIDriverCrew + ".Portrait" + this.GetEvent().AIDriverCrewNumber, string.Empty, null);
			this.PortraitBackgroundLeft.gameObject.SetActive(false);
			this.PortraitBackgroundRight.gameObject.SetActive(false);
			TexturePack.RequestTextureFromBundle(this.GetEvent().AIDriverCrew + ".Background", delegate(Texture2D texture)
			{
				//this.PortraitBackgroundLeft.SetTexture(texture);
				//this.PortraitBackgroundRight.SetTexture(texture);
				//this.PortraitBackgroundLeft.StartFade();
				//this.PortraitBackgroundRight.StartFade();
				this.PortraitBackgroundLeft.gameObject.SetActive(true);
				this.PortraitBackgroundRight.gameObject.SetActive(true);
			});
		}
	}

	public void OnRaceButton()
	{
		IRestriction restriction = WorldTourChoiceItem.GetRestriction(this.GetEvent());
		if (restriction == null)
		{
			IGameState gs = new GameStateFacade();
			PopupData onMapPinTapPopup = this.m_pin.GetOnMapPinTapPopup();
			if (onMapPinTapPopup.IsEligible(gs))
			{
				PopUpManager.Instance.TryShowPopUp(onMapPinTapPopup.GetPopup(new PopUpButtonAction(this.GoToRace), null), PopUpManager.ePriority.Default, null);
			}
			else
			{
				this.GoToRace();
			}
			return;
		}
		if (this.isWorldTourBossCar)
		{
			CarInfo car = CarDatabase.Instance.GetCar(this.GetEvent().AICar);
			string[] array = car.PrizeInfoButton.Split(new char[]
			{
				','
			});
			TierXManager.Instance.OnGoToSpecificWorldTourHub(array[1]);
			return;
		}
		restriction.RestrictionButtonPressed();
	}

	private void GoToRace()
	{
		PinDetail pin = TierXManager.Instance.GetPin(this.m_pin);
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(pin.EventID);
		eventByEventIndex.SetWorldTourPinPinDetail(pin);
		GameStateFacade gameStateFacade = new GameStateFacade();
		gameStateFacade.SetPinToShown(TierXManager.Instance.CurrentThemeName, this.m_pin);
		if (eventByEventIndex == null)
		{
			return;
		}
		if (eventByEventIndex.ForceUserInCar)
		{
			IEnumerable<CarGarageInstance> compatibleCars = eventByEventIndex.GetCompatibleCars();
			if (!compatibleCars.Any<CarGarageInstance>())
			{
				return;
			}
			IOrderedEnumerable<CarGarageInstance> source = (from p in compatibleCars
			orderby p.CurrentPPIndex descending
			select p).ThenBy((CarGarageInstance b) => CarDataDefaults.IsBossCar(b.CarDBKey));
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			activeProfile.CurrentlySelectedCarDBKey = source.First<CarGarageInstance>().CarDBKey;
			activeProfile.UpdateCurrentCarSetup();
			activeProfile.UpdateCurrentPhysicsSetup();
			CarInfoUI.Instance.SetCurrentCarIDKey(source.First<CarGarageInstance>().CarDBKey);
		}
		if (eventByEventIndex.IsRelay)
		{
			RaceEventInfo.Instance.PopulateFromRaceEvent(eventByEventIndex);
			ScreenManager.Instance.PushScreen(ScreenID.RelayResults);
		}
		else
		{
			RaceEventInfo.Instance.PopulateFromRaceEvent(eventByEventIndex, eCarTier.TIER_X, true);
			if (!CrewProgressionSetup.PreRaceSetupForNarrativeScene(ScreenID.Invalid))
			{
				PinDetail worldTourPinPinDetail = eventByEventIndex.GetWorldTourPinPinDetail();
				if (worldTourPinPinDetail == null || !worldTourPinPinDetail.ActivateVSLoadingScreen())
				{
					SceneManagerFrontend.ButtonStart();
				}
			}
		}
	}

	private void SetupDifficultyRating()
	{
		RaceEventData @event = this.GetEvent();
		this.DifficultySpriteT.gameObject.SetActive(true);
		this.DifficultyGraphic.gameObject.SetActive(true);
		this.DifficultyShadowGraphic.gameObject.SetActive(true);
		RaceEventDifficulty.Rating rating = RaceEventDifficulty.Instance.GetRating(@event, false);
		string @string = RaceEventDifficulty.Instance.GetString(rating);
		this.DifficultySpriteT.text = @string;
		string texture = RaceEventDifficulty.Instance.GetTexture(rating);
		Texture2D texture2D = (Texture2D)Resources.Load(texture);
		if (texture2D == null)
		{
			return;
		}
		//this.DifficultyGraphic.renderer.material.SetTexture("_MainTex", texture2D);
	}
}
