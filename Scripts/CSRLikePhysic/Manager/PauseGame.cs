using Fabric;
using KingKodeStudio;
using UnityEngine;
using Component = Fabric.Component;

[AddComponentMenu("GT/CarHUD/PauseGame")]
public class PauseGame : MonoBehaviour
{
	public static bool isGamePaused;

	public static bool hasPopup;

	public static bool disablePause;

	public static void Initialise()
	{
        ApplicationManager.WillResignActiveEvent += OnApplicationWillResignActive;
        AndroidApplicationManager.WillLoseFocusEvent += OnApplicationWillResignActive;
	}

	private void Start()
	{
		isGamePaused = false;
		hasPopup = false;
	}

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }
#endif

    public static void Pause()
	{
		Pause(true);
	}

	public static void Pause(bool withPopup)
	{
		if (RaceEventInfo.Instance.IsNonPausable())
			return;
		
		if (!isGamePaused && !disablePause)
		{
			isGamePaused = true;
			hasPopup = withPopup;
			RaceCarAudio.FadeDownCarAudio(0f);
            AudioManager.Instance.PlaySound("GamePause", null);
            if (hasPopup)
            {
                ScreenManager.Instance.PushScreen(ScreenID.Pause);
            }
		}
	}

	public static void UnPause()
	{
		if (isGamePaused)
		{
			isGamePaused = false;
			if (RaceController.Instance.Machine.StateBefore(RaceStateEnum.end))
			{
				RaceCarAudio.FadeUpCarAudio(0f);
			}
			if (hasPopup)
			{
                if (ScreenManager.Instance.CurrentScreen == ScreenID.Pause
                    && ScreenManager.Instance.CurrentState == ScreenManager.State.IDLE)
                {
                    ScreenManager.Instance.PopScreen();
                }
				hasPopup = false;
			}
            Component componentByName = FabricManager.Instance.GetComponentByName("Audio_SFXAudio");

            if (componentByName != null)
		        componentByName.MixerPitch = 1f;
		}
	}

	public static void OnApplicationWillResignActive()
	{
		PauseIfInRace();
	}

	public static void PauseIfInRace()
	{
        if (SceneLoadManager.Instance != null && 
            SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Race && 
            !isGamePaused && ScreenManager.Instance != null &&
            !(ScreenManager.Instance.ActiveScreen is RaceRewardScreen) && 
            //(ScreenManager.Active.CurrentScreen != ScreenID.RaceRewards && 
             //ScreenManager.Active.CurrentScreen != ScreenID.LevelUp &&
            RaceController.Instance != null && !RaceController.Instance.HasHumanTimeBeenSet())
        {
            Pause();
        }
	}


    
}
