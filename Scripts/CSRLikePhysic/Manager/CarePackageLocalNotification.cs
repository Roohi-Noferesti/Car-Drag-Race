using System;

[Serializable]
public class CarePackageLocalNotification
{
	public string Message;

	public string ActionText;

    public string LargeIconInSideCountry;

    public string LargeIconOutSideCountry;

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Message);
    }
}
