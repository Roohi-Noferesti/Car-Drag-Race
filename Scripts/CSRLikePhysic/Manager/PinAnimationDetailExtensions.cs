using DataSerialization;
using System;
using UnityEngine;

public static class PinAnimationDetailExtensions
{
	public static AnimationEvent GetAnimationEvent(this PinAnimationDetail pad)
	{
		return new AnimationEvent
		{
			functionName = "PlayPinAnimation",
			stringParameter = pad.PinLabel + ":" + pad.Name,
			time = pad.EventTime
		};
	}

	public static bool IsEligible(this PinAnimationDetail pad, IGameState gameState)
	{
		return pad.AnimationRequirements.IsEligible(gameState);
	}
}
