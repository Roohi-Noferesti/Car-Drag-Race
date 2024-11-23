using System;

public class MetricsCurrencySpend
{
	public enum ESpendType
	{
		PaidSpend,
		MixedSpend,
		FreeSpend,
		Unknown
	}

	public MetricsCurrencySpend.ESpendType spendType = MetricsCurrencySpend.ESpendType.Unknown;

	public EMetricsCurrencyType currencyType = EMetricsCurrencyType.Unknown;

	public int spendAmount;

	public int paidSpend;

	public int freeSpend;

	private bool m_IsSetup;

	public bool Setup(int to_spend, EMetricsCurrencyType currency_spent)
	{
		if (to_spend < 0)
		{
			to_spend *= -1;
		}
		this.currencyType = currency_spent;
		this.spendAmount = to_spend;
		Account currentAccount = UserManager.Instance.currentAccount;
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		int num;
		int num2;
		switch (this.currencyType)
		{
		case EMetricsCurrencyType.Cash:
			num = currentAccount.IAPCash;
			num2 = activeProfile.IAPCashSpent;
			break;
		case EMetricsCurrencyType.Gold:
			num = currentAccount.IAPGold;
			num2 = activeProfile.IAPGoldSpent;
			break;
		case EMetricsCurrencyType.Keys_Bronze:
			num = currentAccount.IAPKeys_Bronze;
			num2 = activeProfile.IAPGachaBronzeKeysSpent;
			break;
		case EMetricsCurrencyType.Keys_Silver:
			num = currentAccount.IAPKeys_Silver;
			num2 = activeProfile.IAPGachaSilverKeysSpent;
			break;
		case EMetricsCurrencyType.Keys_Gold:
			num = currentAccount.IAPKeys_Gold;
			num2 = activeProfile.IAPGachaGoldKeysSpent;
			break;
		default:
			return false;
		}
		if (num2 >= num)
		{
			this.spendType = MetricsCurrencySpend.ESpendType.FreeSpend;
			this.paidSpend = 0;
			this.freeSpend = to_spend;
		}
		else if (num < num2 + to_spend)
		{
			this.spendType = MetricsCurrencySpend.ESpendType.MixedSpend;
			this.paidSpend = num - num2;
			this.freeSpend = to_spend - this.paidSpend;
		}
		else
		{
			this.spendType = MetricsCurrencySpend.ESpendType.PaidSpend;
			this.paidSpend = to_spend;
			this.freeSpend = 0;
		}
		this.m_IsSetup = true;
		return true;
	}

	public bool Setup(int to_spend, GachaCostType currency_spent)
	{
		switch (currency_spent)
		{
		case GachaCostType.Free:
			return this.Setup(to_spend, EMetricsCurrencyType.Keys_Bronze);
		case GachaCostType.Keys_Bronze:
			return this.Setup(to_spend, EMetricsCurrencyType.Keys_Bronze);
		case GachaCostType.Keys_Silver:
			return this.Setup(to_spend, EMetricsCurrencyType.Keys_Silver);
		case GachaCostType.Keys_Gold:
			return this.Setup(to_spend, EMetricsCurrencyType.Keys_Gold);
		case GachaCostType.Gold:
			return this.Setup(to_spend, EMetricsCurrencyType.Gold);
		case GachaCostType.Cash:
			return this.Setup(to_spend, EMetricsCurrencyType.Cash);
		case GachaCostType.IAP:
			return this.Setup(to_spend, EMetricsCurrencyType.Keys_Bronze);
		default:
			return false;
		}
	}

	private string GetCurrencyFlowForJSONParameters()
	{
		switch (this.spendType)
		{
		case MetricsCurrencySpend.ESpendType.PaidSpend:
			return "paid_spend";
		case MetricsCurrencySpend.ESpendType.MixedSpend:
			return "mixed_spend";
		case MetricsCurrencySpend.ESpendType.FreeSpend:
			return "free_spend";
		default:
			return string.Empty;
		}
	}

	public bool AddToMetricsDictionary(MetricsDictionary parametersDict)
	{
		if (!this.m_IsSetup)
		{
			return false;
		}
        //MetricsCurrency.AddToMetricsDictionary(parametersDict, this.currencyType, this.GetCurrencyFlowForJSONParameters());
		parametersDict.Set<int>("amount1", -this.spendAmount);
		parametersDict.Set<int>("amount2", -this.paidSpend);
		parametersDict.Set<int>("amount3", -this.freeSpend);
		return true;
	}

	public void AddPaidSpendToAccount()
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		switch (this.currencyType)
		{
		case EMetricsCurrencyType.Cash:
			activeProfile.AddIAPSpentCash(this.paidSpend);
			break;
		case EMetricsCurrencyType.Gold:
			activeProfile.AddIAPSpentGold(this.paidSpend);
			break;
		case EMetricsCurrencyType.Keys_Bronze:
		case EMetricsCurrencyType.Keys_Silver:
		case EMetricsCurrencyType.Keys_Gold:
			activeProfile.AddIAPSpentGachaKeys(this.currencyType, this.paidSpend);
			break;
		}
	}
}
