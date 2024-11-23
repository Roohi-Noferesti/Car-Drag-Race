using System;
using I2.Loc;
//using Tutorial;

namespace Objectives.Impl
{
	public class GetXPPValueForCurrentCar : AbstractObjective
	{
		public int PPValueTarget = 1;

		public override string GetDescription()
		{
			return string.Format(LocalizationManager.GetTranslation(this.Description), this.PPValueTarget.ToString());
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
			CarGarageInstance currentCar = activeProfile.GetCurrentCar();
			if (currentCar != null)
			{
				int currentPPIndex = currentCar.CurrentPPIndex;
				if (currentPPIndex >= this.PPValueTarget)
				{
					base.ForceComplete();
				}
			}
		}
	}
}
