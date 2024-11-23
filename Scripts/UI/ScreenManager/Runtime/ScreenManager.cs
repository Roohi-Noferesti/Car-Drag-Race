using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KingKodeStudio
{
    public class ScreenManager : MonoBehaviour
    {
        public enum State
        {
            IDLE,
            SCREEN_CHANGE_UPDATE,
            SETTING_UP_NEW_SCREEN,
            SHUTTING_DOWN_CURRENT_SCREEN,
            STARTING_UP_NEW_SCREEN,
            WAIT,
            SCREEN_FADE_IN,
            SCREEN_FADE_OUT
        }

        private readonly List<ScreenID> noneFadeScreenID = new List<ScreenID>()
        {
            ScreenID.Dummy,
            ScreenID.PinsFlags,
            ScreenID.Customise,
            ScreenID.NativeBanner,
            ScreenID.Invalid,
            ScreenID.Pause
        };

        public delegate void ScreenChangedEventHandler(ScreenID zNewScreenID);

        public static ScreenManager Instance { get; private set; }

        private readonly Stack<ScreenID> m_screenStack = new Stack<ScreenID>();
        private HUDScreen m_activeScreen;
        public State CurrentState { get; private set; }
        private bool m_lastInstructionWasPop;
        private bool m_lastInstructionUseGameObject;
        private HUDScreen m_instructionScreenGameObject;
        private bool m_shuttingDown;
        private bool m_startingUp;
        public static event ScreenChangedEventHandler ScreenChanged;
        public static event Action<State> ScreenStateChanged;

        private EventSystem m_eventSystem;
        private bool m_waitForFadeIn;
        private bool m_waitForFadeOut;
        private bool m_useFade;
        private ScreenID m_lastScreenID;

        public bool Interactable
        {
            get
            {
                if (m_eventSystem == null)
                {
                    m_eventSystem = EventSystem.current;
                }
                return m_eventSystem.enabled;
            }
            set
            {
                if (m_eventSystem == null)
                {
                    m_eventSystem = EventSystem.current;
                }
                m_eventSystem.enabled = value;

            }
        }

        public HUDScreen ActiveScreen
        {
            get { return m_activeScreen; }
        }

        public bool Wait { get; set; }

        public ScreenID InitialScreen
        {
            get { return ScreenID.Splash; }
        }

        public bool IsTranslating
        {
            get { return CurrentState != State.IDLE; }
        }

        public int Count
        {
            get { return m_screenStack.Count; }
        }

        public ScreenID CurrentScreen
        {
            get
            {
                return (!(ActiveScreen == null)) ? ActiveScreen.ID : ScreenID.Invalid;
            }
        }

        public ScreenID LastScreen
        {
            get { return m_lastScreenID; }
        }

        public bool CurrentScreenAlreadyOnStack { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        public ScreenID NominalNextScreen()
        {
            if (m_lastInstructionUseGameObject)
            {
                return m_instructionScreenGameObject.ID;
            }
            else
            {
                if (m_screenStack.Count <= 0)
                {
                    return InitialScreen;

                }
                else
                {
                    return m_screenStack.Peek();
                }
            }
        }

        private void ScreenStackHasChanged(bool isPop = false, bool fromGameObject = false,
            HUDScreen screenObject = null)
        {
            m_lastInstructionWasPop = isPop;
            m_lastInstructionUseGameObject = (fromGameObject && screenObject != null);
            if (m_lastInstructionUseGameObject)
            {
                m_instructionScreenGameObject = screenObject;
            }
            else
            {
                m_instructionScreenGameObject = null;
            }

            //Debug.Log("stack changed : " + CurrentState + "   " + NominalNextScreen());
            if (CurrentState !=State.STARTING_UP_NEW_SCREEN
                && CurrentState!=State.SHUTTING_DOWN_CURRENT_SCREEN)
            {
                CurrentState = State.SCREEN_CHANGE_UPDATE;
                m_shuttingDown = false;
                m_startingUp = false;
            }
            //Debug.Log("stack changed : state is now" + m_currentState);
        }

        public void PushScreen(ScreenID screenID)
        {
            m_screenStack.Push(screenID);
            ScreenStackHasChanged(false, false, null);
        }

        public void PopScreen()
        {
            if (m_screenStack.Count > 0)
            {
                m_screenStack.Pop();
                ScreenStackHasChanged(true, false, null);
            }
        }

        protected IEnumerator _waitForAnimatorFinishedPlaying(Animator anim, string animName)
        {
            var endStateReached = false;
            while (!endStateReached)
            {
                if (!anim.IsInTransition(0))
                {
                    var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                    endStateReached =
                        stateInfo.IsName(animName)
                        && stateInfo.normalizedTime >= 1;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public virtual void PopToScreen(ScreenID zID)
        {
            if (zID == m_screenStack.Peek())
            {
                return;
            }
            if (!m_screenStack.Contains(zID))
            {
                return;
            }
            while (m_screenStack.Count > 1 && m_screenStack.Peek() != zID)
            {
                m_screenStack.Pop();
            }
            ScreenStackHasChanged(true, false, null);
        }

        public bool IsScreenOnStack(ScreenID screenID)
        {
            return m_screenStack.Contains(screenID);
        }

        private void Update()
        {
            //Debug.Log(CurrentState + "   " + m_shuttingDown + "   " + NominalNextScreen());
            switch (CurrentState)
            {
                case State.SCREEN_CHANGE_UPDATE:
                    CurrentState = State.SHUTTING_DOWN_CURRENT_SCREEN;
                    //Debug.Log(m_currentState);
                    break;
                case State.SHUTTING_DOWN_CURRENT_SCREEN:
                    if (m_activeScreen != null)
                    {
                        if (!m_shuttingDown)
                        {
                            if (m_activeScreen.Wait(false))
                            {
                                return;
                            }
                            m_activeScreen.OnBeforeDeactivate();
                            m_activeScreen.CurrentState = HUDScreen.VisibleState.Hide;
                            m_shuttingDown = true;
                            OnScreenStateChanged(State.SHUTTING_DOWN_CURRENT_SCREEN);
                        }
                        else if (m_activeScreen.CurrentState == HUDScreen.VisibleState.Hide)
                        {
                            ShutdownCurrentScreen();
                        }
                    }
                    else
                    {
                        m_useFade = false;
                        var nextScreenID = m_lastInstructionWasPop ? m_lastScreenID : NominalNextScreen();
                        if (IsScreenValidForFade(nextScreenID))
                        {
                            CurrentState = State.SCREEN_FADE_IN;
                            m_waitForFadeIn = false;
                            //Debug.Log(nextScreenID + "   is valid for fade,Going to fade");
                        }
                        else
                        {
                            CurrentState = State.SETTING_UP_NEW_SCREEN;
                            //Debug.Log(nextScreenID + "   is not valid for fade,goint to Settting up");
                        }
                    }
                    break;
                case State.SCREEN_FADE_IN:
                    if (!m_waitForFadeIn)
                    {
                        m_waitForFadeIn = true;
                        m_useFade = true;
                        if (LoadingScreenManager.ScreenFadeQude != null &&
                            LoadingScreenManager.ScreenFadeQude.FadeState != FadeQuadLoad.FadeState.fadeToBlack)
                        {
                            //Debug.Log("wait to true");
                            LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 1), .3F, () =>
                            {
                                //Debug.Log("wait to false");
                                CurrentState = State.SETTING_UP_NEW_SCREEN;
                                //Debug.Log(m_currentState);
                            });
                        }
                    }
                    break;
                case State.SETTING_UP_NEW_SCREEN:
                    CurrentState = State.WAIT;
                    
                    if (m_lastInstructionUseGameObject)
                        m_activeScreen = m_instructionScreenGameObject;
                    else
                        m_activeScreen = CreateScreenGO(NominalNextScreen());

                    m_activeScreen.CurrentState = HUDScreen.VisibleState.Hide;
                    SetupCurrentScreen(m_lastInstructionWasPop);
                    InvokeScreenChangedEvent(CurrentScreen);

                    //Debug.Log(m_currentState + "   is setting up");
                    break;
                case State.WAIT:
                    if (m_activeScreen.Wait(true))
                    {
                        return;
                    }

                    //Debug.Log("resume " + m_activeScreen.ID);

                    if (m_useFade)
                    {
                        CurrentState = State.SCREEN_FADE_OUT;
                        //Debug.Log(m_activeScreen.ID + " is goint to use fade");
                        m_waitForFadeOut = false;
                    }
                    else
                    {
                        CurrentState = State.STARTING_UP_NEW_SCREEN;
                        m_activeScreen.CurrentState = HUDScreen.VisibleState.Visible;
                        m_activeScreen.OnActivate(m_lastInstructionWasPop);
                        //Debug.Log("opening  " + m_activeScreen.name + "   at " + Time.time);
                    }

                    break;
                case State.SCREEN_FADE_OUT:
                    if (!m_waitForFadeOut)
                    {
                        m_waitForFadeOut = true;
                        m_activeScreen.CurrentState = HUDScreen.VisibleState.Visible;
                        m_activeScreen.OnActivate(m_lastInstructionWasPop);
                        LoadingScreenManager.ScreenFadeQude.FadeTo(new Color(0, 0, 0, 0), .3F, () =>
                        {
                            CurrentState = State.STARTING_UP_NEW_SCREEN;
                            //Debug.Log(m_activeScreen.ID + "   " + m_currentState);

                        });
                    }
                    break;
                case State.STARTING_UP_NEW_SCREEN:
                    if (!m_startingUp)
                    {
                        m_startingUp = true;
                        //m_activeScreen.CurrentState = HUDScreen.VisibleState.Visible;
                        OnScreenStateChanged(State.STARTING_UP_NEW_SCREEN);
                    }
                    else if (m_activeScreen.CurrentState == HUDScreen.VisibleState.Visible)
                    {
                        if (m_activeScreen.ID == NominalNextScreen())
                        {
                            CurrentState = State.IDLE;
                        }
                        else
                        {
                            GTDebug.Log(GTLogChannel.Screens,"state changed 2 for screen : "+ NominalNextScreen());
                            CurrentState = State.SCREEN_CHANGE_UPDATE;
                            m_shuttingDown = false;
                            m_startingUp = false;
                        }
                        //Debug.Log( m_currentState);
                        m_activeScreen.OnAfterActivate();
                        var zShow = false;
                        if (m_activeScreen != null)
                        {
                            zShow = ActiveScreen.ShowCarStats;
                        }
                        if (CarInfoUI.Instance != null)
                        {
                            CarInfoUI.Instance.ShowCarStats(zShow);
                        }
                        if (m_activeScreen is ZHUDScreen)
                        {
                            (m_activeScreen as ZHUDScreen).TriggerScreenActivateEvent();
                        }
                    }
                    break;
            }

            //if (m_activeScreen != null && m_activeScreen.ID != NominalNextScreen())
            //{
            //    ScreenStackHasChanged(m_lastInstructionWasPop, m_lastInstructionUseGameObject,
            //        m_instructionScreenGameObject);
            //}

        }

        

        protected virtual void SetupCurrentScreen(bool zAlreadyOnStack)
        {
            if (m_activeScreen == null)
            {
            }
            var dummyControls =
                m_activeScreen.gameObject.GetComponentsInChildren<BaseDummyControl>();
            for (var i = 0; i < dummyControls.Length; i++)
            {
                dummyControls[i].ForceAwake();
            }
            var runtTimeControls =
                m_activeScreen.gameObject.GetComponentsInChildren<BaseRuntimeControl>();
            for (var j = 0; j < runtTimeControls.Length; j++)
            {
                runtTimeControls[j].OnActivate();
            }
            if (m_activeScreen != null)
            {
                BackgroundManager.ShowBackground(m_activeScreen.ScreenBackground);
            }

            TutorialBubblesManager.Instance.OnScreenChanged();
            m_activeScreen.OnCreated(zAlreadyOnStack);
            if(CarDatabase.Instance.isReady)
                GameDatabase.Instance.CarePackages.ScheduleSuitableCarePackage();
        }

        public HUDScreen CreateScreenGO(ScreenID zID)
        {
            var screenPrefab = GetScreenPrefab(zID);
            var instance = Instantiate(screenPrefab, gameObject.transform, false);
            instance.name = zID + "Screen";
            return instance.GetComponent<HUDScreen>();
        }

        private GameObject GetScreenPrefab(ScreenID zID)
        {
            var path = "Prefabs/UIScreens/" + zID + "Screen";
            var prefab = (GameObject)Resources.Load(path);
            if (gameObject == null)
            {
            }
            return prefab;
        }

        private void ShutdownCurrentScreen()
        {
            if (CarInfoUI.Instance != null)
            {
                CarInfoUI.Instance.ShowCarStats(false);
            }
            if (m_activeScreen != null)
            {
                m_activeScreen.OnDeactivate();
                var componentsInChildren =
                    m_activeScreen.gameObject.GetComponentsInChildren<BaseRuntimeControl>(true);
                for (var i = 0; i < componentsInChildren.Length; i++)
                {
                    componentsInChildren[i].OnDeactivate();
                }
                var iD = m_lastScreenID = m_activeScreen.ID;
                UICacheManager.Instance.ReleaseAutoItems();
                DestroyImmediate(m_activeScreen.gameObject);
                m_activeScreen = null;
                CleanDownManager.Instance.OnScreenPopped(iD);
            }
        }

        public void PushScreenWithFakedHistory(ScreenID zID, ScreenID[] screenIds)
        {
            for (var i = 0; i < screenIds.Length; i++)
            {
                var t = screenIds[i];
                m_screenStack.Push(t);
            }
            m_screenStack.Push(zID);
            ScreenStackHasChanged(false, false, null);
        }

        public void PushScreenWithFakedHistory(ScreenID underScreen)
        {
            PushScreenWithFakedHistory(underScreen, new ScreenID[0]);
        }

        private void InvokeScreenChangedEvent(ScreenID zNewScreenID)
        {
            if (PlayerProfileManager.Instance != null)
            {
                PlayerProfileManager.Instance.OnScreenChanged(zNewScreenID);
            }
            if (ScreenChanged != null)
            {
                ScreenChanged(zNewScreenID);
            }
        }

        private static void OnScreenStateChanged(State state)
        {
            var handler = ScreenStateChanged;
            if (handler != null) handler(state);
        }

        public void UpdateImmediately()
        {
            ShutdownCurrentScreen();
            m_activeScreen = CreateScreenGO(NominalNextScreen());
            SetupCurrentScreen(m_lastInstructionWasPop);
            InvokeScreenChangedEvent(CurrentScreen);
            CurrentState = State.STARTING_UP_NEW_SCREEN;
            m_activeScreen.CurrentState = HUDScreen.VisibleState.Visible;
            m_activeScreen.OnActivate(m_lastInstructionWasPop);
            //We may need to update all state here, I mean in one frame so ignore fade in /fade out
        }


        public bool IsScreenValidForFade(ScreenID zID)
        {
            return !noneFadeScreenID.Contains(zID);
        }

        public virtual void PushSingletonScreen(ScreenID zID)
        {
            FlushInstancesOf(zID);
            m_screenStack.Push(zID);
            ScreenStackHasChanged(false, false, null);
        }


        public void FlushInstancesOf(ScreenID zID)
        {
            if (!m_screenStack.Contains(zID))
            {
                return;
            }
            var stack = new Stack<ScreenID>();
            if (m_screenStack.Peek() == zID)
            {
            }
            for (var i = m_screenStack.Count; i > 0; i--)
            {
                var screenID = m_screenStack.Pop();
                if (screenID != zID)
                {
                    stack.Push(screenID);
                }
            }
            for (var j = stack.Count; j > 0; j--)
            {
                m_screenStack.Push(stack.Pop());
            }
        }

        public virtual void SwapScreen(ScreenID zID)
        {
            PopScreen();
            PushScreen(zID);
        }

        public string CurrentStackString()
        {
            var text = string.Empty;
            foreach (var current in m_screenStack)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text += ":";
                }
                text += current.ToString();
            }
            return text;
        }

        public void RefreshTopScreen()
        {
            var iD = m_activeScreen.ID;
            PopScreen();
            ScreenStackHasChanged(true, false, null);
            PushScreen(iD);
        }

        public void PopToOrPushScreen(ScreenID zID)
        {
            if (IsScreenOnStack(zID))
            {
                PopToScreen(zID);
            }
            else
            {
                PushScreen(zID);
            }
        }

        public void ClearAlreadyOnStackFlag()
        {
            CurrentScreenAlreadyOnStack = false;
        }

        
        public virtual void PushScreenFromGameObject(HUDScreen zScreen, ScreenID zID)
        {
            m_screenStack.Push(zID);
            ScreenStackHasChanged(false, true, zScreen);
        }
    }
}