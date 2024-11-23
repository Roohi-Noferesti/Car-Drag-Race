using DataSerialization;
using System;
using UnityEngine;

public static class EventSelectEventExtensions
{
	public static AnimationEvent GetAnimationEvent(this EventSelectEvent ese)
	{
		return new AnimationEvent
		{
			functionName = ese.FunctionName,
			time = ese.EventTime
		};
	}

	public static bool IsEligible(this EventSelectEvent ese, IGameState gameState)
	{
		return ese.EventRequirements.IsEligible(gameState);
	}
}
