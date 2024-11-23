using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Currencies", menuName = "Currencies")]
public class CurrenciesFortumo : ScriptableObject
{

    public List<Currency> Currencies;


    [System.Serializable]
    public struct Currency
    {
        public string[] CountryCodes;
        public string CurrencyID;
        public string CurrencySymbol;
        public string CurrencyFormat;
    }
}
