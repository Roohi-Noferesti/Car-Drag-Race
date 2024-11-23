using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;

public class UILeaderboardRecord : SelectableScrollerCellView
{
    [SerializeField] private TextMeshProUGUI m_rowText;
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_starText;
    //[SerializeField] private TextMeshProUGUI m_levelText;
    [SerializeField] private TextMeshProUGUI m_rewardText;
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private TextMeshProUGUI m_carNameText;
    [SerializeField] private RawImage m_image;
    private LeaderboardRecord m_recordData;

    public void SetValues(LeaderboardRecord record,string rewardText = null)
    {
        Row = HasRecord ? record.Rank : -1;
        Name = record.UserDisplayName;//LocalizationManager.FixRTL_IfNeeded(record.UserDisplayName);
        if (!string.IsNullOrEmpty(record.CarKey))
        {
            var car = CarDatabase.Instance.GetCar(record.CarKey);
            CarName = LocalizationManager.GetTranslation(car.ShortName);
            m_carNameText.gameObject.SetActive(true);
        }
        else
        {
            if (m_carNameText!=null)
                m_carNameText.gameObject.SetActive(false);
        }

        if (record.LongScoreValue > 0)
        {
            m_starText.transform.parent.gameObject.SetActive(true);
            Star = record.LongScoreValue;
        }
        else
        {
            if (m_starText!=null)
                m_starText.transform.parent.gameObject.SetActive(false);
        }


        if (record.FloatScoreValue > 0)
        {
            m_scoreText.gameObject.SetActive(true);
            Score = record.FloatScoreValue;
        }
        else
        {
            if (m_scoreText!=null)
                m_scoreText.gameObject.SetActive(false);
        }

        //Level = GameDatabase.Instance.XPEvents.CurrentLevelForXP(record.Level);
        Reward = rewardText;

        //ImageUtility.LoadImage(record.ImageUrl, UserImageSize.Size_50, (res, tex) =>
        //{
        //    if (res)
        //    {
        //        m_image.texture = tex;
        //    }
        //    else
        //    {
        //        m_image.texture = m_defaultPlayerTexture;
        //    }
        //});
        var avatarID = string.IsNullOrEmpty(record.AvatarID) ? "avatar_1" : record.AvatarID;
        ResourceManager.LoadAssetAsync<Texture2D>(avatarID,ServerItemBase.AssetType.avatar,true, tex =>
        {
            m_image.texture = tex;
        });
        m_recordData = record;
    }

    public bool HasRecord
    {
        get { return true; }
        //m_recordData != null && (m_recordData.LongScoreValue > 0 || m_recordData.FloatScoreValue > 0); }
    }

    public long Row
    {
        set { m_rowText.text = value == -1 ? "-" : String.Format("{0}", value); }
    }

    public string Name
    {
        set { m_NameText.text = value; }
    }

    public string CarName
    {
        set { m_carNameText.text = value; }
    }

    public long Star
    {
        set { m_starText.text = String.Format("{0}", value); }
    }

    public float Score
    {
        set
        {
            m_scoreText.text = String.Format("{0:#0.000}", value);
        }
    }

    //public int Level
    //{
    //    set
    //    {
    //        if (m_levelText != null)
    //            m_levelText.text = String.Format("{0}", value);
    //    }
    //}

    public string Reward
    {
        set
        {
            if (m_rewardText != null)
                m_rewardText.text = value;
        }
    }

    public void ShowProfile()
    {
        LeaderboardScreen.ShowProfile(m_recordData.UserID);
    }

}
