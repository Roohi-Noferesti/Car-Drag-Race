using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CarePackageRewardDetails
{
	public int Gold;

	public int Cash;

	public int Upgrades;

	public List<CarePackageConsumableReward> Consumables;

	public int NonNegativeGold
	{
		get
		{
			return this.nonNegativeRewardValue(this.Gold);
		}
	}

	public int NonNegativeCash
	{
		get
		{
			return this.nonNegativeRewardValue(this.Cash);
		}
	}

	public int NonNegativeUpgrades
	{
		get
		{
			return this.nonNegativeRewardValue(this.Upgrades);
		}
	}

	public CarePackageRewardDetails()
	{
		this.Gold = 0;
		this.Cash = 0;
		this.Upgrades = 0;
		this.Consumables = new List<CarePackageConsumableReward>();
	}

	public CarePackageRewardDetails(CarePackageRewardDetails other)
	{
		this.Gold = other.Gold;
		this.Cash = other.Cash;
		this.Upgrades = other.Upgrades;
		this.Consumables = new List<CarePackageConsumableReward>(other.Consumables);
	}

	public void GiveToPlayer()
	{
		this.Consumables.ForEach(delegate(CarePackageConsumableReward consumable)
		{
			consumable.GiveToPlayer();
		});
		PlayerProfileManager.Instance.ActiveProfile.AddGold(this.NonNegativeGold,"reward","CarePackageConsumableReward");
		PlayerProfileManager.Instance.ActiveProfile.AddCash(this.NonNegativeCash,"reward","CarePackageConsumableReward");
		PlayerProfileManager.Instance.ActiveProfile.AddFreeUpgrade(this.NonNegativeUpgrades);
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	private int nonNegativeRewardValue(int value)
	{
		if (value >= 0)
		{
			return value;
		}
		return 0;
	}

	public static CarePackageRewardDetails operator +(CarePackageRewardDetails left, CarePackageRewardDetails right)
	{
		return new CarePackageRewardDetails
		{
			Gold = left.Gold + right.Gold,
			Cash = left.Cash + right.Cash,
			Upgrades = left.Upgrades + right.Upgrades,
			Consumables = left.Consumables.Concat(right.Consumables).ToList<CarePackageConsumableReward>()
		};
	}
}
