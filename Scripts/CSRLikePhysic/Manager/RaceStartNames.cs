using I2.Loc;
using UnityEngine;

public class RaceStartNames : MonoBehaviour
{
	public enum State
	{
		Idle,
		ShowingCar1,
		ShowingCar2,
		StartGrid,
		FadeOff
	}

	public GameObject CarInfo1;

	public GameObject CarInfo2;

	private float fadeOffTimer;

	private float fadeStartTimer;

	private float fadeOffStart = 1f;

	public Transform CarInfo1Grid;

	public Transform CarInfo1IntroForwards;

	public Transform CarInfo1IntroBackwards;

	public Transform CarInfo2Grid;

	public Transform CarInfo2IntroForwards;

	public Transform CarInfo2IntroBackwards;

	public State state
	{
		get;
		private set;
	}

	public static RaceStartNames Instance
	{
		get;
		private set;
	}

	public RaceStartNames()
	{
		this.state = State.Idle;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	}

	private void OnShotEnd()
	{
		if (IngameTutorial.IsInTutorial && !IngameTutorial.IsIn2ndTutorialRace)
		{
			return;
		}
		State state = this.state;
		if (state != State.Idle)
		{
			if (state == State.ShowingCar1)
			{
				this.Car2ShotActivate();
			}
		}
		else
		{
			this.Car1ShotActivate();
		}
	}

	private void Start()
	{
		SequenceManager.Instance.OnShotEnd += new SequenceManager.ShotEndDelegate(this.OnShotEnd);
		this.Disable();
	}

	private void OnDestroy()
	{
		SequenceManager.Instance.OnShotEnd -= new SequenceManager.ShotEndDelegate(this.OnShotEnd);
	}

	public void Car1ShotActivate()
	{
		this.state = State.ShowingCar1;
		this.Enable();
	}

	public void Car2ShotActivate()
	{
		this.state = State.ShowingCar2;
	}

	public void GridSideView()
	{
		if (this.state == State.ShowingCar2 || this.state == State.ShowingCar1)
		{
			this.state = State.StartGrid;
			this.CarInfo1.transform.localPosition = this.CarInfo1Grid.localPosition;
			this.CarInfo1.transform.localRotation = this.CarInfo1Grid.localRotation;
			this.CarInfo2.transform.localPosition = this.CarInfo2Grid.localPosition;
			this.CarInfo2.transform.localRotation = this.CarInfo2Grid.localRotation;
			this.CarInfo2.GetComponent<RaceStartInfoPane>().SetScale(1.5f);
		}
	}

	public void FadeOff()
	{
		this.state = State.FadeOff;
	}

	private void Update()
	{
		if (this.state == State.StartGrid)
		{
			if (this.fadeStartTimer >= this.fadeOffStart)
			{
				this.state = State.FadeOff;
				this.fadeStartTimer = 0f;
			}
			else
			{
				this.fadeStartTimer += Time.deltaTime;
			}
		}
		if (this.state != State.FadeOff)
		{
			return;
		}
		this.fadeOffTimer += Time.deltaTime;
		if (this.fadeOffTimer > 1f)
		{
			this.state = State.Idle;
			this.Disable();
		}
		this.CarInfo1.GetComponent<RaceStartInfoPane>().SetAlpha(1f - this.fadeOffTimer);
		this.CarInfo2.GetComponent<RaceStartInfoPane>().SetAlpha(1f - this.fadeOffTimer);
	}

	private void LateUpdate()
	{
		if (this.state == State.ShowingCar1 || this.state == State.ShowingCar2)
		{
			if (Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)).direction.z > 0f)
			{
				this.CarInfo1.transform.localPosition = this.CarInfo1IntroForwards.localPosition;
				this.CarInfo1.transform.localRotation = this.CarInfo1IntroForwards.localRotation;
				this.CarInfo2.transform.localPosition = this.CarInfo2IntroForwards.localPosition;
				this.CarInfo2.transform.localRotation = this.CarInfo2IntroForwards.localRotation;
			}
			else
			{
				this.CarInfo1.transform.localPosition = this.CarInfo1IntroBackwards.localPosition;
				this.CarInfo1.transform.localRotation = this.CarInfo1IntroBackwards.localRotation;
				this.CarInfo2.transform.localPosition = this.CarInfo2IntroBackwards.localPosition;
				this.CarInfo2.transform.localRotation = this.CarInfo2IntroBackwards.localRotation;
			}
		}
	}

	private void Enable()
	{
		this.fadeOffTimer = 0f;
		this.CarInfo1.SetActive(true);
		this.CarInfo2.SetActive(true);
		float headStartTime = 0f;
		float headStartTime2 = 0f;
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent.IsRelay || currentEvent.AutoHeadstart)
		{
			float timeDifference = RaceEventInfo.Instance.CurrentEvent.GetTimeDifference();
			if (timeDifference > 0f)
			{
				headStartTime = timeDifference;
			}
			else
			{
				headStartTime2 = -timeDifference;
			}
		}
		RacePlayerInfoComponent component = CompetitorManager.Instance.LocalCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
		string zName = PlayerProfileManager.Instance.ActiveProfile.DisplayNameWithYOUFallback();
		CarInfo car = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.LocalPlayerCarDBKey);
		string zCarName = LocalizationManager.GetTranslation(car.MediumName);
		int num = component.PPIndex;
		eCarTier eCarTier = component.CarTier;
		if (IngameTutorial.IsInTutorial)
		{
			num = RaceEventInfo.Instance.HumanCarGarageInstance.CurrentPPIndex;
			eCarTier = RaceEventInfo.Instance.HumanCarGarageInstance.CurrentTier;
		}
		if (RaceEventInfo.Instance.CurrentEvent.IsTestDriveAndCarSetup() || RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
		{
			CarGarageInstance loanCarGarageInstance = RaceEventInfo.Instance.CurrentEvent.LoanCarGarageInstance;
			num = loanCarGarageInstance.CurrentPPIndex;
			eCarTier = loanCarGarageInstance.CurrentTier;
		}
		this.CarInfo1.GetComponent<RaceStartInfoPane>().Populate(zName, zCarName, eCarTier, num, headStartTime, true);
		RacePlayerInfoComponent component2 = CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
		string zName2 = RaceEventInfo.Instance.GetRivalName().ToUpper();
		CarInfo car2 = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.OpponentCarDBKey);
		string zCarName2 = LocalizationManager.GetTranslation(car2.MediumName);
		int num2 = component2.PPIndex;
		eCarTier zTier = component2.CarTier;
		if (RaceEventInfo.Instance.CurrentEvent.AutoDifficulty)
		{
			num2 = Mathf.Min(num2, RaceEventInfo.Instance.CurrentEvent.MaxPerformancePotentialIndex);
		}
		if (RaceEventInfo.Instance.IsDailyBattleEvent)
		{
			num2 = num;
			zTier = eCarTier;
		}
		if (IngameTutorial.IsInTutorial)
		{
			num2 = RaceEventInfo.Instance.AICarGarageInstance.CurrentPPIndex;
			zTier = RaceEventInfo.Instance.AICarGarageInstance.CurrentTier;
		}
		this.CarInfo2.GetComponent<RaceStartInfoPane>().Populate(zName2, zCarName2, zTier, num2, headStartTime2, false);
	}

	private void Disable()
	{
		this.CarInfo1.SetActive(false);
		this.CarInfo2.SetActive(false);
		this.CarInfo2.GetComponent<RaceStartInfoPane>().SetScale(1f);
	}
}
