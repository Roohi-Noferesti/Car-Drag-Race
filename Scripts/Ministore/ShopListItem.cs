using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;

public class ShopListItem : MonoBehaviour
{
	public delegate void TapEventHandler(ShopListItem item);

	protected ProductData _product;

	public TextMeshProUGUI TitleText;

    public TextMeshProUGUI SubTitleText;

    public DataDrivenObject DataDrivenNode;

	public Image BGLeft;

	public Image BGRight;

	public Image BGCenter;

    public Image OverlayInUse;

	public GameObject SilverLabel;

    public TextMeshProUGUI BestValue;

    public TextMeshProUGUI GoodValue;

    public TextMeshProUGUI MostPopular;

    public TextMeshProUGUI Recommended;

    public TextMeshProUGUI GreatDeal;

	public GameObject RedLabel;
	public GameObject DiscountLabel;

    public TextMeshProUGUI BonusText;

	public GameObject InfoText;

    public TextMeshProUGUI NumberText;

    public TextMeshProUGUI ThemeText;

	public Image LabelIcon;

    public RuntimeButton Button;

	private BaseRuntimeControl.State _currentState;

    public event TapEventHandler Tap;

	public ProductData Product
	{
		get
		{
			return this._product;
		}
	}

	protected BaseRuntimeControl.State CurrentState
	{
		get
		{
			return this._currentState;
		}
		set
		{
			Color white = Color.white;
			this._currentState = value;
			switch (this._currentState)
			{
			case BaseRuntimeControl.State.Active:
				white = Color.white;
				break;
			case BaseRuntimeControl.State.Pressed:
				white = new Color(0.7f, 0.7f, 0.7f);
				break;
			case BaseRuntimeControl.State.Disabled:
				white = new Color(0.4f, 0.4f, 0.4f);
				break;
			}
			//if (this.OverlayInUse != null)
			//{
   //             //this.OverlayInUse.SetColor(white);
			//}
			this.ApplyTint(white);
		}
	}

	public bool NullElement
	{
		get
		{
			return this._product == null;
		}
	}

	protected void RaiseTapEvent()
	{
		if (this.Tap != null)
		{
			this.Tap(this);
		}
	}

	protected virtual void ApplyTint(Color tint)
	{
		if (this.LabelIcon != null)
		{
            //this.LabelIcon.SetColor(tint);
		}
		if (this.BGLeft != null)
		{
            //this.BGLeft.SetColor(tint);
		}
		if (this.BGRight != null)
		{
            //this.BGRight.SetColor(tint);
		}
		if (this.BGCenter != null)
		{
            //this.BGCenter.SetColor(tint);
		}
	}

	private void Update()
	{
        //if (this.CurrentState == BaseRuntimeControl.State.Active)
        //{
        //    if (this.Button != null && this.Button.Runtime.UIButton.controlState == UIButton.CONTROL_STATE.ACTIVE)
        //    {
        //        this.CurrentState = BaseRuntimeControl.State.Pressed;
        //    }
        //}
        //else if (this.Button != null && this.CurrentState == BaseRuntimeControl.State.Pressed && this.Button.Runtime.UIButton.controlState != UIButton.CONTROL_STATE.ACTIVE)
        //{
        //    this.CurrentState = BaseRuntimeControl.State.Active;
        //}
	}

	protected virtual void Awake()
	{
		if (this.TitleText != null)
		{
			this.TitleText.text = string.Empty;
		}
		this.CurrentState = BaseRuntimeControl.State.Disabled;
        if (this.Button != null)
        {
            this.Button.gameObject.SetActive(true);
            this.Button.AddValueChangedDelegate(OnClick);
        }
		if (this.RedLabel != null)
		{
			this.RedLabel.gameObject.SetActive(false);
		}
		if (this.DiscountLabel != null)
		{
			this.DiscountLabel.gameObject.SetActive(false);
		}
	}

	public void Show()
	{
	}

	public void Hide()
	{
	}

	public void CreateDummy()
	{
        gameObject.SetActive(false);
		//if (this.SilverLabel != null)
		//{
		//	this.SilverLabel.gameObject.SetActive(false);
		//}
		//if (this.MostPopular != null)
		//{
		//	this.MostPopular.gameObject.SetActive(false);
		//}
		//if (this.BestValue != null)
		//{
		//	this.BestValue.gameObject.SetActive(false);
		//}
		//if (this.Recommended != null)
		//{
		//	this.Recommended.gameObject.SetActive(false);
		//}
		//if (this.GreatDeal != null)
		//{
		//	this.GreatDeal.gameObject.SetActive(false);
		//}
	}

	public void Create(ProductData product, ShopScreen.ItemType type, GameObject uiPrefab,Sprite sprite)
	{
		this._product = product;
		if (this.SilverLabel != null)
		{
			this.SilverLabel.gameObject.SetActive(product.GtProduct.Sticker != GTProduct.StickerType.None);
		}
		if (this.MostPopular != null)
		{
			this.MostPopular.gameObject.SetActive(product.GtProduct.Sticker == GTProduct.StickerType.MostPopular);
		}
		if (this.BestValue != null)
		{
			this.BestValue.gameObject.SetActive(product.GtProduct.Sticker == GTProduct.StickerType.BestValue);
		}
		if (this.GoodValue != null)
		{
			this.GoodValue.gameObject.SetActive(product.GtProduct.Sticker == GTProduct.StickerType.GoodValue);
		}
		if (this.Recommended != null)
		{
			this.Recommended.gameObject.SetActive(product.GtProduct.Sticker == GTProduct.StickerType.Recommended);
		}
		if (this.GreatDeal != null)
		{
			this.GreatDeal.gameObject.SetActive(product.GtProduct.Sticker == GTProduct.StickerType.GreatDeal);
		}
		if (type == ShopScreen.ItemType.Cash && product.GtProduct.BonusCash > 0)
		{
			if (this.RedLabel != null)
			{
				this.RedLabel.gameObject.SetActive(true);
			}
			string cashString = CurrencyUtils.GetCashString(product.GtProduct.BonusCash);
			this.BonusText.text = string.Format(LocalizationManager.GetTranslation("TEXT_SHOP_BONUS_LABEL"),cashString);
		}
		if (type == ShopScreen.ItemType.Gold && product.GtProduct.BonusGold > 0)
		{
			if (this.RedLabel != null)
			{
				this.RedLabel.gameObject.SetActive(true);
			}

		    string goldString = product.GtProduct.BonusGold.ToString("#,###0").ToNativeNumber();//CurrencyUtils.GetGoldStringWithIcon(product.GtProduct.BonusGold);
		    this.BonusText.text = string.Format(LocalizationManager.GetTranslation("TEXT_SHOP_BONUS_LABEL"), goldString);
		}
		bool isFirstPack = product.GtProduct._code.ToLower().Contains("pack_1");
		if ((type == ShopScreen.ItemType.Cash || type == ShopScreen.ItemType.Gold) && ProductManager.Instance.discount != 0 && this.DiscountLabel != null && !isFirstPack) {
			this.DiscountLabel.gameObject.SetActive(true);
			if (ProductManager.Instance.discount == 1) {
				this.DiscountLabel.GetComponentInChildren<TextMeshProUGUI>().text = "%15";
			} else if (ProductManager.Instance.discount == 2) {
				this.DiscountLabel.GetComponentInChildren<TextMeshProUGUI>().text = "%30";
			}
		}
		if (this.InfoText)
		{
			this.InfoText.SetActive(false);
		}
		this.SetupProductSprite(type, product.AnimFrameIndex, uiPrefab,sprite);
		if (this.TitleText != null)
		{
            this.TitleText.text = product.AppStoreProduct.Title;
		}
		this.CurrentState = BaseRuntimeControl.State.Active;

        NumberText.text = this.GetButtonText(product, type);

        if (this.Button != null)
        {
            //this.Button.ForceAwake();
            //this.Button.SetText(this.GetButtonText(product, type), true, true);
            //this.Button.Runtime.UIButton.SetSize(this.BGCenter.width + this.BGRight.width + this.BGLeft.width, 0.5f);
            //this.Button.Runtime.UIButton.transform.position = this.Button.gameObject.transform.position;
            //this.Button.Runtime.EnableFeatureCreepFridayHack();
        }
		if (this.SubTitleText != null)
		{
            this.SubTitleText.text = product.AppStoreProduct.LocalisedPrice;
		}
		//float z = Random.Range(-20f, -60f);
		//if (this.RedLabel != null)
		//{
		//	this.RedLabel.transform.localEulerAngles = new Vector3(this.RedLabel.transform.localEulerAngles.x, this.RedLabel.transform.localEulerAngles.y, z);
		//}
		float z2 = Random.Range(20f, -20f);
		if (this.SilverLabel != null)
		{
			this.SilverLabel.transform.localEulerAngles = new Vector3(this.SilverLabel.transform.localEulerAngles.x, this.SilverLabel.transform.localEulerAngles.y, z2);
		}
	}

	protected string GetButtonText(ProductData product, ShopScreen.ItemType type)
	{
		if (type == ShopScreen.ItemType.Cash)
		{
			int zCash = product.GtProduct.Cash + product.GtProduct.BonusCash;
			return CurrencyUtils.GetCashString(zCash);
		}
		if (type == ShopScreen.ItemType.Gold)
		{
			int zGold = product.GtProduct.Gold + product.GtProduct.BonusGold;
			return CurrencyUtils.GetGoldStringWithIcon(zGold);
		}
		if (type != ShopScreen.ItemType.OfferPack)
		{
            return product.AppStoreProduct.Title;
		}
		return LocalizationManager.GetTranslation("TEXT_BUTTON_BUY");
	}

    protected void SetupProductSprite(ShopScreen.ItemType type, int frameIndex, GameObject uiPrefab, Sprite sprite)
	{
        if (type != ShopScreen.ItemType.Cash && type != ShopScreen.ItemType.Gold && type != ShopScreen.ItemType.OfferPack)
        {
            return;
        }
        //GameObject go = this.DataDrivenNode.CreateDataDrivenScreen(uiPrefab);
        //this.OverlayInUse = go.GetComponentInChildren<Image>();
        if (this.OverlayInUse != null)
	        this.OverlayInUse.sprite = sprite;
        //if (this.OverlayInUse == null)
        //{
        //    return;
        //}
        //this.OverlayInUse.PlayAnim(0, frameIndex);
        //this.OverlayInUse.PauseAnim();
        //UVAnimation curAnim = this.OverlayInUse.GetCurAnim();
        //SPRITE_FRAME currentFrame = curAnim.GetCurrentFrame();
        //float num = 0f;
        //float num2 = 0f;
        //if (type == ShopScreen.ItemType.Cash)
        //{
        //    num = 5.12f;
        //    num2 = 2.56f;
        //}
        //else if (type == ShopScreen.ItemType.Gold)
        //{
        //    num = 2.56f;
        //    num2 = 5.12f;
        //}
        //else if (type == ShopScreen.ItemType.OfferPack)
        //{
        //    num = 2.56f;
        //    num2 = 2.58f;
        //}
        //this.OverlayInUse.SetSize(currentFrame.uvs.width * num, currentFrame.uvs.height * num2);
	}

    void OnDestroy()
    {
        if (this.Button != null)
        {
            this.Button.RemoveValueChangedDelegate(OnClick);
        }
    }

	private void OnClick()
	{
		if (this._product == null)
		{
			return;
		}
		this.RaiseTapEvent();
	}
}
