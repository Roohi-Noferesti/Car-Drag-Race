using System;
using System.Collections.Generic;
using UnityEngine;

public static class PrizeomaticCarPieceChooser
{
	public enum CARSLOT
	{
		Rare,
		Desirible1,
		Desirible2,
		Common1,
		Common2,
		Common3,
		MAX
	}

	private static string[] _CurrentSlotKeys = new string[6];

	private static List<string> _CommonCarDBKeys = null;

	private static List<string> _DesirableCarDBKeys = null;

	private static List<string> _RareCarDBKeys = null;

	private static readonly Dictionary<Reward, PrizeomaticCarPieceChooser.CARSLOT> rewardToCardSlot = new Dictionary<Reward, PrizeomaticCarPieceChooser.CARSLOT>
	{
		{
			Reward.SportCarPart,
			PrizeomaticCarPieceChooser.CARSLOT.Rare
		},
		{
			Reward.DesiribleCarPart1,
			PrizeomaticCarPieceChooser.CARSLOT.Desirible1
		},
		{
			Reward.DesiribleCarPart2,
			PrizeomaticCarPieceChooser.CARSLOT.Desirible2
		},
		{
			Reward.CommonCarPart1,
			PrizeomaticCarPieceChooser.CARSLOT.Common1
		},
		{
			Reward.CommonCarPart2,
			PrizeomaticCarPieceChooser.CARSLOT.Common2
		},
		{
			Reward.CommonCarPart3,
			PrizeomaticCarPieceChooser.CARSLOT.Common3
		}
	};

	private static bool SetUpCarKeys()
	{
		List<WinnableCar> winnableCars = GameDatabase.Instance.Online.GetWinnableCars();
		if (winnableCars == null)
		{
			return false;
		}
		PrizeomaticCarPieceChooser._CommonCarDBKeys = new List<string>();
		PrizeomaticCarPieceChooser._DesirableCarDBKeys = new List<string>();
		PrizeomaticCarPieceChooser._RareCarDBKeys = new List<string>();
		for (int i = 0; i < winnableCars.Count; i++)
		{
			WinnableCar winnableCar = winnableCars[i];
			if (winnableCar != null)
			{
				switch (winnableCar.Rarity)
				{
				case CarRarity.Common:
					PrizeomaticCarPieceChooser._CommonCarDBKeys.Add(winnableCar.CarDBKey);
					break;
				case CarRarity.Rare:
					PrizeomaticCarPieceChooser._DesirableCarDBKeys.Add(winnableCar.CarDBKey);
					break;
				case CarRarity.SuperRare:
					PrizeomaticCarPieceChooser._RareCarDBKeys.Add(winnableCar.CarDBKey);
					break;
				}
			}
		}
		return true;
	}

	public static void UpdateCarPool(bool purchasedPiece)
	{
		if ((PrizeomaticCarPieceChooser._CommonCarDBKeys == null || PrizeomaticCarPieceChooser._DesirableCarDBKeys == null || PrizeomaticCarPieceChooser._RareCarDBKeys == null) && !PrizeomaticCarPieceChooser.SetUpCarKeys())
		{
			return;
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		for (int i = 0; i < PrizeomaticCarPieceChooser._CurrentSlotKeys.Length; i++)
		{
			PrizeomaticCarPieceChooser._CurrentSlotKeys[i] = string.Empty;
		}
		for (int j = 0; j < PrizeomaticCarPieceChooser._CommonCarDBKeys.Count; j++)
		{
			if (activeProfile.CarIsAPartiallyWonMultiplayerCar(PrizeomaticCarPieceChooser._CommonCarDBKeys[j]))
			{
				if (string.IsNullOrEmpty(PrizeomaticCarPieceChooser._CurrentSlotKeys[3]))
				{
					PrizeomaticCarPieceChooser._CurrentSlotKeys[3] = PrizeomaticCarPieceChooser._CommonCarDBKeys[j];
				}
				else if (string.IsNullOrEmpty(PrizeomaticCarPieceChooser._CurrentSlotKeys[4]))
				{
					PrizeomaticCarPieceChooser._CurrentSlotKeys[4] = PrizeomaticCarPieceChooser._CommonCarDBKeys[j];
				}
				else if (string.IsNullOrEmpty(PrizeomaticCarPieceChooser._CurrentSlotKeys[5]))
				{
					PrizeomaticCarPieceChooser._CurrentSlotKeys[5] = PrizeomaticCarPieceChooser._CommonCarDBKeys[j];
				}
			}
		}
		for (int k = 0; k < PrizeomaticCarPieceChooser._DesirableCarDBKeys.Count; k++)
		{
			if (activeProfile.CarIsAPartiallyWonMultiplayerCar(PrizeomaticCarPieceChooser._DesirableCarDBKeys[k]))
			{
				if (string.IsNullOrEmpty(PrizeomaticCarPieceChooser._CurrentSlotKeys[1]))
				{
					PrizeomaticCarPieceChooser._CurrentSlotKeys[1] = PrizeomaticCarPieceChooser._DesirableCarDBKeys[k];
				}
				else if (string.IsNullOrEmpty(PrizeomaticCarPieceChooser._CurrentSlotKeys[2]))
				{
					PrizeomaticCarPieceChooser._CurrentSlotKeys[2] = PrizeomaticCarPieceChooser._DesirableCarDBKeys[k];
				}
			}
		}
		for (int l = 0; l < PrizeomaticCarPieceChooser._RareCarDBKeys.Count; l++)
		{
			if (activeProfile.CarIsAPartiallyWonMultiplayerCar(PrizeomaticCarPieceChooser._RareCarDBKeys[l]) && string.IsNullOrEmpty(PrizeomaticCarPieceChooser._CurrentSlotKeys[0]))
			{
				PrizeomaticCarPieceChooser._CurrentSlotKeys[0] = PrizeomaticCarPieceChooser._RareCarDBKeys[l];
			}
		}
		for (int m = 0; m < 6; m++)
		{
			PrizeomaticCarPieceChooser.CARSLOT currentCarSlot = (PrizeomaticCarPieceChooser.CARSLOT)m;
			if (string.IsNullOrEmpty(PrizeomaticCarPieceChooser._CurrentSlotKeys[m]) || activeProfile.CarIsAFullyWonMultiplayerCar(PrizeomaticCarPieceChooser._CurrentSlotKeys[m]))
			{
				PrizeomaticCarPieceChooser._CurrentSlotKeys[m] = PrizeomaticCarPieceChooser.GetNewCarDBKey(currentCarSlot);
			}
		}
	}

	public static bool AllCarsCompleted()
	{
		PrizeomaticCarPieceChooser.UpdateCarPool(false);
		List<WinnableCar> winnableCars = GameDatabase.Instance.Online.GetWinnableCars();
		for (int i = 0; i < winnableCars.Count; i++)
		{
			if (!PlayerProfileManager.Instance.ActiveProfile.CarIsAFullyWonMultiplayerCar(winnableCars[i].CarDBKey))
			{
				return false;
			}
		}
		return true;
	}

	public static bool AllCarsCompletedForRarity(CarRarity rarity)
	{
		PrizeomaticCarPieceChooser.UpdateCarPool(false);
		List<WinnableCar> winnableCars = GameDatabase.Instance.Online.GetWinnableCars();
		List<WinnableCar> list = winnableCars.FindAll((WinnableCar q) => q.Rarity == rarity && !PlayerProfileManager.Instance.ActiveProfile.CarIsAFullyWonMultiplayerCar(q.CarDBKey));
		return list.Count == 0;
	}

	public static string GetCarKey(PrizeomaticCarPieceChooser.CARSLOT slot)
	{
		return PrizeomaticCarPieceChooser._CurrentSlotKeys[(int)slot];
	}

	private static string GetNewCarDBKey(PrizeomaticCarPieceChooser.CARSLOT currentCarSlot)
	{
		List<string> list = PrizeomaticCarPieceChooser.GetCarsToWin(currentCarSlot);
		if (list == null || list.Count == 0)
		{
			return string.Empty;
		}
		list = list.FindAll((string x) => !PlayerProfileManager.Instance.ActiveProfile.CarIsAFullyWonMultiplayerCar(x));
		list = list.FindAll((string x) => !PlayerProfileManager.Instance.ActiveProfile.CarIsAPartiallyWonMultiplayerCar(x));
		list = list.FindAll((string x) => !PrizeomaticCarPieceChooser.CarIsAlreadyAssignedToSlot(x));
		if (currentCarSlot == PrizeomaticCarPieceChooser.CARSLOT.Common1 || currentCarSlot == PrizeomaticCarPieceChooser.CARSLOT.Common2 || currentCarSlot == PrizeomaticCarPieceChooser.CARSLOT.Common3)
		{
			list = list.FindAll((string x) => !PrizeomaticCarPieceChooser.CarIsAHigherTier(x));
		}
		if (list.Count == 0)
		{
			return string.Empty;
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		string text = list[index];
		PrizeomaticCarPieceChooser.SetCarDBKeyForSlot(text, currentCarSlot);
		return text;
	}

	private static bool CarIsAlreadyAssignedToSlot(string carkey)
	{
		for (int i = 0; i < 6; i++)
		{
			if (PrizeomaticCarPieceChooser._CurrentSlotKeys[i] == carkey)
			{
				return true;
			}
		}
		return false;
	}

	private static bool CarIsAHigherTier(string carkey)
	{
		CarInfo car = CarDatabase.Instance.GetCar(carkey);
		return car == null || car.BaseCarTier > RaceEventQuery.Instance.getHighestUnlockedClass();
	}

    private static List<string> GetCarsToWin(PrizeomaticCarPieceChooser.CARSLOT carslot)
    {
        switch (carslot)
        {
            case PrizeomaticCarPieceChooser.CARSLOT.Rare:
                return PrizeomaticCarPieceChooser._RareCarDBKeys;
            case PrizeomaticCarPieceChooser.CARSLOT.Desirible1:
            case PrizeomaticCarPieceChooser.CARSLOT.Desirible2:
                return PrizeomaticCarPieceChooser._DesirableCarDBKeys;
            case PrizeomaticCarPieceChooser.CARSLOT.Common1:
            case PrizeomaticCarPieceChooser.CARSLOT.Common2:
            case PrizeomaticCarPieceChooser.CARSLOT.Common3:
                return PrizeomaticCarPieceChooser._CommonCarDBKeys;
            default:
                return null;
        }
    }

    private static void SetCarDBKeyForSlot(string newDBKey, PrizeomaticCarPieceChooser.CARSLOT prizeSlot)
	{
		PrizeomaticCarPieceChooser._CurrentSlotKeys[(int)prizeSlot] = newDBKey;
	}

	public static PrizeomaticCarPieceChooser.CARSLOT ConvertRewardToCarslot(Reward rewardType)
	{
		if (PrizeomaticCarPieceChooser.rewardToCardSlot.ContainsKey(rewardType))
		{
			return PrizeomaticCarPieceChooser.rewardToCardSlot[rewardType];
		}
		return PrizeomaticCarPieceChooser.CARSLOT.MAX;
	}
}
