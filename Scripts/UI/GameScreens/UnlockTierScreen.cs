using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockTierScreen : ZHUDScreen
{
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private GameObject[] m_effects;
    private eCarTier m_tier;

    public override ScreenID ID
    {
        get { return ScreenID.TierUnlocked; }
    }


    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        var currentEvent = RaceEventInfo.Instance.CurrentEvent;

        m_tier = currentEvent.GetTierEvent().GetCarTier() + 1;
        var str = LocalizationManager.GetTranslation("UNLOCK_TIERSCREEN_TITLE");
        var tierText = LocalizationManager.GetTranslation("TEXT_" + m_tier);
        var formatText = string.Format(str, tierText);
        m_titleText.text = formatText;
        foreach (var effect in m_effects)
        {
            effect.SetActive(false);
        }
        Animator.Play(OpenAnimationName);
    }

    public void GotoShowroom()
    {
        ShowroomScreen.ShowScreenWithTierCarList(m_tier, false, true);
    }


    public void OnShareButton()
    {
        int num = (int)(RaceEventQuery.Instance.getHighestUnlockedClass() + 1);
        SocialController.Instance.OnShareButton(SocialController.MessageType.UNLOCK_TIER, num.ToString(), false, false);
    }

    public void SetShareButtonString(bool forceGenericString = false)
    {
        //string text;
        //if (SocialController.Instance.SocialRewardAllowed() && !forceGenericString)
        //{
        //    string colouredCashString = CurrencyUtils.GetColouredCashString(GameDatabase.Instance.Social.GetCashRewardForTwitter());
        //    text = string.Format(LocalizationManager.GetTranslation("TEXT_SHARE_TO_GET"), colouredCashString);
        //}
        //else
        //{
        //    text = LocalizationManager.GetTranslation("TEXT_MENU_ICON_SHARE");
        //}
        //this.TwitterButton.SetText(text, true, true);
    }
}
