using System;
using System.Collections.Generic;
using DataSerialization;

[Serializable]
public class PopupStatusData
{
    public string Desc;
    public bool IsActive = true;
	public int ID;

    public List<string> ScreenIDStrings;

    public int Priority;

	public float ShowDelay;

	public PopupData Popup = PopupData.CreateDontShowPopupData();

	public void Initialise()
	{
        this.Popup.Initialise();
	}

    public bool IsEligible(IGameState gs)
    {
        bool hasSeenBefore = PlayerProfileManager.Instance.ActiveProfile.GetPopupSeenCount(ID) > 0;

        bool flag = IsActive && this.Popup.IsEligible(gs) &&
                    (!hasSeenBefore || !Popup.CheckForShowOnlyOnce) &&
                    PlayerProfileManager.Instance.ActiveProfile.IsPopupValid(this.ID);
        if (flag)
        {
        }
        return flag;
    }

    public PopUp GetPopup()
	{
		if (PlayerProfileManager.Instance.ActiveProfile.GetPopupSeenCount(this.ID) == 0)
		{
			PlayerProfileManager.Instance.ActiveProfile.SetPopupFirstSeenTime(this.ID);
		}
	    return this.Popup.GetPopup(null, null);
	}

	public void PopupShowSuccess()
	{
		PlayerProfileManager.Instance.ActiveProfile.IncreasePopupSeenCount(this.ID);
	}

    public ScreenID ScreenIDEnum
    {
        get
        {
            return EnumHelper.FromString<ScreenID>(ScreenIDStrings[0]);
        }
    }
}
