using UnityEngine;

public class WorkshopRibbon : MonoBehaviour
{
    //public Image NewCarRibbon;

    //public TextMeshProUGUI NewCarRibbonText;

	private void Update()
	{
        //if (!GarageManager.Instance.CarIsLoaded)// || GarageManager.Instance.WorldTourPosterInFlight)
        //{
        //    return;
        //}
        SceneManagerGarage instance = SceneManagerGarage.Instance;
        if (!instance.CarIsLoaded || instance.WorldTourPosterInFlight)
        {
            return;
        }
		string currentlySelectedCarDBKey = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
        //if (FriendsRewardManager.Instance.ShouldShowNewCarRibbon)
        //{
        //    this.EnableRibbon("TEXT_FRIENDS_RIBBON_NEW_CAR", currentlySelectedCarDBKey);
        //}
		/*else */if (PlayerProfileManager.Instance.ActiveProfile.IsCarNew(currentlySelectedCarDBKey))
		{
			this.EnableRibbon("TEXT_RIBBON_NEW_CAR", currentlySelectedCarDBKey);
		}
		base.enabled = false;
	}

	private void EnableRibbon(string msg, string carDBKey)
	{
        //this.NewCarRibbon.gameObject.SetActive(true);
        //this.NewCarRibbonText.Text = LocalizationManager.GetTranslation(msg);
		GarageCameraManager.Instance.TriggerNewCarCameraSequence(carDBKey);
	}
}
