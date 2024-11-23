using System;
using DataSerialization;

[Serializable]
public class TutorialBubbleParent
{
    public string Name
    {
        get
        {
            if (Names.Length > 0)
                return Names[0];
            return String.Empty;
        }
    }
    public string[] Names;
	public int Selector;

	public EligibilityRequirements ParentRequirements = EligibilityRequirements.CreateAlwaysEligible();

	public bool IsEligible(IGameState gs)
	{
        return this.ParentRequirements.IsEligible(gs);
	}
}
