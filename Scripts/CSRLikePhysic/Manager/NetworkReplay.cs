using System.Collections.Generic;

public class NetworkReplay
{
	private int replayBufferIndex;

	private bool hasWarnedOutOfData;

	private bool throttleToggle;

	private List<ReplayEvent> CurrentData;

	public int physicsFrameIndex
	{
		get;
		private set;
	}

	public bool running
	{
		get;
		private set;
	}

	public eReplayMode ReplayMode
	{
		get;
		set;
	}

	public NetworkReplayData ReplayData
	{
		get;
		private set;
	}

	public NetworkReplay()
	{
		this.ReplayData = new NetworkReplayData();
	}

	public void SetReplayData(NetworkReplayData data)
	{
		this.ReplayData = data;
	}

	public void SetPhase(eReplayPhase phase)
	{
		this.CurrentData = ((phase != eReplayPhase.GRID_PHASE) ? this.ReplayData.RaceReplayData : this.ReplayData.GridReplayData);
		this.physicsFrameIndex = 0;
		this.throttleToggle = false;
		this.hasWarnedOutOfData = false;
		this.replayBufferIndex = 0;
	}

	public void Start()
	{
		this.running = true;
		this.physicsFrameIndex = 0;
	}

	public void Reset()
	{
		this.physicsFrameIndex = 0;
		this.SetPhase(eReplayPhase.GRID_PHASE);
		this.hasWarnedOutOfData = false;
		this.throttleToggle = false;
		this.running = false;
	}

	public void ClearReplayData()
	{
		this.ReplayData.RaceReplayData.Clear();
		this.ReplayData.GridReplayData.Clear();
	}

	public void FixedUpdate()
	{
		if (!this.running)
		{
			return;
		}
		this.physicsFrameIndex++;
	}

	private bool CheckHaveReplayData()
	{
		if (this.replayBufferIndex >= this.CurrentData.Count)
		{
			if (!this.hasWarnedOutOfData)
			{
				this.hasWarnedOutOfData = true;
			}
			return false;
		}
		return true;
	}

	public void FillCarInputsGrid(out DriverInputs driverInputs)
	{
		driverInputs.GearChangeUp = false;
		driverInputs.GearChangeDown = false;
		driverInputs.Nitrous = false;
		driverInputs.Throttle = 0f;
		if (!this.running)
		{
			return;
		}
		bool flag = this.CheckHaveReplayData();
		if (flag && this.physicsFrameIndex == this.CurrentData[this.replayBufferIndex].FrameIndex)
		{
			if (!this.throttleToggle && this.CurrentData[this.replayBufferIndex].Throttle)
			{
				this.throttleToggle = true;
				this.replayBufferIndex++;
			}
			else if (this.throttleToggle && !this.CurrentData[this.replayBufferIndex].Throttle)
			{
				this.throttleToggle = false;
				this.replayBufferIndex++;
			}
		}
		if (this.throttleToggle)
		{
			driverInputs.Throttle = 1f;
		}
	}

	public void FillCarInputsRace(out DriverInputs driverInputs)
	{
		driverInputs.GearChangeUp = false;
		driverInputs.GearChangeDown = false;
		driverInputs.Throttle = 0f;
		driverInputs.Nitrous = false;
		if (!this.CheckHaveReplayData())
		{
			return;
		}
		if (this.physicsFrameIndex == this.CurrentData[this.replayBufferIndex].FrameIndex)
		{
			if (this.CurrentData[this.replayBufferIndex].GearUp)
			{
				driverInputs.GearChangeUp = true;
			}
			if (this.CurrentData[this.replayBufferIndex].GearDown)
			{
				driverInputs.GearChangeDown = true;
			}
			if (this.CurrentData[this.replayBufferIndex].NitrousDown)
			{
				driverInputs.Nitrous = true;
			}
			if (this.CurrentData[this.replayBufferIndex].Throttle)
			{
				driverInputs.Throttle = 1f;
			}
			this.replayBufferIndex++;
		}
	}

	public void AddGridFrameData(DriverInputs driverInputs)
	{
		if (!this.running)
		{
			return;
		}
		ReplayEvent item;
		item.FrameIndex = this.physicsFrameIndex;
		bool flag = false;
		if (driverInputs.Throttle > 0f && !this.throttleToggle)
		{
			this.throttleToggle = true;
			flag = true;
		}
		else if (driverInputs.Throttle == 0f && this.throttleToggle)
		{
			this.throttleToggle = false;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		item.Throttle = (driverInputs.Throttle > 0f);
		item.GearUp = driverInputs.GearChangeUp;
		item.GearDown = driverInputs.GearChangeDown;
		item.NitrousDown = driverInputs.Nitrous;
		this.ReplayData.GridReplayData.Add(item);
	}

	public void AddRaceFrameData(DriverInputs driverInputs)
	{
		if (!driverInputs.GearChangeDown && !driverInputs.GearChangeUp && !driverInputs.Nitrous)
		{
			return;
		}
		if (!this.running)
		{
			return;
		}
		ReplayEvent item;
		item.FrameIndex = this.physicsFrameIndex;
		item.Throttle = (driverInputs.Throttle > 0f);
		item.GearUp = driverInputs.GearChangeUp;
		item.GearDown = driverInputs.GearChangeDown;
		item.NitrousDown = driverInputs.Nitrous;
		this.ReplayData.RaceReplayData.Add(item);
	}
}
