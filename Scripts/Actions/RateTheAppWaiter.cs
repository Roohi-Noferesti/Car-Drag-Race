using KingKodeStudio;
using UnityEngine;

public class RateTheAppWaiter : MonoBehaviour
{
	public void Update()
	{
		int nativePromptResult = RateTheAppNagger.GetNativePromptResult();
		if (nativePromptResult == 0 || !BuildType.CanShowRate())
		{
			return;
		}
		if (nativePromptResult > 0)
		{
            //RateTheAppNagger.TriggerRateAppPage();
            ScreenManager.Instance.PushScreen(ScreenID.UserRatingGame);
        }
		Destroy(base.gameObject);
	}
}
