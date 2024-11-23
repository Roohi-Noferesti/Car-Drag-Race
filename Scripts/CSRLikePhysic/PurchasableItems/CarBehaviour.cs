using System.Collections.Generic;
using System.Linq;

namespace PurchasableItems
{
	public class CarBehaviour : IBehaviour
	{
		private PlayerProfile Profile;

		public CarBehaviour()
		{
			this.Profile = PlayerProfileManager.Instance.ActiveProfile;
		}

		public bool IsOwned(string value)
		{
			string[] source = value.Split(new char[]
			{
				','
			});
			return source.All((string q) => this.Profile.IsCarOwned(this.CarDBKey(q)));
		}

		public void Apply(string value)
		{
			string[] array = value.Split(new char[]
			{
				','
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string value2 = array2[i];
				string text = this.CarDBKey(value2);
				this.AwardCar(text, this.IsElite(value2), this.Profile.IsCarOwned(text));
			}
			this.Profile.UpdateCurrentCarSetup();
			this.Profile.UpdateCurrentPhysicsSetup();
			PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
		}

		private void AwardCar(string carDBKey, bool IsElite, bool alreadyOwned)
		{
			this.Profile.GiveCar(carDBKey, 0, alreadyOwned);
			CarGarageInstance carFromID = this.Profile.GetCarFromID(carDBKey);
			carFromID.EliteCar |= IsElite;
		}

		public void Revoke(string value)
		{
			List<string> carsToRemove = value.Split(new char[]
			{
				','
			}).ToList<string>();
			this.Profile.CarsOwned.RemoveAll((CarGarageInstance car) => carsToRemove.Contains(car.CarDBKey));
		}

		protected string CarDBKey(string value)
		{
			return value.Replace("_Elite", string.Empty);
		}

		protected bool IsElite(string value)
		{
			return value.Contains("_Elite");
		}
	}
}
