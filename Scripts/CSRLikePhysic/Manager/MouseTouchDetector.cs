using System;
using UnityEngine;

public static class MouseTouchDetector
{
	private static int touchIndexCounter = 0;

	private static GenericTouch mouseTouch = GenericTouch.Empty;

	private static GenericTouch mouseTouchLastFrame = GenericTouch.Empty;

	private static bool touching = false;

	public static bool Touching
	{
		get
		{
			return MouseTouchDetector.touching;
		}
	}

	public static GenericTouch MouseTouch
	{
		get
		{
			return MouseTouchDetector.mouseTouch;
		}
	}

	private static bool LookForNewTouches()
	{
		MouseTouchDetector.mouseTouch.VALID = false;
		if (Input.GetMouseButton(0))
		{
			MouseTouchDetector.touching = true;
			MouseTouchDetector.mouseTouch.Phase = TouchPhase.Began;
			MouseTouchDetector.mouseTouch.TouchID = ++MouseTouchDetector.touchIndexCounter;
			MouseTouchDetector.mouseTouch.FingerIndex = 0;
			MouseTouchDetector.mouseTouch.Position = Input.mousePosition;
			MouseTouchDetector.mouseTouch.Velocity = Vector2.zero;
			MouseTouchDetector.mouseTouch.StartPosition = Input.mousePosition;
			MouseTouchDetector.mouseTouch.DeltaPosition = Vector2.zero;
			MouseTouchDetector.mouseTouch.TotalTime = 0f;
			MouseTouchDetector.mouseTouch.VALID = true;
			MouseTouchDetector.mouseTouch.SnapshotPositionNew = MouseTouchDetector.mouseTouch.Position;
			MouseTouchDetector.mouseTouch.SnapshotPosition = MouseTouchDetector.mouseTouch.Position;
			MouseTouchDetector.mouseTouch.SnapshotTime = 0f;
			return true;
		}
		return false;
	}

	private static void ProcessTouchWhilstFingerDown()
	{
		MouseTouchDetector.mouseTouch.TotalTime = MouseTouchDetector.mouseTouch.TotalTime + Time.fixedDeltaTime;
		MouseTouchDetector.mouseTouch.Position = Input.mousePosition;
		MouseTouchDetector.mouseTouch.DeltaPosition = MouseTouchDetector.mouseTouch.Position - MouseTouchDetector.mouseTouchLastFrame.Position;
		MouseTouchDetector.mouseTouch.Velocity = MouseTouchDetector.mouseTouch.DeltaPosition / Time.fixedDeltaTime;
		if (MouseTouchDetector.mouseTouch.TotalTime - MouseTouchDetector.mouseTouch.SnapshotTime > 0.1f)
		{
			MouseTouchDetector.mouseTouch.SnapshotTime = MouseTouchDetector.mouseTouch.SnapshotTime + 0.1f;
			MouseTouchDetector.mouseTouch.SnapshotPosition = MouseTouchDetector.mouseTouch.SnapshotPositionNew;
			MouseTouchDetector.mouseTouch.SnapshotPositionNew = MouseTouchDetector.mouseTouch.Position;
		}
		if (!Input.GetMouseButton(0))
		{
			MouseTouchDetector.mouseTouch.Phase = TouchPhase.Ended;
			return;
		}
		if (MouseTouchDetector.mouseTouch.TotalDisplacement > 10f)
		{
			MouseTouchDetector.mouseTouch.Phase = TouchPhase.Moved;
		}
		else
		{
			MouseTouchDetector.mouseTouch.Phase = TouchPhase.Stationary;
		}
	}

	public static void Update()
	{
		MouseTouchDetector.mouseTouchLastFrame = MouseTouchDetector.mouseTouch;
		if (!MouseTouchDetector.Touching)
		{
			MouseTouchDetector.mouseTouch = GenericTouch.Empty;
			bool flag = MouseTouchDetector.LookForNewTouches();
			if (flag)
			{
				return;
			}
		}
		else
		{
			if (MouseTouchDetector.mouseTouch.Phase == TouchPhase.Ended)
			{
				MouseTouchDetector.touching = false;
			}
			MouseTouchDetector.ProcessTouchWhilstFingerDown();
		}
	}
}
