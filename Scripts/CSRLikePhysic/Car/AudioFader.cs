using Fabric;
using System;
using UnityEngine;

public class AudioFader
{
	private float _startVolume;

	private float _targetVolume;

	private float _duration;

	private float _delay;

	private float _elapsed;

	public AudioFader(float volume, float duration, float delay = 0f)
	{
		this._targetVolume = volume;
		this._duration = duration;
		this._delay = delay;
		this._startVolume = -1f;
	}

	public bool Update(ref Fabric.Component component)
	{
		this._elapsed += Time.deltaTime;
		float num = this._elapsed - this._delay;
		if (num >= 0f)
		{
			if (this._startVolume < 0f)
			{
				this._startVolume = component.Volume;
			}
			float t = num / this._duration;
			component.Volume = Mathf.Lerp(this._startVolume, this._targetVolume, t);
			if (num >= this._duration)
			{
				return true;
			}
		}
		return false;
	}
}
