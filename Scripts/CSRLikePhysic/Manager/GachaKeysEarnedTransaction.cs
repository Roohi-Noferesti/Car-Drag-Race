using LitJson;
using System;

public class GachaKeysEarnedTransaction : PlayerProfileTransactionBase
{
	private int m_KeysEarned;

	private EGachaKeysEarnedReason m_Reason;

	public GachaKeysEarnedTransaction() : base(ETransactionAction.GachaKeysEarned)
	{
	}

	public GachaKeysEarnedTransaction(int keysEarned, EGachaKeysEarnedReason reason) : base(ETransactionAction.GachaKeysEarned)
	{
		this.m_KeysEarned = keysEarned;
		this.m_Reason = reason;
	}

	public GachaKeysEarnedTransaction(int keysEarned, GachaType eGachaType, EGachaKeysEarnedReason reason) : base(ETransactionAction.GachaKeysEarned)
	{
		this.m_KeysEarned = keysEarned;
		this.m_Reason = reason;
		switch (eGachaType)
		{
		case GachaType.Bronze:
			this.m_Action = ETransactionAction.GachaBronzeKeysEarned;
			break;
		case GachaType.Silver:
			this.m_Action = ETransactionAction.GachaSilverKeysEarned;
			break;
		case GachaType.Gold:
			this.m_Action = ETransactionAction.GachaGoldKeysEarned;
			break;
		}
	}

	protected override void SerializeSpecificDataToJson(JsonDict dict)
	{
		dict.Set("increase", this.m_KeysEarned);
		dict.SetEnum<EGachaKeysEarnedReason>("reason", this.m_Reason);
	}

	protected override void SerializeSpecificDataFromJson(JsonDict dict)
	{
		this.m_KeysEarned = dict.GetInt("increase");
		this.m_Reason = dict.GetEnum<EGachaKeysEarnedReason>("reason");
	}

	public override string ToString()
	{
		return string.Format("Gacha Keys earned {0}, Reason {1}, Action {2}, Timestamp {3}", new object[]
		{
			this.m_KeysEarned,
			this.m_Reason,
			this.m_Action,
			this.m_TimeStamp
		});
	}
}
