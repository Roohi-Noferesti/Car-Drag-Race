using System;
using System.Collections;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
	public enum ePriority
	{
		Default,
		Objective,
		System,
		SystemUrgent
	}

	private class ScheduledPopUp
	{
		public PopUp PopUp
		{
			get;
			private set;
		}

		public HashSet<ScreenID> ValidScreens
		{
			get;
			private set;
		}

		public ScheduledPopUp(PopUp pop, HashSet<ScreenID> validScreens)
		{
			this.PopUp = pop;
			this.ValidScreens = validScreens;
		}
	}

	public string PopUpCrewLeaderPrefabFilename;

	public string PopUpPrefabFilename;

    public string PopUpPrefabVeryBigFilename;

	public string PopUpPrefabSmallFilename;

	public string PopupPrefabSmallImageFilename;

	public string PopUpPrefabImageFilename;

	public string PopUpPrefabSocialFilename;

    public string PopUpPrefabConnectionFileName;

	public string PopUpPrefabRewardCardFilename;

	public string PopUpPrefabMultiplayerBillboard;

	public string PopUpPrefabDynamicImage;

	public string PopUpPrefabWaitSpinner;

	public string PopUpPrefabLegal;

	public string PopUpPrefabStarterPack;

	public string PopUpPrefabMiniStore;

	public string PopUpPrefabVerifyAge;

	public string PopUpPrefabPrivacyPolicy;

	public string PopUpPrefabIAPBundle;

	public string PopUpPrefabSpotPrize;

	public string PopupPrefabMultipleChoice;

	public string PopupPrefabRestore;

	public string PopupUserRate;

	private GameObject ourPopUpObject;

	private PopUp popup;

	private ePriority currentPriority;

	//private bool OldGestureValue = true;

	private int OldGestureValueRefCount;

	private Queue<ScheduledPopUp> m_PopUpQueue = new Queue<ScheduledPopUp>();

	public bool isShowingPopUp;

	public string graphics_raceOfficialPrefab;

	public string graphics_mechanicPrefab;

    public string graphics_agentPrefab;

	public string graphics_postmanPrefab;

	public string graphics_stargazerPrefab;

	public string graphics_bloggerPrefab;

	public string graphics_crewLeaderTier1;

	public string graphics_crewLeaderTier2;

	public string graphics_crewLeaderTier3;

	public string graphics_crewLeaderTier4;

	public string graphics_crewLeaderTier5;

	public string graphics_greenLightGas;

	public string graphics_greenLightShift;

	public bool WaitingToShowPopup;

	private bool WaitingForGraphicLoad;

    private bool WaitingForItemGraphicLoad;

	private string GraphicBundleToLoad = string.Empty;

	private Action OnPopupShown;
    private bool m_carInfoUIWasVisible;

    public static PopUpManager Instance
	{
		get;
		private set;
	}

	public ePriority CurrentPriority
	{
		get
		{
			return this.currentPriority;
		}
	}

	public PopUpDialogue PopUpScreenInstance
	{
		get
		{
			if (this.ourPopUpObject == null)
			{
				return null;
			}
			return this.ourPopUpObject.GetComponent<PopUpDialogue>();
		}
	}

	public PopUp GetCurrentPopUp()
	{
		return this.popup;
	}

	private void Start()
	{
		if (Instance != null)
		{
		}
		Instance = this;

	    if (UICamera.Instance != null)
	    {
	        GetComponentInChildren<Canvas>().worldCamera = UICamera.Instance.Camera;
	    }
	}

	private bool TryShowScheduledPopUp(ScheduledPopUp sPop)
	{
        //ScreenID iD = ScreenManager.Instance.ActiveScreen.ID;
		return (sPop.ValidScreens == null /*|| sPop.ValidScreens.Contains(iD)*/) && this.GetCurrentPopUp() == null && this.TryShowPopUp(sPop.PopUp, ePriority.Default, null);
	}

	public void SchedulePopUp(PopUp pop, HashSet<ScreenID> validScreens)
	{
		ScheduledPopUp scheduledPopUp = new ScheduledPopUp(pop, validScreens);
		if (!this.TryShowScheduledPopUp(scheduledPopUp))
		{
			this.m_PopUpQueue.Enqueue(scheduledPopUp);
		}
	}

	private void UpdatePopUpQueue()
	{
		if (this.m_PopUpQueue.Count > 0)
		{
			ScheduledPopUp sPop = this.m_PopUpQueue.Peek();
			if (this.TryShowScheduledPopUp(sPop))
			{
				this.m_PopUpQueue.Dequeue();
			}
		}
	}

	public bool CurrentPopUpMatchesID(PopUpID id)
	{
		if (this.PopUpScreenInstance == null)
		{
			return false;
		}
		PopUpID popUpID = this.PopUpScreenInstance.GetPopUpID();
		return popUpID == id;
	}

	private GameObject GetPrefab(string zFilename)
	{
		string text = "PopUpPrefabs/" + zFilename;
		GameObject result = (GameObject)Resources.Load(text);
		if (text == null)
		{
		}
		return result;
	}

	public void InitiateKillPopup()
	{
		if (this.popup == null)
		{
			return;
		}

        if (this.PopUpScreenInstance.HasAnimator)
	    {
	        StartCoroutine(KillPopupWhenAnimationEnd(PopUpScreenInstance));
	    }
		else if (this.popup.DelayKillPopupByFrames <= 0)
		{
			this.KillPopUp();
		}
		else
		{
            base.StartCoroutine("WaitToKillPopup");
		}

	}

	private IEnumerator WaitToKillPopup()
	{
	    var frame = 0;
        while (frame < popup.DelayKillPopupByFrames)
	    {
	        yield return new WaitForEndOfFrame();
	        frame++;
	    }
        this.KillPopUp();
	}

    public bool KillPopUp()
    {
        //if (ScreenManager.Instance.ActiveScreen is CSRCarouselScreen)
        //{
        //    CSRCarouselScreen cSRCarouselScreen = ScreenManager.Instance.ActiveScreen as CSRCarouselScreen;
        //    foreach (BaseList current in cSRCarouselScreen.CarouselLists)
        //    {
        //        current.CurrentState = BaseRuntimeControl.State.Active;
        //    }
        //}
        if (m_carInfoUIWasVisible)
        {
            //CarInfoUI.Instance.ShowCarStats(true);
        }
        if (this.popup != null && this.popup.KillAction != null)
        {
            this.popup.KillAction();
        }
        if (this.PopUpScreenInstance != null)
        {
            this.PopUpScreenInstance.preKill();
        }

        if (this.ourPopUpObject != null)
        {
            Destroy(this.ourPopUpObject);
        }
        if (this.isShowingPopUp)
        {
            this.OldGestureValueRefCount--;
            if (this.OldGestureValueRefCount < 0)
            {
                this.OldGestureValueRefCount = 0;
            }
            if (this.OldGestureValueRefCount == 0)
            {
                //TouchManager.Instance.GesturesEnabled = this.OldGestureValue;
            }
        }
        this.ourPopUpObject = null;
        this.popup = null;
        this.isShowingPopUp = false;
        //if (BaseDevice.ActiveDevice.HasHardwareBackButton())
        //{
        //    if (CommonUI.Instance.NavBar.WasShowingBackButton)
        //    {
        //        CommonUI.Instance.NavBar.ShowBackButton();
        //    }
        //}
        //else if (CommonUI.Instance.NavBar.IsShowingBackButton)
        //{
        //    CommonUI.Instance.NavBar.ShowBackButton();
        //}
        return true;
    }

    private IEnumerator KillPopupWhenAnimationEnd(PopUpDialogue popUpDialogue)
    {
        var animator = popUpDialogue.GetComponent<Animator>();
        animator.Play("Close");
        var endStateReached = false;
        while (!endStateReached)
        {
            if (animator == null)
            {
                break;
            }
            if (!animator.IsInTransition(0))
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                endStateReached =
                    stateInfo.IsName("Close")
                    && stateInfo.normalizedTime >= 1;
            }
            yield return 0;
        }
        this.KillPopUp();
    }

	public void DefaultButtonResponse()
	{
		this.InitiateKillPopup();
	}

	public Texture2D GetAdvisorTexture(string zFilename)
	{
		string path = "CharacterCards/" + zFilename;
		Texture2D texture2D = (Texture2D)Resources.Load(path);
		if (texture2D == null)
		{
		}
		return texture2D;
	}

	public Texture2D GetLanguageNotSupportedTexture()
	{
	    return null;
	    //string path = "Misc/lowmem_" + LocalisationManager.ISO6391ToString(LocalisationManager.LowMemoryOverride).ToLower() + "_msg";
	    //Texture2D texture2D = Resources.Load(path) as Texture2D;
	    //if (texture2D == null)
	    //{
	    //}
	    //return texture2D;
	}

	public bool CanShowPopUp(PopUp pop)
	{
        if (ScreenManager.Instance.CurrentScreen == ScreenID.CareerModeMap)
        {
            CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
            if (careerModeMapScreen != null && careerModeMapScreen.GoToRaceAnimating)
            {
                return false;
            }
        }
        else if (ScreenManager.Instance.CurrentScreen == ScreenID.LevelUp
                 && !pop.IsSocial &&
                 pop.ID!=PopUpID.PreAdPromptPopup
                 && pop.ID!=PopUpID.AdResult
                 && pop.ID!=PopUpID.WaitSpinner)
        {
            return false;
        }
		return true;
	}

	private void Update()
	{
        if (this.WaitingToShowPopup && !this.WaitingForGraphicLoad && !this.WaitingForItemGraphicLoad && ScreenManager.Instance.CurrentState == ScreenManager.State.IDLE)
		{
			if (this.popup != null)
			{
				this.Show();
			}
			this.WaitingToShowPopup = false;
		}
		this.UpdatePopUpQueue();
	}

	public bool TryShowPopUp(PopUp popup, ePriority priority = ePriority.Default, Action onShown = null)
	{
		if (this.TryShowPopUpInternal(popup, priority))
        {
         //   ScreenManager.Instance.Interactable = true;
			this.OnPopupShown = onShown;
            if (!string.IsNullOrEmpty(popup.GraphicPath))
            {
                this.WaitingForGraphicLoad = true;
                this.GraphicBundleToLoad = popup.GraphicPath;
                TexturePack.RequestTextureFromBundle(popup.GraphicPath, delegate(Texture2D tex)
                {
                    if (popup.GraphicPath == this.GraphicBundleToLoad)
                    {
                        popup.Graphic = tex;
                        this.WaitingForGraphicLoad = false;
                    }
                });
            }
            else
            {
                this.WaitingForGraphicLoad = false;
            }

            if (!string.IsNullOrEmpty(popup.ItemGraphicPath))
            {
                this.WaitingForItemGraphicLoad = true;
                this.GraphicBundleToLoad = popup.ItemGraphicPath;
                TexturePack.RequestTextureFromBundle(popup.ItemGraphicPath, delegate (Texture2D tex)
                {
                    if (popup.ItemGraphicPath == this.GraphicBundleToLoad)
                    {
                        popup.ItemGraphic = tex;
                        this.WaitingForItemGraphicLoad = false;
                    }
                });
            }
            else
            {
                this.WaitingForItemGraphicLoad = false;
            }
            Debug.Log(popup.BodyText);
			return true;
			
		}
		return false;
	}

	public void ForcePopup(PopUp pop, ePriority priority = ePriority.SystemUrgent, Action onShown = null)
	{
		if (this.popup != null)
		{
			this.KillPopUp();
		}
		base.StopCoroutine("WaitToKillPopup");
		this.OnPopupShown = onShown;
		this.popup = pop;
		this.currentPriority = priority;
		this.Show();
	}

	private bool TryShowPopUpInternal(PopUp pop, ePriority zPriority)
	{
		if (!this.CanShowPopUp(pop))
		{
			return false;
		}
		if (this.WaitingToShowPopup && zPriority <= this.currentPriority)
		{
			return false;
		}
		if (this.popup != null)
		{
			if (zPriority <= this.currentPriority)
			{
				return false;
			}
			this.KillPopUp();
		}
		base.StopCoroutine("WaitToKillPopup");
		this.popup = pop;
		this.currentPriority = zPriority;
		this.WaitingToShowPopup = true;
		return true;
	}

	private void Show()
	{
        //if (CommonUI.Instance.NavBar.IsShowingBackButton)
        //{
        //    CommonUI.Instance.NavBar.DisableBackButton();
        //}
		if (this.popup.IsWaitSpinner)
		{
			this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabWaitSpinner));
		}
		else if (this.popup.IsDynamicImage)
		{
			this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabDynamicImage));
		}
		else if (this.popup.Graphic == null && string.IsNullOrEmpty(this.popup.ItemGraphicPath))
		{
		    if (this.popup.IsConnection)
		    {
                this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabConnectionFileName));
		    }
			else if (this.popup.IsSocial)
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabSocialFilename));
			}
			else if (this.popup.IsBig)
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabFilename));
			}
            else if (this.popup.IsVeryBig)
            {
                this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabVeryBigFilename));
            }
			else if (this.popup.IsCard)
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabRewardCardFilename));
			}
			else if (this.popup.IsLegal)
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabLegal));
			}
			else if (this.popup.IsProfileRestore)
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopupPrefabRestore));
			}
			else if (this.popup.IsStarterPack)
			{
                this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabStarterPack));
                StarterPackPopupController component = this.ourPopUpObject.GetComponent<StarterPackPopupController>();
                component.StarterPackItem1 = this.popup.StarterPackItem1;
                component.StarterPackItem2 = this.popup.StarterPackItem2;
                component.StarterPackOfferItem = this.popup.StarterPackOfferItem;
			}
			else if (this.popup.IsMiniStore)
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabMiniStore));
			}
			else if (this.popup.IsIAPBundle)
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabIAPBundle));
			}
			else if (this.popup.IsSpotPrize)
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabSpotPrize));
			}
            else if (this.popup.IsAgeVerification)
            {
                this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabVerifyAge));
            }
            else if (this.popup.IsPrivacyPolicy)
            {
                this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabPrivacyPolicy));
            }
		    else if (popup.IsRatePopup)
		    {
			    ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopupUserRate));
		    }
            else
			{
				this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpPrefabSmallFilename));
			}
		}
		else if (this.popup.IsCrewLeader)
		{
			this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopUpCrewLeaderPrefabFilename));
		}
		else if (this.popup.ID == PopUpID.LowMemoryLanguageMessage)
		{
			this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(this.PopupPrefabSmallImageFilename));
		}
		else
		{
		    string zFilename = this.PopUpPrefabImageFilename;//(this.popup.Buttons != null) ? this.PopupPrefabMultipleChoice : this.PopUpPrefabImageFilename;
			this.ourPopUpObject = (GameObject)Instantiate(this.GetPrefab(zFilename));
		}
		PopUpDialogue popupDialog = this.ourPopUpObject.GetComponent<PopUpDialogue>();
        popupDialog.transform.localPosition = Vector3.zero;
	    popupDialog.transform.SetParent(transform,false);;//base.gameObject.transform;
		popupDialog.Setup(this.popup);
		if (this.popup.profiles != null)
		{
            (popupDialog as PopUpRestore).descriptionLeft.Setup(this.popup.profiles[0]);
            (popupDialog as PopUpRestore).descriptionRight.Setup(this.popup.profiles[1]);
		}
        if (this.popup.Buttons != null)
        {
            PopUpMultipleChoice component3 = this.ourPopUpObject.GetComponent<PopUpMultipleChoice>();
            if (component3 != null)
            {
                component3.Setup(this.popup.Buttons);
            }
        }
        //GameObjectHelper.MakeLocalPositionPixelPerfect(component2.transform);
        //Transform[] componentsInChildren = component2.gameObject.GetComponentsInChildren<Transform>();
        //Transform[] array = componentsInChildren;
        //for (int i = 0; i < array.Length; i++)
        //{
        //    Transform zTrans = array[i];
        //    GameObjectHelper.MakeLocalPositionPixelPerfect(zTrans);
        //}
        //if (ScreenManager.Instance.ActiveScreen is CSRCarouselScreen)
        //{
        //    CSRCarouselScreen cSRCarouselScreen = ScreenManager.Instance.ActiveScreen as CSRCarouselScreen;
        //    foreach (BaseList current in cSRCarouselScreen.CarouselLists)
        //    {
        //        current.CurrentState = BaseRuntimeControl.State.Disabled;
        //    }
        //}
        //if (this.OldGestureValueRefCount == 0)
        //{
        //    this.OldGestureValue = TouchManager.Instance.GesturesEnabled;
        //    TouchManager.Instance.GesturesEnabled = false;
        //}
		this.OldGestureValueRefCount++;
		this.isShowingPopUp = true;
		if (this.OnPopupShown != null)
		{
			this.OnPopupShown();
			this.OnPopupShown = null;
		}

	    m_carInfoUIWasVisible = CarInfoUI.Instance.isVisible;
	    //CarInfoUI.Instance.ShowCarStats(false);
	}
}
