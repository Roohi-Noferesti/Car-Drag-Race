using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProgressionPopupsDatabase : ConfigurationAssetLoader
{
    private Dictionary<ScreenID, List<PopupStatusData>> screenPopups = new Dictionary<ScreenID, List<PopupStatusData>>();

	public ProgressionPopupsConfiguration Configuration
	{
		get;
		private set;
	}

	public ProgressionPopupsDatabase() : base(GTAssetTypes.configuration_file, "ProgressionPopupsConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.Configuration = (ProgressionPopupsConfiguration) scriptableObject;//JsonConverter.DeserializeObject<ProgressionPopupsConfiguration>(assetDataString);
		this.screenPopups.Clear();
		foreach (PopupStatusData current in this.Configuration.PopupsData)
		{
			current.Initialise();
      //      foreach (var screenId in current.ScreenIds)
		    //{
                if (!this.screenPopups.ContainsKey(current.ScreenIDEnum))
                {
                    this.screenPopups[current.ScreenIDEnum] = new List<PopupStatusData>();
                }
                this.screenPopups[current.ScreenIDEnum].Add(current);
		    //}
		}
	}

    public List<PopupStatusData> GetProgressionPopupsForScreen(ScreenID screen)
	{
		if (!this.screenPopups.ContainsKey(screen))
		{
			return new List<PopupStatusData>();
		}
		return this.screenPopups[screen];
	}

    public List<PopupStatusData> GetEligibleProgressionPopupsForScreen(ScreenID screen, IGameState gs = null)
	{
		if (!this.screenPopups.ContainsKey(screen))
		{
			return new List<PopupStatusData>();
		}
		if (gs == null)
		{
            gs = new GameStateFacade();
		}
		return this.screenPopups[screen].FindAll((PopupStatusData pd) => pd.IsEligible(gs));
	}

    public PopupStatusData GetProgressionPopupDataForScreen(ScreenID screen, IGameState gs = null)
	{
		List<PopupStatusData> source = (from p in this.GetEligibleProgressionPopupsForScreen(screen, gs)
		orderby p.Priority descending
		select p).ToList<PopupStatusData>();
		return source.FirstOrDefault<PopupStatusData>();
	}

    public PopUp GetProgressionPopupForScreen(ScreenID screen)
	{
		PopupStatusData progressionPopupDataForScreen = this.GetProgressionPopupDataForScreen(screen, null);
		return (progressionPopupDataForScreen == null) ? null : progressionPopupDataForScreen.GetPopup();
	}

    public bool ShowProgressionPopupForScreen(ScreenID screen, IGameState gs = null)
	{
#if UNITY_EDITOR
        if (GameDatabase.Instance.EventDebugConfiguration.DontShowAnyProgressionPopup)
            return false;
#endif
		if (PopUpManager.Instance.isShowingPopUp)
		{
			return false;
		}
		PopupStatusData popupData = this.GetProgressionPopupDataForScreen(screen, gs);
		if (popupData == null)
		{
			return false;
		}
		if (popupData.ShowDelay > 0f)
		{
            CoroutineManager.Instance.StartCoroutineDelegate(() => this.ShowPopupWithDelay(popupData));
		}
		else
		{
			PopUp popup = popupData.GetPopup();
			if (!PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null))
			{
				return false;
			}
			popupData.PopupShowSuccess();
		}
		return true;
	}

	private IEnumerator ShowPopupWithDelay(PopupStatusData popupData)
	{
        ScreenManager.Instance.Interactable = false;
        yield return new WaitForSeconds(popupData.ShowDelay);
        PopUp popup = popupData.GetPopup();
        if (PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null))
        {
            popupData.PopupShowSuccess();
        }
        ScreenManager.Instance.Interactable = true;
    }
}
