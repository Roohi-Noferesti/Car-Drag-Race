using System;
using UnityEngine;

public class AsyncBundleCallback
{
	public string name
	{
		get;
		private set;
	}

	public BundleCallbackDelegate callback
	{
		get;
		private set;
	}

	public GameObject owner
	{
		get;
		private set;
	}

	public BundleFailureCallbackDelegate failureCallback
	{
		get;
		private set;
	}

	public AsyncBundleCallback(string inName, BundleCallbackDelegate cb, GameObject inOwner, BundleFailureCallbackDelegate fcb)
	{
		this.name = inName;
		this.callback = cb;
		this.owner = inOwner;
		this.failureCallback = fcb;
	}
}
