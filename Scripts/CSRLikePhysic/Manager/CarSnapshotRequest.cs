using System;
using UnityEngine;

public class CarSnapshotRequest
{
	public CarGarageInstance GarageInstance;

	public int SnapshotSize;

	public AsyncBundleSlotDescription CarSlot;

	public AsyncBundleSlotDescription LiverySlot;

	public CarSnapshotType SnapshotType;

	public Action<Texture2D> ResultCallback;
}
