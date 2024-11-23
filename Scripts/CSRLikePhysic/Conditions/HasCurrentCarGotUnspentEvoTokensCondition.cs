using System.Linq;
using DataSerialization;

public class HasCurrentCarGotUnspentEvoTokensCondition : EligibilityConditionBase
{
	protected override bool IsConditionValid(IGameState gameState, EligibilityConditionDetails details)
	{
		if (!gameState.CurrentCarUsesEvoParts())
		{
			return false;
		}
		int num = Enumerable.Range(0, (int)CarUpgradeData.NUM_UPGRADE_LEVELS).Sum((int i) => gameState.GetCurrentCarEvoPartsEarned(i));
		int currentCarNumEvoPartsSpent = gameState.GetCurrentCarNumEvoPartsSpent();
		return currentCarNumEvoPartsSpent < num;
	}
}
