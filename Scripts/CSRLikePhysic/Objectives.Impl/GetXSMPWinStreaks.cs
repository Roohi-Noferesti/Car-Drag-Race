using System;

namespace Objectives.Impl
{
	public class GetXSMPWinStreaks : AbstractWinRaces
	{
		//[SerializeInProfile]
		//private int m_winStreak;

		internal override void Clear()
		{
			base.Clear();
			//this.m_winStreak = 0;
		}

		[Command]
		public void FinishdRaceWithEventInfo(RaceEventInfo raceEventInfo, bool isWinner)
		{
		}

		public override void UpdateState()
		{
            //if (/*RaceEventDatabase.instance.EventData == null ||*/ this.IsComplete)
            //{
            //    return;
            //}
            //if (base.CurrentProgress < PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins)
            //{
            //    base.CurrentProgress = PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins;
            //}
            //if (PlayerProfileManager.Instance.ActiveProfile.SMPChallengeWins >= this.NumberOfRaces)
            //{
            //    base.ForceComplete();
            //}
		}
	}
}
