using System;
using I2.Loc;
using UnityEngine;

namespace Objectives.Impl
{
	public class RaceXMilesInADay : AbstractObjective
	{
		[SerializeInProfile]
		private float m_metersRaced;

		public float m_milesRacedTarget;

	    [SerializeInProfile] private long m_resetDay;//= AbsoluteTime.AddAbsoluteTimeSeconds(AbsoluteTime.GetAbsoluteTimeNow(false), 86400L);

		internal override void Clear()
		{
            base.Clear();
            this.m_metersRaced = 0f;
            this.m_resetDay = ServerSynchronisedTime.AddAbsoluteTimeSeconds(ServerSynchronisedTime.GetAbsoluteTimeNow(false), 86400L);
		}

		private void Start()
		{
			this.TotalProgressSteps = (int)this.m_milesRacedTarget;
		    if (m_resetDay == 0)
		        m_resetDay = ServerSynchronisedTime.AddAbsoluteTimeSeconds(ServerSynchronisedTime.GetAbsoluteTimeNow(false),
		            86400L);
		}

		public override TimeSpan GetTimeLimit()
		{
			var timeSpan = TimeSpan.Zero;
			if (ObjectiveManager.Instance != null && ObjectiveManager.Instance.m_enableObjectivesV2)
			{
				timeSpan = base.GetTimeLimit();
			}
			else
			{
                timeSpan = ServerSynchronisedTime.GetAbsoluteRemaingTimeUntil(this.m_resetDay);
                if (timeSpan < TimeSpan.Zero)
                {
                    return TimeSpan.Zero;
                }
			}
			return timeSpan;
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
            this.m_metersRaced += RaceEventInfo.Instance.RaceDistanceMetres;
            ObjectiveManager.Instance.ForceUpdateObjectiveInProfile(this.ID);
		}

		public override string GetDescription()
        {
            return string.Format(LocalizationManager.GetTranslation(this.Description), this.m_milesRacedTarget.ToString());
		}

	    public override void UpdateState()
		{
			if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
			{
				return;
			}
            //Debug.Log(m_metersRaced);
            if (this.m_resetDay <= ServerSynchronisedTime.GetAbsoluteTimeNow(false))
            {
                //Debug.Log("here");
                this.m_metersRaced = 0f;
                base.CurrentProgress = 0;
                this.m_resetDay = ServerSynchronisedTime.AddAbsoluteTimeSeconds(ServerSynchronisedTime.GetAbsoluteTimeNow(false), 86400L);
                ObjectiveManager.Instance.ForceUpdateObjectiveInProfile(this.ID);
            }
	        float num = this.m_metersRaced/1000;//1609.3f;
			num = (float)Math.Round((double)num, 2);
			if (base.CurrentProgress < (int)num)
			{
				base.CurrentProgress = (int)num;
			}
			this.TotalProgressStepsOverride = num;
			float num2 = num / this.m_milesRacedTarget;
			num2 = (float)Math.Round((double)num2, 2);
			this.ProgressOverride = num2;
	        float num3 = this.m_milesRacedTarget*1000;//1609.3f;
			num3 = (float)Math.Round((double)num3, 2);
			if (this.m_metersRaced >= num3)
			{
				base.ForceComplete();
			}
		}
	}
}
