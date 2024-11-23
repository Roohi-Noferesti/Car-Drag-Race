using LitJson;
using System;

public class GachaKeysSpentTransaction : PlayerProfileTransactionBase
{
	private int m_KeysSpent;

	private EGachaKeysSpentReason m_Reason;

	public GachaKeysSpentTransaction() : base(ETransactionAction.GachaKeysSpent)
	{
	}

	public GachaKeysSpentTransaction(int keysSpent, EGachaKeysSpentReason reason) : base(ETransactionAction.GachaKeysSpent)
	{
		this.m_KeysSpent = keysSpent;
		this.m_Reason = reason;
	}

	public GachaKeysSpentTransaction(int keysSpent, GachaType eGachaType, EGachaKeysSpentReason reason) : base(ETransactionAction.GachaKeysSpent)
	{
		this.m_KeysSpent = keysSpent;
		this.m_Reason = reason;
		switch (eGachaType)
		{
		case GachaType.Bronze:
			this.m_Action = ETransactionAction.GachaBronzeKeysSpent;
			break;
		case GachaType.Silver:
			this.m_Action = ETransactionAction.GachaSilverKeysSpent;
			break;
		case GachaType.Gold:
			this.m_Action = ETransactionAction.GachaGoldKeysSpent;
			break;
		}
	}

	protected override void SerializeSpecificDataToJson(JsonDict dict)
	{
		dict.Set("increase", this.m_KeysSpent);
		dict.SetEnum<EGachaKeysSpentReason>("reason", this.m_Reason);
	}

	protected override void SerializeSpecificDataFromJson(JsonDict dict)
	{
		this.m_KeysSpent = dict.GetInt("increase");
		this.m_Reason = dict.GetEnum<EGachaKeysSpentReason>("reason");
	}

	public override string ToString()
	{
		return string.Format("Gacha Keys spent {0}, Reason {1}, Action {2}, Timestamp {3}", new object[]
		{
			this.m_KeysSpent,
			this.m_Reason,
			this.m_Action,
			this.m_TimeStamp
		});
	}
}
