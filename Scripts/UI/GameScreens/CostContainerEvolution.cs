public class CostContainerEvolution : CostContainerBlueButton
{
	private TuningScreen GetTuningScreen()
	{
        //if (ScreenManager.Active.CurrentScreen != ScreenID.Tuning)
        //{
        //    return null;
        //}
        //return ScreenManager.Instance.ActiveScreen as TuningScreen;
	    return null;
	}

    public void AnimateTuningScreenFlash()
	{
		//TuningScreen tuningScreen = this.GetTuningScreen();
        //if (tuningScreen != null)
        //{
        //    tuningScreen.Flash.StartFlashAnimation();
        //}
	}

	public void OnAnimationFinished()
	{
		//TuningScreen tuningScreen = this.GetTuningScreen();
        //if (tuningScreen != null)
        //{
        //    tuningScreen.UnlockEvoToken();
        //    tuningScreen.GetComponent<ScreenShakeTrigger>().DoCameraShake();
        //    AnimationUtils.PlayAnim(tuningScreen.GetComponent<UnityEngine.Animation>(), "PostResearchAnimation");
        //}
	}
}
