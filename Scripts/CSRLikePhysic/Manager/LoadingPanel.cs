using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
	private const string PipAdd = "•";

	private const float TimeBetweenPips = 0.3f;

	private const float PipResetTime = 0.6f;

	private const int MaxPipLength = 5;

	public GameObject BigBackground;

	public GameObject LittleBackground;

	public GameObject LoadingDevice;

	public GameObject StuffForRaceEvent;

	public Text DistanceTitle;

	public Image DistanceGraphic;

	public Text DifficultyTitle;

	public Image DifficultyGraphic;

    public Text GoldValue;

    public Text CashValue;

    public Text LoadingText;

    public Text LoadingPips;

    public TextMeshProUGUI TipText;

    public Text CashValueText;

    public Text GoldValueText;

    public Text EventTitle;

    public Text EventBody;

	private static Vector3 LoadingPositionForBig = new Vector3(0f, -0.8f, -0.03f);

	//private static Vector3 LoadingPositionForLittle = new Vector3(0f, 0.2f, -0.03f);

	//private bool isInSmallMode;

	private bool isHiddenForPopUp;

	private float CurrentPipTime;

	private int CurrentPipLength;

	private void Update()
	{
        //this.UpdatePips();
        //if (this.isInSmallMode)
        //{
        //    if (PopUpManager.Instance.isShowingPopUp && !this.isHiddenForPopUp)
        //    {
        //        this.LittleBackground.SetActive(false);
        //        this.LoadingDevice.SetActive(false);
        //        this.isHiddenForPopUp = true;
        //        return;
        //    }
        //    if (!PopUpManager.Instance.isShowingPopUp && this.isHiddenForPopUp)
        //    {
        //        this.LittleBackground.SetActive(true);
        //        this.LoadingDevice.SetActive(true);
        //        this.isHiddenForPopUp = false;
        //        return;
        //    }
        //}
	}

	private void UpdatePips()
	{
		this.CurrentPipTime += Time.deltaTime;
		if (this.CurrentPipLength >= 5)
		{
			if (this.CurrentPipTime >= 0.6f)
			{
				this.LoadingPips.text = string.Empty;
				this.CurrentPipLength = 0;
				this.CurrentPipTime = 0f;
			}
			return;
		}
		if (this.CurrentPipTime >= 0.3f)
		{
			this.CurrentPipTime = 0f;
            Text expr_72 = this.LoadingPips;
            expr_72.text += "•";
			this.CurrentPipLength++;
		}
	}

	private void ResetLoading(string loadingstring = "TEXT_LOADING")
	{
        //this.LoadingText.text = LocalizationManager.GetTranslation(loadingstring);
        //this.LoadingPips.text = string.Empty;
        //this.CurrentPipLength = 0;
        //this.CurrentPipTime = 0f;
	}

	public void SetUpForSmall(string text)
	{
        //this.StuffForRaceEvent.SetActive(false);
        //this.BigBackground.SetActive(false);
        //this.LittleBackground.SetActive(false);
        //this.LoadingDevice.SetActive(false);
        //this.isHiddenForPopUp = true;
        //this.LoadingDevice.transform.localPosition = LoadingPositionForLittle;
        //Vector3 localPosition = this.TipText.transform.localPosition;
        //localPosition.y = -0.64f;
        //this.TipText.transform.localPosition = localPosition;
        this.TipText.text = TipsManager.Instance.GetRandomTipForCurrentEvent();
		this.ResetLoading(text);
		//this.isInSmallMode = true;
	}

	public void SetUpForRaceEvent(RaceEventData raceData)
	{
        //CommonUI.Instance.NavBar.Show(false);
		this.StuffForRaceEvent.SetActive(true);
		this.BigBackground.SetActive(true);
		this.LittleBackground.SetActive(false);
		this.LoadingDevice.SetActive(true);
		this.LoadingDevice.transform.localPosition = LoadingPositionForBig;
		Vector3 localPosition = this.TipText.transform.localPosition;
		localPosition.y = -0.4f;
		this.TipText.transform.localPosition = localPosition;
		this.SetTitleAndDescription(raceData);
		this.NewEventDistanceToUpdate(raceData);
		this.NewEventDifficultyToUpdate(raceData);
		int goldPrize = (int) raceData.RaceReward.GoldPrize;
		int cashReward = raceData.RaceReward.GetCashReward();
		this.SetRewardObjectsPosition(goldPrize, cashReward);
        this.TipText.text = TipsManager.Instance.GetRandomTipForCurrentEvent();
		this.ResetLoading("TEXT_LOADING");
		//this.isInSmallMode = false;
	}

	public void SetLoadingText(string text)
	{
        this.LoadingText.text = text;
	}

	private void SetTitleAndDescription(RaceEventData raceData)
	{
        this.EventTitle.text = LocalizationManager.GetTranslation(raceData.EventName).ToUpper();
		if (raceData.Parent != null)
		{
            this.EventBody.text = raceData.Parent.GetPinString(raceData);
		}
		else
		{
            this.EventBody.text = string.Empty;
		}
	}

	private void NewEventDistanceToUpdate(RaceEventData raceData)
	{
		string text = "Map_Screen/map_track_length_";
		if (raceData.IsHalfMile)
		{
            this.DistanceTitle.text = LocalizationManager.GetTranslation("TEXT_DISTANCE_HALF_MILE");
			text += "half";
		}
		else
		{
            this.DistanceTitle.text = LocalizationManager.GetTranslation("TEXT_DISTANCE_QUARTER_MILE");
			text += "quarter";
		}
		Texture2D texture2D = Resources.Load(text) as Texture2D;
		if (texture2D == null)
		{
		}
		//float num = 200f;
        //this.DistanceGraphic.renderer.material.SetTexture("_MainTex", texture2D);
        //this.DistanceGraphic.Setup(232f / num, 24f / num, new Vector2(0f, 23f), new Vector2(232f, 24f));
	}

	private void NewEventDifficultyToUpdate(RaceEventData raceData)
	{
		RaceEventDifficulty.Rating rating = RaceEventDifficulty.Instance.GetRating(raceData, true);
		string @string = RaceEventDifficulty.Instance.GetString(rating);
        this.DifficultyTitle.text = @string;
		string text = "Map_Screen/map_difficulty_";
		switch (rating)
		{
		case RaceEventDifficulty.Rating.Easy:
			text += "01_easy";
			break;
		case RaceEventDifficulty.Rating.Challenging:
			text += "02_challenging";
			break;
		case RaceEventDifficulty.Rating.Difficult:
			text += "03_hard";
			break;
		case RaceEventDifficulty.Rating.Extreme:
			text += "04_extreme";
			break;
		}
		Texture2D texture2D = Resources.Load(text) as Texture2D;
		if (texture2D == null)
		{
		}
		//float num = 200f;
        //this.DifficultyGraphic.renderer.material.SetTexture("_MainTex", texture2D);
        //this.DifficultyGraphic.Setup(232f / num, 24f / num, new Vector2(0f, 23f), new Vector2(232f, 24f));
	}

	private void SetRewardObjectsPosition(int gold, int cash)
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
        this.CashValueText.text = CurrencyUtils.GetCashString(cash);
		if (gold == 0)
		{
            this.GoldValueText.text = string.Empty;
            //this.CashValueText.Anchor = SpriteText.Anchor_Pos.Middle_Center;
		}
		else if (cash == 0)
		{
            this.CashValueText.text = string.Empty;
            this.GoldValueText.text = CurrencyUtils.GetGoldString(gold);
            //this.GoldValueText.Anchor = SpriteText.Anchor_Pos.Middle_Center;
		}
		else
		{
            //this.CashValueText.Anchor = SpriteText.Anchor_Pos.Middle_Left;
            this.GoldValueText.text = CurrencyUtils.GetGoldString(gold);
			//float num = 0.03f;
            //float num2 = (this.CashValueText.GetWidth(this.CashValueText.Text) + this.GoldValueText.GetWidth(this.GoldValueText.Text)) / 2f;
            //zero.x = -num2 - num;
            //zero2.x = num2 + num;
		}
		this.CashValueText.gameObject.transform.localPosition = zero;
		this.GoldValueText.gameObject.transform.localPosition = zero2;
	}
}
