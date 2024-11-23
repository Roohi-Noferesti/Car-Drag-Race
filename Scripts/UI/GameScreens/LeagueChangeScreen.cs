using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Metrics;
using TMPro;
using UnityEngine.UI;

public class LeagueChangeScreen : ZHUDScreen 
{
    public static int BeforeLeague;
    public static int AfterLeague;

    [SerializeField] private TextMeshProUGUI m_title;
    [SerializeField] private TextMeshProUGUI m_body;
    [SerializeField] private TextMeshProUGUI m_leagueName;
    [SerializeField] private Image m_leagueImage;
    [SerializeField]
    private Sprite m_regularSprite;
    [SerializeField]
    private Sprite m_bronzeSprite;
    [SerializeField]
    private Sprite m_silverSprite;
    [SerializeField]
    private Sprite m_goldSprite;
    [SerializeField]
    private Sprite m_diamondSprite;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.LeagueChange;
        }
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        var currentleague = (LeagueData.LeagueName)AfterLeague;
        string titleText;
        string bodyText;
        if (AfterLeague >= BeforeLeague)
        {
            titleText = LocalizationManager.GetTranslation("TEXT_LEAGUE_LEVELUP_TITLE");
            bodyText = LocalizationManager.GetTranslation("TEXT_LEAGUE_LEVELUP_DESC");

        }
        else
        {
            titleText = LocalizationManager.GetTranslation("TEXT_LEAGUE_LEVELDOWN_TITLE");
            bodyText = LocalizationManager.GetTranslation("TEXT_LEAGUE_LEVELDOWN_DESC");
        }

        m_title.text = titleText;
        m_body.text = bodyText;
        m_leagueName.text = GameDatabase.Instance.StarDatabase.GetLeagueLocalizationName(currentleague);
        m_leagueImage.sprite = GetLeagueSprite(currentleague);
    }

    private Sprite GetLeagueSprite(LeagueData.LeagueName leagueName)
    {
        switch (leagueName)
        {
                case LeagueData.LeagueName.Regular:
                return m_regularSprite;
                case LeagueData.LeagueName.Bronze:
                return m_bronzeSprite;
                case LeagueData.LeagueName.Silver:
                return m_silverSprite;
                case LeagueData.LeagueName.Golden:
                return m_goldSprite;
                case LeagueData.LeagueName.Diamond:
                return m_diamondSprite;
        }
        return null;
    }

    public void OnNextButton()
    {
        GameDatabase.Instance.StarDatabase.LeagueChangePlayerToStar();
        LegacyStarStats.DetectedLeagueChange = false;
        Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
        {
            {
                Parameters.BeforeLeague,
                BeforeLeague.ToString()
            },
            {
                Parameters.AfterLeague,
                AfterLeague.ToString()
            }
        };
        Log.AnEvent(Events.LeagueChange, data);
        Close();
    }
}
