using DataSerialization;
using System;
using System.Reflection;
using UnityEngine;

public class EventHubOpponent : MonoBehaviour
{
	public CarSnapshotRaceOpponent OpponentSnapshot;

    //public SpriteRoot[] TierColouredSprites;

	public GameObject OverlayGlow;

	private RaceEventData race;

	public bool isSet;

	public void Setup(RaceEventData race)
	{
		this.race = race;
		this.SetTierColor(false);
		this.OpponentSnapshot.Setup(race, null);
	}

	public void SetupFromCarOverride(CarOverride carOverride)
	{
		RaceEventData obj = new RaceEventData();
		IGameState gameState = new GameStateFacade();
		string themeID = carOverride.ThemeID ?? TierXManager.Instance.CurrentThemeName;
		int num = gameState.ChoiceSelection(themeID, carOverride.SequenceID, carOverride.ScheduledPinID);
		this.ShowAsUsed(false);
		PinDetail pinDetail;
		if (!carOverride.ChoiceSequenceID.Contains("Choice"))
		{
			pinDetail = TierXManager.Instance.ThemeDescriptor.PinDetails.Find((PinDetail p) => p.PinID == carOverride.ChoiceSequenceID);
		}
		else
		{
			PinSequence sequence = TierXManager.Instance.PinSchedule.GetSequence(carOverride.ChoiceSequenceID);
			if (num < 0 || num >= sequence.Pins.Count)
			{
				this.SetTierColor(true);
                //Texture2D texture2D = (Texture2D)Resources.Load("Career/question-mark");
                //this.OpponentSnapshot.renderer.material.SetTexture("_MainTex", texture2D);
                //global::Sprite component = this.OpponentSnapshot.gameObject.GetComponent<global::Sprite>();
                //component.Setup(component.width / 2.5f, component.height / 2.5f, new UnityEngine.Vector2(0f, (float)(texture2D.height - 1)), new UnityEngine.Vector2((float)texture2D.width, (float)texture2D.height));
				return;
			}
			ScheduledPin scheduledPin = sequence.Pins[num];
			string choicePinID = scheduledPin.PinID;
			pinDetail = TierXManager.Instance.ThemeDescriptor.PinDetails.Find((PinDetail p) => p.PinID == choicePinID);
		}
		int eventID = pinDetail.EventID;
		RaceEventData eventByEventIndex = GameDatabase.Instance.Career.GetEventByEventIndex(eventID);
		Type typeFromHandle = typeof(RaceEventData);
		foreach (string current in CarOverride.EventFieldsToCopy)
		{
			FieldInfo field = typeFromHandle.GetField(current);
			if (field != null)
			{
				object value = field.GetValue(eventByEventIndex);
				field.SetValue(obj, value);
			}
		}
		this.Setup(obj);
		this.isSet = true;
	}

	private void SetTierColor(bool forceGrey = false)
	{
		UnityEngine.Color color = UnityEngine.Color.grey;
		if (!forceGrey)
		{
			CarInfo carInfo = this.GetCarInfo();
			color = GameDatabase.Instance.Colours.GetTierColour(carInfo.BaseCarTier);
		}
        //SpriteRoot[] tierColouredSprites = this.TierColouredSprites;
        //for (int i = 0; i < tierColouredSprites.Length; i++)
        //{
        //    SpriteRoot spriteRoot = tierColouredSprites[i];
        //    spriteRoot.SetColor(color);
        //}
	}

	private CarInfo GetCarInfo()
	{
		return CarDatabase.Instance.GetCar(this.race.AICar);
	}

	public bool UsesCar(string carkey)
	{
		return this.race != null && this.race.AICar == carkey;
	}

	public void ShowAsUsed(bool used)
	{
		this.OverlayGlow.SetActive(used);
	}
}
