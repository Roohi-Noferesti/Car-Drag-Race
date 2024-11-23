public class RaceStateBase
{
	public RaceStateEnum id;

	protected RaceStateMachine machine;

	public RaceStateBase(RaceStateMachine inMachine, RaceStateEnum inId)
	{
		this.machine = inMachine;
		this.id = inId;
		this.machine.AddState(this);
	}

	public virtual void Enter()
	{
	}

	public virtual void Exit()
	{
	}

	public virtual void FixedUpdate()
	{
	}
}
