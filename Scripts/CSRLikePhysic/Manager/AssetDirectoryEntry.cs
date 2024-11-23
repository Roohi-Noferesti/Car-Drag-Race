using System;

public class AssetDirectoryEntry
{
	public int version
	{
		get;
		private set;
	}

	public AssetQuality quality
	{
		get;
		private set;
	}

	public bool isOnDevice
	{
		get;
		private set;
	}

	public bool isCached
	{
		get;
		private set;
	}

	public bool isLocal
	{
		get
		{
			return this.isOnDevice || this.isCached;
		}
	}

	public AssetDirectoryEntry(int inVersion, AssetQuality inQuality, bool inOnDevice, bool inCached)
	{
		this.version = inVersion;
		this.quality = inQuality;
		this.isOnDevice = inOnDevice;
		this.isCached = inCached;
	}
}
