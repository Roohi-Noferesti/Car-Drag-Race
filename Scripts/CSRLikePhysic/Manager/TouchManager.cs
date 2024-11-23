using System;
using System.Collections;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
	private const float TIME_BETWEEN_BUTTON_PRESSES = 0.33f;

	private const float DISPLACEMENT_REQUIRED_FOR_FLICKS = 0.02f;

	public bool GesturesEnabled;

	private List<GestureSample> gestureFIFOBuffer = new List<GestureSample>(4);

	private TouchCollection touchCollection = new TouchCollection();

	private float DISPLACEMENT_REQUIRED_FOR_DRAGS = 0.2f;

	private float MAX_DISPLACEMENT_FOR_TAPS = 0.2f;

	private float VELOCITY_REQUIRED_FOR_FLICKS = 0.1f;

	private bool lookForDragStart;

	private static float _allowedToPressTimer;

	private static float _allowedToEnterGesturesTimer;

	private static bool _carouselPressedLock;

	public static TouchManager Instance
	{
		get;
		private set;
	}

	public bool IsGestureAvailable
	{
		get
		{
			return this.gestureFIFOBuffer.Count > 0;
		}
	}

	public TouchCollection TouchCollection
	{
		get
		{
			return this.touchCollection;
		}
	}

	private void Awake()
	{
		if (TouchManager.Instance != null)
		{
			return;
		}
		TouchManager.Instance = this;
	}

	public static bool TryAndGetCarouselPressedLock()
	{
		if (!TouchManager._carouselPressedLock)
		{
			TouchManager._carouselPressedLock = true;
			return true;
		}
		return false;
	}

	public static void ReleaseCarouselPressedLock()
	{
		TouchManager._carouselPressedLock = false;
	}

	public static bool AttemptToUseButton(string name)
	{
		if (TouchManager._allowedToPressTimer == 0f)
		{
			TouchManager._allowedToPressTimer = 0.33f;
			return true;
		}
		return false;
	}

	public static void DisableGesturesFor(float time)
	{
		TouchManager._allowedToEnterGesturesTimer = Mathf.Max(TouchManager._allowedToEnterGesturesTimer, time);
	}

	public static void DisableButtonsFor(float time)
	{
		TouchManager._allowedToPressTimer = Mathf.Max(TouchManager._allowedToPressTimer, time);

        Instance.StopCoroutine(_disableInputForSeconds(time));
	    Instance.StartCoroutine(_disableInputForSeconds(time));
	}


    private static IEnumerator _disableInputForSeconds(float time)
    {
        ScreenManager.Instance.Interactable = false;
        yield return new WaitForSeconds(time);
        ScreenManager.Instance.Interactable = true;
    }

    void Start()
	{
        float pixelDensity = ResolutionManager.PixelDensity;
		this.DISPLACEMENT_REQUIRED_FOR_DRAGS *= pixelDensity;
		this.MAX_DISPLACEMENT_FOR_TAPS *= pixelDensity;
		this.VELOCITY_REQUIRED_FOR_FLICKS *= pixelDensity;
	}

	private void FixedUpdate()
	{
		MouseTouchDetector.Update();
		this.touchCollection.Update();
		if (this.GesturesEnabled && _allowedToEnterGesturesTimer == 0f)
		{
			this.UpdateGestureDetection();
			GestureEventSystem.Instance.CheckTouchManagerForGestures();
			GestureEventSystem.Instance.UpdateContinuousGestures();
		}
	}

	private void LateUpdate()
	{
		TouchManager._allowedToPressTimer = this.UpdateTimer(TouchManager._allowedToPressTimer);
		TouchManager._allowedToEnterGesturesTimer = this.UpdateTimer(TouchManager._allowedToEnterGesturesTimer);
	}

	private float UpdateTimer(float currentTime)
	{
		if (currentTime > 0f)
		{
			currentTime -= Time.deltaTime;
			if (currentTime <= 0f)
			{
				currentTime = 0f;
			}
		}
		else
		{
			currentTime = 0f;
		}
		return currentTime;
	}

	private void AddTapToGestureBuffer(GenericTouch myTouch)
	{
		GestureSample item = default(GestureSample);
		item.GestureType = GestureType.Tap;
		item.TimeStamp = Time.time;
		item.TouchID = myTouch.TouchID;
		this.gestureFIFOBuffer.Add(item);
	}

	private void AddDragToGestureBuffer(GenericTouch myTouch)
	{
		GestureSample item = default(GestureSample);
		item.GestureType = GestureType.Drag;
		item.TimeStamp = Time.time;
		item.TouchID = myTouch.TouchID;
		this.gestureFIFOBuffer.Add(item);
	}

	private void AddFlickToGestureBuffer(GenericTouch myTouch)
	{
		GestureSample item = default(GestureSample);
		item.GestureType = GestureType.Flick;
		item.TimeStamp = Time.time;
		item.TouchID = myTouch.TouchID;
		this.gestureFIFOBuffer.Add(item);
	}

	private void UpdateGestureDetection()
	{
		if (this.touchCollection.TouchCount == 1)
		{
			GenericTouch touchFromIndex = this.touchCollection.GetTouchFromIndex(0);
			if (!touchFromIndex.VALID)
			{
				return;
			}
			if (touchFromIndex.Phase == TouchPhase.Began)
			{
				this.lookForDragStart = true;
			}
			else if (touchFromIndex.Phase == TouchPhase.Ended)
			{
				if (touchFromIndex.TotalDisplacement <= this.MAX_DISPLACEMENT_FOR_TAPS)
				{
					this.AddTapToGestureBuffer(touchFromIndex);
					return;
				}
				if (touchFromIndex.AverageEndingVelocity.magnitude >= this.VELOCITY_REQUIRED_FOR_FLICKS && touchFromIndex.TotalDisplacement >= 0.02f)
				{
					this.AddFlickToGestureBuffer(touchFromIndex);
				}
			}
			else if (touchFromIndex.TotalDisplacement >= this.DISPLACEMENT_REQUIRED_FOR_DRAGS && this.lookForDragStart)
			{
				this.AddDragToGestureBuffer(touchFromIndex);
				this.lookForDragStart = false;
			}
		}
	}

	private void LookForPinch()
	{
	}

	public GestureSample ReadGesture()
	{
		if (this.gestureFIFOBuffer.Count == 0)
		{
			Debug.Break();
		}
		GestureSample result = this.gestureFIFOBuffer[0];
		this.gestureFIFOBuffer.RemoveAt(0);
		return result;
	}
}
