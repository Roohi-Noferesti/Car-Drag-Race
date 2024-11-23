using UnityEngine;

public class HeadstartRaceCountdown : RaceCountdown
{
	public float timeDifference;

	public HeadstartRaceCountdown(RaceCentreMessaging messaging) : base(messaging)
	{
	}

	public void AICountdownStarted()
	{
		this.shown = false;
		this.timeDifference = RaceEventInfo.Instance.CurrentEvent.GetTimeDifference();
		if (this.timeDifference < 0f)
		{
			this.targetTime = Time.time - this.timeDifference;
		}
	}

	private bool CanStartCountDown()
	{
		return RaceEventInfo.Instance.CurrentEvent != null && (RaceEventInfo.Instance.CurrentEvent.AutoHeadstart || RelayManager.IsCurrentEventRelay()) && Time.time > this.targetTime;
	}

	public void Update()
	{
		if (!this.shown && this.state == eState.WAITING && this.CanStartCountDown())
		{
			this.timer = 0f;
			this.state = eState.LEAD_IN;
		}
		if (this.state == eState.WAITING)
		{
			return;
		}
		this.timer += Time.fixedDeltaTime;
		switch (this.state)
		{
		case eState.LEAD_IN:
			if (this.timer > this.messaging.LeadInTime)
			{
				this.state = eState.LIGHTCOUNTDOWN;
				this.timer = this.messaging.BetweenLightsTime + 0.01f;
				this.messaging.CentreMessageTextEnabled(true);
			}
			break;
		case eState.LIGHTCOUNTDOWN:
			if (this.timer > this.messaging.BetweenLightsTime)
			{
				this.timer = 0f;
				if (this.countdownValue == 0)
				{
					this.state = eState.GREEN;
					this.messaging.ShowGo();
				}
				else
				{
					this.messaging.ShowCountdownNumber(this.countdownValue);
					this.countdownValue--;
				}
			}
			break;
		case eState.GREEN:
			if (this.timer > this.messaging.GreenTime)
			{
				this.state = eState.FINISHED;
				this.timer = 0f;
			}
			break;
		case eState.FINISHED:
			this.messaging.CentreMessageTextEnabled(false);
			this.countdownValue = this.countdownFromNumber;
			this.state = eState.WAITING;
			this.shown = true;
			break;
		}
	}
}
