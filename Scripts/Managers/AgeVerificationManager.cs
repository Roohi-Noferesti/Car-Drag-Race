using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeVerificationManager
{
    private static AgeVerificationManager _instance;
    private const string USER_AGE_VERIFIED_KEY = "USER_AGE_VERIFIED";
    private const string USER_YEAR_OF_BIRTH_KEY = "USER_YEAR_OF_BIRTH";
    private const string PRIVACY_POLICY_VERIFIED_KEY = "PRIVACY_POLICY_VERIFIED";
    public bool HasAgeVerified { get; set; }

    public int UserYearOfBirth { get; set; }

    public bool HasPrivacyPolicyVerified { get; set; }

    public static AgeVerificationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AgeVerificationManager();
                _instance.Init();
            }
            return _instance;
        }
    }

    public bool IsUnder13
    {
        get { return GTDateTime.Now.Year - UserYearOfBirth <= 13; }
    }

    public bool IsUnder17
    {
        get { return GTDateTime.Now.Year - UserYearOfBirth <= 17; }
    }


    public void Init()
    {
        HasAgeVerified = PlayerPrefs.GetInt(USER_AGE_VERIFIED_KEY, 0) == 1;
        HasPrivacyPolicyVerified = PlayerPrefs.GetInt(PRIVACY_POLICY_VERIFIED_KEY, 0) == 1;
        UserYearOfBirth = PlayerPrefs.GetInt(USER_YEAR_OF_BIRTH_KEY, 2021);
    }

    public void SaveChanges()
    {
        PlayerPrefs.SetInt(USER_AGE_VERIFIED_KEY, HasAgeVerified ? 1 : 0);
        PlayerPrefs.SetInt(PRIVACY_POLICY_VERIFIED_KEY, HasPrivacyPolicyVerified ? 1 : 0);
        PlayerPrefs.SetInt(USER_YEAR_OF_BIRTH_KEY, UserYearOfBirth);
    }

    public void SetToDefault()
    {
        UserYearOfBirth = 2004;
        HasAgeVerified = true;
        HasPrivacyPolicyVerified = true;
    }
}
