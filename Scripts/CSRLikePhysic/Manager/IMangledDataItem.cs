using System;

public interface IMangledDataItem
{
	bool Validate();

	void Mangle(MangleInvoker invoker);

	void InitialiseMangle(long initialValue, MangleInvoker invoker);

	void Reset();

	void TriggerMetricsEvent();
}
