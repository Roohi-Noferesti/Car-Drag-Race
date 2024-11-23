using DataSerialization;
using UnityEngine;

public class RandomRangeCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		float value = Random.value;
		return base.IsInRange(value, details);
	}
}
