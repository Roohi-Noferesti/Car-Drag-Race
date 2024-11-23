using System;
using I2.Loc;
//using Tutorial;

namespace Objectives.Impl
{
    public class GetXStar : AbstractObjective
	{
		public int StarValueTarget = 1;

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.StarValueTarget.ToString());
		}

        public override void UpdateState()
        {
            //if (!TutorialQuery.IsObjectiveComplete("IntroduceObjectives"))
            //{
            //    return;
            //}
            if (RaceEventDatabase.instance.EventData == null || this.IsComplete)
            {
                return;
            }
            PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            int star = activeProfile.PlayerStar;
            if (CurrentProgress <= star)
            {
                CurrentProgress = star;
            }
            if (star >= this.StarValueTarget)
            {
                base.ForceComplete();
            }
        }
	}
}
