using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using I2.Loc;
using UnityEngine;

public class UpgradeScrollerController : CarouselScrollerController
{
    [SerializeField] private Sprite[] m_upgradesIcons;
    private CarUpgradeSetup m_currentUpgradeSetup;
    [SerializeField] private float m_cellSize = 376;

    private Dictionary<eUpgradeType, UpgradeButtonCellView> m_cellView =
        new Dictionary<eUpgradeType, UpgradeButtonCellView>();
    public eUpgradeType CurrentUpgrade
    {
        get
        {
            if (SelectedIndex >= 0 && SelectedIndex < CarUpgrades.ValidUpgrades.Count)
                return CarUpgrades.ValidUpgrades[SelectedIndex];
            else
            {
                return CarUpgrades.ValidUpgrades[0];
            }
        }
        set
        {
            SelectedIndex = CarUpgrades.ValidUpgrades.IndexOf(value);
        }
    }

    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        return CarUpgrades.ValidUpgrades.Count;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return m_cellSize;
    }

    public override void UpdateCellData(EnhancedScrollerCellView cellView, int dataIndex, int cellIndex)
    {
        UpgradeButtonCellView upgradeButtonView = (UpgradeButtonCellView) cellView;
        var upgrade = CarUpgrades.ValidUpgrades[dataIndex];
        var title_key = CarUpgrades.UpgradeTextIDsForLocalisation[upgrade];
        upgradeButtonView.Icon = m_upgradesIcons[dataIndex];
        upgradeButtonView.Title = LocalizationManager.GetTranslation(title_key);
        upgradeButtonView.UpgradeType = upgrade;
        //upgradeButtonView.IsSelected = CurrentUpgrade == upgrade;
        if (m_currentUpgradeSetup != null)
        {
            upgradeButtonView.UpgradeLevel = m_currentUpgradeSetup.UpgradeStatus[upgrade].levelFitted;
        }

        if (!m_cellView.ContainsKey(upgrade))
        {
            m_cellView.Add(upgrade, upgradeButtonView);
        }
        m_cellView[upgrade] = upgradeButtonView;
        m_cellView[upgrade].name = upgrade.ToString();
    }

    public void SetCarUpgradeSetup(CarUpgradeSetup upgradeSetup)
    {
        m_currentUpgradeSetup = upgradeSetup;
    }

    public void SetUpgradeLevel(eUpgradeType upgradeType, int level)
    {
        m_cellView[upgradeType].UpgradeLevel = level;
    }

    public void PlayFitAnimation(eUpgradeType upgradeType)
    {
        m_cellView[upgradeType].PlayFitAnimation();
    }
}