using System;
using System.Collections.Generic;

public class PrizeomaticController
{
	public List<PrizeOMaticCard> cards = new List<PrizeOMaticCard>();

	public int numOfAttempts;

	public int numOfBonusAttempts;

	public int numOfCarPieces;

	public string carToAwardDBKey;

	public bool HasWonFuelTank;

	public Reward AwardedCarPartType;

	public List<int> carPartsWon = new List<int>();
	
	public bool IsBonusRewardsTurn;

	public static PrizeomaticController Instance;

	public static void Create()
	{
		if (PrizeomaticController.Instance == null)
		{
			PrizeomaticController.Instance = new PrizeomaticController();
			PrizeomaticController.Instance.IsBonusRewardsTurn = false;
		}
	}

	public static void Destroy()
	{
		PrizeomaticController.Instance = null;
	}
}
