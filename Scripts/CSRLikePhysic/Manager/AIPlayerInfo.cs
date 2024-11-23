using System.Collections.Generic;

public class AIPlayerInfo : PlayerInfo
{
	public AIPlayerInfo() : base(new AIPersona())
	{
		RacePlayerInfoComponent racePlayerInfoComponent = base.AddComponent<RacePlayerInfoComponent>();
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		racePlayerInfoComponent.CarDBKey = currentEvent.AICar;
		racePlayerInfoComponent._ppIndex = currentEvent.GetAIPerformancePotentialIndex();
        //if (!string.IsNullOrEmpty(currentEvent.AIDriverLivery) && currentEvent.AIDriverLivery != "No Livery")
        //{
        //    racePlayerInfoComponent.AppliedLivery = currentEvent.AIDriverLivery;
        //}
		if (currentEvent.Parent != null)
		{
			racePlayerInfoComponent._carTier = currentEvent.Parent.GetTierEvents().GetCarTier();
		}
		else
		{
			racePlayerInfoComponent._carTier = CarDatabase.Instance.GetCar(racePlayerInfoComponent._carDBKey).BaseCarTier;
		}
		Dictionary<eUpgradeType, CarUpgradeStatus> dictionary = new Dictionary<eUpgradeType, CarUpgradeStatus>();
		CarUpgradeSetup.SetDefaultUpgradeStatus(dictionary);
		CarUpgradeSetup aICarUpgradeSetup = currentEvent.GetAICarUpgradeSetup();
		dictionary[eUpgradeType.BODY].levelFitted = aICarUpgradeSetup.UpgradeStatus[eUpgradeType.BODY].levelFitted;
		dictionary[eUpgradeType.ENGINE].levelFitted = aICarUpgradeSetup.UpgradeStatus[eUpgradeType.ENGINE].levelFitted;
		dictionary[eUpgradeType.INTAKE].levelFitted = aICarUpgradeSetup.UpgradeStatus[eUpgradeType.INTAKE].levelFitted;
		dictionary[eUpgradeType.NITROUS].levelFitted = aICarUpgradeSetup.UpgradeStatus[eUpgradeType.NITROUS].levelFitted;
		dictionary[eUpgradeType.TRANSMISSION].levelFitted = aICarUpgradeSetup.UpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted;
		dictionary[eUpgradeType.TURBO].levelFitted = aICarUpgradeSetup.UpgradeStatus[eUpgradeType.TURBO].levelFitted;
		dictionary[eUpgradeType.TYRES].levelFitted = aICarUpgradeSetup.UpgradeStatus[eUpgradeType.TYRES].levelFitted;
		racePlayerInfoComponent.CarUpgradeStatus = dictionary;
	}
}
