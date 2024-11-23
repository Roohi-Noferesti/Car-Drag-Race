using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;

public abstract class AgentCarDeal
{
	public string Car
	{
		get;
		private set;
	}

	public AgentCarDeal(string car)
	{
		this.Car = car;
	}

	public virtual int GetGoldPrice()
	{
		return CarDatabase.Instance.GetCar(this.Car).GoldPrice;
	}

    public abstract void SetupCostContainer(CostContainer container, UnityAction buttonAction);

	public abstract string GetWorkshopScreenPopupBodyText();

	public abstract void OnPopupShown();

	public virtual void OnCompleted()
	{
		PlayerProfileManager.Instance.ActiveProfile.CarDealCompleted();
	}

	public string LocalisedCarName()
	{
		CarInfo car = CarDatabase.Instance.GetCar(this.Car);
		return LocalizationManager.GetTranslation(car.ShortName);
	}

	public string LocalisedManufacturerName()
	{
		CarInfo car = CarDatabase.Instance.GetCar(this.Car);
	    return LocalizationManager.GetTranslation(ManufacturerDatabase.Instance.Manufacturers[car.ManufacturerID].translatedName);
	}

	public virtual string GetDiscountMetricParam()
	{
		return "NA";
	}

	public virtual string GetFreeUpgradesMetricParam()
	{
		return "NA";
	}

	public virtual string GetCashbackMetricParam()
	{
		return "NA";
	}

	private static IEnumerable<string> GetOfferCarsForTier(eCarTier tier)
	{
        CarDeal carconfig = GameDatabase.Instance.DealConfiguration.Car;
        PlayerProfile profile = PlayerProfileManager.Instance.ActiveProfile;
        List<CarInfo> carsOfTier = CarDatabase.Instance.GetCarsOfTier(tier);
        IEnumerable<CarInfo> source = from q in carsOfTier
                                      where q.GoldPrice > 0 && !profile.IsCarOwned(q.Key)
                                      select q;
        IEnumerable<CarInfo> source2 = from q in source
                                       where q.HasBeenAvailableInShowroomForNSeasons(carconfig.SeasonsBeforeCarCanBeOffered)
                                       select q;
        return from q in source2
               select q.Key;
	}

	private static bool IsPriorityCar(string key)
	{
	    return GameDatabase.Instance.DealConfiguration.Car.PriorityCars != null && GameDatabase.Instance.DealConfiguration.Car.PriorityCars.Contains(key);
	}

	private static int CarDealOrdering(string q)
	{
		return Mathf.Abs(q.GetHashCode() % 104729) + ((!IsPriorityCar(q)) ? 104729 : 0);
	}

	private static string FindNextCarToOffer(IEnumerable<string> carsToOffer)
	{
		carsToOffer = from q in carsToOffer
		orderby CarDealOrdering(q)
		select q;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (string.IsNullOrEmpty(activeProfile.LastCarDealCarOffered))
		{
			return carsToOffer.First<string>();
		}
		IEnumerator<string> enumerator = carsToOffer.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current == activeProfile.LastCarDealCarOffered)
			{
				break;
			}
		}
		if (enumerator.MoveNext())
		{
			return enumerator.Current;
		}
		return carsToOffer.First<string>();
	}

	private static bool FindNextOfferInSet(IEnumerable<string> carsToOffer, IEnumerable<int> discountsToUse, out AgentCarDeal nextDeal)
	{
		if (carsToOffer.Count<string>() == 0)
		{
			nextDeal = null;
			return false;
		}
		nextDeal = CreateDealForCar(FindNextCarToOffer(carsToOffer), discountsToUse);
		return true;
	}

	private static int GetCashbackAmount()
	{
        eCarTier highestUnlockedClass = RaceEventQuery.Instance.getHighestUnlockedClass();
        return GameDatabase.Instance.DealConfiguration.Car.CashbackAmountForUnlockedTier[(int)highestUnlockedClass];
	}

	public static int ApplyDiscountToPrice(int price, int discount)
	{
		return price - price * discount / 100;
	}

	public static bool IsDiscountDealAffordable(int availableGold, int originalPrice, int discountPercentage)
	{
		return availableGold >= ApplyDiscountToPrice(originalPrice, discountPercentage);
	}

	private static bool TryGetDiscountForDeal(string car, IEnumerable<int> discountsToUse, out int discount)
	{
        PlayerProfile profile = PlayerProfileManager.Instance.ActiveProfile;
        CarDeal cardeal = GameDatabase.Instance.DealConfiguration.Car;
        if (profile.LastCarDealDiscountRepeatCount >= cardeal.NumberOfTimesToRepeatDiscount)
        {
            if (!profile.LastCarDealWasCashback)
            {
                discount = 0;
                return false;
            }
            discount = discountsToUse.FirstOrDefault((int q) => q > profile.LastCarDealDiscount);
            if (discount == 0)
            {
                discount = profile.LastCarDealDiscount;
            }
        }
        else if (profile.LastCarDealDiscount > 0)
        {
            discount = profile.LastCarDealDiscount;
        }
        else
        {
            discount = discountsToUse.First<int>();
        }
        int carPrice = CarDatabase.Instance.GetCar(car).GoldPrice;
        int goldAvailable = PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold();
        if (!AgentCarDeal.IsDiscountDealAffordable(goldAvailable, carPrice, discount))
        {
            return true;
        }
        discount = discountsToUse.LastOrDefault((int q) => !AgentCarDeal.IsDiscountDealAffordable(goldAvailable, carPrice, q));
        return discount > 0;
	}

	private static AgentCarDeal CreateDealForCar(string car, IEnumerable<int> discountsToUse)
	{
		int discountPercentage = 0;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if ((car != "AudiTTRS" || activeProfile.LastCarDealTier() == eCarTier.TIER_2 || RaceEventQuery.Instance.getHighestUnlockedClass() != eCarTier.TIER_1) && TryGetDiscountForDeal(car, discountsToUse, out discountPercentage))
		{
            return new AgentCarDealDiscount(car, discountPercentage);
		}
        return new AgentCarDealCashback(car, AgentCarDeal.GetCashbackAmount());
	}

	private static void GetAvailableOffersAndDiscounts(out IEnumerable<string> carsToOffer, out IEnumerable<int> discountsToUse)
	{
        eCarTier unlockedTier = RaceEventQuery.Instance.getHighestUnlockedClass();
        eCarTier eCarTier = unlockedTier;
        BaseCarTierEvents tierEvents = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.GetTierEvents(eCarTier);
        CarDeal cardeal = GameDatabase.Instance.DealConfiguration.Car;
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        carsToOffer = null;
        discountsToUse = null;
        bool userConverted = activeProfile.PlayerBoughtConsumable;//UserManager.Instance.currentAccount.UserConverted;
	    if (tierEvents.CrewBattleEvents.NumEventsComplete() >= cardeal.CrewDefeatedBeforeOfferingNextTier &&
	        eCarTier < eCarTier.TIER_5 &&
	        activeProfile.CarsOwned.Any((CarGarageInstance q) => q.CurrentTier == unlockedTier))
	    {
	        eCarTier++;
	        if (eCarTier != activeProfile.LastCarDealTier())
	        {
	            activeProfile.ResetCarDealsDiscount();
	        }
	        carsToOffer = AgentCarDeal.GetOfferCarsForTier(eCarTier);
	        if (userConverted)
	        {
	            discountsToUse = cardeal.DiscountsForNextCarTier[(int) unlockedTier].DiscountsForSpenders;
	        }
	        else
	        {
	            discountsToUse = cardeal.DiscountsForNextCarTier[(int) unlockedTier].DiscountsForNonSpenders;
	        }
	    }
	    else
	    {
	        carsToOffer = AgentCarDeal.GetOfferCarsForTier(eCarTier.TIER_5);
	        if (unlockedTier < eCarTier.TIER_5 && activeProfile.LastCarDealTier() == eCarTier.TIER_5)
	        {
	            carsToOffer = carsToOffer.Concat(AgentCarDeal.GetOfferCarsForTier(eCarTier.TIER_4));
	        }
	        if (userConverted)
	        {
	            discountsToUse = cardeal.DiscountsForSupercars[(int) unlockedTier].DiscountsForSpenders;
	        }
	        else
	        {
	            discountsToUse = cardeal.DiscountsForSupercars[(int) unlockedTier].DiscountsForNonSpenders;
	        }
	    }
	    carsToOffer = carsToOffer.Except(cardeal.ExcludedCars);
	}

	public static bool TryGetNextAvailableDeal(out AgentCarDeal nextDeal)
	{
		IEnumerable<string> carsToOffer = null;
		IEnumerable<int> discountsToUse = null;
		GetAvailableOffersAndDiscounts(out carsToOffer, out discountsToUse);
		return FindNextOfferInSet(carsToOffer, discountsToUse, out nextDeal);
	}

	public static List<AgentCarDeal> GetAllAvailableDeals()
	{
		IEnumerable<string> enumerable = null;
		IEnumerable<int> discountsToUse = null;
		List<AgentCarDeal> list = new List<AgentCarDeal>();
		GetAvailableOffersAndDiscounts(out enumerable, out discountsToUse);
		enumerable = from q in enumerable
		orderby CarDealOrdering(q)
		select q;
		foreach (string current in enumerable)
		{
			list.Add(CreateDealForCar(current, discountsToUse));
		}
		return list;
	}

    private static bool DoesTransitionDealApply(CarTransitionDeal deal, bool ignoreOldCarRequirement,
        bool ignoreProgression)
    {
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        return RaceEventInfo.Instance.CurrentEventTier == deal.TriggerEventTier &&
               (activeProfile.CarsOwned.Any((CarGarageInstance q) => q.CarDBKey == deal.OldCar) ||
                ignoreOldCarRequirement) &&
               !activeProfile.CarsOwned.Any((CarGarageInstance q) => q.CarDBKey == deal.OfferCar) &&
               (RaceEventInfo.Instance.CurrentEvent.GetProgressionRaceEventNumber() == deal.TriggerEventCrewRace ||
                ignoreProgression);
    }

    public static bool TryGetTransitionDeal(out AgentCarDeal nextDeal, bool ignoreOldCarRequirement = false, bool ignoreProgression = false)
    {
        DealConfiguration dealConfiguration = GameDatabase.Instance.DealConfiguration;
        CarTransitionDeal carTransitionDeal = dealConfiguration.Car.CarTransitionDeals.FirstOrDefault((CarTransitionDeal q) => DoesTransitionDealApply(q, ignoreOldCarRequirement, ignoreProgression));
        if (carTransitionDeal == null)
        {
            nextDeal = null;
            return false;
        }
        nextDeal = new AgentCarDealFreeUpgrades(carTransitionDeal);
        return true;
    }
}
