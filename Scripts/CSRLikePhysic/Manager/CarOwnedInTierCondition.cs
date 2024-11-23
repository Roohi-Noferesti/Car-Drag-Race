using DataSerialization;

public class CarOwnedInTierCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		foreach (CarGarageInstance current in PlayerProfileManager.Instance.ActiveProfile.CarsOwned)
		{
		    var baseCarTier = CarDatabase.Instance.GetCar(current.CarDBKey).BaseCarTier;
            if (baseCarTier == (eCarTier)(details.Tier - 1))
			{
				return true;
			}
		}
		return false;
	}
}
