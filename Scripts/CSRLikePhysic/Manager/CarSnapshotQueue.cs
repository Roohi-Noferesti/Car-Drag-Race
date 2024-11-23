using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class CarSnapshotQueue : MonoBehaviour
{
	private Queue<CarSnapshotRequest> requestQueue = new Queue<CarSnapshotRequest>();

	private HashSet<CarSnapshotRequest> cancelledRequests = new HashSet<CarSnapshotRequest>();

	public static CarSnapshotQueue Instance
	{
		get;
		private set;
	}

	public bool RenderInProgress
	{
		get;
		private set;
	}

	private void Awake()
	{
		CarSnapshotQueue.Instance = this;
		this.RenderInProgress = false;
	}

	private void SetupSnapshotManager(CarSnapshotRequest request)
	{
		CarSnapshotManager.Instance.SnapshotSize = request.SnapshotSize;
		CarSnapshotManager.Instance.CarSlot = request.CarSlot;
		CarSnapshotManager.Instance.LiverySlot = request.LiverySlot;
		CarSnapshotManager.Instance.SnapshotType = request.SnapshotType;
	}

	private void GenerateSnapshot(CarSnapshotRequest request)
	{
		if (this.cancelledRequests.Contains(request))
		{
			this.cancelledRequests.Remove(request);
			this.RenderInProgress = false;
			return;
		}
		this.SetupSnapshotManager(request);
		CarSnapshotManager.Instance.GenerateSnapshot(request.GarageInstance, delegate(Texture2D tex)
		{
			if (!this.cancelledRequests.Contains(request))
			{
				request.ResultCallback(tex);
			}
			else
			{
				this.cancelledRequests.Remove(request);
			}
			this.ClearUnusedAsyncSlots(request);
			Resources.UnloadUnusedAssets();
			CarSnapshotManager.Instance.ResetToDefaults();
			this.RenderInProgress = false;
		});
	}

	private void AsyncResetToDefaults()
	{
		if (!this.RenderInProgress)
		{
			CarSnapshotManager.Instance.ResetToDefaults();
		}
	}

	public void RequestSnapshot(CarSnapshotRequest request)
	{
		this.SetupSnapshotManager(request);
		CarSnapshotManager.Instance.AsyncLoadSnapshot(request.GarageInstance, delegate(Texture2D tex, bool ok)
		{
			if (!this.cancelledRequests.Contains(request))
			{
				if (ok)
				{
					this.AsyncResetToDefaults();
					request.ResultCallback(tex);
				}
				else
				{
					this.requestQueue.Enqueue(request);
				}
			}
			else
			{
				this.AsyncResetToDefaults();
				this.cancelledRequests.Remove(request);
			}
		});
	}

	public void CancelSnapshot(CarSnapshotRequest cancelledRequest)
	{
		this.cancelledRequests.Add(cancelledRequest);
	}

	private void ClearUnusedAsyncSlots(CarSnapshotRequest request)
	{
		if (this.requestQueue.All((CarSnapshotRequest r) => r.CarSlot != request.CarSlot))
		{
			AsyncSwitching.Instance.ClearSlot(request.CarSlot);
		}
		if (this.requestQueue.All((CarSnapshotRequest r) => r.LiverySlot != request.LiverySlot))
		{
			AsyncSwitching.Instance.ClearSlot(request.LiverySlot);
		}
	}

	private void Update()
	{
		if (this.requestQueue.Count > 0 && !this.RenderInProgress)
		{
			this.RenderInProgress = true;
			this.GenerateSnapshot(this.requestQueue.Dequeue());
		}
	}
}
