using System;

[Serializable]
public class BundleOfferData
{
	public int ID;

    public bool Active = true;

    public int Priority;

	public bool IsOfferRepeatable;

    public BundleOfferPopupData PopupData;//= new DontShowOfferPopupData();

    public void Initialise()
	{
		this.PopupData.Initialise();
	}

	public bool IsEligible(IGameState gs)
	{
		bool flag = this.PopupData.IsEligible(gs) && PlayerProfileManager.Instance.ActiveProfile.IsPopupValid(this.ID);
		if (flag)
		{
		}
		return flag;
	}

	public void PopupShowSuccess()
	{
		if (!this.PopupData.HasBeenShown)
		{
			this.PopupData.HasBeenShown = true;
		}
		PlayerProfileManager.Instance.ActiveProfile.IncreasePopupSeenCount(this.ID);
	}

	public PopUp GetPopUp()
	{
		if (PlayerProfileManager.Instance.ActiveProfile.GetPopupSeenCount(this.ID) == 0)
		{
			PlayerProfileManager.Instance.ActiveProfile.SetPopupFirstSeenTime(this.ID);
		}
		PlayerProfileManager.Instance.ActiveProfile.IncreasePopupSeenCount(this.ID);
		return this.PopupData.GetPopUp(this.ID, null, null);
	}
}
