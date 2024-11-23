using System;
using UnityEngine;

public class GarageCarLoader
{
	private CarGarageInstance _garageCar;

	private AsyncBundleSlotDescription _carSlot;

	private AsyncBundleSlotDescription _liverySlot;

	private GameObject _owner;

	private CarInfo _carInfo;

	private string _lastCarLoaded;

	private Action<CarGarageInstance, GameObject, GameObject> _succesCallback;

	private Action<CarGarageInstance> _failCallback;

	private bool _carLoaded;

	private bool _liveryRequired;

	private bool _liveryLoaded;

	private bool _loadOk;

	private bool _aborted;

	public string LastCarLoaded
	{
		get
		{
			return this._lastCarLoaded;
		}
		set
		{
			this._lastCarLoaded = value;
		}
	}

	public GarageCarLoader(CarGarageInstance car, AsyncBundleSlotDescription carSlot, AsyncBundleSlotDescription liverySlot, GameObject owner)
	{
		this._garageCar = car;
		this._carSlot = carSlot;
		this._liverySlot = liverySlot;
		this._owner = owner;
		this._carInfo = CarDatabase.Instance.GetCar(car.CarDBKey);
	}

	public void Abort()
	{
		this._aborted = true;
		if (this._failCallback != null)
		{
			this._failCallback(this._garageCar);
		}
	}

	public void Load(Action<CarGarageInstance, GameObject, GameObject> succes, Action<CarGarageInstance> fail)
	{
		this._aborted = false;
		this._carLoaded = false;
		this._liveryRequired = false;
		this._liveryLoaded = false;
		this._loadOk = true;
		this._succesCallback = succes;
		this._failCallback = fail;
		bool useExisting = this._lastCarLoaded == this._carInfo.Key;
		this._lastCarLoaded = this._carInfo.Key;
        if (AsyncSwitching.IsLiveryName(this._garageCar.AppliedLiveryName))
        {
            this._liveryRequired = true;
            AsyncSwitching.Instance.RequestAsset(this._liverySlot, this._garageCar.AppliedLiveryName, new BundleCallbackDelegate(this.LiveryLoaded), this._owner, true, null);
        }
        else
        {
            this._liveryLoaded = true;
        }
		string name = this._carInfo.ModelPrefabString;
		if (!string.IsNullOrEmpty(this._carInfo.EliteOverrideCarAssetID) && this._garageCar.IsEliteLiveryApplied)
		{
			name = this._carInfo.EliteOverrideCarAssetID;
		}
        AsyncSwitching.Instance.RequestAsset(this._carSlot, name, this.CarLoaded, this._owner, useExisting, delegate
        {
            this._failCallback(this._garageCar);
        });
	}

	private void LiveryLoaded(bool succes, string name)
	{
		if (this._aborted)
		{
			return;
		}
		if (succes)
		{
		}
		this._liveryLoaded = true;
		this.Loaded();
	}

	private void CarLoaded(bool succes, string name)
	{
		if (this._aborted)
		{
			return;
		}
		this._carLoaded = true;
		if (!succes)
		{
			this._loadOk = false;
		}
		this.Loaded();
	}

	private void Loaded()
	{
		if (this._aborted)
		{
			return;
		}
		if (!this._liveryLoaded || !this._carLoaded)
		{
			return;
		}
		if (!this._loadOk)
		{
			this._failCallback(this._garageCar);
			return;
		}
	    GameObject car = AsyncSwitching.Instance.GetCar(this._carSlot);
	    GameObject arg = (!this._liveryRequired) ? null : AsyncSwitching.Instance.GetLivery(this._liverySlot);
		CarVisuals component = car.GetComponent<CarVisuals>();
		if (component != null && this._carInfo.IsBossCarOverride)
		{
			Color forcedColor = new Color();// = new Color(this._carInfo.ColourOverrideR, this._carInfo.ColourOverrideG, this._carInfo.ColourOverrideB);
			component.ForceColor(forcedColor);
		}
		if (this._succesCallback != null)
		{
			this._succesCallback(this._garageCar, car, arg);
		}
	}
}
