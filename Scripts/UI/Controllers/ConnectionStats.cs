using UnityEngine;

public class ConnectionStats : MonoSingleton<ConnectionStats>, IPersistentUI
{
    [SerializeField] private GameObject m_disconnectionIndicator;
    [SerializeField] private LegacyStarStats m_starStats;
    [SerializeField] private StarLeagueStats m_leagueStats;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            m_disconnectionIndicator.SetActive(false);
        }

    }
    public void OnScreenChanged(ScreenID screen)
    {

    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }


    void Update()
    {
        if (!PolledNetworkState.IsNetworkConnected || !LegacyLeaderboardManager.Instance.UserStarFetched
            || !ServerSynchronisedTime.Instance.IsServerTimeMatchClient)
        {
            if (!m_disconnectionIndicator.activeSelf)
            {
                m_disconnectionIndicator.SetActive(true);
            }

            if (m_starStats.IsVisible())
            {
                m_starStats.Show(false);
            }

            if (m_leagueStats.IsVisible())
            {
                m_leagueStats.Show(false);
            }
        }
        else
        {
            if (m_disconnectionIndicator.activeSelf)
            {
                m_disconnectionIndicator.SetActive(false);
            }

            if (!m_starStats.IsVisible())
            {
                m_starStats.Show(true);
            }

            if (!m_leagueStats.IsVisible())
            {
                m_leagueStats.Show(true);
            }
        }
    }

    public void HideIndicator()
    {
        m_disconnectionIndicator.SetActive(false);
    }
}
