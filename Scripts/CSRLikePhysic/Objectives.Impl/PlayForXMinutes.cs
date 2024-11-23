using System;
using I2.Loc;

namespace Objectives.Impl
{
	public class PlayForXMinutes : AbstractObjective
	{
		[SerializeInProfile]
		private int m_objStartPlayTimeCount = -1;

		public int m_playTimeCountTarget = 15;

		internal override void Clear()
		{
			base.Clear();
			this.m_objStartPlayTimeCount = -1;
		}

		private void Start()
		{
			this.TotalProgressSteps = this.m_playTimeCountTarget;
			if (this.m_objStartPlayTimeCount == -1)
			{
				PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
				this.m_objStartPlayTimeCount = activeProfile.TotalPlayTime;
			}
		}

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.m_playTimeCountTarget.ToString());
		}

		public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
			PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
			int num = activeProfile.TotalPlayTime - this.m_objStartPlayTimeCount;
			if (base.CurrentProgress < num)
			{
				base.CurrentProgress = num;
			}
			if (num >= this.m_playTimeCountTarget)
			{
				base.ForceComplete();
			}
		}
	}
}
