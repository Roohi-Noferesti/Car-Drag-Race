using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace KingKodeStudio
{
    [Serializable]
    public class HUDScreen : MonoBehaviour
    {

        public enum VisibleState
        {
            None,
            Visible,
            Hide
        }

        protected Animator m_animator;
        private bool m_firstRun = true;
        [SerializeField] protected HudScreenVisibilityType visibilityType;
        [SerializeField] protected string m_openAnimation = "Open";
        [SerializeField] protected string m_closeAnimation = "Close";
        [SerializeField] protected BackgroundManager.BackgroundType m_screenBackground;
        public bool ShowCarStats;

        public HudScreenVisibilityType VisibilityType
        {
            get { return visibilityType; }
        }

        public string OpenAnimationName
        {
            get { return m_openAnimation; }
        }

        public string CloseAnimationName
        {
            get { return m_closeAnimation; }
        }

        protected virtual void Awake()
        {
            if (m_animator == null)
                m_animator =GetComponent<Animator>();
        }

        public void Open()
        {
            ScreenManager.Instance.PushScreen(this.ID);
        }

        public void Open(ScreenID screenID)
        {
            ScreenManager.Instance.PushScreen(screenID);
        }

        public virtual void Close()
        {
            ScreenManager.Instance.PopScreen();
        }

        public virtual VisibleState CurrentState
        {
            get
            {
                switch (VisibilityType)
                {
                    case HudScreenVisibilityType.Animation:
                        if (m_firstRun)
                        {
                            GTDebug.Log(GTLogChannel.Screens,"return visibile true for " + name+"   "+Time.time);
                            return gameObject.activeSelf?VisibleState.Visible : VisibleState.Hide;
                        }
                        if (Animator != null && !Animator.isActiveAndEnabled)
                        {
                            return VisibleState.Hide;
                        }

                        //var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
                        //if (stateInfo.IsName(OpenAnimationName) && stateInfo.normalizedTime>=.98)
                        //    return true;
                        //return false;
                        //var isClose = (Animator != null &&
                        //              Animator.GetCurrentAnimatorStateInfo(0).IsName(CloseAnimationName));
                        //return Animator != null && !isClose;
                        if (Animator != null && Animator.GetCurrentAnimatorStateInfo(0).IsName(OpenAnimationName)
                            && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .99F)
                        {
                            return VisibleState.Visible;
                        }
                        else if (Animator != null && Animator.GetCurrentAnimatorStateInfo(0).IsName(CloseAnimationName)
                                 && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .99F)
                        {
                            return VisibleState.Hide;
                        }
                        else
                        {
                            return VisibleState.None;
                        }
                    case HudScreenVisibilityType.SetActiveObject:
                        return gameObject.activeSelf?VisibleState.Visible : VisibleState.Hide;
                }
                return VisibleState.None;
            }
            set
            {
                switch (VisibilityType)
                {
                    case HudScreenVisibilityType.SetActiveObject:
                        //We want to events be fired so we should delay deactivating object
                        if (value == VisibleState.Hide && !m_firstRun)
                        {
                            if (gameObject.activeSelf)
                            {
                                StartCoroutine(_delayedDeactivateObject());
                            }
                        }
                        else
                        {
                            gameObject.SetActive(value == VisibleState.Visible);
                        }

                        break;
                    case HudScreenVisibilityType.Animation:
                        //Debug.Log("set visibility "+name+"  "+m_firstRun+"   "+value);
                        if (m_firstRun && value == VisibleState.Hide)
                        {
                            if (gameObject.activeSelf)
                                gameObject.SetActive(false);
                        }
                        else
                        {
                            if (!gameObject.activeSelf)
                            {
                                gameObject.SetActive(true);
                            }
                            if (Animator != null)
                            {
                                if (value == VisibleState.Visible)
                                {
                                    if (!string.IsNullOrEmpty(OpenAnimationName))
                                        Animator.Play(OpenAnimationName);
                                    else
                                    {
                                        gameObject.SetActive(true);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(CloseAnimationName))
                                        Animator.Play(CloseAnimationName);
                                    else
                                    {
                                        gameObject.SetActive(false);
                                    }
                                }
                            }
                        }
                        m_firstRun = false;
                        break;
                }
            }
        }



        public int SiblingIndex
        {
            get { return transform.GetSiblingIndex(); }
        }

        public Animator Animator
        {
            get
            {
                if (m_animator == null)
                    m_animator = GetComponent<Animator>();
                return m_animator;
            }
        }

        public BackgroundManager.BackgroundType ScreenBackground
        {
            get { return m_screenBackground; }
        }

        public virtual ScreenID ID
        {
            get
            {
                return ScreenID.Invalid;
            }
        }

        public enum HudScreenScreenType
        {
            Normal = 0,
            Dialog = 1
        }

        public enum HudScreenVisibilityType
        {
            SetActiveObject = 0,
            Animation = 1
        }

        public void BringToFront()
        {
            transform.SetAsFirstSibling();
        }

        public void BringToBack()
        {
            transform.SetAsLastSibling();
        }

        private IEnumerator _delayedDeactivateObject()
        {
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
        }

        public void ForceHide()
        {
            if (gameObject.activeSelf)
                StartCoroutine(_delayedDeactivateObject());
        }

        public void ForceHideImmediately()
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        public void reset()
        {
            m_firstRun = true;
            GTDebug.Log(GTLogChannel.Screens,name + "   m_firstRun set to true " + Time.time);
        }

        protected virtual void OnDestroy()
        {

        }

        public virtual bool Wait(bool startingUp)
        {
            return false;
        }

        public virtual void OnCreated(bool zAlreadyOnStack)
        {

        }

        public virtual void OnActivate(bool zAlreadyOnStack)
        {
            MetricsIntegration.Instance.LogCrash("Screen Changed To: " + ID.ToString());
            if (CheatEngine.Instance != null) {
                CheatEngine.Instance.ScreenChangedTo(ID.ToString());
            }
        }

        public virtual void OnBeforeDeactivate()
        {

        }

        public virtual void OnDeactivate()
        {

        }

        public struct ScreenEventArgs
        {
            public bool Wait { get; set; }
        }

        protected virtual void Update()
        {
            if (!PopUpManager.Instance.isShowingPopUp)
            {
                if (InputManager.BackButtonPressed())
                {
                    if (!this.IgnoreHardwareBackButton() && ScreenManager.Instance.CurrentScreen != ScreenID.Splash)
                    {
                        this.OnHardwareBackButton();
                    }
                }
                else if (InputManager.EnterPressed())
                {
                    this.OnEnterPressed();
                }
            }
        }

        public virtual bool IgnoreHardwareBackButton()
        {
            return false;
        }

        public virtual void OnHardwareBackButton()
        {
            if (CommonUI.Instance.IsShowingBackButton || ScreenManager.Instance.CurrentScreen == ScreenID.Home)
            {
                MenuAudio.Instance.playSound(AudioSfx.MenuClickBack);
                if (ScreenManager.Instance.CurrentScreen == ScreenID.Home)
                {
                    this.QuitAppPopup();
                }
                else if (CommonUI.Instance.IsShowingBackButton)
                {
                    this.RequestBackup();
                }
            }
        }

        protected void QuitAppPopup()
        {
            PopUp popup = new PopUp
            {
                Title = "TEXT_BUTTON_QUIT",
                BodyText = "TEXT_POPUPS_ARE_YOU_SURE",
                ConfirmAction = Application.Quit,//AndroidSpecific.Quit,
                ConfirmText = "TEXT_BUTTON_YES",
                CancelText = "TEXT_BUTTON_NO",
                ID = PopUpID.QuitApp
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
        }

        public virtual void RequestBackup()
        {
            ScreenManager.Instance.PopScreen();
        }

        // BaseScreen
        protected virtual void OnEnterPressed()
        {

        }

        public virtual void OnAfterActivate()
        {
            
        }

        public virtual bool HasBackButton()
        {
            return true;
        }
    }

}
