using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TestScreenNameSpace
{
    public abstract class ScreenManager : MonoBehaviour
    {
        public enum State
        {
            IDLE,
            SCREEN_CHANGE_LATE_UPDATE,
            SCREEN_CHANGE_UPDATE,
            PREPARING_SCREEN
        }

        public delegate void ScreenChangedEventHandler(ScreenID zNewScreenID);

        private State _currentState;

        private bool LastInstructionWasPop;

        private bool LastInstructionUseGameObject;

        private BaseScreen InstructionScreenGameObject;

        private Stack<ScreenID> screenStack = new Stack<ScreenID>();

        private BaseScreen _activeScreen;

        public event ScreenChangedEventHandler ScreenChanged;

        public State CurrentState
        {
            get
            {
                return this._currentState;
            }
        }

        public ScreenID InitialScreen
        {
            get
            {
                return ScreenID.Splash;
            }
        }

        public ScreenID CurrentScreen
        {
            get
            {
                return (!(this.ActiveScreen == null)) ? this.ActiveScreen.ID : ScreenID.Invalid;
            }
        }

        public int NumScreensOnStack
        {
            get
            {
                return this.screenStack.Count;
            }
        }

        public BaseScreen ActiveScreen
        {
            get
            {
                return this._activeScreen;
            }
        }

        protected abstract GameObject GetScreenPrefab(ScreenID zID);

        protected abstract bool UseLateUpdate(ScreenID zID);

        private void InvokeScreenChangedEvent(ScreenID zNewScreenID)
        {
            if (PlayerProfileManager.Instance != null)
            {
                PlayerProfileManager.Instance.OnScreenChanged(zNewScreenID);
            }
            if (this.ScreenChanged != null)
            {
                this.ScreenChanged(zNewScreenID);
            }
        }

        protected virtual void Awake()
        {
            this.DestroyAllScreensInScene();
        }

        protected virtual void OnDestroy()
        {
        }

        public void RescaleUI()
        {
            //ResolutionManager.RecalculatePixelDensity();
            //GUICamera.Instance.SetCameraZoom(1f);
            //if (this.CurrentScreen != ScreenID.CareerModeMap)
            //{
            //    MapScreenCache.Unload();
            //    MapScreenCache.Load();
            //}
            //EZScreenPlacement[] array = UnityEngine.Object.FindObjectsOfType(typeof(EZScreenPlacement)) as EZScreenPlacement[];
            //EZScreenPlacement[] array2 = array;
            //for (int i = 0; i < array2.Length; i++)
            //{
            //    EZScreenPlacement eZScreenPlacement = array2[i];
            //    eZScreenPlacement.SetCamera();
            //}
            //FadeQuad[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(FadeQuad)) as FadeQuad[];
            //FadeQuad[] array4 = array3;
            //for (int j = 0; j < array4.Length; j++)
            //{
            //    FadeQuad fadeQuad = array4[j];
            //    fadeQuad.ForceUpdateScale();
            //}
            //GameObject gameObject = GameObject.Find("GradientBackground");
            //if (gameObject != null)
            //{
            //    gameObject.GetComponent<MenuGradientBackground>().ForceSetupDimensions();
            //}
            //CommonUI.Instance.NavBar.CalculateSize();
            //GameObject gameObject2 = GameObject.Find("MenuBackground(Clone)");
            //if (gameObject2 != null)
            //{
            //    gameObject2.GetComponent<MenuBackground>().CalculateSize();
            //}
            //LoadingScreenManager.Instance.CalculateSize();
        }

        public void ScreenStackHasChanged(bool isPop = false, bool fromGameObject = false, BaseScreen ScreenObject = null)
        {
            this.LastInstructionWasPop = isPop;
            this.LastInstructionUseGameObject = (fromGameObject && ScreenObject != null);
            if (this.LastInstructionUseGameObject)
            {
                this.InstructionScreenGameObject = ScreenObject;
            }
            else
            {
                this.InstructionScreenGameObject = null;
            }
            if (this._currentState != State.PREPARING_SCREEN)
            {
                if (this.screenStack.Count == 0 || this.UseLateUpdate(this.screenStack.Peek()))
                {
                    this._currentState = State.SCREEN_CHANGE_LATE_UPDATE;
                }
                else
                {
                    this._currentState = State.SCREEN_CHANGE_UPDATE;
                }
            }
        }

        private void DestroyAllScreensInScene()
        {
            BaseScreen[] array = FindObjectsOfType(typeof(BaseScreen)) as BaseScreen[];
            BaseScreen[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                BaseScreen baseScreen = array2[i];
                DestroyImmediate(baseScreen.gameObject);
            }
        }

        protected virtual void Start()
        {
        }

        public BaseScreen CreateScreenGO(ScreenID zID)
        {
            GameObject screenPrefab = this.GetScreenPrefab(zID);
            GameObject gameObject = (GameObject)Instantiate(screenPrefab);
            gameObject.name = zID.ToString() + "Screen";
            gameObject.transform.parent = base.gameObject.transform;
            return gameObject.GetComponent<BaseScreen>();
        }

        public virtual void PushScreenWithFakedHistory(ScreenID zID, ScreenID[] zFakeHistory)
        {
            for (int i = 0; i < zFakeHistory.Length; i++)
            {
                ScreenID t = zFakeHistory[i];
                this.screenStack.Push(t);
            }
            this.screenStack.Push(zID);
            this.ScreenStackHasChanged(false, false, null);
        }

        public void RemoveScreenFromStack(ScreenID zID)
        {
            Stack<ScreenID> stack = new Stack<ScreenID>();
            if (zID == this.CurrentScreen)
            {
                return;
            }
            while (this.screenStack.Count > 0)
            {
                ScreenID screenID = this.screenStack.Pop();
                if (screenID != zID)
                {
                    stack.Push(screenID);
                }
            }
            while (stack.Count > 0)
            {
                this.screenStack.Push(stack.Pop());
            }
        }

        public void UpdateImmediately()
        {
            this.ChangeScreen();
        }

        private void SetupNewScreen(ScreenID zID)
        {
            if (this._activeScreen != null)
            {
                this.ShutdownCurrentScreen();
            }
            using (MemoryScope.Start(zID.ToString(), "Screens"))
            {
                if (this.LastInstructionUseGameObject)
                {
                    this.InstructionScreenGameObject.gameObject.SetActive(true);
                    this._activeScreen = this.InstructionScreenGameObject;
                }
                else
                {
                    this._activeScreen = this.CreateScreenGO(zID);
                }
                this.SetupCurrentScreen(this.LastInstructionWasPop);
                this._currentState = State.IDLE;
                this.InvokeScreenChangedEvent(this._activeScreen.ID);
            }
        }

        protected abstract bool PrepareForNewScreen(ScreenID zID, Action setupNewScreen);

        private void ChangeScreen()
        {
            ScreenID StackTopScreen = (this.screenStack.Count <= 0) ? this.InitialScreen : this.screenStack.Peek();
            if (!this.PrepareForNewScreen(StackTopScreen, delegate
            {
                this.SetupNewScreen(StackTopScreen);
            }))
            {
                this.SetupNewScreen(StackTopScreen);
            }
            else
            {
                this._currentState = State.PREPARING_SCREEN;
            }
        }

        public void Update()
        {
            if (this._currentState == State.SCREEN_CHANGE_UPDATE)
            {
                this.ChangeScreen();
            }
        }

        public void LateUpdate()
        {
            if (this._currentState == State.SCREEN_CHANGE_LATE_UPDATE)
            {
                this.ChangeScreen();
            }
        }

        public virtual void PushScreen(ScreenID zID)
        {
            this.screenStack.Push(zID);
            this.ScreenStackHasChanged(false, false, null);
        }

        public void FlushInstancesOf(ScreenID zID)
        {
            if (!this.screenStack.Contains(zID))
            {
                return;
            }
            Stack<ScreenID> stack = new Stack<ScreenID>();
            if (this.screenStack.Peek() == zID)
            {
            }
            for (int i = this.screenStack.Count; i > 0; i--)
            {
                ScreenID screenID = this.screenStack.Pop();
                if (screenID != zID)
                {
                    stack.Push(screenID);
                }
            }
            for (int j = stack.Count; j > 0; j--)
            {
                this.screenStack.Push(stack.Pop());
            }
        }

        public virtual void PushSingletonScreen(ScreenID zID)
        {
            this.FlushInstancesOf(zID);
            this.screenStack.Push(zID);
            this.ScreenStackHasChanged(false, false, null);
        }

        public virtual void PushScreenFromGameObject(BaseScreen zScreen, ScreenID zID)
        {
            this.screenStack.Push(zID);
            this.ScreenStackHasChanged(false, true, zScreen);
        }

        public virtual void SwapScreen(ScreenID zID)
        {
            this.PopScreen();
            this.PushScreen(zID);
        }

        public ScreenID PeekScreen()
        {
            return this.screenStack.Peek();
        }

        public bool IsScreenOnStack(ScreenID screen)
        {
            foreach (ScreenID current in this.screenStack)
            {
                if (current == screen)
                {
                    return true;
                }
            }
            return false;
        }

        public string CurrentStackString()
        {
            string text = string.Empty;
            foreach (ScreenID current in this.screenStack)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text += ":";
                }
                text += current.ToString();
            }
            return text;
        }

        public bool IsNominalPreviousScreen(ScreenID screen)
        {
            if (this.screenStack.Count < 2)
            {
                return false;
            }
            int num = 1;
            ScreenID screenID = this.screenStack.ToArray()[num];
            return screenID == screen;
        }

        public virtual void PopToScreen(ScreenID zID)
        {
            if (zID == this.screenStack.Peek())
            {
                return;
            }
            if (!this.screenStack.Contains(zID))
            {
                return;
            }
            while (this.screenStack.Count > 1 && this.screenStack.Peek() != zID)
            {
                this.screenStack.Pop();
            }
            this.ScreenStackHasChanged(true, false, null);
        }

        public void PopToOrPushScreen(ScreenID zID)
        {
            if (this.IsScreenOnStack(zID))
            {
                this.PopToScreen(zID);
            }
            else
            {
                this.PushScreen(zID);
            }
        }

        public virtual void PopScreen()
        {
            this.screenStack.Pop();
            this.ScreenStackHasChanged(true, false, null);
        }

        private void ShutdownCurrentScreen()
        {
            this._activeScreen.OnDeactivate();
            BaseRuntimeControl[] componentsInChildren = this._activeScreen.gameObject.GetComponentsInChildren<BaseRuntimeControl>(true);
            BaseRuntimeControl[] array = componentsInChildren;
            for (int i = 0; i < array.Length; i++)
            {
                BaseRuntimeControl baseRuntimeControl = array[i];
                baseRuntimeControl.OnDeactivate();
            }
            ScreenID iD = this._activeScreen.ID;
            UICacheManager.Instance.ReleaseAutoItems();
            DestroyImmediate(this._activeScreen.gameObject);
            this._activeScreen = null;
            CleanDownManager.Instance.OnScreenPopped(iD);
        }

        protected virtual void SetupCurrentScreen(bool zAlreadyOnStack)
        {
            if (this._activeScreen == null)
            {
            }
            BackgroundManager.ShowBackground(this._activeScreen.ScreenBackground);
            BaseDummyControl[] componentsInChildren = this._activeScreen.gameObject.GetComponentsInChildren<BaseDummyControl>();
            BaseDummyControl[] array = componentsInChildren;
            for (int i = 0; i < array.Length; i++)
            {
                BaseDummyControl baseDummyControl = array[i];
                baseDummyControl.ForceAwake();
            }
            BaseRuntimeControl[] componentsInChildren2 = this._activeScreen.gameObject.GetComponentsInChildren<BaseRuntimeControl>();
            BaseRuntimeControl[] array2 = componentsInChildren2;
            for (int j = 0; j < array2.Length; j++)
            {
                BaseRuntimeControl baseRuntimeControl = array2[j];
                baseRuntimeControl.OnActivate();
            }
            this._activeScreen.OnActivate(zAlreadyOnStack);
            //GameDatabase.Instance.CarePackages.ScheduleSuitableCarePackage();
        }

        public ScreenID NominalNextScreen()
        {
            if (this.LastInstructionUseGameObject)
            {
                return this.InstructionScreenGameObject.ID;
            }
            return (this.screenStack.Count <= 0) ? this.InitialScreen : this.screenStack.Peek();
        }

        public void RefreshTopScreen()
        {
            ScreenID iD = this._activeScreen.ID;
            this.PopScreen();
            this.ScreenStackHasChanged(true, false, null);
            this.PushScreen(iD);
        }
    }

}

