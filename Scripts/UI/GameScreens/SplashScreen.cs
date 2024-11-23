using System.Collections.Generic;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CSR
{
	public class SplashScreen : ZHUDScreen
	{
		private enum LogoState
		{
			FadeIn,
			In,
			FadeOut,
			Off
		}

		private enum BackgroundState
		{
			In,
			FadeOut,
			Off
		}

		private const float LogoOnTime = 1.7f;

		private const float LogoFadeTimeIn = 0.8f;

		private const float LogoFadeTimeOut = 0.5f;

		private const float BackgroundOnTime = 1.8f;

		private const float BackgroundFadeTimeOut = 0.3f;

		public PrefabPlaceholder Logo;

		public Slider ProgressBar;

		public GameObject LoadingSpinnerRoot;

		public TextMeshProUGUI LoadingSpinnerText;

		private List<GameObject> m_BackgroundGameObjectList = new List<GameObject>();

		private LogoState m_LogoState;

		private float m_LogoTimer = 0.8f;

		private BackgroundState m_BackgroundState;

		private float m_BackgroundTimer = 1.8f;

        public TextMeshProUGUI SplashScreenText;

		public override ScreenID ID
		{
			get
			{
				return ScreenID.Splash;
			}
		}

	    public override void OnCreated(bool zAlreadyOnStack)
		{
            //this.LoadingSpinnerRoot.gameObject.SetActive(false);
            //this.LoadingSpinnerText.text = AndroidSpecific.GetString("com_facebook_loading");
            //this.ProgressBar.gameObject.SetActive(false);
            if (!GTSystemOrder.BeenTriggered && !GTSystemOrder.SystemsReady)
			{
                GTSystemOrder.StartUpGameSystems();
			}
            //if (LocalisationManager.GetSystemLanguage() == LocalisationManager.ISO6391.ZH)
            //{
            //    Vector3 localPosition = this.Logo.GetGameObject().transform.localPosition;
            //    localPosition.y += 0.3f;
            //    this.Logo.GetGameObject().transform.localPosition = localPosition;
            //}
            //this.SplashScreenText.text = LocalizationManager.GetTranslation("TEXT_LEGAL_FICTITIOUS_CHARACTERS");
			if (MapScreenCache.Map == null)
			{
				MapScreenCache.Load();
			}
			MapScreenCache.Map.gameObject.SetActive(false);
            BackgroundManager.ShowBackground(this.ScreenBackground, true);
            //this.m_BackgroundGameObjectList = (from x in this.GetChildrenRecursive(this.Center)
            //where x.renderer != null
            //select x).ToList<GameObject>();
            //base.OnActivate(zAlreadyOnStack);
            if (GTPlatform.Runtime == GTPlatforms.OSX)
			{
				return;
			}
            //this.Logo.transform.localPosition = new Vector3(0f, 1.28f, 0.25f);
            //NavBarAnimationManager.Instance.HideNow();
            //this.CurrentAnimState = CSRScreen.AnimState.OUT;
			this.m_LogoState = LogoState.FadeIn;
			this.m_BackgroundState = BackgroundState.In;
			this.Update();
            enabled = true;
		}

		private List<GameObject> GetChildrenRecursive(GameObject parentObject)
		{
			List<GameObject> result = new List<GameObject>();
			this.GetChildrenRecursive(parentObject, ref result);
			return result;
		}

		private void GetChildrenRecursive(GameObject parentObject, ref List<GameObject> objectList)
		{
			Transform[] componentsInChildren = parentObject.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				if (!(transform.gameObject == parentObject))
				{
					GameObject gameObject = transform.gameObject;
					this.GetChildrenRecursive(gameObject, ref objectList);
					objectList.Add(gameObject);
				}
			}
		}

		private void UpdateLogoFade()
		{
			this.m_LogoTimer -= Time.deltaTime;
			if (this.m_LogoTimer <= 0f)
			{
				this.m_LogoTimer = 0f;
				switch (this.m_LogoState)
				{
				case LogoState.FadeIn:
					if (GTSystemOrder.CanGoIntoGame())
					{
						this.m_LogoState = LogoState.In;
						this.m_LogoTimer = 1.7f;
					}
					break;
				case LogoState.In:
					this.m_LogoState = LogoState.FadeOut;
					this.m_LogoTimer = 0.5f;
					break;
				case LogoState.FadeOut:
					this.m_LogoState = LogoState.Off;
					break;
				}
			}
			float logoAlpha = 1f;
			switch (this.m_LogoState)
			{
			case LogoState.FadeIn:
				logoAlpha = 1f - this.m_LogoTimer / 0.8f;
				break;
			case LogoState.In:
				logoAlpha = 1f;
				break;
			case LogoState.FadeOut:
				logoAlpha = this.m_LogoTimer / 0.5f;
				break;
			case LogoState.Off:
				logoAlpha = 0f;
				break;
			}
			this.SetLogoAlpha(logoAlpha);
		}

		private void UpdateBackgroundFade()
		{
			this.m_BackgroundTimer -= Time.deltaTime;
			BackgroundState backgroundState;
			if (this.m_BackgroundTimer <= 0f)
			{
				this.m_BackgroundTimer = 0f;
				backgroundState = this.m_BackgroundState;
				if (backgroundState != BackgroundState.In)
				{
					if (backgroundState == BackgroundState.FadeOut)
					{
						this.m_BackgroundState = BackgroundState.Off;
					}
				}
				else if (GTSystemOrder.CanGoIntoGame())
				{
					this.m_BackgroundState = BackgroundState.FadeOut;
					this.m_BackgroundTimer = 0.3f;
				}
			}
			float backgroundScreenTint = 1f;
			backgroundState = this.m_BackgroundState;
			if (backgroundState != BackgroundState.FadeOut)
			{
				if (backgroundState == BackgroundState.Off)
				{
					backgroundScreenTint = 0f;
				}
			}
			else
			{
				backgroundScreenTint = this.m_BackgroundTimer / 0.3f;
			}
			this.SetBackgroundScreenTint(backgroundScreenTint);
		}

		protected override void Update()
		{
			this.UpdateLogoFade();
			this.UpdateBackgroundFade();
            base.Update();
            //Debug.Log(m_LogoState + "   " + m_BackgroundState);
            if (this.m_LogoState == LogoState.Off && this.m_BackgroundState == BackgroundState.Off)
            {
	            try {
		            FindObjectOfType<SplashLoadingOverlay>().Close();
	            }
	            catch {}

	            if (PlayerProfileWeb.RecoveredProfileExists())
                {
                    enabled = false;
                    return;
                }

                if (!PlayerProfileManager.Instance.ActiveProfile.HasCompletedSecondTutorialRace())
                {
//                    Debug.Log("System is ready : " + GTSystemOrder.SystemsReady);
                    if (GTSystemOrder.SystemsReady)
                    {
//                        Debug.Log("Starting tutorial");
                        //MenuAudio.Instance.stopMusicPlaying();
                        PopupDataButtonActionExtensions.PopupDataButtonActionType.NEW_PLAYER.Execute();
                    }
                }
                else
                {
//                    Debug.Log("Go to home");
                    enabled = false;
                    ScreenManager.Instance.PushScreen(ScreenID.Home);
                }
			}
		}

		private void SetLogoAlpha(float zAlpha)
		{
            //Transform transform = this.Logo.GetGameObject().transform;
            //for (int num = 0; num != transform.childCount; num++)
            //{
            //    GameObject gameObject = transform.GetChild(num).gameObject;
            //    if (gameObject.activeSelf && gameObject.GetComponent(typeof(Renderer)))
            //    {
            //        //gameObject.renderer.material.SetColor("_Tint", new Color(1f, 1f, 1f, zAlpha));
            //    }
            //}
		}

		private void SetBackgroundScreenTint(float tint)
		{
			foreach (GameObject current in this.m_BackgroundGameObjectList)
			{
                //current.renderer.material.SetColor("_Tint", new Color(tint, tint, tint, 1f));
			}
		}

	    public bool ShouldShowLanguageMessage()
		{
			bool result = false;
            //if (/*LocalisationManager.LowMemoryOverride != LocalisationManager.ISO6391.NONE &&*/ !PlayerProfileManager.Instance.ActiveProfile.HasSeenLowMemoryLanguageMessage)
            //{
            //    PopUp popup = new PopUp
            //    {
            //        ConfirmAction = new PopUpButtonAction(this.LanguageMessageOk),
            //        ConfirmText = "TEXT_BUTTON_OK",
            //        ID = PopUpID.LowMemoryLanguageMessage,
            //        Graphic = PopUpManager.Instance.GetLanguageNotSupportedTexture()
            //    };
            //    PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
            //    result = true;
            //}

            return result;
		}

		private void LanguageMessageOk()
		{
			PlayerProfileManager.Instance.ActiveProfile.HasSeenLowMemoryLanguageMessage = true;
			PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
            ScreenManager.Instance.PushScreen(ScreenID.Home);
            //ScreenManager.Instance.PushScreen(ScreenID.Home);
		}
	}
}
