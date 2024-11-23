using UnityEngine;

namespace PurchasableItems
{
	public class FuelTankBehaviour : IBehaviour
	{
		private PlayerProfile Profile;

		public FuelTankBehaviour()
		{
			if (Application.isPlaying)
			{
				this.Profile = PlayerProfileManager.Instance.ActiveProfile;
			}
		}

		public bool IsOwned(string value)
		{
			return this.Profile.HasUpgradedFuelTank;
		}

		public void Apply(string value)
		{
			this.Profile.HasUpgradedFuelTank = true;
			if (!this.Profile.HasReceivedFuelTankUpgradeRefill)
			{
				this.Profile.HasReceivedFuelTankUpgradeRefill = true;
				FuelManager.Instance.FillTank(FuelAnimationLockAction.DONTCARE);
			}
			FuelManager.Instance.OnFuelTankUpgraded();
		}

		public void Revoke(string value)
		{
			this.Profile.HasUpgradedFuelTank = false;
			FuelManager.Instance.OnFuelTankUpgradeRevoked();
		}
	}
}
