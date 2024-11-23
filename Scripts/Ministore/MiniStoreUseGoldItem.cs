using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniStoreUseGoldItem : ShopListItem
{
	public TextMeshProUGUI GoldTitle;

	public RuntimeButton GoldButton;

	public TextMeshProUGUI GoldCost;

	protected override void Awake()
	{
		base.Awake();
		if (this.GoldButton != null)
		{
			this.GoldButton.gameObject.SetActive(true);
		}
	}

	public void Create(GameObject UIPrefab, int GoldValue)
	{
		base.SetupProductSprite(ShopScreen.ItemType.Gold, 0, UIPrefab,null);
		if (this.GoldTitle != null)
		{
			this.GoldTitle.text = LocalizationManager.GetTranslation("TEXT_MINI_STORE_USE_YOUR_GOLD");
		}
        //if (this.GoldButton != null && this.WindowPane != null)
        //{
        //    this.GoldButton.ForceAwake();
        //    this.GoldButton.Runtime.UIButton.SetSize(0.875f * this.WindowPane.Width, 0.95f * this.WindowPane.Height);
        //    float x = this.WindowPane.Width * 0.5f;
        //    float num = this.WindowPane.Height * 0.5f;
        //    this.GoldButton.Runtime.UIButton.transform.position = this.WindowPane.gameObject.transform.position + new Vector3(x, -num, 0f);
        //    this.GoldButton.Runtime.EnableFeatureCreepFridayHack();
        //}
		if (this.GoldCost != null)
		{
			this.GoldCost.text = CurrencyUtils.GetGoldStringWithIcon(GoldValue);
		}
		base.CurrentState = BaseRuntimeControl.State.Active;
	}

	public void OnClick()
	{
		base.RaiseTapEvent();
	}
}
