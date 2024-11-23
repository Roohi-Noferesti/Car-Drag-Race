using UnityEngine;

public class RaceStatePreCountdown : RaceStateBase
{
	public RaceStatePreCountdown(RaceStateMachine inMachine) : base(inMachine, RaceStateEnum.precountdown)
	{
	}

	public override void Enter()
	{
        string name = SequenceManager.Instance.ActiveSequence.name;
        if (name == "PreLights" || name == "PreLights1Car")
        {
            SequenceManager.Instance.PlaySequence("RaceGo");
        }
        if (RaceStartNames.Instance != null)
        {
            RaceStartNames.Instance.GridSideView();
        }
        RaceHUDController.Instance.HUDAnimator.IntroduceHUD();
	}

	public override void FixedUpdate()
	{
	    if (IngameTutorial.IsInTutorial &&
	        (IngameTutorial.Instance.CurrentLesson is IngameLessonThrottle ||
	         IngameTutorial.Instance.CurrentLesson is IngameLessonRace2Throttle))
	    {
	        CompetitorManager.Instance.UpdateGridFixedUpdate();
	        return;
	    }
	    CompetitorManager.Instance.UpdatePreCountDownFixedUpdate();
        if (RaceEventInfo.Instance.IsSMPEvent || (RaceController.Instance.Inputs != null && RaceController.Instance.Inputs.InputState.Throttle))
        {
            this.machine.SetState(RaceStateEnum.countdown);
        }

        if (CheatEngine.Instance != null && CheatEngine.Instance.forceEndRaceState != ForceEndRaceState.DONTCARE)
        {
            this.machine.SetState(RaceStateEnum.countdown);
        }
    }
}
