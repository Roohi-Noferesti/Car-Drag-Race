using KingKodeStudio;

public class RaceStateExit : RaceStateBase
{
	private int waitFrames;

	public RaceStateExit(RaceStateMachine inMachine) : base(inMachine, RaceStateEnum.exit)
	{
	}

	public override void Enter()
	{
		this.StopCarAudio();
		this.waitFrames = 2;
        //CommonUI.Instance.CarNameStats.ResetHaveCar();
	}

	public override void FixedUpdate()
	{
        if (this.waitFrames <= 0)
        {
            return;
        }
        this.waitFrames--;
        if (this.waitFrames > 0)
        {
            return;
        }
        this.CleanupOtherResources();
        this.CleanupCarResources();
        RaceController.Instance.SetRaceRunning(false);
        if (RaceRewardScreen.Instance != null)
        {
            UnityEngine.Object.Destroy(RaceRewardScreen.Instance.gameObject);
        }
	    if (TouchManager.Instance != null)
	        TouchManager.Instance.GesturesEnabled = true;
        ScreenManager.Instance.PopToScreen(ScreenID.Dummy);
        SceneLoadManager.Instance.LoadRequestedScene();
	}

	private void StopCarAudio()
	{
		CompetitorManager.Instance.StopAudio();
	}

	private void CleanupOtherResources()
	{
        if (RaceLightsManager.instance != null)
        {
            RaceLightsManager.instance.gameObject.SetActive(false);
        }
        SequenceManager.Instance.StopSequence();
	}

	private void CleanupCarResources()
	{
        CompetitorManager.Instance.DestroyCompetitorComponents();
        AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.AICar);
        AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.AICarLivery);
	}
}
