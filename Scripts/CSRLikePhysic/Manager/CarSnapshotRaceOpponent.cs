using System;
using UnityEngine;

public class CarSnapshotRaceOpponent : CarSnapshotBase
{
	public void Setup(RaceEventData race, Action loadedCallback = null)
	{
		if (race != null)
		{
			string aICar = race.AICar;
			CarGarageInstance carGarageInstance = new CarGarageInstance();
		    var carinfo = CarDatabase.Instance.GetCar(aICar);
            carGarageInstance.SetupNewGarageInstance(carinfo);
			carGarageInstance.ApplyLivery(race.AIDriverLivery);
			carGarageInstance.NumberPlate = new NumberPlate();
			carGarageInstance.NumberPlate.Text = GameDatabase.Instance.AIPlayers.GetAIDriverData(race.AIDriver).NumberPlateString;
			if (false)//race.UseAIColour)
			{
                //carGarageInstance.ColorOverride = race.GetAIColour();
                //carGarageInstance.UseColorOverride = true;
			}
			else
			{
				carGarageInstance.ColorOverride = Color.magenta;
				carGarageInstance.UseColorOverride = false;
			}
			base.LoadCarSnapshot(carGarageInstance, null);
		}
	}
}
