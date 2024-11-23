using System;

public abstract class RewardBase
{
	public int[] Weight = new int[4];

	public int Chance
	{
		get;
		set;
	}

	public virtual ERewardType RewardType
	{
		get;
		set;
	}

	public virtual int GetRewardAmount()
	{
		return 1;
	}

	public virtual bool SetRewardDataValue(string key_name, string value)
	{
		int num = 0;
		switch (key_name)
		{
		case "Chance":
		{
			int chance;
			if (int.TryParse(value, out chance))
			{
				this.Chance = chance;
			}
			return true;
		}
		case "Weight_Bronze":
			if (int.TryParse(value, out num))
			{
				this.Weight[0] = num;
			}
			return true;
		case "Weight_Silver":
			if (int.TryParse(value, out num))
			{
				this.Weight[1] = num;
			}
			return true;
		case "Weight_Gold":
			if (int.TryParse(value, out num))
			{
				this.Weight[2] = num;
			}
			return true;
		}
		return false;
	}

	public virtual void AwardReward(MetricsTrackingID metricsId)
	{
	}

	public void AwardReward()
	{
		this.AwardReward(new MetricsTrackingID());
	}
}
