using System;
using DataSerialization;
using Metrics;

public class SendMetricEventPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		if (string.IsNullOrEmpty(details.StringValue))
		{
			this.LogWarning("(null)");
			return;
		}
		try
		{
			Events theEvent = EnumHelper.FromString<Events>(details.StringValue);
			Log.AnEvent(theEvent);
		}
		catch (ArgumentException)
		{
			this.LogWarning(details.StringValue);
		}
	}

	private void LogWarning(string str)
	{
	}
}
