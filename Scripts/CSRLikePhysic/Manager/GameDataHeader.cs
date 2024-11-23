using System;

[Serializable]
public class GameDataHeader
{
	public string checkSum_Apple;

	public string checkSum_Android;

	public string checkSum_Windows;

	public string GetPlatformDependentChecksum()
	{
		return this.checkSum_Android;
	}
}
