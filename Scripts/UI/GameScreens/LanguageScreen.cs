using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageScreen : ZHUDScreen
{
    [SerializeField] private GameObject m_languagePrefab;
    [SerializeField] private Transform m_layout;
    //[SerializeField] private RuntimeButton m_okButton;

    //private Dictionary<string,Toggle> m_languageToggle;



    [SerializeField] private LanguageFont[] _languageFonts;
    [SerializeField] private Dictionary<string, GameObject> buttonOverlays;


    public override ScreenID ID
    {
        get { return ScreenID.Language; }
    }

    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);
        m_languagePrefab.SetActive(false);
        GTDebug.Log(GTLogChannel.Screens,"on after activated language screen");
        if (!GTSystemOrder.BeenTriggered && !GTSystemOrder.SystemsReady)
        {
            GTDebug.Log(GTLogChannel.Screens,"StartUpGameSystems language screen");
            GTSystemOrder.StartUpGameSystems();
        }
        CoroutineManager.Instance.StartCoroutine(_initializeLanguages());
    }

    private IEnumerator _initializeLanguages()
    {
        //m_okButton.CurrentState = BaseRuntimeControl.State.Hidden;
        //while (!LocalizationManager.IsReady)
        //{
        //    yield return 0;
        //}

        GTDebug.Log(GTLogChannel.Screens,"loading language screen");
        var languages = LocalizationManager.GetAllLanguages();
        //m_languageToggle = new Dictionary<string, Toggle>();
        bool showMyLanguage = true;
#if UNITY_IOS
        showMyLanguage = BasePlatform.ActivePlatform.InsideCountry;
#endif
        buttonOverlays = new Dictionary<string, GameObject>();
        for (var i = 0; i < languages.Count; i++)
        {
            var language = languages[i];
            if(language==string.Concat("Per","sian") && !showMyLanguage)
                continue;
            var languageInstance = (GameObject)Instantiate(m_languagePrefab);
            languageInstance.name = language;
            languageInstance.transform.SetParent(m_layout, false);
            buttonOverlays.Add(language, languageInstance.transform.Find("Overlay").gameObject);
            buttonOverlays[language].SetActive(false);
            //var toggle = languageInstance.GetComponent<Toggle>();
            //toggle.group = m_layout.GetComponent<ToggleGroup>();
            //toggle.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetTranslation("TEXT_LANGUAGE_" + language.ToUpper());
            //toggle.isOn = LocalizationManager.CurrentLanguage == language;
            var languageFont = _languageFonts.FirstOrDefault(l => l.Name == language);
            if (languageFont != null)
            {
                languageInstance.GetComponentInChildren<TextMeshProUGUI>().font = languageFont.Font;
                languageInstance.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetTranslation("TEXT_LANGUAGE_" + language.ToUpper());
                languageInstance.gameObject.SetActive(true);
            } else {
                languageInstance.gameObject.SetActive(false);
            }
            
            //m_languageToggle.Add(language, toggle);
        }
        if(ScreenManager.Instance.LastScreen == ScreenID.Options)
            buttonOverlays[LocalizationManager.CurrentLanguage].SetActive(true);

        //m_okButton.CurrentState = BaseRuntimeControl.State.Active;
        yield break;
    }
    
    
    public void OnLanguageButtonClicked()
    {
        string lang = EventSystem.current.currentSelectedGameObject.name;

        if (ScreenManager.Instance.LastScreen == ScreenID.Options)
        {
            if (LocalizationManager.CurrentLanguage == lang)
            {
                foreach (GameObject go in buttonOverlays.Values)
                {
                    go.SetActive(false);
                }

                buttonOverlays[lang].SetActive(true);
                OnOKButton();
                return;
            }
            
        }
        ChangeLanguage(lang);

    }

    private void ChangeLanguage(string lang)
    {
        LocalizationManager.CurrentLanguage = lang;

        foreach(GameObject go in buttonOverlays.Values)
        {
            go.SetActive(false);
        }
        buttonOverlays[lang].SetActive(true);
        OnOKButton();
    }

    public void OnChangeLanguage(bool on)
    {
        if (!on)
            return;
        /*foreach (var lang in m_languageToggle)
        {
            if (lang.Value.isOn)
            {
                LocalizationManager.CurrentLanguage = lang.Key;
                break;
            }
        }*/
    }

    public void OnOKButton()
    {
        UserManager.Instance.currentAccount.Language = LocalizationManager.CurrentLanguage;
        UserManager.Instance.SaveCurrentAccount();
        Close();
    }
}

[System.Serializable]
public class LanguageFont
{
    public string Name;
    public TMP_FontAsset Font;
}
