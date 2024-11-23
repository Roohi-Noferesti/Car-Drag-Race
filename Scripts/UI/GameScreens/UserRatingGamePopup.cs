using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using I2.Loc;
using System;
using TMPro;
using RTLTMPro;
using Metrics;

public class UserRatingGamePopup : PopUpDialogue
{
    //moeen
    // we are going to rate application by user independent from main store rating
    // we just send user to store if user give 5 STARS on this screen

    #region references
    [SerializeField] private float TimeBeforeClose;
    [SerializeField] private Sprite StarOffSprite;
    [SerializeField] private Sprite StarOnSprite;
    [SerializeField] private GameObject StarsParent;
    [SerializeField] private GameObject[] StarButtons;
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private TextMeshProUGUI LoadingText;
    [SerializeField] private TMP_InputField CommentField;
    [SerializeField] private TextMeshProUGUI ShowingComment;
    #endregion

    

    private int UserGivenStars = 0;
    private bool StarsAreInteractible = true;

    public static UserRatingGamePopup
        Instance
    { get; private set; }


    protected override void Start()
    {
        base.Start();
        Instance = this;
        InitStars();
        InitPanel();
        CommentField.onValueChanged.RemoveAllListeners();
        CommentField.onValueChanged.AddListener(MakeTextRTL);
    }
    
    private void InitPanel()
    {
        SetCloseButtonCallback();
        SetSubmitButtonCallback(OnSubmitButtonForStarsPressed);
    }



    private void InitStars()
    {
        for (int i = 0; i < StarButtons.Length; i++)
        {
            int result = i + 1;
            StarButtons[i].GetComponent<Button>().onClick.AddListener(delegate { OnGiveStarButtonPressed(result); });
        }
    }


    private void SetSubmitButtonCallback(UnityAction action)
    {
        ConfirmButton.AddValueChangedDelegate(action);
    }

    private void SetCloseButtonCallback()
    {
       // CloseButton.AddValueChangedDelegate(Close);
    }

    private void OnGiveStarButtonPressed(int number)
    {
        if (!StarsAreInteractible) return;
        UserGivenStars = number;
        for (int i = 0; i < number; i++)
        {
            StarButtons[i].GetComponent<Image>().sprite = StarOnSprite;
        }
        for (int i = number; i < StarButtons.Length; i++)
        {
            StarButtons[i].GetComponent<Image>().sprite = StarOffSprite;
        }

    }
    private void OnSubmitButtonForStarsPressed()
    {
        if (UserGivenStars >= 5)
        {
            RateTheAppNagger.TriggerRateAppPage();
            Log.AnEvent(Events.ConfirmedRateApp);
            LoadingText.text = LocalizationManager.GetTranslation("TEXT_USERRATE_RESPONSE");
            LoadingText.gameObject.SetActive(true);
            ConfirmButton.gameObject.SetActive(false);
            StarsParent.SetActive(false);
            ConfirmButton.CurrentState = BaseRuntimeControl.State.Disabled;
            Close();
        }
        else
        {
            StarsParent.SetActive(false);
            CommentField.gameObject.SetActive(true);
            TitleText.text = LocalizationManager.GetTranslation("TEXT_USERRATE_TITLE_COMMENT");
            SetSubmitButtonCallback(OnSubmitButtonForCommentPressed);

        }
        StarsAreInteractible = false;
    }

    private void OnSubmitButtonForCommentPressed()
    {

        CommentField.gameObject.SetActive(false);
        LoadingText.gameObject.SetActive(true);
        LoadingText.text = LocalizationManager.GetTranslation("TEXT_USERRATE_WAIT");
        ConfirmButton.CurrentState = BaseRuntimeControl.State.Disabled;
        // SENDING MESSAGE
        JsonDict parameters = new JsonDict();
        parameters.Set("comment", CommentField.text);
        parameters.Set("username", UserManager.Instance.currentAccount.Username);
        parameters.Set("rate", UserGivenStars.ToString());

        if (PolledNetworkState.IsNetworkConnected && ServerSynchronisedTime.Instance.ServerTimeValid)
        {
            WebRequestQueue.Instance.StartCall("rtw_save_user_rate", "descrption", parameters, null);
        }
        Close();
    }

    //private void ResponseCallback(string zHTTPContent, string zError, int zStatus, object zUserData)
    //{
    //    GTDebug.Log(GTLogChannel.Account, "rate response content is  : " + zHTTPContent);
    //    LoadingText.text = LocalizationManager.GetTranslation("TEXT_USERRATE_DONE");
    //    StopCoroutine("HandleTimeout");
    //    Close();
    //}

    public void Close()
    {
        PopUpManager.Instance.KillPopUp();
    }
    private IEnumerator ReturnTextField()
    {
        yield return new WaitForSeconds(2);
        ConfirmButton.CurrentState = BaseRuntimeControl.State.Active;
        LoadingText.gameObject.SetActive(false);
        CommentField.gameObject.SetActive(true);


    }



    // we have a secondary text under text field that should be returned to rtl to show.
    private void MakeTextRTL(string result)
    {
        ShowingComment.text = result;
    }


}
