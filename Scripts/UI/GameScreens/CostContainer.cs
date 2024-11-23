using System;
using System.Linq;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;

public class CostContainer : MonoBehaviour
{
	public GameObject CostContainerBlueButtonPrefab;

	public GameObject CostContainerCashbackPrefab;

	public GameObject CostContainerFreeUpgradesPrefab;

	public GameObject CostContainerGoldAndCashPrefab;

	public GameObject CostContainerGoldOrCashPrefab;
	
	public GameObject CostContainerBonusPrefab;

	public GameObject CostContainerTitleOnlyPrefab;

	public GameObject CostContainerMultiplayerPrefab;

	public GameObject CostContainerDirectIAPPrefab;

	public GameObject CostContainerOfferPackIAPPrefab;

	public GameObject CostContainerEvolutionPrefab;

    public GameObject CostContainerLockPrefab;

    public GameObject DiscountParent;

    public Sprite SilverCoin;

	private GameObject childObjectPrefab;

	private GameObject childObject;

	private bool buyButtonsEnabled = true;

	public string Title
	{
		get;
		private set;
	}

    void Awake()
    {
        HideAllExcept();
    }

    private void HideAllExcept(params GameObject[] objects)
    {
        HideIfEligible(CostContainerBlueButtonPrefab, objects);
        HideIfEligible(CostContainerGoldAndCashPrefab, objects);
        // HideIfEligible(CostContainerBonusPrefab, objects);
        HideIfEligible(CostContainerTitleOnlyPrefab, objects);
        HideIfEligible(CostContainerLockPrefab, objects);
        HideIfEligible(CostContainerFreeUpgradesPrefab, objects);
        HideIfEligible(CostContainerCashbackPrefab, objects);
        HideIfEligible(CostContainerMultiplayerPrefab, objects);
        HideIfEligible(CostContainerOfferPackIAPPrefab, objects);

        foreach (var o in objects)
        {
            if (!o.activeSelf)
                o.SetActive(true);
        }
    }

    private void HideIfEligible(GameObject obj, params GameObject[] objects)
    {
        if (obj!=null && obj.activeSelf && objects != null &&
            !objects.Contains(obj))
        {
            obj.SetActive(false);
        }
    }

    private T SetupChildObject<T>(GameObject prefab) where T : Component
    {
        HideAllExcept(prefab);

		if (this.childObjectPrefab == prefab)
		{
			return this.childObject.GetComponent<T>();
		}
		if (this.childObject != null)
		{
			this.childObject.SetActive(false);
            //UnityEngine.Object.Destroy(this.childObject);
		}
        //GameObject gameObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
        //gameObject.transform.parent = base.transform;
        //gameObject.transform.localPosition = Vector3.zero;
        prefab.SetActive(true);
		this.buyButtonsEnabled = true;
        this.childObject = prefab;
		this.childObjectPrefab = prefab;
        return prefab.GetComponent<T>();
	}

    public void SetupForDiscountDeal(int goldCost, int discount, UnityAction action)
	{
		CostContainerGoldOrCash costContainerGoldOrCash = this.SetupChildObject<CostContainerGoldOrCash>(this.CostContainerGoldOrCashPrefab);
		costContainerGoldOrCash.Setup(this.Title, discount);
		costContainerGoldOrCash.SetupButtonForGold(goldCost, action);
		this.Title = costContainerGoldOrCash.TitleText.text;
	}

	public void SetupForCashbackDeal(int goldCost, int cashback, UnityAction action)
	{
		CostContainerCashback costContainerCashback = this.SetupChildObject<CostContainerCashback>(this.CostContainerCashbackPrefab);
		costContainerCashback.Setup(goldCost, cashback, action);
        this.Title = costContainerCashback.TitleText.text;
	}

	public void SetupForFreeUpgradeDeal(int goldCost, int upgradeLevel, UnityAction action, int cashSaving)
	{
		CostContainerFreeUpgrades costContainerFreeUpgrades = this.SetupChildObject<CostContainerFreeUpgrades>(this.CostContainerFreeUpgradesPrefab);
		costContainerFreeUpgrades.Setup(upgradeLevel, goldCost, action, cashSaving);
        this.Title = costContainerFreeUpgrades.TitleText.text;
	}

    public void SetupForCost(int Cash, int Gold, string title, UnityAction method_cash, UnityAction method_gold, UnityAction method_key)
	{
		//CostContainerBonus costContainerBonus = CostContainerBonusPrefab.GetComponent<CostContainerBonus>();
        SetDiscountActive(false);
		if (Cash > 0 && Gold > 0)
		{
			CostContainerGoldAndCash costContainerGoldAndCash = this.SetupChildObject<CostContainerGoldAndCash>(this.CostContainerGoldAndCashPrefab);
			//costContainerBonus.SetState(true);
			costContainerGoldAndCash.Setup(title, Gold, Cash, method_cash, method_gold);
		}
		else if (Cash == 0 && Gold == 0)
		{
			CostContainerBlueButton costContainerBlueButton = this.SetupChildObject<CostContainerBlueButton>(this.CostContainerBlueButtonPrefab);
			//costContainerBonus.SetState(false);
			costContainerBlueButton.SetupFree(this.Title, method_cash);
		}
		else
		{
            CostContainerGoldOrCash costContainerGoldOrCash = this.SetupChildObject<CostContainerGoldOrCash>(this.CostContainerGoldOrCashPrefab);
            costContainerGoldOrCash.Setup(title, 0);
			if (Cash > 0)
			{
				costContainerGoldOrCash.SetupButtonForCash(Cash, method_cash);
			}
			else if(Gold>0)
			{
				costContainerGoldOrCash.SetupButtonForGold(Gold, method_gold);
				//costContainerBonus.SetState(true);
			}
		}
		this.Title = title;
	}
    
    public void SetupForBonus(eCarTier tier, int  LiveryGoldCost, string itemId)
    {
	    CostContainerBonus costContainerBonus = CostContainerBonusPrefab.GetComponent<CostContainerBonus>();
	    CheckToShowBonus(LiveryGoldCost);
	    costContainerBonus.Setup(PaintScreen.CalculateLiveryRaceBonus(tier, LiveryGoldCost, itemId));
    }

    public void CheckToShowBonus(int LiveryGoldCost)
    {
	    CostContainerBonus costContainerBonus = CostContainerBonusPrefab.GetComponent<CostContainerBonus>();
	    if (!RemoteConfigABTest.CheckRemoteConfigValue())
	    {
		    costContainerBonus.SetState(false);
		    return;
	    }
	    if (LiveryGoldCost > 0)
	    {
		    costContainerBonus.SetState(true);
	    }
	    else
	    {
		    costContainerBonus.SetState(false);
	    }
    }

	public void SetupForTitleOnly(string title)
	{
        SetDiscountActive(false);
		CostContainerTitleOnly costContainerTitleOnly = this.SetupChildObject<CostContainerTitleOnly>(this.CostContainerTitleOnlyPrefab);
		costContainerTitleOnly.SetText(title);
		this.Title = title;
	}

    public void SetupForLock(string title)
    {
        SetDiscountActive(false);
        CostContainerTitleOnly costContainerTitleOnly = this.SetupChildObject<CostContainerTitleOnly>(this.CostContainerLockPrefab);
        costContainerTitleOnly.SetText(title);
        this.Title = title;
    }

    public void SetupForRaceYourFriends(string title, string buttonText, BaseRuntimeControl.State state, UnityAction method)
    {
        SetDiscountActive(false);
		CostContainerInfoButton costContainerInfoButton = this.SetupChildObject<CostContainerInfoButton>(this.CostContainerMultiplayerPrefab);
		costContainerInfoButton.Setup(title, CostContainerInfoButton.Icons.RaceYourFriends, buttonText, state, method, true);
		this.Title = title;
	}

    public void SetupForInstallClassics(string title, string buttonText, BaseRuntimeControl.State state, UnityAction method)
	{
        SetDiscountActive(false);
		CostContainerInfoButton costContainerInfoButton = this.SetupChildObject<CostContainerInfoButton>(this.CostContainerMultiplayerPrefab);
		costContainerInfoButton.Setup(title, CostContainerInfoButton.Icons.CSRClassics, buttonText, state, method, true);
		this.Title = title;
	}

    public void SetupForWorldTour(string title, string buttonText, BaseRuntimeControl.State state, UnityAction method)
	{
        SetDiscountActive(false);
		CostContainerInfoButton costContainerInfoButton = this.SetupChildObject<CostContainerInfoButton>(this.CostContainerMultiplayerPrefab);
		costContainerInfoButton.Setup(title, CostContainerInfoButton.Icons.WorldTour, buttonText, state, method, true);
		this.Title = title;
	}

    public void SetupForInternational(string title, string buttonText, BaseRuntimeControl.State state, UnityAction method)
	{
        SetDiscountActive(false);
		CostContainerInfoButton costContainerInfoButton = this.SetupChildObject<CostContainerInfoButton>(this.CostContainerMultiplayerPrefab);
		costContainerInfoButton.Setup(title, CostContainerInfoButton.Icons.International, buttonText, state, method, true);
		this.Title = title;
	}

	public void SetupForMultiplayer(string title, string buttonText, BaseRuntimeControl.State state, UnityAction method)
	{
        SetDiscountActive(false);
		CostContainerInfoButton costContainerInfoButton = this.SetupChildObject<CostContainerInfoButton>(this.CostContainerMultiplayerPrefab);
		costContainerInfoButton.Setup(title, CostContainerInfoButton.Icons.Multiplayer, buttonText, state, method, true);
		this.Title = title;
	}

	public void SetupForBlueButton(string title, string buttonText, BaseRuntimeControl.State state, UnityAction method)
	{
        SetDiscountActive(false);
		CostContainerBlueButton costContainerBlueButton = this.SetupChildObject<CostContainerBlueButton>(this.CostContainerBlueButtonPrefab);
		costContainerBlueButton.Setup(title, buttonText, state, method, true);
		this.Title = title;
	}

    public void SetupForAutoFit()//string title, string buttonText, BaseRuntimeControl.State state, UnityAction method)
    {
        CostContainerBlueButton costContainerBlueButton = this.SetupChildObject<CostContainerBlueButton>(this.CostContainerBlueButtonPrefab);
        //costContainerBlueButton.Hide();
        var buttonText = LocalizationManager.GetTranslation("TEXT_COST_BOX_HEADING_OWNED");
        costContainerBlueButton.Setup(String.Empty, buttonText, BaseRuntimeControl.State.Disabled, null, true);
        //this.Title = title;
    }

    public void SetupForEvolutionButton(string title, string buttonText, BaseRuntimeControl.State state, UnityAction method)
	{
        SetDiscountActive(false);
		CostContainerBlueButton costContainerBlueButton = this.SetupChildObject<CostContainerBlueButton>(this.CostContainerEvolutionPrefab);
		costContainerBlueButton.Setup(title, buttonText, state, method, true);
		this.Title = title;
	}

	public void SetButtonsEnabled(bool enabled)
	{
		if (this.buyButtonsEnabled == enabled || this.childObject == null)
		{
			return;
		}
        //DummyTextButton[] componentsInChildren = this.childObject.GetComponentsInChildren<DummyTextButton>();
        //DummyTextButton[] array = componentsInChildren;
        //for (int i = 0; i < array.Length; i++)
        //{
        //    DummyTextButton dummyTextButton = array[i];
        //    dummyTextButton.Runtime.CurrentState = ((!enabled) ? BaseRuntimeControl.State.Disabled : BaseRuntimeControl.State.Active);
        //}
		this.buyButtonsEnabled = enabled;
	}

	public void SetupForDirectIAP(string title, string bodyText, string buttonText, bool isCurrencyAvailable, MonoBehaviour target, UnityAction method)
	{
        SetDiscountActive(false);
		CostContainerDirectIAP costContainerDirectIAP = this.SetupChildObject<CostContainerDirectIAP>(this.CostContainerDirectIAPPrefab);
		costContainerDirectIAP.Setup(title, bodyText, buttonText, isCurrencyAvailable, target, method);
	}

	public void SetupForOfferPackPurchase(string title, string carCountText, string bodyText, string buttonText, bool isCurrencyAvailable, UnityAction method)
	{
        SetDiscountActive(false);
		CostContainerOfferPackIAP costContainerOfferPackIAP = this.SetupChildObject<CostContainerOfferPackIAP>(this.CostContainerOfferPackIAPPrefab);
		costContainerOfferPackIAP.Setup(title, carCountText, bodyText, buttonText, isCurrencyAvailable, method);
	}

	public void PressDefaultButton()
	{
		if (this.childObject == null)
		{
			return;
		}
        //DummyTextButton componentInChildren = this.childObject.GetComponentInChildren<DummyTextButton>();
        //if (componentInChildren == null)
        //{
        //    return;
        //}
        //componentInChildren.Runtime.OnPress();
	}

    private void SetDiscountActive(bool value)
    {
        if (DiscountParent != null)
            DiscountParent.SetActive(value);
    }
}
