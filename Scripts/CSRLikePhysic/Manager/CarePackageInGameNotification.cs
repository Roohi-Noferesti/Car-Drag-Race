using System;

[Serializable]
public class CarePackageInGameNotification
{
	public enum eConfirmAction
	{
		Reward,
		TierShowroom,
		TuningScreen,
		ConsumablesOverview,
		MultiplayerPane,
		MapScreen
	}

	public string Title;

	public string Body;

	public string Image;

	public string ImageCaption;

	public string CancelText;

	public string ConfirmText;

    public eConfirmAction ConfirmActionEnum;

	public int BossTier = -1;

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Body);
    }
}
