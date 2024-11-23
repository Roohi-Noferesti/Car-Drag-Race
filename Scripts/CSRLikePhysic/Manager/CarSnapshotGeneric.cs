using System;

public class CarSnapshotGeneric : CarSnapshotBase
{
	public void Setup(string carDBKey, int appliedColourIndex, string appliedLivery, Action loadedCallback = null)
	{
		this.loadedCallback = loadedCallback;
		CarGarageInstance carGarageInstance = new CarGarageInstance();
	    var carinfo = CarDatabase.Instance.GetCar(carDBKey);
        carGarageInstance.SetupNewGarageInstance(carinfo);
		carGarageInstance.AppliedColourIndex = appliedColourIndex;
		carGarageInstance.ApplyLivery(appliedLivery);
		base.LoadCarSnapshot(carGarageInstance, null);
	}

	public void Setup(CarGarageInstance garageInstance, Action loadedCallback = null)
	{
		this.loadedCallback = loadedCallback;
		base.LoadCarSnapshot(garageInstance, null);
	}
}
