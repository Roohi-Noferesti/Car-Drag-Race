using System;
using I2.Loc;

namespace Objectives.Impl
{
	public abstract class AbstractWinRaces : AbstractObjective
	{
		public int NumberOfRaces = 1;

		internal override void Clear()
		{
			base.Clear();
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.NumberOfRaces.ToString());
		}
	}
}
