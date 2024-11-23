using Metrics;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class MangledPlayerProfileInteger : IMangledDataItem
{
	public bool mangled;

	public long mangledDataItem;

	private MangleInvoker mangleInvoker;

	protected abstract int PlayerProfileDataItem
	{
		get;
		set;
	}

	public long SecureDataItem
	{
		get
		{
			return MemValidatorUtilities.MangleValue(this.mangledDataItem);
		}
	}

	public bool Validate()
	{
		long num = MemValidatorUtilities.MangleValue(this.PlayerProfileDataItem);
		return !this.mangled || num == this.mangledDataItem;
	}

	public void Mangle(MangleInvoker invoker)
	{
		this.mangledDataItem = MemValidatorUtilities.MangleValue(this.PlayerProfileDataItem);
		this.mangled = true;
		this.mangleInvoker = invoker;
	}

	public void InitialiseMangle(long initialValue, MangleInvoker invoker)
	{
		this.mangledDataItem = MemValidatorUtilities.MangleValue(initialValue);
		this.mangled = true;
		this.mangleInvoker = invoker;
	}

	public void Reset()
	{
		this.TriggerMetricsEvent();
		this.Mangle(MangleInvoker.ResetFromMemoryHack);
	}

	public void TriggerMetricsEvent()
	{
		Dictionary<Parameters, string> dictionary = new Dictionary<Parameters, string>
		{
			{
				Parameters.ValueIdentifier,
				base.GetType().ToString()
			},
			{
				Parameters.ProfileValue,
				this.PlayerProfileDataItem.ToString()
			},
			{
				Parameters.MangledValue,
				this.mangledDataItem.ToString()
			},
			{
				Parameters.UnmangledValue,
				this.SecureDataItem.ToString()
			},
			{
				Parameters.InvokedBy,
				this.mangleInvoker.ToString()
			}
		};
		dictionary.ToList<KeyValuePair<Parameters, string>>().ForEach(delegate(KeyValuePair<Parameters, string> kvp)
		{
		});
		Log.AnEvent(Events.MemoryValidatorFailed126, dictionary);
	}
}
