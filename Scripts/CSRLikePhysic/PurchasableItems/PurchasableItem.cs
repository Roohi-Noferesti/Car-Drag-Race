using System;
using System.Collections.Generic;
using UnityEngine;

namespace PurchasableItems
{
	[Serializable]
	public class PurchasableItem
	{
		public enum ProductType
		{
			BundleCar,
			Car,
			FuelTank
		}

		private static Dictionary<ProductType, Type> ProductTypeMapping = new Dictionary<ProductType, Type>
		{
			{
				ProductType.BundleCar,
				typeof(CarBehaviour)
			},
			{
				ProductType.Car,
				typeof(CarBehaviour)
			},
			{
				ProductType.FuelTank,
				typeof(FuelTankBehaviour)
			}
		};

        public string IAPCode;

        public ProductType Type;

		public bool IsAvailable;

		public string Value;


		[NonSerialized]
		private IBehaviour AssociatedBehaviour = new DummyBehaviour();

		public bool IsOwned
		{
			get
			{
				return this.AssociatedBehaviour.IsOwned(this.Value);
			}
		}

		public void Initialise()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			Type type;
			if (ProductTypeMapping.TryGetValue((ProductType)this.Type, out type))
			{
				this.AssociatedBehaviour = (Activator.CreateInstance(type) as IBehaviour);
			}
		}

		public void Apply()
		{
			if (this.IsOwned)
			{
				return;
			}
			this.AssociatedBehaviour.Apply(this.Value);
		}

		public void Revoke()
		{
			if (!this.IsOwned)
			{
				return;
			}
			this.AssociatedBehaviour.Revoke(this.Value);
		}
	}
}
