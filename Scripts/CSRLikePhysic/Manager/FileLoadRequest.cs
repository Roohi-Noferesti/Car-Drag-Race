using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FileLoadRequest
{
	public BundleLoadedDelegate ready
	{
		get;
		private set;
	}

	public IBundleOwner owner
	{
		get;
		private set;
	}

	public FileLoadRequest(BundleLoadedDelegate zReadyDelegate, IBundleOwner zOwner)
	{
		this.ready = zReadyDelegate;
		this.owner = zOwner;
	}

	public bool Matches(BundleLoadedDelegate zReadyDelegate, IBundleOwner zOwner)
	{
		return zReadyDelegate == this.ready && zOwner == this.owner;
	}

	public void Success(string assetID, AssetBundle bundle)
	{
        //Debug.Log("bundle load success : " + assetID);
        this.ready(assetID, bundle, this.owner);
	}

	[Conditional("CSR_DEBUG_LOGGING")]
	public void Failure(string assetID)
	{
	}
}
