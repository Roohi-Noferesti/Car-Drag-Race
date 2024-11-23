using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class PlayXArcadeGameSession : AbstractObjective
	{
		[SerializeInProfile]
		private int m_targetCount;

		public int TargetValue;

		[Command]
		public void CounterPlayArcadeGame()
		{
			this.m_targetCount++;
		}

		internal override void Clear()
		{
			base.Clear();
			this.m_targetCount = 0;
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.TargetValue.ToString());
		}

		public override void UpdateState()
		{
			if (this.IsActive && !this.IsComplete)
			{
				if (base.CurrentProgress < this.m_targetCount)
				{
					base.CurrentProgress = this.m_targetCount;
				}
				if (this.m_targetCount >= this.TargetValue)
				{
					base.ForceComplete();
				}
			}
		}
	}
}
