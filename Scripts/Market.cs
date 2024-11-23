using System.Collections;
using System.Collections.Generic;
using KingKodeStudio.IAB.Zarinpal;
using UnityEngine;

public static class Market
{
    public static string GetMarket()
    {
        var config = PurchasingModuleSelection.Config;
        var insideCountry = BasePlatform.ActivePlatform.InsideCountry;

        if (insideCountry && (config.IsAndroidZarinpal || config.IsIosZarinpal))
        {
            return "Zarinpal";
        }

        if (config.UseUnityIAPSetting)
        {
            return PurchasingModuleSelection.StoreName;
        }
        if (config.IsBazaar)
        {
            return "Bazaar";
        }
        if (config.IsMyket)
        {
            return "Myket";
        }

        return "None";
    }
}
