using System;
using System.Collections.Generic;

[Serializable]
public class NarrativeSceneLine
{
	public string SceneLineID;

	public NarrativeStateData StateData;

	public List<NarrativeSceneResponse> Responses;

	public List<NarrativeStateDataGroup> PreStates;

	public List<NarrativeStateDataGroup> PostStates;

	public void Initialise()
	{
		if (this.StateData != null)
		{
			this.StateData.Initialise();
		}
		if (this.PreStates != null)
		{
			this.PreStates.ForEach(delegate(NarrativeStateDataGroup p)
			{
				p.Initialise();
			});
		}
		if (this.PostStates != null)
		{
			this.PostStates.ForEach(delegate(NarrativeStateDataGroup p)
			{
				p.Initialise();
			});
		}
		foreach (NarrativeSceneResponse current in this.Responses)
		{
			current.Initialise();
		}
	}

	public NarrativeSceneResponse GetResponse(IGameState gs)
	{
		if (this.Responses.Count == 0)
		{
			return null;
		}
		return this.Responses.FindAll((NarrativeSceneResponse q) => q.IsValid(gs)).MinItem((NarrativeSceneResponse q) => q.Priority);
	}
}
