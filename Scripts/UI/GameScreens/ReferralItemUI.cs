using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;

public class ReferralItemUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_referralID;
    [SerializeField]
    private TextMeshProUGUI m_referralName;
    [SerializeField]
    private RawImage m_referralAvatar;
    [SerializeField]
    private RuntimeTextButton m_collectRewardButton;
    [SerializeField]
    private TextMeshProUGUI m_rewardText;
    public ReferralItem ReferralItem { get; private set; }

    public void Setup(ReferralItem referralItem,int goldReward,UnityAction onCollectButton)
    {
        var gold = CurrencyUtils.GetGoldStringWithIcon(goldReward);
        m_rewardText.text = gold;
        ReferralItem = referralItem;
        UpdateUI(onCollectButton);

        ResourceManager.LoadAssetAsync<Texture2D>(referralItem.AssociatedAvatarID, ServerItemBase.AssetType.avatar, true, tex =>
        {
            m_referralAvatar.texture = tex;
        });
    }

    public void UpdateUI(UnityAction onCollectButton)
    {
        m_referralID.text = ReferralItem.AssociatedUserID.ToString();
        m_referralName.text = ReferralItem.AssociatedUserName;

        m_collectRewardButton.AddValueChangedDelegate(onCollectButton);
        m_collectRewardButton.CurrentState = BaseRuntimeControl.State.Active;
        return;

        if (!ReferralItem.HasCollectedReward)
        {
            m_collectRewardButton.AddValueChangedDelegate(onCollectButton);
            m_collectRewardButton.CurrentState = BaseRuntimeControl.State.Active;
        }
        else
        {
            m_collectRewardButton.SetText("TEXT_REWARD_COLLECTED", false, false);
            m_collectRewardButton.CurrentState = BaseRuntimeControl.State.Disabled;
        }
    }
}
