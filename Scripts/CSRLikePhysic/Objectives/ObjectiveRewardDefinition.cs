using LitJson;
using System;

namespace Objectives
{
    [System.Serializable]
	public class ObjectiveRewardDefinition
	{
		public bool valid;

		public string rewardID;

		public ERewardType rewardType;

		public int rewardAmount;

		public int[] rewardAmountPerTier = new int[5];

		public eUpgradeType partType;

		public int partGrade;

		public ObjectiveRewardDefinition(string id, ref JsonDict rewardDefinitionDict)
		{
			this.rewardID = id;
			if (!rewardDefinitionDict.ContainsKey("rewardType") || !rewardDefinitionDict.TryGetEnum<ERewardType>("rewardType", out this.rewardType))
			{
				this.valid = false;
				return;
			}
			if (!rewardDefinitionDict.ContainsKey("rewardAmount") || !rewardDefinitionDict.TryGetValue("rewardAmount", out this.rewardAmount))
			{
				if (!rewardDefinitionDict.ContainsKey("rewardAmountPerTier") || !rewardDefinitionDict.TryGetValue("rewardAmountPerTier", out this.rewardAmountPerTier))
				{
					this.valid = false;
					return;
				}
				this.rewardAmount = -1;
			}
			if (!rewardDefinitionDict.ContainsKey("partType") || !rewardDefinitionDict.TryGetEnum<eUpgradeType>("partType", out this.partType))
			{
			}
			if (!rewardDefinitionDict.ContainsKey("partGrade") || !rewardDefinitionDict.TryGetValue("partGrade", out this.partGrade))
			{
			}
			this.valid = true;
		}

		public CSR2ApplyableReward GetCSR2ApplyableReward()
		{
			return new CSR2ApplyableReward(this.rewardType, this.rewardAmount)
			{
				partType = this.partType,
				partGrade = this.partGrade
			};
		}
	}
}
