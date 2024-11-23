using System;

namespace Objectives.Impl
{
	public class LogOn : AbstractObjective
	{
		[Command]
		public void LoggedOn()
		{
			base.ForceComplete();
		}
	}
}
