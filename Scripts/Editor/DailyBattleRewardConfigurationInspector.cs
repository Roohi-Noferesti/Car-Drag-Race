using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DailyBattleRewardConfiguration))]
public class DailyBattleRewardConfigurationInspector : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Set lose to 1 hour"))
        {
            if ( !EditorUtility.DisplayDialog("reset to 1 hour",
                    "Are you sure you want to reset all lose reward to 1 hour", "yes", "no"))
                return;

            var db = target as DailyBattleRewardConfiguration;
            foreach (var rewardContainer in db.RewardContainer)
            {
                foreach (var tierReward in rewardContainer.TierRewards)
                {
                    tierReward.LoseRewardList[0].CooldownTime = new SerializedTimeSpan(0, 1, 0, 0);
                    tierReward.LoseRewardList[0].RewardIcon = tierReward.WinRewardList[0].RewardIcon;
                    tierReward.LoseRewardList[0].RewardValue = 0;
                    tierReward.LoseRewardList[0].RewardId = tierReward.WinRewardList[0].RewardId;
                    tierReward.LoseRewardList[0].RewardType = tierReward.WinRewardList[0].RewardType;
                }
            }

            EditorUtility.SetDirty(target);
        }
    }
}
