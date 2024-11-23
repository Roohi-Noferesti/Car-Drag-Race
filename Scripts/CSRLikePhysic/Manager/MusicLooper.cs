using UnityEngine;

public class MusicLooper
{
	private const float scheduleAdvance = 1f;

	private AudioSource current;

	private AudioSource previous;

	private double nextEventTime;

	public float volume
	{
		get
		{
			return this.current.volume;
		}
		set
		{
			this.current.volume = value;
			this.previous.volume = value;
		}
	}

	public void Initialise(AudioSource current,AudioSource previous)
	{
	    this.current = current;//(o.AddComponent<AudioSource>() as AudioSource);
	    this.previous = previous;//(o.AddComponent<AudioSource>() as AudioSource);
		this.nextEventTime = AudioSettings.dspTime + 1.0;
	}

	public bool Update(AudioClip nextClip)
	{
		if (nextClip != null)
		{
			double dspTime = AudioSettings.dspTime;
			if (dspTime + 1.0 > this.nextEventTime)
			{
				this.ScheduleNextClip(nextClip);
				return true;
			}
		}
		return false;
	}

	public void Interrupt(AudioClip nextClip)
	{
		this.Stop();
		this.Update(nextClip);
	}

	public void Stop()
	{
		this.nextEventTime = AudioSettings.dspTime + 0.066666670143604279;
		this.current.SetScheduledEndTime(this.nextEventTime);
		if (this.previous.isPlaying)
		{
			this.previous.SetScheduledEndTime(this.nextEventTime);
		}
	}

	private void FlipSources()
	{
		AudioSource audioSource = this.current;
		this.current = this.previous;
		this.previous = audioSource;
	}

	private void ScheduleNextClip(AudioClip nextClip)
	{
		this.FlipSources();
		if (this.nextEventTime < AudioSettings.dspTime)
		{
			this.nextEventTime = AudioSettings.dspTime;
		}
		this.current.clip = nextClip;
		this.current.PlayScheduled(this.nextEventTime);
		this.nextEventTime += (double)nextClip.length;
	}
}
