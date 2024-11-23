using System;
using System.Collections.Generic;
using UnityEngine;

public static class TouchHelper
{
	private static int touchIDCounter;

	public static GenericTouch CreateFromUnityTouch(Touch zTouch, List<GenericTouch> zPreviousTouchList)
	{
		GenericTouch empty = GenericTouch.Empty;
		empty.Position = zTouch.position;
		empty.Phase = zTouch.phase;
		if (zTouch.phase == TouchPhase.Began)
		{
			empty.TouchID = ++TouchHelper.touchIDCounter;
			empty.StartPosition = zTouch.position;
			empty.FingerIndex = zTouch.fingerId;
			empty.DeltaPosition = Vector2.zero;
			empty.VALID = true;
			empty.SnapshotPositionNew = zTouch.position;
			empty.SnapshotPosition = zTouch.position;
			empty.SnapshotTime = 0f;
		}
		else
		{
			bool flag = false;
			GenericTouch genericTouch = GenericTouch.Empty;
			foreach (GenericTouch current in zPreviousTouchList)
			{
				if (current.FingerIndex == zTouch.fingerId)
				{
					genericTouch = current;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return GenericTouch.Empty;
			}
			empty.StartPosition = genericTouch.StartPosition;
			empty.TotalTime = genericTouch.TotalTime + Time.fixedDeltaTime;
			empty.FingerIndex = genericTouch.FingerIndex;
			empty.TouchID = genericTouch.TouchID;
			empty.VALID = genericTouch.VALID;
			empty.DeltaPosition = empty.Position - genericTouch.Position;
			empty.Velocity = empty.DeltaPosition / Time.fixedDeltaTime;
			empty.SnapshotPositionNew = genericTouch.SnapshotPositionNew;
			empty.SnapshotPosition = genericTouch.SnapshotPosition;
			empty.SnapshotTime = genericTouch.SnapshotTime;
			if (empty.TotalTime - empty.SnapshotTime > 0.1f)
			{
				empty.SnapshotTime += 0.1f;
				empty.SnapshotPosition = empty.SnapshotPositionNew;
				empty.SnapshotPositionNew = empty.Position;
			}
			if (genericTouch.Phase == TouchPhase.Ended || !genericTouch.VALID)
			{
				return GenericTouch.Empty;
			}
		}
		return empty;
	}
}
