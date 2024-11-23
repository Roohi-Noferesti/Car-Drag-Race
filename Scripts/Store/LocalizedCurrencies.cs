using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//moeen
// this class is mostly designed to work with fortumo payment
// time zone possiblities moved to CurrenciesFortumo Scriptable object for simplicity but user currency references maintained in here

public static class LocalizedCurrencies
{
    public static string UserCurrencyCode = "SAR";// just saving user currency that is purchasing with. its chosen by androidspecific time zone
    public static string UserCurrencySymbol = "ریال";// just saving user currency that is purchasing with. its chosen by androidspecific time zone
    public static string UserCurrencyFormat = "{0} {1}";// just saving user currency that is purchasing with. its chosen by androidspecific time zone

    //// list of fortumo possible countries with all possible time zone names and currencies
    //public static Dictionary<string, List<string>> Currencies = new Dictionary<string, List<string>>()
    //{
    //    {"USD",new List<string>(){"usa","america","washington"} },
    //    {"IQD", new List<string>(){"baghdad","baqdad","iraq","بغداد","عراق","العراق"} },
    //    {"MXN", new List<string>(){"mexico","mexico city", "ciudad de méxico","central", "cancun","merida","ojinaga","tijuana","mazatlan","chihuahua","matamoros","monterrey","hermosillo","santa isabel","puerto vallerta" } },
    //    {"INR",new List<string>(){"india","dehli","new dehli","mumbai", "मुंबई", "gurgaon", "गुडगाँव", "calcutta", "कलकत्ता", "bangalore", "बैंगलोर", "hyderabad", "हैदराबाद", "भारत", "नई दिल्ली" } },
    //    {"PHP",new List<string>(){ "philippines", "philippine", "manila" } },
    //    {"SAR",new List<string>(){"saudi arabia","mecca", "riyadh","العربية", "الریاض","مكة" , "السعودية" , "العربية السعودية" } }
    //};

}
