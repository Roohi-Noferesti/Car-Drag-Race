using Metrics;
using System;
using System.Collections.Generic;

public abstract class AwardPrizeBase
{
	public AwardPrizeBase()
	{
	}

	public void SendMetricsEvent()
	{
		string metricsTypeString = this.GetMetricsTypeString();
		string metricsCarToAwardString = this.GetMetricsCarToAwardString();
		int metricsFuelPipsToAward = this.GetMetricsFuelPipsToAward();
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.CardType,
				metricsTypeString
			},
			{
				Parameters.CarPart,
				metricsCarToAwardString
			},
			{
				Parameters.FuelPips,
				metricsFuelPipsToAward.ToString()
			}
		};
		Log.AnEvent(Events.PrizeCardWon, data);
	}

	public virtual string GetMetricsCarToAwardString()
	{
		return string.Empty;
	}

	public virtual int GetMetricsFuelPipsToAward()
	{
		return 0;
	}

	public abstract void AwardPrize();

	public abstract void TakePrizeAwayFromProfile();

	public abstract string GetMetricsTypeString();

    public abstract string GetPrizeString();
}
