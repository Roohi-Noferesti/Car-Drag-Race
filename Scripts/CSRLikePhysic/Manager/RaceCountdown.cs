public class RaceCountdown
{
	public enum eState
	{
		WAITING,
		LEAD_IN,
		LIGHTCOUNTDOWN,
		GREEN,
		FINISHED,
		WIN_LOSE
	}

	protected const float countdownTime = 3.14f;

	protected float timer;

	protected eState state;

	protected int countdownFromNumber = 3;

	protected int countdownValue;

	protected RaceCentreMessaging messaging;

	protected bool shown;

	protected float targetTime = 3.40282347E+38f;

	public bool IsWaiting
	{
		get
		{
			return this.state == eState.WAITING;
		}
	}

	public bool Shown
	{
		get
		{
			return this.shown;
		}
	}

	public eState State
	{
		get
		{
			return this.state;
		}
		set
		{
			this.state = value;
		}
	}

	public RaceCountdown(RaceCentreMessaging messaging)
	{
		this.messaging = messaging;
		this.Reset();
	}

	public virtual void Reset()
	{
		this.timer = 0f;
		this.countdownValue = this.countdownFromNumber;
		this.state = eState.WAITING;
		this.shown = false;
		this.targetTime = 3.40282347E+38f;
	}
}
