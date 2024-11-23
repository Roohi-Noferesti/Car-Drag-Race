using System;
using I2.Loc;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public abstract class ShopScreenBase : ZHUDScreen 
{
    [SerializeField] protected BasicGridView m_gridView;
    [SerializeField] protected CostContainer m_costContainer;
    [SerializeField] protected bool m_dataDriven = false;
    public event Action<string> ItemChanged;

    protected string m_itemBeenPurchased;

    protected CostType m_currentBuyMode;
    protected TuningScreen.BlueButtonPressMode _blueButtonMode;
    public UIBasicItem[] Items
    {
        get { return m_gridView.Items; }
    }

    protected override void Awake()
    {
        base.Awake();
        if (!m_dataDriven)
        {
            m_gridView.Init();
        }
    }

    public virtual void ToggleItem()
    {
        RefreshUIElement_BlueButtonMode();
        RefreshUIElement_CostContainer();
        OnItemChanged(m_gridView.SelectedItemID);
    }

    protected virtual void RefreshUIElement_BlueButtonMode()
    {
        this._blueButtonMode = TuningScreen.BlueButtonPressMode.None;
        if (this.IsSelectedItemOwned)
        {
            if (!this.IsSelectedItemFitted)
            {
                this._blueButtonMode = TuningScreen.BlueButtonPressMode.FitPart;
            }
        }
        else if (this.IsSelectedItemBeingDelivered)
        {
            this._blueButtonMode = TuningScreen.BlueButtonPressMode.DeliverPart;
        }
        else if (this.CanSelectedItemBePurchased)
        {
            this._blueButtonMode = TuningScreen.BlueButtonPressMode.BuyPart;
        }
    }


    protected virtual void RefreshUIElement_CostContainer()
    {
        if (this.IsSelectedItemOwned)
        {
            if (this.IsSelectedItemFitted)
            {
                this.m_costContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_FITTED"), BaseRuntimeControl.State.Disabled, OnBlueButton);
            }
            else
            {
                this.m_costContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_FIT"), BaseRuntimeControl.State.Active, OnBlueButton);
            }
        }
        else
        {
            if (this.CanSelectedItemBePurchased)
            {
                string title2 = (!TuningScreen.ExpressMode) ? LocalizationManager.GetTranslation("TEXT_BUTTON_UPGRADE").ToUpper() : CurrencyUtils.GetColouredGoldString(LocalizationManager.GetTranslation("TEXT_TRACKSIDE"));
                this.m_costContainer.SetupForCost(this.GetCashPrice(), this.GetGoldPrice(), title2, OnBlueButtonCashOption, OnBlueButtonGoldOption, OnBlueButtonGoldOption);
            }
            else
            {
                this.m_costContainer.SetupForBlueButton(string.Empty, LocalizationManager.GetTranslation("TEXT_BUTTON_LOCKED"), BaseRuntimeControl.State.Disabled, OnBlueButton);
            }
        }
    }

    protected void OnBlueButton()
    {
        switch (this._blueButtonMode)
        {
            case TuningScreen.BlueButtonPressMode.None:
                return;
            case TuningScreen.BlueButtonPressMode.FitPart:
                this.OnBlueFitButton();
                return;
            case TuningScreen.BlueButtonPressMode.BuyPart:
                this.OnBlueBuyButton();
                return;
            case TuningScreen.BlueButtonPressMode.DeliverPart:
                this.OnBlueDeliverButton();
                return;
            case TuningScreen.BlueButtonPressMode.UnlockEvoPart:
                this.OnBlueEvoUnlockButton();
                return;
            default:
                return;
        }
    }



    protected abstract void OnBlueEvoUnlockButton();

    protected abstract void OnBlueDeliverButton();

    protected abstract void OnBlueBuyButton();

    protected abstract void OnBlueFitButton();

    protected virtual int GetGoldPrice()
    {
        //return GameDatabase.Instance.ServerItemDatabase.GetGoldPrice(m_gridView.SelectedItemID);
        return 0;
    }
    
    protected virtual string GetBonusID()
    {
        return null;
    }

    protected virtual int GetCashPrice()
    {
        //return GameDatabase.Instance.ServerItemDatabase.GetCashPrice(m_gridView.SelectedItemID);
        return 0;
    }

    protected void OnBlueButtonGoldOption()
    {
        m_currentBuyMode = CostType.GOLD;
        OnBlueButton();
    }

    protected void OnBlueButtonCashOption()
    {
        m_currentBuyMode = CostType.CASH;
        OnBlueButton();
    }

    public abstract bool CanSelectedItemBePurchased { get; }

    protected abstract bool IsSelectedItemFitted { get; }

    protected abstract bool IsSelectedItemOwned { get; }

    public abstract bool IsSelectedItemBeingDelivered { get; }

    protected abstract string GetItemBeenPurchased();

    protected virtual void OnItemChanged(string obj)
    {
        var handler = ItemChanged;
        if (handler != null) handler(obj);
    }

    public bool IsFree()
    {
        return GetCashPrice() == 0 && GetGoldPrice() == 0;
    }
}
