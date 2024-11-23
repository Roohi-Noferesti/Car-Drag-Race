using System;
using System.Collections.Generic;

[Serializable]
public class NarrativeStateDataGroup
{
	public List<NarrativeStateData> States;

	public void Initialise()
	{
		if (this.States != null)
		{
			this.States.ForEach(delegate(NarrativeStateData s)
			{
				s.Initialise();
			});
		}
	}
}
