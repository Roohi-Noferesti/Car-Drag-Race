using System;
using UnityEngine;

public class RaceCarLightingProcessor : MonoBehaviour
{
	private CarVisuals _targetCar;

	[HideInInspector]
	public float Weight;

	public Light ResultLight
	{
		get;
		protected set;
	}

	public CarVisuals TargetCar
	{
		get
		{
			return this._targetCar;
		}
		set
		{
			this._targetCar = value;
			base.enabled = (this._targetCar != null);
		}
	}
}
