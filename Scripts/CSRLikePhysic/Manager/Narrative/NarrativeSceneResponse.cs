using DataSerialization;
using System;

[Serializable]
public class NarrativeSceneResponse
{
	public string SceneLineID;

	public EligibilityRequirements Requirements;

	public int Priority;

	public void Initialise()
	{
		if (this.Requirements != null)
		{
			this.Requirements.Initialise();
		}
	}

	public bool IsValid(IGameState gs)
	{
		return this.Requirements == null || this.Requirements.IsEligible(gs);
	}
}
