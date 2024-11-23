using System;
using UnityEngine;

public class CrewStateTransitionWithinCrew : BaseCrewState
{
	private int crew;

	private int member;

	private float startOffsetX;

	private float targetOffsetX;

	private float delay = 0.4f;

	private float totalTime = 1f;

	public CrewStateTransitionWithinCrew(CrewProgressionScreen zParentScreen, int zCrew, int zMember) : base(zParentScreen)
	{
		this.crew = zCrew;
		this.member = zMember;
	}

	public CrewStateTransitionWithinCrew(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.crew = config.StateDetails.SlotIndex;
		this.member = config.StateDetails.MemberIndex;
		this.delay = config.StateDetails.Delay;
		this.totalTime = config.StateDetails.TotalTime;
	}

	public override void OnEnter()
	{
		bool flag = (BaseDevice.ActiveDevice.GetScreenWidth() >= 1000);
		if (this.member == 0)
		{
			this.targetOffsetX = 0f;
		}
		else if (this.member == 1)
		{
			if (flag)
			{
				this.targetOffsetX = 0.4f;
			}
			else
			{
				this.targetOffsetX = 0.7f;
			}
		}
		else if (flag)
		{
			this.targetOffsetX = 1f;
		}
		else
		{
			this.targetOffsetX = 1.3f;
		}
		float num = this.parentScreen.WidthOfEachCrew * (float)this.crew;
		this.startOffsetX = num;
		this.targetOffsetX += num;
		this.parentScreen.offsetX = this.startOffsetX * -1f;
	}

	public override bool OnMain()
	{
		base.OnMain();
		if (this.timeInState < this.delay)
		{
			return false;
		}
		float num = (this.timeInState - this.delay) / this.totalTime;
		bool result = false;
		if (num >= 1f)
		{
			result = true;
		}
		num = this.parentScreen.CurveS.Evaluate(num);
		num = Mathf.Clamp(num, 0f, 1f);
		this.parentScreen.offsetX = (num * this.targetOffsetX + (1f - num) * this.startOffsetX) * -1f;
		return result;
	}

	public override void OnExit()
	{
	}
}
