using System;
using System.Collections.Generic;

[Serializable]
public class PopupsProfileData
{
	private const string PROGRESSION_DATA = "pppd";

	private const string POPUP_DATA_ID = "ppid";

	private const string POPUP_DATA_COUNT = "ppcn";

	private const string POPUP_DATA_FIRST_SEEN = "ppfs";

	private const string POPUP_ISVALID = "ppiv";

	public List<PopupProfileData> Popups = new List<PopupProfileData>();

    public void ToJson(ref JsonDict jsonDict)
    {
        jsonDict.SetObjectList<PopupProfileData>("pppd", this.Popups, new SetObjectDelegate<PopupProfileData>(this.SetPopupsData));
    }

    private void SetPopupsData(PopupProfileData popupData, ref JsonDict jsonDict)
    {
        jsonDict.Set("ppid", popupData.PopupID);
        jsonDict.Set("ppcn", popupData.SeenCount);
        jsonDict.Set("ppfs", popupData.FirstSeenTime);
        jsonDict.Set("ppiv", popupData.IsValid);
    }

    public void FromJson(ref JsonDict jsonDict)
    {
        if (!jsonDict.TryGetObjectList<PopupProfileData>("pppd", out this.Popups, new GetObjectDelegate<PopupProfileData>(this.GetPopupsData)))
        {
            this.Popups = new List<PopupProfileData>();
        }
    }

    private void GetPopupsData(JsonDict jsonDict, ref PopupProfileData popupData)
    {
        jsonDict.TryGetValue("ppid", out popupData.PopupID);
        jsonDict.TryGetValue("ppcn", out popupData.SeenCount);
        jsonDict.TryGetValue("ppfs", out popupData.FirstSeenTime);
        jsonDict.TryGetValue("ppiv", out popupData.IsValid);
    }

	public int GetPopupSeenCount(int popupID)
	{
		PopupProfileData popupProfileData = this.Popups.Find((PopupProfileData p) => p.PopupID == popupID);
		if (popupProfileData == null)
		{
			return 0;
		}
		return popupProfileData.SeenCount;
	}

	public void IncreasePopupSeenCount(int popupID)
	{
		PopupProfileData popupProfileData = this.Popups.Find((PopupProfileData p) => p.PopupID == popupID);
		if (popupProfileData == null)
		{
			popupProfileData = new PopupProfileData();
			popupProfileData.PopupID = popupID;
			this.Popups.Add(popupProfileData);
		}
		popupProfileData.SeenCount++;
	}

	public void ResetPopupSeenCount(int popupID)
	{
		PopupProfileData popupProfileData = this.Popups.Find((PopupProfileData p) => p.PopupID == popupID);
		if (popupProfileData != null)
		{
			popupProfileData.SeenCount = 0;
		}
	}

	public DateTime GetPopupFirstSeenTime(int popupID)
	{
		PopupProfileData popupProfileData = this.Popups.Find((PopupProfileData p) => p.PopupID == popupID);
		if (popupProfileData == null)
		{
            return GTDateTime.Now;
		}
		return popupProfileData.FirstSeenTime;
	}

	public void SetPopupFirstSeenTime(int popupID)
	{
		PopupProfileData popupProfileData = this.Popups.Find((PopupProfileData p) => p.PopupID == popupID);
		if (popupProfileData == null)
		{
			popupProfileData = new PopupProfileData();
			popupProfileData.PopupID = popupID;
			this.Popups.Add(popupProfileData);
		}
        popupProfileData.FirstSeenTime = GTDateTime.Now;
	}

	public bool IsPopupValid(int popupID)
	{
		PopupProfileData popupProfileData = this.Popups.Find((PopupProfileData p) => p.PopupID == popupID);
		return popupProfileData == null || popupProfileData.IsValid;
	}

	public void SetPopupIsValid(int popupID, bool valid)
	{
		PopupProfileData popupProfileData = this.Popups.Find((PopupProfileData p) => p.PopupID == popupID);
		if (popupProfileData == null)
		{
			popupProfileData = new PopupProfileData();
			popupProfileData.PopupID = popupID;
			this.Popups.Add(popupProfileData);
		}
		popupProfileData.IsValid = valid;
	}

	public void ResetPopupData(int popupID)
	{
		PopupProfileData popupProfileData = this.Popups.Find((PopupProfileData p) => p.PopupID == popupID);
		if (popupProfileData != null)
		{
			this.Popups.Remove(popupProfileData);
		}
	}
}
