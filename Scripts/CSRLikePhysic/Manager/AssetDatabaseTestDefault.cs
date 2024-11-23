using System;

public class AssetDatabaseTestDefault : AssetDatabaseTestBase
{
	public override AssetDatabaseTestBase.DatabaseStatus AsynchronousStatus(bool fromStartup = false)
	{
		AssetDirectory assetDirectory;
		return (!base.SynchronousStatus(out assetDirectory, null, fromStartup)) ? AssetDatabaseTestBase.DatabaseStatus.Fail : AssetDatabaseTestBase.DatabaseStatus.Pass;
	}
}
