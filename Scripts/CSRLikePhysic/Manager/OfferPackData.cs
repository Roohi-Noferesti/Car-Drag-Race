using DataSerialization;
using System;
using System.Collections.Generic;

[Serializable]
public class OfferPackData
{
	public int ID;

	public string ProductCode;

	public string InfoText;

	public int AnimFrameIndex = 1;

	public List<string> CarsInPack = new List<string>();

	public string ShowroomTitleString;

	public string ShowroomBodyString;

	public EligibilityRequirements Requirements = EligibilityRequirements.CreateAlwaysEligible();

	public void Initialise()
	{
		this.Requirements.Initialise();
	}

	public virtual bool IsEligible(IGameState gs)
	{
		return this.Requirements.IsEligible(gs);
	}

	public int GetCarsRemainingInPack()
	{
		IGameState gs = new GameStateFacade();
		return this.CarsInPack.FindAll((string q) => !gs.IsCarOwned(q)).Count;
	}
}
