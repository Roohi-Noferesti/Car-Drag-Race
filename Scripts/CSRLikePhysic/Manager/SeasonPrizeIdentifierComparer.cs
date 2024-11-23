using System;
using System.Collections.Generic;

public class SeasonPrizeIdentifierComparer : IComparer<SeasonPrizeIdentifier>
{
	public int Compare(SeasonPrizeIdentifier first, SeasonPrizeIdentifier second)
	{
		if (first == null && second == null)
		{
			return 0;
		}
		if (first == null)
		{
			return 1;
		}
		if (second == null)
		{
			return -1;
		}
		if (first.LeaderboardID < second.LeaderboardID)
		{
			return -1;
		}
		if (first.LeaderboardID > second.LeaderboardID)
		{
			return 1;
		}
		SeasonPrizeMetadata prizeMetadata = GameDatabase.Instance.SeasonPrizes.GetPrizeMetadata(first);
		SeasonPrizeMetadata prizeMetadata2 = GameDatabase.Instance.SeasonPrizes.GetPrizeMetadata(second);
		if (prizeMetadata.Type > prizeMetadata2.Type)
		{
			return -1;
		}
		if (prizeMetadata.Type < prizeMetadata2.Type)
		{
			return 1;
		}
		switch (prizeMetadata.Type)
		{
		case SeasonPrizeMetadata.ePrizeType.Car:
		{
			bool flag = CarDatabase.Instance.PeekGetCar(prizeMetadata.Data);
			bool flag2 = CarDatabase.Instance.PeekGetCar(prizeMetadata2.Data);
			if (!flag && !flag2)
			{
				return 0;
			}
			if (flag && !flag2)
			{
				return 1;
			}
			if (!flag && flag2)
			{
				return 1;
			}
			CarInfo car = CarDatabase.Instance.GetCar(prizeMetadata.Data);
			CarInfo car2 = CarDatabase.Instance.GetCar(prizeMetadata2.Data);
			return car.BaseCarTier.CompareTo(car2.BaseCarTier);
		}
		case SeasonPrizeMetadata.ePrizeType.Gold:
		case SeasonPrizeMetadata.ePrizeType.Cash:
		{
			int num = -1;
			int value = -1;
			prizeMetadata.GetPrizeDataAsInt(out num);
			prizeMetadata2.GetPrizeDataAsInt(out value);
			return num.CompareTo(value);
		}
		}
		return 0;
	}
}
