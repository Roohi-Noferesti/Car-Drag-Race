using System;

public class AssetDatabaseTest
{
	private AssetDatabaseTestBase assetDatabaseTest;

	public AssetDatabaseTest()
	{
		this.assetDatabaseTest = new AssetDatabaseTestDefault();
	}

	public AssetDatabaseTestBase.DatabaseStatus AsynchronousStatus(bool fromStartup = false)
	{
		return this.assetDatabaseTest.AsynchronousStatus(fromStartup);
	}

	public bool SynchronousStatus(out AssetDirectory zDirectory, bool fromStartup = false)
	{
		return this.assetDatabaseTest.SynchronousStatus(out zDirectory, null, fromStartup);
	}
}
