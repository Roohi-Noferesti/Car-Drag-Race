using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;

public class RaceEventDifficulty
{
	public enum Rating
	{
		Easy,
		Challenging,
		Difficult,
		Extreme
	}

	private string[] Strings = new string[]
	{
		"TEXT_DIFFICULTY_EASY",
		"TEXT_DIFFICULTY_CHALLENGING",
		"TEXT_DIFFICULTY_HARD",
		"TEXT_DIFFICULTY_EXTREME"
	};

	private Color[] Colours = new Color[]
	{
		new Color(0.47f, 0.53f, 0.6f, 1f),
		new Color(0.79f, 1f, 0.44f, 1f),
		new Color(1f, 0.54f, 0f, 1f),
		new Color(0.7f, 0.102f, 0.102f, 1f)
	};

	private readonly string textureFile = "Map_Screen/map_difficulty_";

	private string[] TextureEnds = new string[]
	{
		"01_easy",
		"02_challenging",
		"03_hard",
		"04_extreme"
	};

	public static RaceEventDifficulty Instance
	{
		get;
		private set;
	}

	public static void Create()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = new RaceEventDifficulty();
	}

	public string GetString(Rating zRating)
	{
		return LocalizationManager.GetTranslation(this.Strings[(int)zRating]);
	}

	public Color GetColour(int zIndex)
	{
		return this.Colours[zIndex];
	}

	public Color GetColour(Rating zRating)
	{
		return this.GetColour((int)zRating);
	}

	public Color GetColour(float percentageDifficulty)
	{
		int num = EnumHelper.CountNames<Rating>();
		int num2 = Mathf.FloorToInt(percentageDifficulty * (float)num);
		num2 = Mathf.Clamp(num2, 0, num - 1);
		return this.GetColour(num2);
	}

	public string GetTexture(Rating zRating)
	{
		return this.textureFile + this.TextureEnds[(int)zRating];
	}

	public int GetHumanCarPPIndex(RaceEventData zData)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		CarGarageInstance carFromID = activeProfile.GetCarFromID(zData.GetHumanCar());
		if (carFromID != null)
		{
			return carFromID.CurrentPPIndex;
		}
		if (zData.SwitchBackRace)
		{
			return zData.Group.RaceEvents[1].GetAIPerformancePotentialIndex();
		}
		return 0;
	}

	public Rating GetRating(RaceEventData zData, bool relayLeg = false)
	{
        if (zData.EventID == 602)
        {
            return Rating.Easy;
        }
		if (zData.IsDailyBattle())
		{
			return Rating.Challenging;
		}
		if (zData.AutoHeadstart)
		{
			if (zData.Group.IsGrindAutoHeadstart)
			{
				return (Rating)zData.Group.RaceEvents.FindIndex((RaceEventData x) => x == zData);
			}
			return Rating.Difficult;
		}
		else
		{
			int playerCarPPIndex = 0;
			if (zData.IsTestDriveAndCarSetup())
			{
				playerCarPPIndex = zData.LoanCarGarageInstance.CurrentPPIndex;
			}
			else if (!string.IsNullOrEmpty(zData.GetHumanCar()))
			{
				if (relayLeg || !zData.IsRelay)
				{
					playerCarPPIndex = this.GetHumanCarPPIndex(zData);
				}
				else if (zData.Group != null && zData.Group.RaceEvents != null)
				{
					return RelayManager.GetDifficultyForRelay();
				}
			}
			else if (zData.ForceUserInCar)
			{
				IEnumerable<CarGarageInstance> compatibleCars = zData.GetCompatibleCars();
				if (compatibleCars.Any<CarGarageInstance>())
				{
					IOrderedEnumerable<CarGarageInstance> source = (from p in compatibleCars
					orderby p.CurrentPPIndex descending
					select p).ThenBy((CarGarageInstance b) => CarDataDefaults.IsBossCar(b.CarDBKey));
					playerCarPPIndex = source.First<CarGarageInstance>().CurrentPPIndex;
				}
				else
				{
					PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
					playerCarPPIndex = activeProfile.GetCurrentCar().CurrentPPIndex;
				}
			}
			else
			{
				PlayerProfile activeProfile2 = PlayerProfileManager.Instance.ActiveProfile;
				playerCarPPIndex = activeProfile2.GetCurrentCar().CurrentPPIndex;
			}
			int aIPerformancePotentialIndex = zData.GetAIPerformancePotentialIndex();
			int ppDif = playerCarPPIndex - aIPerformancePotentialIndex;
			if (zData.AutoDifficulty)
			{
                return this.GetRatingForPPDeltaIndexForMatchRace(ppDif);
			}
			if (PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining > 0)
			{
				ppDif += GameDatabase.Instance.Relay.GetMechanicPPBoost();
			}

            return this.GetRatingForPPDeltaIndex(ppDif);
		}
	}

	protected Rating GetRatingForTimeDelta(float deltaTime)
	{
		if (deltaTime < -0.361f)
		{
			return Rating.Easy;
		}
		if (deltaTime < -0.072f)
		{
			return Rating.Challenging;
		}
		if (deltaTime < 0.144f)
		{
			return Rating.Difficult;
		}
		return Rating.Extreme;
	}

	protected Rating GetRatingForPPDeltaIndexForMatchRace(int deltaIndex)
	{
		if (deltaIndex >= -TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Easy)
		{
			return Rating.Easy;
		}
		if (deltaIndex >= -TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Challenging)
		{
			return Rating.Challenging;
		}
		if (deltaIndex >= -TierXManager.Instance.ThemeDescriptor.DifficultyDeltas.Difficult)
		{
			return Rating.Difficult;
		}
		return Rating.Extreme;
	}

    protected Rating GetRatingForPPDeltaIndex(int deltaIndex)
    {
        if (deltaIndex > 25)
        {
            return Rating.Easy;
        }
        if (deltaIndex > 5)
        {
            return Rating.Challenging;
        }
        if (deltaIndex > -10)
        {
            return Rating.Difficult;
        }
        return Rating.Extreme;
    }

	protected Rating ReduceRating(Rating currentRating)
	{
		int num = currentRating - Rating.Challenging;
		if (num < 0)
		{
			return Rating.Easy;
		}
		return (Rating)num;
	}

	public Rating GetOnlineRating(PlayerReplay replayData)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int currentPPIndex = activeProfile.GetCurrentCar().CurrentPPIndex;
		float num = CarPerformanceIndexCalculator.GetQMTimeForPPIndex(currentPPIndex);
		float deltaTime = num - replayData.replayData.finishTime;
		Rating rating = this.GetDifficultyForDeltaTime(deltaTime);
        //if (MultiplayerUtils.FakeAComsumableActive && rating < RaceEventDifficulty.Rating.Difficult)
        //{
        //    rating = RaceEventDifficulty.Rating.Difficult;
        //}
		Rating rating2 = rating;
		if (activeProfile.IsConsumableActive(eCarConsumables.EngineTune))
		{
			rating = this.ReduceRating(rating);
		}
		if (activeProfile.IsConsumableActive(eCarConsumables.Nitrous) && activeProfile.GetCurrentCar().UpgradeStatus[eUpgradeType.NITROUS].levelFitted > 0)
		{
			rating = this.ReduceRating(rating);
		}
		if (activeProfile.IsConsumableActive(eCarConsumables.Tyre))
		{
			rating = this.ReduceRating(rating);
		}
		if (rating2 - rating > 1)
		{
			rating = rating2 - 1;
		}
		bool flag = false;
		if (activeProfile.IsConsumableActive(eCarConsumables.EngineTune))
		{
			num -= 0.15f;
			flag = true;
		}
		if (activeProfile.IsConsumableActive(eCarConsumables.Tyre))
		{
			flag = true;
			num -= 0.08f;
		}
		if (activeProfile.IsConsumableActive(eCarConsumables.Nitrous))
		{
			flag = true;
			num -= 0.12f;
		}
		if (flag)
		{
			deltaTime = num - replayData.replayData.finishTime;
			Rating difficultyForDeltaTime = this.GetDifficultyForDeltaTime(deltaTime);
			if (difficultyForDeltaTime < rating)
			{
				rating = difficultyForDeltaTime;
			}
		}
		return rating;
	}

	public Rating GetDifficultyForDeltaTime(float deltaTime)
	{
		DifficultySettings difficultySettings = null;//= DifficultyManager.GetDifficultySettings(DifficultyManager.CurrentEventType);
		if (deltaTime >= difficultySettings.DifficultyRatingExtreme)
		{
			return Rating.Extreme;
		}
		if (deltaTime >= difficultySettings.DifficultyRatingDifficult)
		{
			return Rating.Difficult;
		}
		if (deltaTime >= difficultySettings.DifficultyRatingChallenging)
		{
			return Rating.Challenging;
		}
		return Rating.Easy;
	}
}
