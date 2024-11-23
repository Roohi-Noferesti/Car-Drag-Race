using UnityEngine;

[AddComponentMenu("GT/Logic/RaceCar")]
public class RaceCar : MonoBehaviour
{
	private Quaternion initialRotation;

	public CarPhysics physics
	{
		get;
		private set;
	}

	public RaceCarAudio carAudio
	{
		get;
		private set;
	}

	public CarVisuals carVisuals
	{
		get;
		private set;
	}

	private void Start()
	{
		initialRotation = carVisuals.BodyNode.transform.rotation;
	}

	public void Initialise(CarInfo info, CarVisuals visuals, bool isLocalPlayer, CarUpgradeSetup carUpgradeSetup)
	{
		carVisuals = visuals;
        physics = this.gameObject.AddComponent<CarPhysics>();
		physics.FrontendMode = false;
		CarPhysicsSetupCreator carPhysicsSetupCreator = new CarPhysicsSetupCreator(info, physics);
		carPhysicsSetupCreator.InitialiseCarPhysics(carUpgradeSetup);
		carPhysicsSetupCreator.ApplyCarUpgrades(carUpgradeSetup);
		if (isLocalPlayer)
		{
			OptimalGearChangeSpeedCalculator optimalGearChangeSpeedCalculator = new OptimalGearChangeSpeedCalculator(carPhysicsSetupCreator.CarPhysics);
			optimalGearChangeSpeedCalculator.CalculateGearChangeSpeeds();
		}
        if (RaceEventInfo.Instance != null && BoostNitrous.HaveBoostNitrous() && RaceEventInfo.Instance.CurrentEvent.IsHighStakesEvent())
        {
            carPhysicsSetupCreator.ApplySuperNitrous(isLocalPlayer ? 1f : 0f);
        }
        carPhysicsSetupCreator.SetStatsAfterUpgrade();
        if (RaceEventInfo.Instance != null && !CompetitorManager.Instance.IsLocalCompetitorOnly())
        {
            RacePlayerInfoComponent component = CompetitorManager.Instance.LocalCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
            RacePlayerInfoComponent component2 = CompetitorManager.Instance.OtherCompetitor.PlayerInfo.GetComponent<RacePlayerInfoComponent>();
            if (RaceEventInfo.Instance.IsDailyBattleEvent)
            {
                component.CarTier = carPhysicsSetupCreator.BaseCarTier;
                component2.CarTier = carPhysicsSetupCreator.BaseCarTier;
                component.PPIndex = carPhysicsSetupCreator.NewPerformanceIndex;
                component2.PPIndex = carPhysicsSetupCreator.NewPerformanceIndex;
            }
            else if (component2.CarTier == eCarTier.TIER_X)
            {
                if (!isLocalPlayer)
                {
                    component2.CarTier = carPhysicsSetupCreator.BaseCarTier;
                }
                if (RaceEventInfo.Instance.CurrentEvent.IsTestDriveAndCarSetup() || RaceEventInfo.Instance.IsFirstOfSwitchBackRace())
                {
                    CarGarageInstance loanCarGarageInstance = RaceEventInfo.Instance.CurrentEvent.LoanCarGarageInstance;
                    component.PPIndex = loanCarGarageInstance.CurrentPPIndex;
                }
            }
        }

        if (/*!skipAudio &&*/ AudioManager.Instantiated)
        {
            string engineName = string.Format("{0} Engine", (!isLocalPlayer) ? "AI" : "Human");
            eAudioEngineType eAudioEngineType = (!isLocalPlayer) ? AudioManager.Instance.AIEngineOverride : AudioManager.Instance.HumanEngineOverride;
            RaceCarAudio raceCarAudio;
            if (eAudioEngineType != eAudioEngineType.ENGINE_NONE)
            {
                raceCarAudio = RaceCarAudio.Find(info.Key, eAudioEngineType, isLocalPlayer);
            }
            else
            {
                raceCarAudio = RaceCarAudio.Find(info.Key, info.CarEngineSound, isLocalPlayer);
            }
            GameObject raceCarAudioInstance = Instantiate(raceCarAudio.gameObject);
            raceCarAudioInstance.name = engineName;
            raceCarAudioInstance.transform.parent = this.gameObject.transform;
            raceCarAudioInstance.transform.localPosition = Vector3.zero;
            carAudio = raceCarAudioInstance.GetComponent<RaceCarAudio>();
            carAudio.Initialise(isLocalPlayer,physics, carUpgradeSetup);
        }


        //var carEngineSound = info.CarEngineSound;
        //GameObject gameObject = Resources.Load("Prefabs/Sounds/EngineAudio_" + carEngineSound) as GameObject;
        //if (gameObject == null)
        //{
        //    gameObject = (Resources.Load("Prefabs/Sounds/EngineAudio_ENGINE_4_CYLINDER") as GameObject);
        //}
        //GameObject gameObject2 = Instantiate(gameObject) as GameObject;
        //gameObject2.transform.parent = base.gameObject.transform;
        //gameObject2.transform.localPosition = Vector3.zero;
        //this.carAudio = gameObject2.GetComponent<RaceCarAudio>();
        //this.carAudio.Initialise(isLocalPlayer,physics, carUpgradeSetup);
	}

	public void OnDestroy()
	{
		if (carVisuals != null)
		{
			carVisuals.BodyNode.transform.rotation = initialRotation;
		}
		if (carAudio != null)
		{
			Destroy(carAudio.gameObject);
		}
        //Destroy(this.physics);
	}

	private void Update()
	{
        if (PauseGame.isGamePaused)
        {
            return;
        }
		UpdateWheelRotations();
		UpdateBodyRoll();
	}

	private void UpdateBodyRoll()
	{
		Quaternion rhs = Quaternion.AngleAxis(physics.LongitudinalRollAmount, Vector3.right);
		carVisuals.BodyNode.transform.rotation = initialRotation * rhs;
	}

	private void UpdateWheelRotations()
	{
		if (RaceController.Instance.Machine.StateIs(RaceStateEnum.enter))
			return;
		float num = physics.WheelAngleDelta;
		num *= 360f;
		foreach (GameObject current in carVisuals.Wheels)
		{
			current.transform.Rotate(Vector3.right, num, Space.World);
		}
	}

	public void StopAudio()
	{
		if (carAudio != null)
		{
			carAudio.Stop();
		}
	}
}
