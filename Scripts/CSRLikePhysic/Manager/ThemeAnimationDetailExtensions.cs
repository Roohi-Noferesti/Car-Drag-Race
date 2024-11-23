using DataSerialization;
using System;
using UnityEngine;

public static class ThemeAnimationDetailExtensions
{
	public static bool IsEligible(this ThemeAnimationDetail tad, IGameState gameState)
	{
		return tad.AnimationRequirements.IsEligible(gameState);
	}

	public static AnimationClip GetAnimationClip(this ThemeAnimationDetail tad, IGameState gameState)
	{
		AnimationClip animationClip = new AnimationClip
		{
			name = tad.Name
		};
		foreach (PinAnimationDetail current in tad.PinAnimations)
		{
			if (current.IsEligible(gameState))
			{
				animationClip.AddEvent(current.GetAnimationEvent());
			}
		}
		foreach (EventSelectEvent current2 in tad.EventSelectEvents)
		{
			if (current2.IsEligible(gameState))
			{
				animationClip.AddEvent(current2.GetAnimationEvent());
			}
		}
		foreach (SoundEventDetail current3 in tad.SoundEvents)
		{
			if (current3.IsEligible(gameState))
			{
				animationClip.AddEvent(current3.GetAnimationEvent());
			}
		}
		return animationClip;
	}
}
