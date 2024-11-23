using System;

namespace Objectives.Impl
{
	public class ChangeCarColour : AbstractObjective
	{
		[Command]
		public void CarColorChanged()
		{
			base.ForceComplete();
		}
	}
}
