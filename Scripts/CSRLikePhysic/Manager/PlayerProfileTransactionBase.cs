using LitJson;
using System;

public abstract class PlayerProfileTransactionBase
{
	protected ETransactionAction m_Action;

	protected int m_TimeStamp;

	private bool m_IsUploading;

	public bool IsUploading
	{
		get
		{
			return this.m_IsUploading;
		}
		set
		{
			this.m_IsUploading = value;
		}
	}

	protected PlayerProfileTransactionBase(ETransactionAction action)
	{
		this.m_Action = action;
		this.m_TimeStamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
	}

	protected abstract void SerializeSpecificDataFromJson(JsonDict dict);

	protected abstract void SerializeSpecificDataToJson(JsonDict dict);

	public void SerializeToJsonDict(JsonDict dict, bool includeUploadingState)
	{
		dict.SetEnum<ETransactionAction>("action", this.m_Action);
		dict.Set("timestamp", this.m_TimeStamp);
		if (includeUploadingState)
		{
			dict.Set("isUploading", this.m_IsUploading);
		}
		this.SerializeSpecificDataToJson(dict);
	}

	public static PlayerProfileTransactionBase CreateObjectFromJson(JsonDict dict)
	{
		ETransactionAction @enum = dict.GetEnum<ETransactionAction>("action");
		int @int = dict.GetInt("timestamp");
		bool isUploading = false;
		if (dict.ContainsKey("isUploading"))
		{
			isUploading = dict.GetBool("isUploading", false);
		}
		PlayerProfileTransactionBase playerProfileTransactionBase = null;
		switch (@enum)
		{
		case ETransactionAction.CashEarned:
			playerProfileTransactionBase = new CashEarnedTransaction();
			break;
		case ETransactionAction.CashSpent:
			playerProfileTransactionBase = new CashSpentTransaction();
			break;
		case ETransactionAction.GoldEarned:
			playerProfileTransactionBase = new GoldEarnedTransaction();
			break;
		case ETransactionAction.GoldSpent:
			playerProfileTransactionBase = new GoldSpentTransaction();
			break;
		case ETransactionAction.GachaKeysEarned:
			playerProfileTransactionBase = new GachaKeysEarnedTransaction();
			break;
		case ETransactionAction.GachaKeysSpent:
			playerProfileTransactionBase = new GachaKeysSpentTransaction();
			break;
		case ETransactionAction.GachaTokensEarned:
            //playerProfileTransactionBase = new GachaTokensEarnedTransaction();
			break;
		case ETransactionAction.GachaTokensSpent:
            //playerProfileTransactionBase = new GachaTokensSpentTransaction();
			break;
		case ETransactionAction.GachaBronzeKeysEarned:
			playerProfileTransactionBase = new GachaKeysEarnedTransaction(0, GachaType.Bronze, EGachaKeysEarnedReason.Unknown);
			break;
		case ETransactionAction.GachaBronzeKeysSpent:
			playerProfileTransactionBase = new GachaKeysSpentTransaction(0, GachaType.Bronze, EGachaKeysSpentReason.Unknown);
			break;
		case ETransactionAction.GachaSilverKeysEarned:
			playerProfileTransactionBase = new GachaKeysEarnedTransaction(0, GachaType.Silver, EGachaKeysEarnedReason.Unknown);
			break;
		case ETransactionAction.GachaSilverKeysSpent:
			playerProfileTransactionBase = new GachaKeysSpentTransaction(0, GachaType.Silver, EGachaKeysSpentReason.Unknown);
			break;
		case ETransactionAction.GachaGoldKeysEarned:
			playerProfileTransactionBase = new GachaKeysEarnedTransaction(0, GachaType.Gold, EGachaKeysEarnedReason.Unknown);
			break;
		case ETransactionAction.GachaGoldKeysSpent:
			playerProfileTransactionBase = new GachaKeysSpentTransaction(0, GachaType.Gold, EGachaKeysSpentReason.Unknown);
			break;
		case ETransactionAction.RPEarned:
            //playerProfileTransactionBase = new RPEarnedTransaction();
			break;
		case ETransactionAction.RPSpent:
            //playerProfileTransactionBase = new RPSpentTransaction();
			break;
		case ETransactionAction.CrewRPEarned:
            //playerProfileTransactionBase = new CrewRPEarnedTransaction();
			break;
		case ETransactionAction.RacesInCrewEarned:
            //playerProfileTransactionBase = new RaceInCrewEarnedTransaction();
			break;
		case ETransactionAction.RacesInCrewSpent:
            //playerProfileTransactionBase = new RaceInCrewSpentTransaction();
			break;
		case ETransactionAction.CrewEventRace:
            //playerProfileTransactionBase = new RaceInCrewEventRaceTransaction();
			break;
		case ETransactionAction.CrewVsRaceResult:
            //playerProfileTransactionBase = new RaceInCrewVsCrewTransaction();
			break;
		case ETransactionAction.TuningAttemptsEarned:
            //playerProfileTransactionBase = new TuningAttemptsEarnedTransaction();
			break;
		case ETransactionAction.TuningAttemptsSpent:
            //playerProfileTransactionBase = new TuningAttemptsSpentTransaction();
			break;
		case ETransactionAction.FreshRPEarned:
            //playerProfileTransactionBase = new FreshRPEarnedTransaction();
			break;
		case ETransactionAction.FusionPartCrewEarned:
            //playerProfileTransactionBase = new FusionPartEarnedTransaction();
			break;
		}
		if (playerProfileTransactionBase != null)
		{
			playerProfileTransactionBase.SerializeSpecificDataFromJson(dict);
			playerProfileTransactionBase.m_TimeStamp = @int;
			playerProfileTransactionBase.m_IsUploading = isUploading;
		}
		return playerProfileTransactionBase;
	}
}
