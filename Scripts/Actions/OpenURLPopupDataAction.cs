using System;
using DataSerialization;
using UnityEngine;

public class OpenURLPopupDataAction : PopupDataActionBase
{
	public override void Execute(EligibilityConditionDetails details)
	{
		this.OpenURL(details.StringValue);
	}

	protected void OpenURL(string url)
	{
		NativeEvents.urlOpenedEvent += new NativeEvents_DelegateUri(this.urlOpenend);
		Application.OpenURL(url);
	}

	private void urlOpenend(Uri url)
	{
        //URLProcessor uRLProcessor = URLProcess.ProcessURL(url);
        //if (uRLProcessor.IsValid)
        //{
        //    uRLProcessor.Execute();
        //}
        //NativeEvents.urlOpenedEvent -= new NativeEvents_DelegateUri(this.urlOpenend);
	}
}
