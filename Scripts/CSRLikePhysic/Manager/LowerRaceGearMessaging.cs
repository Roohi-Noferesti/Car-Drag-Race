using I2.Loc;
using TMPro;
using UnityEngine;

public class LowerRaceGearMessaging : MonoBehaviour
{
	public Color shiftMessageGreen;

	public Color shiftMessageRed;

	public Color shiftMessageBlue;

	public Color startMessageOKColour;

	public TextMeshProUGUI gearMessageText;

	private string cachedLateMessageTranslation;

	private string cachedShiftNowMessageTranslation;

	private string cachedGetReadyMessageTranslation;

	private string cachedMoreRevsMessageTranslation;

	private string cachedGoodRevsMessageTranslation;

	private string cachedLessRevsMessageTranslation;

	private bool showStartRevGuide;

	private Color lastCol = new Color(0f, 0f, 0f, 0f);

	private bool showStartText;

	public void Reset()
	{
		CarGearChangeLightRangesData carGearLightData = GameDatabase.Instance.CarsConfiguration.CarGearLightData;
		this.showStartRevGuide = (carGearLightData.ShouldShowStartRevGuide(CompetitorManager.Instance.LocalCompetitor.CarPhysics.CarTier) && carGearLightData.ShouldShowOKStart(CompetitorManager.Instance.LocalCompetitor.CarPhysics.CarTier));
		this.cachedLateMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_LATE");
		this.cachedShiftNowMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_SHIFT_NOW");
		this.cachedGetReadyMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_GET_READY");
		this.cachedMoreRevsMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_MORE_REVS");
		this.cachedGoodRevsMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_OK_REVS");
		this.cachedLessRevsMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_LESS_REVS");
		this.ResetText();
	}

	private void OnDestroy()
	{
		if (CompetitorManager.Instance.LocalCompetitor != null && CompetitorManager.Instance.LocalCompetitor.CarPhysics != null)
		{
			CompetitorManager.Instance.LocalCompetitor.CarPhysics.GearChangeLogic.FireEnterRPMRegionEvent -= new GearChangeLogic.EnterRPMRegionDelegate(this.EventDisplayChangeInfoMessage);
		}
	}

	public void HookupGearEvents()
	{
		CompetitorManager.Instance.LocalCompetitor.CarPhysics.GearChangeLogic.FireEnterRPMRegionEvent += new GearChangeLogic.EnterRPMRegionDelegate(this.EventDisplayChangeInfoMessage);
	}

	private void Update()
	{
		if (this.gearMessageText.color != this.lastCol)
		{
			Color color = this.gearMessageText.color;//.gameObject.GetComponent<Renderer>().material.GetColor("_Tint");
			color.a = this.gearMessageText.color.a;
            this.gearMessageText.color = color;//.gameObject.GetComponent<Renderer>().material.SetColor("_Tint", color);
			this.lastCol = this.gearMessageText.color;
		}
		GearChangeLogic.OutputState state = RaceHUDController.Instance.hudGearLightsDisplay.state;
		if (state.inNeutralGear && this.showStartRevGuide)
		{
			if (this.showStartText && state.normalisedLightsNumber > state.lateGearStartNumber)
			{
				this.ShowText(this.cachedLessRevsMessageTranslation, string.Empty, this.shiftMessageRed);
			}
			else if (this.showStartText && state.normalisedLightsNumber > state.goodGearStartNumber)
			{
				this.ShowText(this.cachedGoodRevsMessageTranslation, string.Empty, this.startMessageOKColour);
			}
			else if (this.showStartText)
			{
				this.ShowText(this.cachedMoreRevsMessageTranslation, string.Empty, this.shiftMessageRed);
			}
            else if (state.normalisedLightsNumber > state.goodGearStartNumber)
			{
				this.ShowText(this.cachedGoodRevsMessageTranslation, "MessageInBottom", this.startMessageOKColour);
				this.showStartText = true;
			}
		}
		else
		{
			this.showStartText = false;
		}
	}

	private void EventDisplayChangeInfoMessage(RPMRegion rpmRegion, bool areWeRacing)
	{
		if (!areWeRacing)
		{
			return;
		}
		switch (rpmRegion)
		{
		case RPMRegion.NotReady:
			this.ResetText();
			break;
		case RPMRegion.NearlyReady:
			this.ResetText();
			break;
		case RPMRegion.Ready:
			if (areWeRacing)
			{
				this.ShowText(this.cachedGetReadyMessageTranslation, "MessageInBottom", this.shiftMessageBlue);
			}
			break;
		case RPMRegion.Now:
			if (areWeRacing)
			{
				this.ShowText(this.cachedShiftNowMessageTranslation, string.Empty, this.shiftMessageGreen);
			}
			break;
		case RPMRegion.Late:
			if (areWeRacing)
			{
				this.ShowText(this.cachedLateMessageTranslation, string.Empty, this.shiftMessageRed);
			}
			break;
		}
	}

	private void ShowText(string gearMessage, string animName, Color colour)
	{
	    this.gearMessageText.color = colour;
		base.gameObject.transform.localScale = Vector3.one;
		this.gearMessageText.text = gearMessage;
		if (animName != string.Empty)
		{
            //base.GetComponent<Animator>().Play(animName);
		}
	}

	public void ResetText()
	{
		Vector3 localPosition = this.gearMessageText.transform.localPosition;
		this.gearMessageText.text = string.Empty;
        //base.GetComponent<Animator>().Stop();
        //base.gameObject.transform.localPosition = new Vector3(0f, 0f, localPosition.z);
	}
}
