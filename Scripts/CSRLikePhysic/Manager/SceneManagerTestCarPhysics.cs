using System.Collections;
using KingKodeStudio;
using UnityEngine;

public class SceneManagerTestCarPhysics : MonoBehaviour
{
    //[SerializeField] private bool m_enable = true;
    [SerializeField] private RaceEventData m_raceEventData;
    [SerializeField] private bool m_isSpecific;
    [SerializeField] private AutoDifficulty.DifficultyRating difficultyRating;

    private bool m_needInitialize; 
	private void Awake()
	{
        if (SceneLoadManager.Instance!=null && SceneLoadManager.Instance.LastScene == SceneLoadManager.Scene.Frontend)
	    {
	        return;
	    }
        m_needInitialize = Z2HInitialisation.Instance.Init();

        if (m_needInitialize)
        {
            Z2HInitialisation.Instance.okToGoBig = true;
        }

        //CarUpgradeSetup upgradeSetup = new CarUpgradeSetup();
        //RaceEventInfo.Instance.LocalPlayerCarUpgradeSetup = upgradeSetup;
        //upgradeSetup.UpgradeStatus = new Dictionary<eUpgradeType, CarUpgradeStatus>()
        //{
        //    {eUpgradeType.ENGINE, new CarUpgradeStatus() {levelFitted = 1}},
        //    {eUpgradeType.BODY, new CarUpgradeStatus() {levelFitted = 0}},
        //    {eUpgradeType.TURBO, new CarUpgradeStatus() {levelFitted = 0}},
        //    {eUpgradeType.TRANSMISSION, new CarUpgradeStatus() {levelFitted = 0}},
        //    {eUpgradeType.NITROUS, new CarUpgradeStatus() {levelFitted = 0}},
        //    {eUpgradeType.INTAKE, new CarUpgradeStatus() {levelFitted = 0}},
        //    {eUpgradeType.TYRES, new CarUpgradeStatus() {levelFitted = 0}},
        //};
	}

	private IEnumerator Start()
	{
        //if no need to intialize , it means that we are running game from frontend
        if (!m_needInitialize)
        {
            //Debug.Log("here");
            yield break;
        }
        //CompetitorManager.Instance.AddCompetitor(eRaceCompetitorType.LOCAL_COMPETITOR);
        //CompetitorManager.Instance.AddCompetitor(eRaceCompetitorType.AI_COMPETITOR);

        RaceController.Instance.enabled = false;
        while (!GTSystemOrder.SystemsReady)
        {
            yield return 0;
        }

	    RaceEventTopLevelCategory raceEventTopLevel = m_isSpecific ? (RaceEventTopLevelCategory) new CarSpecificEvents() : new RegulationRaceEvents();
        m_raceEventData.Process(new RaceEventGroup(), raceEventTopLevel);
	    if (!m_isSpecific)
	    {
	        var playerCar = new CarGarageInstance();
	        var playercarInfo = CarDatabase.Instance.GetCar(m_raceEventData.HumanCar);
	        playerCar.SetupNewGarageInstance(playercarInfo);
	        AutoDifficulty.GetRandomOpponentForCarAtDifficulty(difficultyRating, ref m_raceEventData, playerCar);
	    }

        CarUpgradeSetup upgradeSetup = new CarUpgradeSetup();
        var carGarageInstance = new CarGarageInstance();
        var car = CarDatabase.Instance.GetCar(m_raceEventData.HumanCar);
        carGarageInstance.SetupNewGarageInstance(car);
	    carGarageInstance.CurrentPPIndex = -1;//randomUpgradeDataSet.PPIndex;
	    m_raceEventData.LoanCarRace = true;
        this.m_raceEventData.SetLoanCarDetails(upgradeSetup, carGarageInstance);
	    RaceEventInfo.Instance.PopulateFromRaceEvent(m_raceEventData);
        RaceController.Instance.enabled = true;
	}
}
