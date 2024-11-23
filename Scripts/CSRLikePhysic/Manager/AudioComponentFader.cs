using Fabric;
using System;
using UnityEngine;

public class AudioComponentFader : MonoBehaviour
{
	public delegate void OnFadeState(AudioComponentFader audioComponentFader);

	private Fabric.Component _component;

	private float _startVolume = -1f;

	private float _targetVolume = -1f;

	private float _duration = -1f;

	private float _delay;

	private float _elapsed;

	private AudioComponentFader.OnFadeState onFadeStart;

	private AudioComponentFader.OnFadeState onFadeComplete;

	private bool fadeEnabled;

	public Fabric.Component component
	{
		get
		{
			if (this._component == null)
			{
				this._component = base.GetComponent<Fabric.Component>();
			}
			return this._component;
		}
	}

	public float startVolume
	{
		get
		{
			return this._startVolume;
		}
		private set
		{
			this._startVolume = value;
		}
	}

	public float targetVolume
	{
		get
		{
			return this._targetVolume;
		}
		private set
		{
			this._targetVolume = value;
		}
	}

	public float duration
	{
		get
		{
			return this._duration;
		}
		private set
		{
			this._duration = value;
		}
	}

	public float delay
	{
		get
		{
			return this._delay;
		}
		private set
		{
			this._delay = value;
		}
	}

	public float elapsed
	{
		get
		{
			return this._elapsed;
		}
		private set
		{
			this._elapsed = value;
		}
	}

	public static void OnFadeStatePause(AudioComponentFader audioComponentFader)
	{
		if (audioComponentFader.component != null)
		{
			audioComponentFader.component.Pause(true);
		}
	}

	public static void OnFadeStateUnpause(AudioComponentFader audioComponentFader)
	{
		if (audioComponentFader.component != null)
		{
			audioComponentFader.component.Pause(false);
		}
	}

	private void Update()
	{
		if (this.fadeEnabled)
		{
			float num = this.elapsed - this.delay;
			bool flag = 0f < num;
			this.elapsed += Time.deltaTime;
			num = this.elapsed - this.delay;
			if (0f < num)
			{
				if (!flag && this.onFadeStart != null)
				{
					this.onFadeStart(this);
				}
				float t = num / this.duration;
				this.component.Volume = Mathf.Lerp(this.startVolume, this.targetVolume, t);
				if (this.duration <= num)
				{
					if (this.onFadeComplete != null)
					{
						this.onFadeComplete(this);
					}
					this.elapsed = 0f;
					this.fadeEnabled = false;
				}
			}
		}
	}

	public void Fade(float targetVolume, float duration, float delay = 0f, AudioComponentFader.OnFadeState onFadeStart = null, AudioComponentFader.OnFadeState onFadeComplete = null)
	{
		this.fadeEnabled = true;
		this.elapsed = 0f;
		this.startVolume = this.component.Volume;
		this.targetVolume = targetVolume;
		this.duration = duration;
		this.delay = delay;
		this.onFadeStart = onFadeStart;
		this.onFadeComplete = onFadeComplete;
	}
}
