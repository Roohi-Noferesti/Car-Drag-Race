using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
	private enum FadeState
	{
		fadein,
		fullon,
		fadeout,
		fulloff,
		waiting,
        waitingForAgeVerification,
        waitingForPrivacyPolicyPopup
	}

	private const float fadeInTime = 0.5f;

	private const float fullOnTime = 0.9f;

	private const float fadeOutTime = 0.5f;

	private const float fullOffTime = 0.5f;

	public GameObject[] SplashScreens;

	private float fadeVal;

	private float screenTimer;

	private List<RawImage> fadeSprites;

	private int currentSprite;

	private Splash.FadeState currentState;

    private Canvas m_canvas;
    
    private bool doneForceUpdate = false;

    private void Awake()
	{
		Z2HInitialisation.Instance.Init(true);
        this.fadeSprites = new List<RawImage>();
	}

	private void Start()
	{
		//Debug.Log("Device id : "+SystemInfo.deviceUniqueIdentifier);
	 //   Debug.Log("Splash scene loaded");
        m_canvas = GetComponentInChildren<Canvas>(true);
        m_canvas.gameObject.SetActive(true);

        if (PermanentData.GetTimeZone("TimeZone") == string.Empty)
        {
	        PermanentData.SaveTimeZone("TimeZone", AndroidSpecific.GetTimeZone().ToString());
        }
        
        GameObject[] splashScreens = this.SplashScreens;
		for (int i = 0; i < splashScreens.Length; i++)
		{
			GameObject gameObject = splashScreens[i];
			if (!(gameObject == null))
			{
                this.fadeSprites.Add(gameObject.GetComponent<RawImage>());
			}
		}
        foreach (RawImage current in this.fadeSprites)
		{
			current.color = new Color(0f, 0f, 0f, 0f);
		}
		this.fadeVal = 0f;
		this.screenTimer = 0f;
		this.currentState = Splash.FadeState.fadein;
		this.currentSprite = 0;
		Z2HInitialisation.Instance.okToGoBig = false;
		if (this.fadeSprites.Count == 0)
		{
            Z2HInitialisation.Instance.okToGoBig = true;
			this.currentState = Splash.FadeState.waiting;
		}

		//Debug.Log("Start of splash ended");
	}

	private void Update()
	{
//	Debug.Log(this.currentState);
		this.screenTimer += Time.deltaTime;
        switch (this.currentState)
        {
            case Splash.FadeState.fadein:
                this.fadeVal = Mathf.Clamp01(this.screenTimer / 1f);
                if (this.screenTimer > 1f)
                {
                    this.screenTimer = 0f;
                    this.currentState = Splash.FadeState.fullon;
                    Z2HInitialisation.Instance.okToGoBig = true;
                }

                break;
            case Splash.FadeState.fullon:
                this.fadeVal = 1f;
                if (this.screenTimer > 2f)
                {
                    this.screenTimer = 0f;
                    this.currentState = Splash.FadeState.fadeout;
                    Z2HInitialisation.Instance.okToGoBig = false;
                }

                break;
            case Splash.FadeState.fadeout:
                this.fadeVal = 1f - Mathf.Clamp01(this.screenTimer / .8f);
                if (this.screenTimer > .8f)
                {
                    this.screenTimer = 0f;
                    this.fadeVal = 0f;
                    this.currentState = Splash.FadeState.fulloff;
                    Z2HInitialisation.Instance.okToGoBig = true;
                    if (this.currentSprite == this.fadeSprites.Count - 1)
                    {
                        this.currentState = Splash.FadeState.waiting;
                        m_canvas.gameObject.SetActive(false);
                        BaseDevice.ActiveDevice.ApplyInitialQuality();
                    }
                }

                break;
            case Splash.FadeState.fulloff:
                this.fadeVal = 0f;
                if (this.screenTimer > 0.8f)
                {
                    this.screenTimer = 0f;
                    this.currentState = Splash.FadeState.fadein;
                    Z2HInitialisation.Instance.okToGoBig = false;
                    this.currentSprite++;
                }

                break;
            case Splash.FadeState.waiting:

                if (Z2HInitialisation.Instance.Complete)
                {
                    this.currentState = Splash.FadeState.waitingForAgeVerification;


                    if (!AgeVerificationManager.Instance.HasPrivacyPolicyVerified)
                    {
                        //Set this for new users only
                        if (UserManager.Instance == null || UserManager.Instance.currentAccount.UserID == 0)
                        {
                            PopUp popup = new PopUp
                            {
                                BodyText = "TEXT_POPUPS_AGE_VERIFICATION_TITLE",
                                IsAgeVerification = true,
                                ID = PopUpID.AgeVerification,
                                ConfirmText = "TEXT_BUTTON_OK",
                            };
                            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
                        }
                        else
                        {
                            AgeVerificationManager.Instance.SetToDefault();
                            AgeVerificationManager.Instance.SaveChanges();
                        }
                    }
                }

                break;
            case FadeState.waitingForAgeVerification:
                if (AgeVerificationManager.Instance.HasAgeVerified && !PopUpManager.Instance.isShowingPopUp)
                {
                    this.currentState = Splash.FadeState.waitingForPrivacyPolicyPopup;

                    if (AgeVerificationManager.Instance.IsUnder13 && !AgeVerificationManager.Instance.HasPrivacyPolicyVerified)
                    {
                        PopUp popup = new PopUp
                        {
                            Title = "TEXT_POPUPS_PRIVACY_POLICY_TITLE",
                            BodyText = "TEXT_POPUPS_PRIVACY_POLICY_BODY",
                            ConfirmText = "TEXT_POPUPS_PRIVACY_POLICY_IAGREE_BUTTON",
                            CancelText = "TEXT_POPUPS_PRIVACY_POLICY_EXIT_TO_PRIVACY_BUTTON",
                            IsPrivacyPolicy = true,
                            ConfirmAction = OnConfirmPrivacyPolicy,
                            CancelAction = OnCancelPrivacyPolicy
                        };
                        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.System, null);
                    }
                    else
                    {
                        LoadFrontend();
                    }
                }

                break;

            case FadeState.waitingForPrivacyPolicyPopup:
                if (!PopUpManager.Instance.isShowingPopUp && AgeVerificationManager.Instance.HasPrivacyPolicyVerified)
                {
                    LoadFrontend();
                }

                break;
        }

        if (this.currentSprite >= this.fadeSprites.Count)
		{
			return;
		}
        RawImage sprite = this.fadeSprites[this.currentSprite];
		sprite.color = new Color(1f, 1f, 1f, this.fadeVal);
	}

    private void OnCancelPrivacyPolicy()
    {
        Application.OpenURL("http://www.turnedondigital.com/privacy-policy");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    private void OnConfirmPrivacyPolicy()
    {
        AgeVerificationManager.Instance.HasPrivacyPolicyVerified = true;
        AgeVerificationManager.Instance.SaveChanges();
        LoadFrontend();
    }

    private void LoadFrontend()
    {
	    FindObjectOfType<SplashLoadingOverlay>().Open();
	    Debug.Log("initialization completed : Loading scene frontend");
        SceneLoadManager.Instance.LoadScene(SceneLoadManager.Scene.Frontend, false);
        enabled = false;
    }
}
