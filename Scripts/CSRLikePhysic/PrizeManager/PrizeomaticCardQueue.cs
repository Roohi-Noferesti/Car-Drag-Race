using System;
using System.Collections.Generic;

public static class PrizeomaticCardQueue
{
	public static List<Reward> queuedRewards
	{
		get;
		private set;
	}

	static PrizeomaticCardQueue()
	{
		PrizeomaticCardQueue.queuedRewards = new List<Reward>();
	}

	public static bool IsThereAPrizeQueued()
	{
		return PrizeomaticCardQueue.queuedRewards.Count > 0;
	}

	public static void AddPrizeToQueue(Reward prize)
	{
		PrizeomaticCardQueue.queuedRewards.Add(prize);
	}

	public static Reward GetPrizeFromQueue()
	{
		if (PrizeomaticCardQueue.queuedRewards.Count <= 0)
		{
			return Reward.RPTiny;
		}
		Reward result = PrizeomaticCardQueue.queuedRewards[0];
		PrizeomaticCardQueue.queuedRewards.RemoveAt(0);
		return result;
	}

	public static int NumberOfPrizesQueued()
	{
		return PrizeomaticCardQueue.queuedRewards.Count;
	}

	public static void RemoveReward(int index)
	{
		PrizeomaticCardQueue.queuedRewards.RemoveAt(index);
	}

	public static string GetLastPrizeQueuedString()
	{
		int count = PrizeomaticCardQueue.queuedRewards.Count;
		if (count <= 0)
		{
			return string.Empty;
		}
		Reward reward = PrizeomaticCardQueue.queuedRewards[count - 1];
		return string.Concat(new object[]
		{
			"Added  ",
			reward.ToString(),
			" prize. ",
			count,
			" prizes are queued "
		});
	}
}
