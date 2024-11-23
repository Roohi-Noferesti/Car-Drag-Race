using System;
using System.Globalization;
using I2.Loc;
using UnityEngine;

public static class CurrencyUtils
{
	public const char CSR_ASCII_CashSymbol = '}';

	public const char CSR_ASCII_GoldSymbol = '{';

	public const char CSR_ASCII_RankSymbol = '<';

	public const char CSR_ASCII_IncrementSymbol = '↑';

	public const char CSR_ASCII_DecrementSymbol = '>';

	public const float TimePerDigitOfValueAnimation = 0.3f;

    public static string CashColour = "<color=#FFE20>{0}</color>";

    public static string GoldColour = "<color=#000000FF>{0}</color>";

    public static string GetColoredCashString(string value)
    {
        return string.Format(CashColour, value);
    }

    public static string GetColoredGoldString(string value)
    {
        return string.Format(GoldColour, value);
    }

	public static string GetColouredCurrentGoldString()
	{
		int currentGold = PlayerProfileManager.Instance.ActiveProfile.GetCurrentGold();
        return currentGold.ToString();
		//return string.Format("{0}{1}{2:#,###0}{3}", new object[]
		//{
		//	GoldColour,
		//	'{',
		//	currentGold,
		//	Color.white
		//});
	}

	public static string GetColouredCurrentCashString()
	{
		int currentCash = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash();
		return string.Format("{0}{1}{2:#,###0}{3}", new object[]
		{
			CashColour,
			'}',
			currentCash,
			Color.white
		});
	}

	public static string GetGoldString(int zGold)
	{
        //return string.Format("{0}{1:#,###0}", '{', zGold);
        var goldString = string.Format("{0:#,###0}", zGold).ToNativeNumber();
        //var localizedString = LocalizationManager.FixRTL_IfNeeded(goldString);
        return goldString;
	}
	
	public static string GetFreeUpgradeString(int amount)
	{
		return amount.ToString() + " " + ((amount==1)?LocalizationManager.GetTranslation("TEXT_PRIZE_FREEUPGRADE"):LocalizationManager.GetTranslation("TEXT_PRIZE_FREEUPGRADES"));
	}
	
	public static string GetFuelPipeString(int amount)
	{
		return string.Format(LocalizationManager.GetTranslation("TEXT_FILL_FUEL_PIPS"), amount);
	}

    public static string GetKeyString(int zKey)
    {
        //return string.Format("{0}{1:#,###0}", '{', zGold);
        var keyString = string.Format("{0:#,###0}", zKey).ToNativeNumber();
        //var localizedString = LocalizationManager.FixRTL_IfNeeded(goldString);
        return keyString;
    }

    public static string GetGoldStringWithIcon(int zGold)
    {
        return string.Format("<sprite=1> {0}", zGold.ToString("#,###0").ToNativeNumber());
    }

    public static string GetColouredGoldString(string str)
	{
		return string.Format("{0}{1}{2}", GoldColour, str, Color.white);
	}

	public static string GetRankPointsString(int rp, bool showSymbol = true, bool symbolOnRight = true)
	{
		if (!showSymbol)
		{
			return string.Format("{0:#,###0}", rp);
		}
		if (symbolOnRight)
		{
			return string.Format("{1:#,###0}{0}", '<', rp);
		}
		return string.Format("{0}{1:#,###0}", '<', rp);
	}

	public static string GetRankPointDeltaString(int rp, bool showSymbol = true)
	{
		char c;
		if (rp > 0)
		{
			c = '↑';
		}
		else if (rp < 0)
		{
			c = '>';
		}
		else
		{
			c = ' ';
		}
		if (rp < 0)
		{
			rp = -rp;
		}
		if (showSymbol)
		{
			return string.Format("{2}{1:#,###0}{0}", '<', rp, c);
		}
		return string.Format("{1}{0:#,###0}", rp, c);
	}

    public static string GetUnformattedCashString(int zCash)
    {
        //return string.Format("{0}{1:#,###0}", '}', zCash);
        return string.Format("{0:#,###0}", zCash);
    }

    public static string GetCashString(int zCash)
	{
        //return string.Format("{0}{1:#,###0}", '}', zCash);
        //return LocalizationManager.FixRTL_IfNeeded(string.Format("{0:#,###0}", zCash));
	    return string.Format("<sprite=0> {0}", zCash.ToString("#,###0").ToNativeNumber());
	}

    public static string GetCashString(long zCash)
    {
        //return string.Format("{0}{1:#,###0}", '}', zCash);
        //return LocalizationManager.FixRTL_IfNeeded(string.Format("{0:#,###0}", zCash));
        return string.Format("<sprite=0> {0}", zCash.ToString("#,###0").ToNativeNumber());
    }

    public static string GetCashNavbarString(int zCash)
    {
        //return string.Format("{0}{1:#,###0}", '}', zCash);
        //return LocalizationManager.FixRTL_IfNeeded(string.Format("{0:#,###0}", zCash));
        return string.Format("{0:#,###0}", zCash).ToNativeNumber();
    }

	public static string GetShortCashString(int zCash)
	{
		string arg = string.Empty;
		if (zCash >= 1000000 && zCash % 1000000 == 0)
		{
			zCash /= 1000000;
			arg = "m";
		}
		else if (zCash >= 1000 && zCash % 1000 == 0)
		{
			zCash /= 1000;
			arg = "k";
		}
		return string.Format("{0}{1:#,###0}{2}", '}', zCash, arg);
	}

	public static string GetColouredGoldString(int zGold)
	{
	    return zGold.ToString();
	    //return string.Format("{0}{1}{2:#,###0}{3}", new object[]
	    //{
	    //    CurrencyUtils.GoldColour,
	    //    '{',
	    //    zGold,
	    //    Color.white
	    //});
	}

	public static string GetColouredCashString(int zCash)
	{
		return string.Format("{0}{1}{2:#,###0}{3}", new object[]
		{
			CashColour,
			'}',
			zCash,
			Color.white
		});
	}

	public static string GetXPString(int zXP)
	{
		return string.Format("{0}{1:#,###0}", LocalizationManager.GetTranslation("TEXT_HUD_XP_SYMBOL"), zXP);
	}

	public static string FormatCurrencyNumber(int Number)
	{
		return string.Format("{0:#,###0}", Number);
	}

	public static string GetColouredCostStringBrief(int zCashCost, int zGoldCost, int zkeyCost)
	{
		if (zCashCost > 0 && zGoldCost > 0)
		{
			return string.Format("{0}{1}{2:#,###0} {3}{4}{5:#,###0}{6}", new object[]
			{
				CashColour,
				'}',
				zCashCost,
				GoldColour,
				'{',
				zGoldCost,
				Color.white
			});
		}
		if (zCashCost > 0)
		{
            return string.Format("<sprite=0> {0}", zCashCost.ToString("#,###0").ToNativeNumber());
		}
		if (zGoldCost > 0)
		{
            return string.Format("<sprite=1> {0}", zGoldCost.ToString("#,###0").ToNativeNumber());
		}
        if (zkeyCost > 0)
        {
            return string.Format("<sprite=2> {0}", zkeyCost.ToString("#,###0").ToNativeNumber());
        }
		return LocalizationManager.GetTranslation("TEXT_COST_NOTHING");
	}

	public static string GetCostStringBrief(int zCashCost, int zGoldCost)
	{
		if (zCashCost > 0 && zGoldCost > 0)
		{
			return string.Format("{0}{1:#,###0} {2}{3:#,###0}", new object[]
			{
				'}',
				zCashCost,
				'{',
				zGoldCost
			});
		}
		if (zCashCost > 0)
		{
            return string.Format("<sprite=0> {0}", zCashCost.ToString("#,###0").ToNativeNumber());
		}
		if (zGoldCost > 0)
		{
            return string.Format("<sprite=1> {0}", zGoldCost.ToString("#,###0").ToNativeNumber());
		}
		return LocalizationManager.GetTranslation("TEXT_COST_NOTHING");
	}

	public static CurrencyStringInfo ParseCurrencyString(string value)
    {
        value = value.ToInvarientDigitChar();
        //Debug.Log("Parsing currency string : "+ value);
		CurrencyStringInfo currencyStringInfo = new CurrencyStringInfo();
		currencyStringInfo.originalString = value;
		bool flag = true;
		string text = string.Empty;
		int num = 0;
		currencyStringInfo.digits = string.Empty;
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if ((c >= '0' && c <= '9'))
			{
				if (!flag && !string.IsNullOrEmpty(text))
				{
					currencyStringInfo.symbols.Add(text);
					currencyStringInfo.symbolDigitOffsets.Add(num);
					flag = true;
				}
				num++;
                currencyStringInfo.digits += c;
			}
			else
			{
				if (flag)
				{
					text = string.Empty;
					flag = false;
				}
				text += c;
			}
		}
		if (!flag)
		{
			currencyStringInfo.symbols.Add(text);
			currencyStringInfo.symbolDigitOffsets.Add(num);
		}
		for (int j = 0; j < currencyStringInfo.symbols.Count; j++)
		{
			string text2 = currencyStringInfo.symbols[j];
			num = currencyStringInfo.symbolDigitOffsets[j];
			if (value.StartsWith(text2))
			{
				currencyStringInfo.prefix = text2;
				currencyStringInfo.symbolClasses.Add(CurrencySymbolClass.CURRENCY_SYMBOL_START);
			}
			else if (value.EndsWith(text2))
			{
				currencyStringInfo.suffix = text2;
				currencyStringInfo.symbolClasses.Add(CurrencySymbolClass.CURRENCY_SYMBOL_END);
			}
			else if (currencyStringInfo.digits.Length - num == 2)
			{
				currencyStringInfo.decimalSeperator = text2;
				currencyStringInfo.symbolClasses.Add(CurrencySymbolClass.CURRENCY_SYMBOL_DECIMAL_POINT);
			}
			else
			{
				currencyStringInfo.thousandsSeperator = text2;
				currencyStringInfo.symbolClasses.Add(CurrencySymbolClass.CURRENCY_SYMBOL_THOUSANDS_SEPARATOR);
			}
		}
		if (string.IsNullOrEmpty(currencyStringInfo.decimalSeperator))
		{
			currencyStringInfo.currencyValue = Convert.ToDouble(currencyStringInfo.digits);
		}
		else
		{
			currencyStringInfo.currencyValue = Convert.ToDouble(currencyStringInfo.digits.Insert(currencyStringInfo.digits.Length - 2, "."));
		}
		return currencyStringInfo;
	}

	public static string FormatCurrencyValue(double value, CurrencyStringInfo info)
    {
        string text;
        var targetAppStore = BasePlatform.ActivePlatform.GetTargetAppStore();
        if (targetAppStore == GTAppStore.GooglePlay || targetAppStore == GTAppStore.UDP || targetAppStore == GTAppStore.iOS)
        {
            text = string.Format("{0:##,###.00}", value);
        }
        else
        {
            text = string.Format("{0:##,###}", value);
        }

        //text = text.Replace(",", info.thousandsSeperator);
        //if (string.IsNullOrEmpty(info.decimalSeperator))
        //{
        //	text = text.Substring(0, text.Length - 3);
        //}
        //else
        //{
        //	text = text.Replace(".", info.decimalSeperator);
        //}
        var result = info.prefix + text + info.suffix;
        //Debug.Log("CheckForCurrency : prefix:" + info.prefix + " - suffix:" + info.suffix+" - final:"+ result);
        return result;
	}
}
