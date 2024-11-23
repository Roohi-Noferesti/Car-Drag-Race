using System;
using UnityEngine;

public struct GenericTouch
{
	public const float TIME_BETWEEN_SNAPSHOTS = 0.1f;

	public Vector2 SnapshotPositionNew;

	public Vector2 SnapshotPosition;

	public float SnapshotTime;

	public int TouchID;

	public int FingerIndex;

	public Vector2 Position;

	public Vector2 Velocity;

	public Vector2 StartPosition;

	public Vector2 DeltaPosition;

	public float TotalTime;

	public TouchPhase Phase;

	public bool VALID;

	public static GenericTouch Empty;

	public Vector2 Displacement
	{
		get
		{
			return this.Position - this.StartPosition;
		}
	}

	public float TotalDisplacement
	{
		get
		{
			return Math.Abs(this.Displacement.magnitude);
		}
	}

	public Vector2 AverageEndingVelocity
	{
		get
		{
			return (this.Position - this.SnapshotPosition) / (this.SnapshotTime + 0.1f);
		}
	}

	static GenericTouch()
	{
		GenericTouch.Empty = default(GenericTouch);
		GenericTouch.Empty.TouchID = -1;
		GenericTouch.Empty.FingerIndex = -1;
		GenericTouch.Empty.Position = Vector2.zero;
		GenericTouch.Empty.Velocity = Vector2.zero;
		GenericTouch.Empty.StartPosition = Vector2.zero;
		GenericTouch.Empty.DeltaPosition = Vector2.zero;
		GenericTouch.Empty.TotalTime = 0f;
		GenericTouch.Empty.Phase = TouchPhase.Began;
		GenericTouch.Empty.VALID = false;
		GenericTouch.Empty.SnapshotPositionNew = Vector2.zero;
		GenericTouch.Empty.SnapshotPosition = Vector2.zero;
		GenericTouch.Empty.SnapshotTime = 0f;
	}
}
