using UnityEngine;

public class RaceHUDManager
{
	public static RaceHUDManager Instance;

	private GameObject RaceHUD;

	private GameObject SpeedLines;

	public static void Initialise()
	{
		if (Instance == null)
		{
			Instance = new RaceHUDManager();
		}
	}

	public void CreateHUD(CarPhysics HumanPhysics, float RaceDistance)
	{
		if (Instance.RaceHUD == null)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/RaceHUD");
			Instance.RaceHUD = Object.Instantiate(original);
			RaceHUDController.Instance.Reset(HumanPhysics, RaceDistance);
		}
		RaceHUDController.Instance.hudRaceGearMessage.HookupGearEvents();
		RaceHUDController.Instance.hudRaceGearMessageLower.HookupGearEvents();
		if (Shader.globalMaximumLOD >= 500 && Instance.SpeedLines == null)
		{
            //GameObject original2 = (GameObject)Resources.Load("SpeedLines");
            //Instance.SpeedLines = Object.Instantiate(original2);
		}
	}

	public void ResetHUD()
	{
		RaceHUDController.Instance.HUDAnimator.Reset();
		RaceHUDController.Instance.hudRaceTime.Reset();
		RaceHUDController.Instance.hudRaceGearMessage.Reset();
		RaceHUDController.Instance.hudRaceGearMessageLower.Reset();
		RaceHUDController.Instance.hudRaceCentreMessage.Reset();
		RaceHUDController.Instance.hudRecordButtonDisplay.Reset();
		BubbleManager.Instance.DismissMessages();
		RaceHUDController.Instance.SetPauseButton();
	}

	public void DestroyHUD()
	{
		if (Instance.RaceHUD != null)
		{
			Object.Destroy(Instance.RaceHUD);
		}
		if (Instance.SpeedLines != null)
		{
			Object.Destroy(Instance.SpeedLines);
			Instance.SpeedLines = null;
		}
	}
}
