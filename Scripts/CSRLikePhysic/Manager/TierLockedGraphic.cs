using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierLockedGraphic : MonoBehaviour
{
	public TextMeshProUGUI TierTitle;

	public Image Shadow;

	public AnimationCurve EaseCurve;

	public GameObject RYFPanelInfo;

	public GameObject RYFTierLeader;

    public TextMeshProUGUI RYFSubtitle;

	public GameObject WorldTourLogo;

	public void Setup(string title, string subTitle, int tierIndex)
	{
		this.TierTitle.text = title;
        //if (SocialController.Instance.isLoggedIntoFacebook)
        //{
        //    int friendsCountForTier = LumpManager.Instance.GetFriendsCountForTier((eCarTier)tierIndex);
        //    if (friendsCountForTier > 0)
        //    {
        //        this.RYFPanelInfo.SetActive(true);
        //        string textID = (friendsCountForTier != 1) ? "TEXT_RYF_FRIENDS_RACING_TIER" : "TEXT_RYF_FRIEND_RACING_TIER";
        //        this.RYFSubtitle.Text = string.Format(LocalizationManager.GetTranslation(textID), friendsCountForTier);
        //        RYFTierLeaderInfo component = this.RYFTierLeader.GetComponent<RYFTierLeaderInfo>();
        //        component.Setup(tierIndex);
        //    }
        //    else
        //    {
        //        this.RYFPanelInfo.SetActive(false);
        //    }
        //}
        //else
        //{
        //    this.RYFPanelInfo.SetActive(false);
        //}
		this.WorldTourLogo.SetActive(tierIndex == 5);
	}

	public void FadeIn(float duration)
	{
	}

	public void FadeOut(float duration)
	{
	}

	private void UpdateAnimation()
	{
	}

	private void Update()
	{
		this.UpdateAnimation();
	}
}
