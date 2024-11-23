using System;
using System.Collections.Generic;

[Serializable]
public class CarDeal
{
	public bool RegularDealsEnabled;

	public bool TransitionDealsEnabled;

	public int NumberOfTimesToRepeatDiscount;

	public List<int> MinRacesBeforeOffer;

	public List<int> MaxRacesBeforeOffer;

	public int CrewDefeatedBeforeOfferingNextTier;

	public int SeasonsBeforeCarCanBeOffered;

	public eCarTier MilestoneBeforeShowingFirstTier;

	public int MilestoneBeforeShowingFirstCrew;

	public List<int> CashbackAmountForUnlockedTier;

	public List<CarDiscount> DiscountsForNextCarTier;

	public List<CarDiscount> DiscountsForSupercars;

	public List<string> ExcludedCars;

	public List<string> PriorityCars;

	public List<CarTransitionDeal> CarTransitionDeals;
}
