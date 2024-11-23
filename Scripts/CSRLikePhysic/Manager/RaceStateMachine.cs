using System.Collections.Generic;
using UnityEngine;

public class RaceStateMachine
{
	public RaceController controller;

	private Dictionary<RaceStateEnum, RaceStateBase> states;

	public RaceStateBase current
	{
		get;
		private set;
	}

	public RaceStateEnum currentId
	{
		get
		{
			return current.id;
		}
	}

	public void Initialise(RaceController inController)
	{
		controller = inController;
		states = new Dictionary<RaceStateEnum, RaceStateBase>();
		RaceStateSetup current = new RaceStateSetup(this);
		new RaceStateEnter(this);
		new RaceStatePreCountdown(this);
		new RaceStateCountdown(this);
		new RaceStateRace(this);
		new RaceStateEnd(this);
		new RaceStateExit(this);
		this.current = current;
		this.current.Enter();
	}

	public void AddState(RaceStateBase newState)
	{
		states[newState.id] = newState;
	}

	public void FixedUpdate()
	{
        //LogUtility.Log(current.id);
		current.FixedUpdate();
	}

	public RaceStateBase GetState(RaceStateEnum id)
	{
		return states[id];
	}

	public TGameState GetState<TGameState>(RaceStateEnum id) where TGameState : RaceStateBase
	{
		return (TGameState)((object)states[id]);
	}

	public bool StateIs(RaceStateEnum id)
	{
		return current.id == id;
	}

	public bool StateBefore(RaceStateEnum id)
	{
		return current.id < id;
	}

	public bool StateAfter(RaceStateEnum id)
	{
		return current.id > id;
	}

	public void SetState(RaceStateEnum id)
	{
		RaceStateBase state = GetState(id);
		if (state == current)
		{
			return;
		}

		Debug.Log("Setting State to: " + id);
		if (current != null)
		{
			current.Exit();
		}
		current = state;
		current.Enter();
	}
}
