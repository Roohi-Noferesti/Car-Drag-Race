using DataSerialization;
using System;

[Serializable]
public class ProgressionMapTextData
{
	public FormatStringData Text = new FormatStringData();

	public int Priority;

	public EligibilityRequirements ShowingRequirements = EligibilityRequirements.CreateNeverEligible();

	public void Initialise()
	{
		this.ShowingRequirements.Initialise();
	}

	public bool IsEligibile(IGameState gs)
	{
		if (this.ShowingRequirements.IsEligible(gs))
		{
		}
		return this.ShowingRequirements.IsEligible(gs);
	}

	public string GetTextString()
	{
		return this.Text.GetFormatString();
	}
}
