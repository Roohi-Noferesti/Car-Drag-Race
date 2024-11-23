using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class NarrativeScene
{
	public string SceneID;

	public NarrativeSceneDetails SceneDetails;

	public NarrativeSceneCharactersDetails CharactersDetails;

	public List<NarrativeSceneLine> Lines;

	public void Initialise()
	{
		foreach (NarrativeSceneLine current in this.Lines)
		{
			current.Initialise();
		}
	}

	public NarrativeSceneLine GetSceneLine(string lineID)
	{
		return this.Lines.DefaultIfEmpty(null).First((NarrativeSceneLine q) => q.SceneLineID == lineID);
	}

	public NarrativeSceneLine GetSceneLineFromResponse(NarrativeSceneResponse response)
	{
		if (response == null)
		{
			return null;
		}
		return this.GetSceneLine(response.SceneLineID);
	}

	public bool CanTriggerHighStakesRace()
	{
		foreach (NarrativeSceneLine current in this.Lines)
		{
			foreach (NarrativeStateDataGroup current2 in current.PostStates)
			{
				if (current2.States.Any((NarrativeStateData q) => q.EnumType == BaseCrewState.NarrativeCrewStateType.DismissScreenWithSuperNitrousCheck))
				{
					return true;
				}
			}
		}
		return false;
	}
}
