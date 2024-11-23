using System;

[Serializable]
public class NarrativeStateData
{
	public string StateType;

	public NarrativeStateDetails StateDetails;

	public BaseCrewState.NarrativeCrewStateType EnumType
	{
		get
		{
			return EnumHelper.FromString<BaseCrewState.NarrativeCrewStateType>(this.StateType);
		}
	}

	public void Initialise()
	{
		if (this.StateDetails != null)
		{
			this.StateDetails.Initialise();
		}
	}
}
