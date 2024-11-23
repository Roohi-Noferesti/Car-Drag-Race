using System.Collections.Generic;

public class CarUpgrades
{
    private static List<eUpgradeType> validUpgrades = new List<eUpgradeType>
    {
        eUpgradeType.ENGINE,
        eUpgradeType.TURBO,
        eUpgradeType.INTAKE,
        eUpgradeType.NITROUS,
        eUpgradeType.BODY,
        eUpgradeType.TYRES,
        eUpgradeType.TRANSMISSION
    };

    private static List<eUpgradeType> z2HValidUpgrades = new List<eUpgradeType>
    {
        eUpgradeType.ENGINE,
        eUpgradeType.TURBO,
        eUpgradeType.NITROUS,
        eUpgradeType.BODY,
        eUpgradeType.TYRES,
        eUpgradeType.TRANSMISSION
    };

    private static Dictionary<eUpgradeType, string> upgradeNames = new Dictionary<eUpgradeType, string>
    {
        {
            eUpgradeType.ENGINE,
            "ENGINE"
        },
        {
            eUpgradeType.TURBO,
            "TURBO"
        },
        {
            eUpgradeType.INTAKE,
            "INTAKE"
        },
        {
            eUpgradeType.NITROUS,
            "NITROUS"
        },
        {
            eUpgradeType.BODY,
            "BODY"
        },
        {
            eUpgradeType.TYRES,
            "TYRES"
        },
        {
            eUpgradeType.TRANSMISSION,
            "TRANS"
        },
        {
            eUpgradeType.INVALID,
            "INVALID"
        }
    };

    private static Dictionary<eUpgradeType, string> upgradeNamesShort = new Dictionary<eUpgradeType, string>
    {
        {
            eUpgradeType.ENGINE,
            "eng"
        },
        {
            eUpgradeType.TURBO,
            "tur"
        },
        {
            eUpgradeType.INTAKE,
            "int"
        },
        {
            eUpgradeType.NITROUS,
            "nit"
        },
        {
            eUpgradeType.BODY,
            "bod"
        },
        {
            eUpgradeType.TYRES,
            "tir"
        },
        {
            eUpgradeType.TRANSMISSION,
            "tra"
        },
        {
            eUpgradeType.INVALID,
            "INVALID"
        }
    };

    private static Dictionary<eUpgradeType, string> upgradeTextIDsForLocalisation = new Dictionary<eUpgradeType, string>
    {
        {
            eUpgradeType.ENGINE,
            "TEXT_MENU_ICON_ENGINE"
        },
        {
            eUpgradeType.TURBO,
            "TEXT_MENU_ICON_TURBO"
        },
        {
            eUpgradeType.INTAKE,
            "TEXT_MENU_ICON_INTAKE"
        },
        {
            eUpgradeType.NITROUS,
            "TEXT_MENU_ICON_NITROUS"
        },
        {
            eUpgradeType.BODY,
            "TEXT_MENU_ICON_BODY"
        },
        {
            eUpgradeType.TYRES,
            "TEXT_MENU_ICON_TYRES"
        },
        {
            eUpgradeType.TRANSMISSION,
            "TEXT_MENU_ICON_GEAR_BOX"
        },
        {
            eUpgradeType.INVALID,
            "INVALID"
        }
    };

    public static List<eUpgradeType> ValidUpgrades
    {
        get { return validUpgrades; }
    }

    //public static List<eUpgradeType> Z2HValidUpgrades
    //{
    //    get { return z2HValidUpgrades; }
    //}

    public static Dictionary<eUpgradeType, string> UpgradeNames
    {
        get { return upgradeNames; }
    }

    public static Dictionary<eUpgradeType, string> UpgradeNamesShort
    {
        get { return upgradeNamesShort; }
    }

    public static Dictionary<eUpgradeType, string> UpgradeTextIDsForLocalisation
    {
        get { return upgradeTextIDsForLocalisation; }
    }
}
