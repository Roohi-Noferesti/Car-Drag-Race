using DataSerialization;
using System;
using UnityEngine;

public static class SoundEventDetailExtensions
{
	public static AnimationEvent GetAnimationEvent(this SoundEventDetail sed)
	{
		return new AnimationEvent
		{
			functionName = "PlaySoundFromStartTime",
			stringParameter = sed.Name + ":" + sed.SoundStart,
			time = sed.EventTime
		};
	}

	public static bool IsEligible(this SoundEventDetail sed, IGameState gameState)
	{
		return sed.SoundRequirements.IsEligible(gameState);
	}
}
