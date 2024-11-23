using UnityEngine;

public class GarageCameraFade : BaseBehaviour
{
	public float FadeInDuration = 0.8f;

	public float FadeOutDuration = 0.3f;

	private bool doingFadeOut;

	private Timer timer;

	public override void OnActivate()
	{
		this.timer = base.GetComponent<Timer>();
		if (this.timer == null)
		{
		}
		this.doingFadeOut = false;
		//GarageCameraManager.Instance.FadePane.FadeTo(new Color(0f, 0f, 0f, 0f), this.FadeInDuration);
	}

    public override void OnUpdate(ref CameraState zResult)
    {
        if (!this.doingFadeOut && this.timer.Duration - this.timer.CurrentTime < this.FadeOutDuration)
        {
            this.doingFadeOut = true;
            if (!GarageCameraManager.Instance.IsCurrentlySwiping)
            {
                //GarageCameraManager.Instance.FadePane.FadeTo(new Color(0f, 0f, 0f, 1f), this.FadeOutDuration);
            }
        }
    }
}
