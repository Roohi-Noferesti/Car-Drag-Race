using I2.Loc;
using Objectives;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceGearMessaging : MonoBehaviour
{
	private const float GlowValueChange = 1f;

	public Color shiftMessageGreen;

	public Color shiftMessageRed;

	public Color shiftMessageBlue;

	public Color startMessageOKColour;

	public TextMeshProUGUI gearMessageText;

	public Shadow gearMessageGlow;

	public Image PaddleChangeIcon;

	private string cachedLateMessageTranslation;

	private string cachedShiftNowMessageTranslation;

	private string cachedGetReadyMessageTranslation;

	private string cachedSlowStartMessageTranslation;

	private string cachedGoodStartMessageTranslation;

	private string cachedOkayStartMessageTranslation;

	private string cachedPerfectStartMessageTranslation;

	private string cachedWheelspinStartMessageTranslation;

	private string cachedVeryEarlyShiftMessageTranslation;

	private string cachedEarlyShiftMessageTranslation;

	private string cachedGoodShiftMessageTranslation;

	private string cachedPerfectShiftMessageTranslation;

	private string cacheLateShiftMessageTranslation;

	private Color lastCol = new Color(0f, 0f, 0f, 0f);

	private float TextGlowValue;

	public void Reset()
	{
		this.cachedLateMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_LATE");
		this.cachedShiftNowMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_SHIFT_NOW");
		this.cachedGetReadyMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_GET_READY");
		this.cachedSlowStartMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_SLOW_START");
		this.cachedGoodStartMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_GOOD_START");
		this.cachedOkayStartMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_OK_START");
		this.cachedPerfectStartMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_PERFECT_START");
		this.cachedWheelspinStartMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_WHEELSPIN_START");
		this.cachedVeryEarlyShiftMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_VERY_EARLY_SHIFT");
		this.cachedEarlyShiftMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_EARLY_SHIFT");
		this.cachedGoodShiftMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_GOOD_SHIFT");
		this.cachedPerfectShiftMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_PERFECT_SHIFT");
		this.cacheLateShiftMessageTranslation = LocalizationManager.GetTranslation("TEXT_HUD_MESSAGE_LATE_SHIFT");
		this.ResetText();
	}

	private void OnDestroy()
	{
		if (CompetitorManager.Instance.LocalCompetitor != null && CompetitorManager.Instance.LocalCompetitor.CarPhysics != null)
		{
			CompetitorManager.Instance.LocalCompetitor.CarPhysics.GearChangeLogic.FireGearChangeUpEvent -= 
                this.EventDisplayShiftMessage;
		}
	}

	public void HookupGearEvents()
	{
		CompetitorManager.Instance.LocalCompetitor.CarPhysics.GearChangeLogic.FireGearChangeUpEvent += 
            this.EventDisplayShiftMessage;
	}

	private void Update()
	{
		if (this.gearMessageText.color != this.lastCol)
		{
		    Color color = this.gearMessageText.color;
			color.a = this.gearMessageText.color.a;
			this.gearMessageText.color =  color;
			this.lastCol = this.gearMessageText.color;
		}
		this.UpdateTextGlow();
	}

	private void EventDisplayChangeInfoMessage(RPMRegion rpmRegion, bool areWeRacing)
	{
		switch (rpmRegion)
		{
		case RPMRegion.NotReady:
			this.ShowBlueIcons(true);
			break;
		case RPMRegion.NearlyReady:
			this.ShowBlueIcons(true);
			break;
		case RPMRegion.Ready:
			if (areWeRacing)
			{
				this.ShowText(this.cachedGetReadyMessageTranslation, "MessageInTop", this.shiftMessageBlue, false);
			}
			this.ShowBlueIcons(areWeRacing);
			break;
		case RPMRegion.Now:
			if (areWeRacing)
			{
				this.ShowText(this.cachedShiftNowMessageTranslation, string.Empty, this.shiftMessageGreen, false);
			}
			this.ShowGreenIcons(areWeRacing);
			break;
		case RPMRegion.Late:
			if (areWeRacing)
			{
				this.ShowText(this.cachedLateMessageTranslation, "MessageOutTop", this.shiftMessageRed, false);
			}
			this.ShowRedIcons(areWeRacing);
			break;
		default:
			this.ShowBlueIcons(areWeRacing);
			break;
		}
	}

	private void EventDisplayShiftMessage(GearChangeRating gearRating)
	{
		this.ShowBlueIcons(true);
		switch (gearRating)
		{
		case GearChangeRating.SlowLaunch:
			this.ShowText(this.cachedSlowStartMessageTranslation, "MessageInOutTop", this.shiftMessageRed, false);
			break;
		case GearChangeRating.OkayLaunch:
			this.ShowText(this.cachedOkayStartMessageTranslation, "MessageInOutTop", this.startMessageOKColour, false);
			break;
		case GearChangeRating.GoodLaunch:
			this.ShowText(this.cachedGoodStartMessageTranslation, "MessageInOutTop", this.shiftMessageBlue, false);
			break;
		case GearChangeRating.PerfectLaunch:
			this.ShowText(this.cachedPerfectStartMessageTranslation, "MessageInOutPerfectGear", this.shiftMessageGreen, true);
            ObjectiveCommand.Execute(new CounterPerfectStart(), true);
			break;
		case GearChangeRating.WheelspinLaunch:
			this.ShowText(this.cachedWheelspinStartMessageTranslation, "MessageInOutTop", this.shiftMessageRed, false);
			break;
		case GearChangeRating.VeryEarly:
			this.ShowText(this.cachedVeryEarlyShiftMessageTranslation, "MessageInOutTop", this.shiftMessageRed, false);
			break;
		case GearChangeRating.Early:
			this.ShowText(this.cachedEarlyShiftMessageTranslation, "MessageInOutTop", this.shiftMessageRed, false);
			break;
		case GearChangeRating.Good:
			this.ShowText(this.cachedGoodShiftMessageTranslation, "MessageInOutTop", this.shiftMessageBlue, false);
			break;
		case GearChangeRating.Perfect:
			this.ShowText(this.cachedPerfectShiftMessageTranslation, "MessageInOutPerfectGear", this.shiftMessageGreen, true);
            ObjectiveCommand.Execute(new CounterPerfectShift(), true);
			break;
		case GearChangeRating.Late:
			this.ShowText(this.cacheLateShiftMessageTranslation, "MessageInOutTop", this.shiftMessageRed, false);
			break;
		}
	}

	private void ShowText(string gearMessage, string animName, Color colour, bool shouldGlow)
	{
		this.gearMessageText.color =  colour;
		base.gameObject.transform.localScale = Vector3.one;
		this.gearMessageText.text = gearMessage;
        //Debug.Log(animName+"   "+Time.time);
		if (animName != string.Empty)
		{
            base.GetComponent<Animator>().Play(animName);
		}
		if (shouldGlow)
		{
			this.StartGlow();
		}
		else
		{
            this.gearMessageGlow.enabled = false;
		}
	}

	public void ResetText()
	{
		Vector3 localPosition = this.gearMessageText.transform.localPosition;
		this.gearMessageText.text = string.Empty;
        //base.GetComponent<Animator>().Stop();
        //base.gameObject.transform.localPosition = new Vector3(0f, 0f, localPosition.z);
        this.gearMessageGlow.enabled = false;
		Color color = new Color(0f, 0f, 0f, 0f);
		this.gearMessageText.color =  color;
        //base.gameObject.transform.localScale = Vector3.one;
	}

	private void ShowBlueIcons(bool updatePaddle)
	{
		if (updatePaddle)
		{
            //this.PaddleChangeIcon.PlayAnim(0, 0);
            //this.PaddleChangeIcon.PauseAnim();
		}
	}

	private void ShowGreenIcons(bool updatePaddle)
	{
		if (updatePaddle)
		{
            //this.PaddleChangeIcon.PlayAnim(0, 1);
            //this.PaddleChangeIcon.PauseAnim();
		}
	}

	private void ShowRedIcons(bool updatePaddle)
	{
		if (updatePaddle)
		{
            //this.PaddleChangeIcon.PauseAnim();
            //this.PaddleChangeIcon.PlayAnim(0, 2);
		}
	}

	private void StartGlow()
	{
		this.TextGlowValue = 1f;
        this.gearMessageGlow.enabled = true;
	}

	private void UpdateTextGlow()
	{
		if (this.TextGlowValue > 0f)
		{
			this.TextGlowValue -= 1f * Time.deltaTime;
			if (this.TextGlowValue < 0f)
			{
				this.TextGlowValue = 0f;
                this.gearMessageGlow.enabled = false;
			}
            Color color = this.gearMessageGlow.effectColor;
            color.a = this.TextGlowValue;
            this.gearMessageGlow.effectColor = color;
		}
	}
}
