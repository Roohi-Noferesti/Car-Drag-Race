using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("GT/Input/InputManager")]
public class InputManager : MonoBehaviour
{
	private InputState mInputState = new InputState();

	private bool UIGearUp;

	private bool UIGearDown;

	private bool UIThrottle;

	private bool UIFakeThrottle;

	private bool UINitrous;

	//private bool UIRecord;

	private bool CatchAll;

	private static int mLastBackFrame;

	private static int mLastEnterFrame;

	public InputState InputState
	{
		get
		{
			return this.mInputState;
		}
	}

    public static InputManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Reset()
	{
		this.mInputState.Reset();
	}

	public void PopulateInputsViaControllers()
	{
	}

	public void PollInputs()
	{
        //if (!PauseGame.isGamePaused || !PauseGame.hasPopup)
        //{
            this.PopulateInputsViaControllers();
            if (this.UIGearUp || (KeyMappings.Instance && KeyMappings.Instance.GetKeyDown(KeyMappings.Action.GEAR_UP)))
            {
                this.mInputState.GearChangeUp = true;
            }
            if (this.UIGearDown || (KeyMappings.Instance && KeyMappings.Instance.GetKeyDown(KeyMappings.Action.GEAR_DOWN)))
            {
                this.mInputState.GearChangeDown = true;
            }
            if (this.UIThrottle || (KeyMappings.Instance && KeyMappings.Instance.GetKey(KeyMappings.Action.THROTTLE)))
            {
                this.mInputState.Throttle = true;
                this.mInputState.CatchAll = true;
            }
            else if (this.UIFakeThrottle)
            {
                this.mInputState.FakeThrottle = true;
            }
            if (this.UINitrous || (KeyMappings.Instance && KeyMappings.Instance.GetKeyDown(KeyMappings.Action.NITRO)) || NitrousTutorial.Instance.NitrousPressed)
            {
                this.mInputState.Nitrous = true;
                NitrousTutorial.Instance.NitrousPressed = false;
            }
            //if (this.UIRecord)
            //{
            //    AudioManager.Instance.PlaySound("MenuForward", null);
            //    VideoCapture.StartRecording();
            //    RaceHUDController.Instance.hudRecordButtonDisplay.transform.parent.gameObject.SetActive(false);
            //}
            if (this.CatchAll)
            {
                this.mInputState.CatchAll = true;
            }
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    PauseGame.PauseIfInRace();
            //}
        //}
        this.ClearUIInput();
	}

	private void ClearUIInput()
	{
		this.UIGearUp = false;
		this.UIGearDown = false;
        //this.UIThrottle = false;
		this.UIFakeThrottle = false;
		this.UINitrous = false;
		//this.UIRecord = false;
		this.CatchAll = false;
	}

    public void UIPressedGearUp()
	{
		this.UIGearUp = true;
	}

    public void UIPressedGearDown()
	{
		this.UIGearDown = true;
	}

    public void UIPressedThrottle()
	{
		this.UIThrottle = true;
	}

    public void UIReleasedThrottle()
    {
        this.UIThrottle = false;
    }

    public void UIPressedFakeThrottle()
	{
		this.UIFakeThrottle = true;
	}

    public void UIPressedNitrous()
	{
		this.UINitrous = true;
	}

    public void UIPressedRecord()
	{
		//this.UIRecord = true;
	}

    public void CatchAllButton()
	{
		this.CatchAll = true;
	}

    public void onPauseButton()
	{
        PauseGame.Pause();
	}

    public void OnApplicationPause(bool pause)
	{
        //if (pause)
        //{
        //    VideoCapture.StopRecording();
        //    VideoCapture.ClearRecording();
        //}
	}

	public static bool BackButtonPressed()
	{
		if (Input.GetKeyUp(KeyCode.Escape) && mLastBackFrame != Time.frameCount)
		{
			mLastBackFrame = Time.frameCount;
			return true;
		}
		return false;
	}

	public static bool EnterPressed()
	{
		return false;
	}

    public void ReleaseThrottleDelayed()
    {
        StartCoroutine(_delayReleaseThrottle());
    }

    private IEnumerator _delayReleaseThrottle()
    {
        UIPressedThrottle();
        yield return new WaitForEndOfFrame();
        UIReleasedThrottle();
    }
}
