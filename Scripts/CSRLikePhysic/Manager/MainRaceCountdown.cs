using I2.Loc;
using UnityEngine;

public class MainRaceCountdown : RaceCountdown
{
	private bool showedHolding;
    private bool m_RunInDebug;

	public bool showCountdownGraphics = true;

	public MainRaceCountdown(RaceCentreMessaging messaging,bool runinDebug) : base(messaging)
	{
	    m_RunInDebug = runinDebug;
	}

	public override void Reset()
	{
		base.Reset();
		this.showedHolding = false;
	}

	public void Update()
	{
	    if (m_RunInDebug)
	        return;
		if (PauseGame.isGamePaused)
		{
			return;
		}
		if (this.state == eState.WAITING && this.messaging.HeadstartRaceCountdown.IsWaiting)
		{
			return;
		}
		if (!this.showedHolding && this.state == eState.LIGHTCOUNTDOWN && this.messaging.HeadstartRaceCountdown.timeDifference < -1f)
		{
			this.messaging.CentreMessageTextEnabled(true);
			this.messaging.ShowText(LocalizationManager.GetTranslation("TEXT_RELAY_HOLDING"));
			this.showedHolding = true;
		}
		this.timer += Time.fixedDeltaTime;
		switch (this.state)
		{
		case eState.LEAD_IN:
			if (this.timer > this.messaging.LeadInTime)
			{
				this.state = eState.LIGHTCOUNTDOWN;
				this.timer = this.messaging.BetweenLightsTime + 0.01f;
				if (this.showCountdownGraphics)
				{
					this.messaging.CentreMessageTextEnabled(true);
				}
			}
			break;
		case eState.LIGHTCOUNTDOWN:
			if (this.timer > this.messaging.BetweenLightsTime)
			{
				this.timer = 0f;
				if (this.countdownValue == 0)
				{
					this.messaging.TriggerRaceStartEvent();
					this.state = eState.GREEN;
					if (this.showCountdownGraphics)
					{
						this.messaging.ShowGo();
					}
				}
				else
				{
					if (this.showCountdownGraphics)
					{
						this.messaging.ShowCountdownNumber(this.countdownValue);
					}
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
			if (this.showCountdownGraphics)
			{
				this.messaging.CentreMessageTextEnabled(false);
			}
			this.countdownValue = this.countdownFromNumber;
			this.state = eState.WAITING;
			break;
		}
		if (!PauseGame.isGamePaused || (PauseGame.isGamePaused && !PauseGame.hasPopup))
		{
			this.messaging.DoTextAnimation();
		}
	}
}
