public class IngameLessonWait : IngameLessonBase
{
	public override void StateOnEnter()
	{
	}

	public override bool StateUpdate()
	{
	    return RaceController.RaceIsRunning();
	}

	public override void StateOnExit()
	{
	}
}
