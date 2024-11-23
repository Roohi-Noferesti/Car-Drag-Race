using DataSerialization;
using Metrics;
using System;
using System.Collections.Generic;

public class FireWTTutorialMetricsPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		string value = details.ThemeID.ToLower();
		if (string.IsNullOrEmpty(value))
		{
			value = TierXManager.Instance.CurrentThemeName;
		}
		Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
		{
			{
				Parameters.RTier,
				value
			},
			{
				Parameters.Title,
				details.StringValue
			}
		};
		Log.AnEvent(Events.WorldTourTutorial, data);
	}
}
