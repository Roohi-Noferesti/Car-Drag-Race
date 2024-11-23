using System;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;

public static class StoreUtility 
{
    public static bool IsCarUpgrade(this VirtualItemType itemtype)
    {
        switch (itemtype)
        {
            case VirtualItemType.CarSpoiler:
            case VirtualItemType.CarBodyUpgrade:
            case VirtualItemType.CarEngineUpgrade:
            case VirtualItemType.CarNitrousUpgrade:
            case VirtualItemType.CarTransmissionUpgrade:
            case VirtualItemType.CarTurboUpgrade:
            case VirtualItemType.CarTyreUpgrade:
            case VirtualItemType.CarSticker:
                return true;
        }
        return false;
    }

    public static VirtualItemType GetItemType(string itemtype)
    {
        VirtualItemType type;
        try
        {
            type = (VirtualItemType)Enum.Parse(typeof(VirtualItemType), itemtype, true);
        }
        catch (Exception)
        {
            type = VirtualItemType.None;
        }

        return type;
    }

    //public static Manufacturer.ManufaturerID GetManufacturer(object value)
    //{
    //    return (Manufacturer.ManufaturerID)(value == DBNull.Value ? 0 : (short)value);
    //}

    public static eUpgradeType ToUpgradeType(this VirtualItemType itemType)
    {
        switch (itemType)
        {
            case VirtualItemType.CarBodyUpgrade:
                return eUpgradeType.BODY;
            case VirtualItemType.CarEngineUpgrade:
                return eUpgradeType.ENGINE;
            case VirtualItemType.CarNitrousUpgrade:
                return eUpgradeType.NITROUS;
            case VirtualItemType.CarTransmissionUpgrade:
                return eUpgradeType.TRANSMISSION;
            case VirtualItemType.CarTyreUpgrade:
                return eUpgradeType.TYRES;
            case VirtualItemType.CarTurboUpgrade:
                return eUpgradeType.TURBO;
            case VirtualItemType.CarIntakeUpgrade:
                return eUpgradeType.INTAKE;
            default:
                return eUpgradeType.INVALID;
        }
    }

    public static ServerItemBase.AssetType ToAssetType(this VirtualItemType itemType)
    {
        switch (itemType)
        {
            case VirtualItemType.BodyShader:
                return ServerItemBase.AssetType.body_shader;
            case VirtualItemType.RingShader:
                return ServerItemBase.AssetType.ring_shader;
            case VirtualItemType.CarSpoiler:
                return ServerItemBase.AssetType.spoiler;
            case VirtualItemType.CarSticker:
                return ServerItemBase.AssetType.sticker;
            case VirtualItemType.HeadLighShader:
                return ServerItemBase.AssetType.headlight_shader;
            default:
                return ServerItemBase.AssetType.none;
        }
    }

    public static string ToXML(this AnimationCurve animationCurve)
    {
        var stringwriter = new StringWriter();
        var serializer = new XmlSerializer(typeof(AnimationCurve));
        serializer.Serialize(stringwriter, animationCurve);
        return stringwriter.ToString();
    }

    public static void SetState(this Button button, BaseRuntimeControl.State state)
    {
        switch (state)
        {
            case BaseRuntimeControl.State.Active:
                button.gameObject.SetActive(true);
                button.interactable = true;
                break;
                case BaseRuntimeControl.State.Disabled:
                button.gameObject.SetActive(true);
                button.interactable = false;
                break;
                case BaseRuntimeControl.State.Hidden:
                button.gameObject.SetActive(false);
                break;
                case BaseRuntimeControl.State.Highlight:
                break;
                case BaseRuntimeControl.State.Pressed:
                break;
        }
    }

    public static string ToNativeNumber(this string input)
    {
        //if (false)//LocalizationManager.CurrentLanguageCode == "fa")
        //{
        //    string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

        //    for (int j = 0; j < persian.Length; j++)
        //        input = input.Replace(j.ToString(), persian[j]);
        //}
        return input;
    }

    public static string ToNativeNumber(this int input)
    {
        return input.ToString().ToNativeNumber();
    }

    public static string ToNativeNumber(this float input)
    {
        return input.ToString().ToNativeNumber();
    }

    public static string ToNativeNumber(this double input)
    {
        return input.ToString().ToNativeNumber();
    }

    public static string ToValidAvatarLink(this string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            return input.Replace("lh5", "lh6");
        }
        return input;
    }

    public static string ConvertNumbers(string problem)
    {
        char[][] numbers = new char[][]
        {
            "۰۱۲۳۴۵۶۷۸۹٬".ToCharArray(),"0123456789,".ToCharArray()
        };
        for (int x = 0; x < numbers[0].Length; x++)
        {
            problem = problem.Replace(numbers[0][x], numbers[1][x]);
        }

        return problem;
    }

    public static string ToCurrency(this string value)
    {
        if (value.Contains("ریال"))
        {
            value = value.Replace("ریال", "");
            value = ConvertNumbers(value);
            double price;
            if (double.TryParse(value, out price))
            {
                price = price / 10;
                return String.Format("ﻥﺎﻣﻮﺗ {0:n0}", price);
            }
            else
            {
                return "ﻥﺎﻣﻮﺗ ﺮﻔﺻ";
            }
        }
        return LocalizationManager.ApplyRTLfix(value);
        //return LocalizationManager.FixRTL_IfNeeded(value);
    }

    public static double ToCurrencyValue(this string value)
    {
        if (value.Contains("ریال"))
        {
            value = value.Replace("ریال", "");
            value = ConvertNumbers(value);
            double price;
            if (double.TryParse(value, out price))
            {
                return price;
            }
            else
            {
                return -1;
            }
        }
        return -1;
    }


    public static string ToInvarientDigitChar(this string str)
    {
        var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        var persianNumbers = new[] {"۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"};
        var arabicNumbers = new[] {"٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"};
        for (int i = 0; i <= 9; i++)
        {
            str = str.Replace(persianNumbers[i], CultureInfo.InvariantCulture.NumberFormat.NativeDigits[i]);
        }

        for (int i = 0; i <= 9; i++)
        {
            str = str.Replace(currentCulture.NumberFormat.NativeDigits[i], CultureInfo.InvariantCulture.NumberFormat.NativeDigits[i]);
        }

        for (int i = 0; i <= 9; i++)
        {
            str = str.Replace(arabicNumbers[i], CultureInfo.InvariantCulture.NumberFormat.NativeDigits[i]);
        }
        return str;
    }

}
