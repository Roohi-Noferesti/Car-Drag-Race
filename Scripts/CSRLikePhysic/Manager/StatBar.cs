using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
	private const float glowPaddingX = 0.03f;

	private const float generalPadding = 0.05f;

    //public PackedSprite background;

    //public PackedSprite mainLeft;

    //public PackedSprite mainCenter;

    //public Image mainDelta;

    //public PackedSprite mainRight;

    public TwoValueSlider sliderBar;

    public Image arrowUp;

    public Image arrowDown;

    //public TextMeshProUGUI title;

    public TextMeshProUGUI currentText;

    public TextMeshProUGUI upgradeText;

	public bool showArrows = true;

	private float _cachedWidth;

	private float _cachedCurrentValue;

	private float _cachedMaxValue;

	private float _cachedUpgradeValue;

	private bool _cachedUpIsGood;

	private void Awake()
	{
        //this.mainLeft.SetColor(CarInfoUI.Instance.barColour);
        //this.mainCenter.SetColor(CarInfoUI.Instance.barColour);
        //this.mainRight.SetColor(CarInfoUI.Instance.barColour);
	}

	public void SetTitle(string zTitle)
	{
        //this.title.text = zTitle;
        //GameObjectHelper.SetLocalX(this.title.gameObject, 0.03f);
	}

	public void Show(bool zShow)
	{
		base.gameObject.SetActive(zShow);
		if (zShow)
		{
			this.Calibrate(this._cachedWidth, this._cachedCurrentValue, this._cachedMaxValue, this._cachedUpgradeValue, this._cachedUpIsGood);
		}
	}

	public void Calibrate(float zWidth, float zCurrentValue, float zMaxValue)
	{
		this.Calibrate(zWidth, zCurrentValue, zMaxValue, 0f, true);
	}

	private void SetTheColoursOfTheBarPieces(float zUpgradeValue, bool zUpIsGood)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
        //this.mainLeft.SetColor(CarInfoUI.Instance.barColour);
        //this.mainCenter.SetColor(CarInfoUI.Instance.barColour);
		if (zUpgradeValue == 0f)
		{
            //this.mainDelta.gameObject.SetActive(false);
            //this.mainRight.SetColor(CarInfoUI.Instance.barColour);
		}
		else
		{
            //this.mainDelta.gameObject.SetActive(true);
			Color color = Color.white;
			if (zUpgradeValue > 0f)
			{
				color = ((!zUpIsGood) ? CarInfoUI.Instance.badColour : CarInfoUI.Instance.goodColour);
			}
			else
			{
				color = ((!zUpIsGood) ? CarInfoUI.Instance.goodColour : CarInfoUI.Instance.badColour);
			}
		    sliderBar.color2 = color;
		    //this.mainDelta.SetColor(color);
		    //this.mainRight.SetColor(color);
		}
	}

	private void SetTheBackground(float zWidth)
	{
        //float num = zWidth - 0.06f;
        //num = GameObjectHelper.To2DP(num);
        //this.background.SetSize(num, this.background.height);
        //GameObjectHelper.SetLocalX(this.background.gameObject, 0.03f);
	}

	private float GetExagerratedWidthOfDelta(float zMaxValue, float zUpgradeValue, float zWidthOfBar)
	{
        //float num = zUpgradeValue / zMaxValue * zWidthOfBar;
        //num = Math.Abs(num);
        //num *= 100f;
        //num = Mathf.Ceil(num);
        //num /= 100f;
        //return To2DP(num);

	    var num = zUpgradeValue/zMaxValue;

	    num = Math.Abs(num);

	    if (num != 0 && num < 0.05)
	    {
	        num = 0.05F;
	    }
	    return num*zMaxValue*Mathf.Sign(zUpgradeValue);
	}

    public static float To2DP(float zVal)
    {
        return (float)Math.Round((double)zVal, 2);
    }

    private float WidthOfCenterSection(float zFilledWidth, float zDelta)
    {
        //float num = zFilledWidth - (this.mainLeft.width - 0.03f);
        //num -= this.mainRight.width - 0.03f;
        //return GameObjectHelper.To2DP(num);
        return 0;
    }

    private void SizeTheBarPieces(float zWidth, float zCurrentValue, float zMaxValue, float zUpgradeValue, bool zUpIsGood)
	{
        //float num = zWidth - 0.06f;
        //float zFilledWidth = GameObjectHelper.To2DP(zCurrentValue / zMaxValue * num);
        //float exagerratedWidthOfDelta = this.GetExagerratedWidthOfDelta(zMaxValue, zUpgradeValue, num);
        //float num2 = this.WidthOfCenterSection(zFilledWidth, exagerratedWidthOfDelta);
        //float zNewX = 0f;
        //float width = this.mainLeft.width;
        //float num3 = num2 + this.mainLeft.width;
        //float num4 = num3;
        //if (zUpgradeValue != 0f)
        //{
        //    if (zUpgradeValue > 0f)
        //    {
        //        num3 += exagerratedWidthOfDelta;
        //    }
        //    else
        //    {
        //        num2 -= exagerratedWidthOfDelta;
        //        num4 -= exagerratedWidthOfDelta;
        //    }
        //}
        //this.mainDelta.SetSize(exagerratedWidthOfDelta, this.mainDelta.height);
        //this.mainCenter.SetSize(num2, this.mainCenter.height);
        //GameObjectHelper.SetLocalX(this.mainLeft.gameObject, zNewX);
        //GameObjectHelper.SetLocalX(this.mainCenter.gameObject, width);
        //GameObjectHelper.SetLocalX(this.mainRight.gameObject, num3);
        //GameObjectHelper.SetLocalX(this.mainDelta.gameObject, num4);
        sliderBar.maxValue = zMaxValue;
        sliderBar.value = zCurrentValue;
        sliderBar.value2 = zCurrentValue + GetExagerratedWidthOfDelta(zMaxValue,zUpgradeValue,0);
	}

	private void SetupText(float zWidth, float zCurrentValue, float zMaxValue, float zUpgradeValue, bool zUpIsGood)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.currentText.text = (zCurrentValue + zUpgradeValue).ToString();
        //GameObjectHelper.SetLocalX(this.currentText.gameObject, zWidth - 0.03f);
		this.arrowUp.gameObject.SetActive(false);
		this.arrowDown.gameObject.SetActive(false);
		if (zUpgradeValue == 0f || !this.showArrows)
		{
			this.upgradeText.gameObject.SetActive(false);
            //GameObjectHelper.SetLocalX(this.currentText.gameObject, zWidth - 0.03f);
		}
		else
		{
            this.upgradeText.text = Math.Abs(zUpgradeValue).ToString();
			this.upgradeText.gameObject.SetActive(true);
            this.SetColour(this.upgradeText, zCurrentValue + zUpgradeValue, zCurrentValue, zUpIsGood);
            //float num = zWidth - this.currentText.TotalWidth - 0.05f - this.arrowUpGood.width - 0.05f;
            //num = GameObjectHelper.To2DP(num);
            //float num2 = zWidth - this.currentText.TotalWidth - 0.05f;
            //num2 = GameObjectHelper.To2DP(num2);
            //GameObjectHelper.SetLocalX(this.upgradeText.gameObject, num);
            //GameObjectHelper.SetLocalX(this.arrowUpGood.gameObject, num2);
            //GameObjectHelper.SetLocalX(this.arrowUpBad.gameObject, num2);
            //GameObjectHelper.SetLocalX(this.arrowDownGood.gameObject, num2);
            //GameObjectHelper.SetLocalX(this.arrowDownBad.gameObject, num2);
		}
	}

	public void Calibrate(float zWidth, float zCurrentValue, float zMaxValue, float zUpgradeValue, bool zUpIsGood)
	{
        //if (!base.gameObject.activeInHierarchy)
        //{
        //    return;
        //}
		if (zWidth == 0f)
		{
			return;
		}
		this._cachedWidth = zWidth;
		this._cachedCurrentValue = zCurrentValue;
		this._cachedMaxValue = zMaxValue;
		this._cachedUpgradeValue = zUpgradeValue;
		this._cachedUpIsGood = zUpIsGood;
		this.SetTheColoursOfTheBarPieces(zUpgradeValue, zUpIsGood);
		this.SetTheBackground(zWidth);
		this.SizeTheBarPieces(zWidth, zCurrentValue, zMaxValue, zUpgradeValue, zUpIsGood);
		this.SetupText(zWidth, zCurrentValue, zMaxValue, zUpgradeValue, zUpIsGood);
	}

	private void SetColour(TextMeshProUGUI spriteTex, float newVal, float oldVal, bool zUpIsGood)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (newVal > oldVal)
		{
			spriteTex.color = ((!zUpIsGood) ? CarInfoUI.Instance.badColour : CarInfoUI.Instance.goodColour);
			if (this.showArrows)
			{
				this.arrowUp.gameObject.SetActive(true);
                this.arrowDown.gameObject.SetActive(false);
                arrowUp.color = ((!zUpIsGood) ? CarInfoUI.Instance.badColour : CarInfoUI.Instance.goodColour);
			}
		}
		else if (newVal < oldVal)
		{
            spriteTex.color = ((!zUpIsGood) ? CarInfoUI.Instance.goodColour : CarInfoUI.Instance.badColour);
			if (this.showArrows)
			{
                this.arrowUp.gameObject.SetActive(false);
				this.arrowDown.gameObject.SetActive(true);
                arrowDown.color = ((!zUpIsGood) ? CarInfoUI.Instance.goodColour : CarInfoUI.Instance.badColour);
			}
		}
		else
		{
            spriteTex.color = Color.white;
		}
	}
}
