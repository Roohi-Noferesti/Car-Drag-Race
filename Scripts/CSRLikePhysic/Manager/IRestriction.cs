using System;

namespace EventPaneRestriction
{
	public interface IRestriction
	{
		bool IsRestrictionActive();

		void RestrictionButtonPressed();

		string RestrictionButtonText();

        void AddRestrictionBubbleGraphics(EventPaneRestrictionBubble bubble);
	}
}
