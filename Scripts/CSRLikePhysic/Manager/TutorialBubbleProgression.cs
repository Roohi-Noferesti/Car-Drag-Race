using System;
using System.Collections.Generic;

[Serializable]
public class TutorialBubbleProgression
{
	public List<TutorialBubble> TutorialBubbles = new List<TutorialBubble>();

	public List<TutorialBubble> GetEligibleBubbles(IGameState gs)
	{
		return  this.TutorialBubbles.FindAll((TutorialBubble tb) => tb.IsEligible(gs));
	}
}
