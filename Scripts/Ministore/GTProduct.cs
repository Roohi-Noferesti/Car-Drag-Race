using System;
using UnityEngine;

[Serializable]
public class GTProduct
{
    [SerializeField]
    public string _code;
    [SerializeField]
    public int _sortIndex;
    [SerializeField]
    public int _gold;
    [SerializeField]
    public int _bonusGold;
    [SerializeField]
    public int _cash;
    [SerializeField]
    public int _bonusCash;
    [SerializeField]
    public int _numSuperNitros;
    [SerializeField]
    public int _numRaceCrew;
    [SerializeField]
    public StickerType _sticker;
    [SerializeField]
    public bool _isConsumable;

    public enum StickerType
	{
		None,
		BestValue,
		MostPopular,
		Recommended,
		GoodValue,
		GreatDeal
	}

    public string Code
    {
        get { return _code; }
        set { _code = value; }
    }

    public int SortIndex
    {
        get { return _sortIndex; }
        set { _sortIndex = value; }
    }

    public int Gold
    {
        get { return _gold; }
        set { _gold = value; }
    }

    public int BonusGold
    {
        get { return _bonusGold; }
        set { _bonusGold = value; }
    }

    public int Cash
    {
        get { return _cash; }
        set { _cash = value; }
    }

    public int BonusCash
    {
        get { return _bonusCash; }
        set { _bonusCash = value; }
    }

    public int NumSuperNitros
    {
        get { return _numSuperNitros; }
        set { _numSuperNitros = value; }
    }

    public int NumRaceCrew
    {
        get { return _numRaceCrew; }
        set { _numRaceCrew = value; }
    }

    public StickerType Sticker
    {
        get { return _sticker; }
        set { _sticker = value; }
    }

    public string CodeWithIdentifier
	{
		get
		{
			string str = BasePlatform.ActivePlatform.GetBundleIdentifier() + ".";
			return str + this.Code;
		}
	}

    public bool IsConsumable
    {
        get { return _isConsumable; }
        set { _isConsumable = value; }
    }

    public override string ToString()
	{
		return string.Format("Product(Code: {0}, SortIndex: {1}, Gold: {2}, BonusGold: {3}, Cash: {4}, BonusCash: {5}, NumSuperNitros: {6}, Sticker: {7}, IsConsumable: {8})", new object[]
		{
			this.Code,
			this.SortIndex,
			this.Gold,
			this.BonusGold,
			this.Cash,
			this.BonusCash,
			this.NumSuperNitros,
			this.NumRaceCrew,
			this.Sticker,
			this.IsConsumable
		});
	}
}
