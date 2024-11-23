using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

[RequireComponent(typeof(EnhancedScroller))]
public class LeaderboardScrollerController : MonoBehaviour,IEnhancedScrollerDelegate 
{
    [SerializeField]
    private UILeaderboardRecord m_itemTemplateOdd;
    [SerializeField]
    private UILeaderboardRecord m_itemTemplateEven;
    [SerializeField]
    private UILeaderboardRecord m_itemTemplateUser;

    [SerializeField]
    private UILeaderboardRecord m_weeklyItemTemplateOdd;
    [SerializeField]
    private UILeaderboardRecord m_weeklyItemTemplateEven;
    [SerializeField]
    private UILeaderboardRecord m_weeklyItemTemplateplayer;

    [SerializeField]
    private UILeaderboardRecord m_separatorCellView;

    [SerializeField]
    private UILeaderboardRecord m_separatorCellViewLeague;

    [SerializeField] private float m_playerSize = 103.4F;
    [SerializeField] private float m_normalSize = 69.2F;
    [SerializeField] private float m_seperatorSize = 69.2F;

    [SerializeField] private float m_playerWeeklySize = 103.4F;
    [SerializeField] private float m_normalWeeklySize = 69.2F;

    private LeaderboardRecord[] m_records;
    private EnhancedScroller m_scroller;
    private int m_topLeaderboardCount;
    private LegacyLeaderboardManager.LeaderboardType m_leaderboardType;
    private bool m_weekly;

    void Awake()
    {
        m_scroller = GetComponent<EnhancedScroller>();
        m_scroller.Delegate = this;
    }

    public void RealodData(LeaderboardRecord[] leaderboardRecords, LegacyLeaderboardManager.LeaderboardType type
        ,int topCount)
    {
        m_records = leaderboardRecords;
        m_topLeaderboardCount = topCount;
        m_leaderboardType = type;
        m_weekly = m_leaderboardType.IsWeekly();
        m_scroller.ReloadData();
    }


    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        if (m_records != null)
            return m_records.Length;
        return 0;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        if (dataIndex == m_topLeaderboardCount)
        {
            return m_seperatorSize;
        }
        else if (m_records[dataIndex].UserID == UserManager.Instance.currentAccount.UserID)
        {
            return m_weekly ? m_playerWeeklySize : m_playerSize;
        }
        else
        {
            return m_weekly ? m_normalWeeklySize : m_normalSize;
        }
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {

        UILeaderboardRecord cellView;
        if (dataIndex == m_topLeaderboardCount)
        {
            cellView = (UILeaderboardRecord)m_scroller.GetCellView(
                m_weekly ? m_separatorCellViewLeague : m_separatorCellView);
        }
        else if (m_records[dataIndex].UserID == UserManager.Instance.currentAccount.UserID)
        {
            cellView =
                (UILeaderboardRecord) m_scroller.GetCellView(m_weekly ? m_weeklyItemTemplateplayer : m_itemTemplateUser);
        }
        else
        {
            cellView =
                (UILeaderboardRecord)
                    m_scroller.GetCellView(cellIndex%2 == 0
                        ? (m_weekly ? m_weeklyItemTemplateEven : m_itemTemplateEven)
                        : (m_weekly ? m_weeklyItemTemplateOdd : m_itemTemplateOdd));
        }
        cellView.SelectedCallback = SelectedCallback;

        var rewardText = string.Empty;
        if (m_weekly)
        {
            //league
          //  var reward = GameDatabase.Instance.CurrenciesConfiguration.WeeklyRewardData.GetRewardByRank((int) m_records[dataIndex].Rank);
          var reward = GameDatabase.Instance.CurrenciesConfiguration.WeeklyRewardData.GetRewardByRank((int)m_records[dataIndex].Rank);//.FirstOrDefault(x => x.LeagueName == UserManager.Instance.currentAccount.CurrentLeagueProp).GetRewardByRank((int)m_records[dataIndex].Rank);
            if (reward != null)
            {
                rewardText = reward.GetRewardText();
                GTDebug.Log(GTLogChannel.Screens,"league===>>>reward text is::::>>>>"+rewardText);
            }
        }
        cellView.SetValues(m_records[dataIndex], rewardText);
        return cellView;
    }

    private void SelectedCallback(EnhancedScrollerCellView cellView)
    {
    }

    public float NormalizedPosition
    {
        get { return m_scroller.ScrollRect.verticalNormalizedPosition; }
        set
        {
            m_scroller.ScrollRect.verticalNormalizedPosition = value;
        }
    }

    public int SelectedPosition
    {
        set
        {
            //var cellCount = m_records != null ? m_records.Length : 1;
            //NormalizedPosition = (1 - (value + 2f)/cellCount);
            m_scroller.JumpToDataIndex(value, 0, 0.5F, false, EnhancedScroller.TweenType.easeInOutCirc
                , 0.5F);
        }
    }

    public LeaderboardRecord[] GetRecords()
    {
        return m_records;
    }
}