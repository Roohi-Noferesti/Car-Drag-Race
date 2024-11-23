using DataSerialization;
using System;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class WorldTourChoiceScreen : ZHUDScreen
{
	public GameObject Choices;

	public TextMeshProUGUI Title;

	public UnityEngine.Color color;

	private static ScheduledPin m_choicePin;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.WorldTourChoice;
		}
	}

	public static void SetupChoice(ScheduledPin choicePin)
	{
		WorldTourChoiceScreen.m_choicePin = choicePin;
	}

	private void AddChoice(int choiceNumber, int totalChoices, ScheduledPin pin)
	{
		float num = -((float)(totalChoices / 2) * 1.2f) + (float)choiceNumber * 1.2f;
		if (totalChoices % 2 == 0)
		{
			num += 0.6f;
		}
		pin.AutoStart = WorldTourChoiceScreen.m_choicePin.AutoStart;
		pin.SetScreenToPushAfterResult(WorldTourChoiceScreen.m_choicePin.GetScreenToPushAfterResult());
		GameObject prefab = Resources.Load("World_Tour/WorldTourChoiceItem") as GameObject;
		WorldTourChoiceItem worldTourChoiceItem = GameObjectHelper.InstantiatePrefab<WorldTourChoiceItem>(prefab, this.Choices);
		worldTourChoiceItem.transform.localPosition = new UnityEngine.Vector3(num, 0f, 0.2f);
		worldTourChoiceItem.Setup(pin);
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		base.OnActivate(zAlreadyOnStack);
		if (WorldTourChoiceScreen.m_choicePin == null || WorldTourChoiceScreen.m_choicePin.ChoiceScreen == null)
		{
			return;
		}
		GameStateFacade gameStateFacade = new GameStateFacade();
		List<ScheduledPin> eligiblePins = WorldTourChoiceScreen.m_choicePin.ChoiceScreen.GetEligiblePins(gameStateFacade, WorldTourChoiceScreen.m_choicePin);
		UnityEngine.Color lhs = WorldTourChoiceScreen.m_choicePin.ChoiceScreen.Colour.AsUnityColor();
		if (lhs != UnityEngine.Color.clear)
		{
			this.color = lhs;
		}
		else
		{
			PinDetail pin = TierXManager.Instance.GetPin(eligiblePins[0]);
			RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(pin.EventID);
			string text = eventByEventIndex.AIDriverCrew;
			if (text.EndsWith("_Crew"))
			{
				text = text.Substring(0, text.LastIndexOf("_Crew"));
				this.color = GameDatabase.Instance.Colours.GetTierColour(text);
			}
			else
			{
				this.color = GameDatabase.Instance.Colours.GetTierColour(CarDatabase.Instance.GetCar(eventByEventIndex.AICar).BaseCarTier);
			}
		}
		//ScreenManager.Instance.CheckerGradientBackground.SetColour(this.color);
	 //   ScreenManager.Instance.CheckerGradientBackground.SetFade(0.8f);
		gameStateFacade.CurrentWorldTourSequenceID = WorldTourChoiceScreen.m_choicePin.ChoiceScreen.SequenceID;
		PopupStatusData progressionPopupDataForScreen = GameDatabase.Instance.ProgressionPopups.GetProgressionPopupDataForScreen(this.ID, gameStateFacade);
		if (progressionPopupDataForScreen != null)
		{
			PopUpManager.Instance.TryShowPopUp(progressionPopupDataForScreen.Popup.GetPopup(new PopUpButtonAction(this.PopulateChoices), null), PopUpManager.ePriority.Default, null);
			progressionPopupDataForScreen.PopupShowSuccess();
		}
		else
		{
			this.PopulateChoices();
		}
	}

	private void PopulateChoices()
	{
		this.Title.text = LocalizationManager.GetTranslation("TEXT_CHOOSE_OPPONENT");
		GameStateFacade gameState = new GameStateFacade();
		List<ScheduledPin> pins = WorldTourChoiceScreen.m_choicePin.ChoiceScreen.GetEligiblePins(gameState, WorldTourChoiceScreen.m_choicePin);
		pins.ForEachWithIndex(delegate(ScheduledPin pin, int i)
		{
			this.AddChoice(i, pins.Count, pin);
		});
	}

	public override void RequestBackup()
	{
		if (WorldTourChoiceScreen.m_choicePin.AutoStart)
		{
			TierXManager.Instance.LoadBackOutTheme(delegate
			{
				TierXManager.Instance.LoadTierXJson(delegate
				{
					ScreenManager.Instance.PopScreen();
				});
			});
		}
		else
		{
			base.RequestBackup();
		}
	}

	public override void OnDeactivate()
	{
		//ScreenManager.Instance.CheckerGradientBackground.SetColour(UnityEngine.Color.white);
		base.OnDeactivate();
	}
}
