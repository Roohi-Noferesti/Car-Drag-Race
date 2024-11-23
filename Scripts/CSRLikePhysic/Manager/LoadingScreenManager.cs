using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class LoadingScreenManager : MonoSingleton<LoadingScreenManager>
{
    private Animator m_animator;
    private const string fadeinAnimation = "FadeToBlack";
    private const string fadeoutAnimation = "FadeOut";
    private const string cyclingAnimation = "Cycle";
	private GameObject screenBlackout;
    private Canvas m_canvas;
    public static ScreenFadeQude ScreenFadeQude { get; private set; }

	public Image LoadingScreenBackground;

	private VSDummy.eVSMode VSMode = VSDummy.eVSMode.None;

	public GameObject EventDialogPrefab;

	private GameObject eventDialog;

	public GameObject MultiplayerDialogPrefab;

	private GameObject multiplayerDialog;

	public bool IsFading { get; private set; }

	public bool IsShowingLoadingPanel
	{
		get;
		private set;
		
	}

    public bool IsShowingLoading { get; private set; }

    protected override void Awake()
    {
	    
        base.Awake();
        if (Instance == this)
        {
            ScreenFadeQude = GetComponentInChildren<ScreenFadeQude>();
        }
        m_canvas = GetComponentInChildren<Canvas>();
        m_animator = GetComponentInChildren<Animator>(true);

        ForceEndLoadingEffects();
        //if (LoadingScreenManager.Instance != null)
        //{
        //}
        //LoadingScreenManager.Instance = this;
        //List<GameObject> list = new List<GameObject>();
        //Image[] componentsInChildren = base.transform.GetComponentsInChildren<Image>(true);
        //Image[] array = componentsInChildren;
        //for (int i = 0; i < array.Length; i++)
        //{
        //    Image sprite = array[i];
        //    list.Add(sprite.gameObject);
        //}
        //this.screenBlackout = list.Find((GameObject q) => q.name == "LoadingScreenBlackout");
        //this.DisableScreenBlackout();
    }

    IEnumerator Start()
    {
        while (!GTSystemOrder.SystemsReady)
        {
            yield return 0;
        }
        m_canvas.worldCamera = UICamera.Instance.Camera;
    }

	private void Update()
	{
        //if (!this.screenBlackout.activeInHierarchy && (this.eventDialog == null || !this.eventDialog.activeInHierarchy) && (this.multiplayerDialog == null || !this.multiplayerDialog.activeInHierarchy))
        //{
        //    base.gameObject.SetActive(false);
        //}
	}

	public bool IsReadyToLoad()
	{
		if (this.multiplayerDialog == null)
		{
			return true;
		}
		if (this.VSMode == VSDummy.eVSMode.International)
		{
			InternationalVersusScreen component = this.multiplayerDialog.GetComponent<InternationalVersusScreen>();
			return component.IsReadyToLoad;
		}
        //MultiplayerVersusScreen component2 = this.multiplayerDialog.GetComponent<MultiplayerVersusScreen>();
	    return false;//component2.IsReadyToLoad;
	}

	private void StartLoadingSinglePlayerEffects()
	{
		bool flag = false;
		this.eventDialog = (GameObject)Instantiate(this.EventDialogPrefab);
	    this.eventDialog.transform.SetParent(m_canvas.transform);
        LoadingPanel loadingPanel = this.eventDialog.GetComponent<LoadingPanel>();
		if (SceneLoadManager.Instance.SceneToLoad == SceneLoadManager.Scene.Race)
		{
			RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
            //if (!RaceEventInfo.Instance.IsHighStakesEvent && !currentEvent.IsTutorial() && currentEvent != null)
            //{
            //    loadingPanel.SetUpForRaceEvent(currentEvent);
            //    flag = true;
            //}
		}
		if (!flag)
		{
            loadingPanel.SetUpForSmall("TEXT_LOADING");
		}
	}

	public void StartLoadingEffects()
	{
		if (!RaceEventInfo.Instance.ShouldCurrentEventUseMultiplayerLoadingScreen() || SceneLoadManager.Instance.SceneToLoad != SceneLoadManager.Scene.Race)
		{
			base.gameObject.SetActive(true);
		    m_canvas.gameObject.SetActive(true);
			this.StartLoadingSinglePlayerEffects();
            m_animator.Play(fadeinAnimation);
		    StartCoroutine(CheckAnimationEnd(cyclingAnimation, 0.5F));
		    //this.screenBlackout.GetComponent<FadeQuadLoad>().StartFade(0f, FadeQuadLoad.FadeState.fadeToBlack);
		}
	}

	public void StopLoadingEffects()
	{
		this.DestroyEventDialog();
		this.DestroyMultiplayerDialog();
        base.gameObject.SetActive(true);
        m_canvas.gameObject.SetActive(true);
        m_animator.Play(fadeoutAnimation);
        StartCoroutine(CheckAnimationEnd(fadeoutAnimation));
        //this.screenBlackout.GetComponent<FadeQuadLoad>().StartFade(1.5f, FadeQuadLoad.FadeState.fadeOut);
	}

	public void PreShowVSLoading(VSDummy.eVSMode mode, VSDummy dummyScreen)
	{
		this.VSMode = mode;
		base.gameObject.SetActive(true);
		Object original;
		if (this.VSMode == VSDummy.eVSMode.International)
		{
			original = Resources.Load("Screens/InternationalVersusScreen");
		}
		else
		{
			original = Resources.Load("Screens/MultiplayerVersusScreen");
		}
		this.multiplayerDialog = (Instantiate(original) as GameObject);
		this.multiplayerDialog.transform.parent = base.transform;
		BaseVersusScreen component = this.multiplayerDialog.GetComponent<BaseVersusScreen>();
		component.SetDummyScreen(dummyScreen);
		this.LoadingScreenBackground.gameObject.SetActive(false);
		this.screenBlackout.SetActive(false);
        //this.screenBlackout.GetComponent<FadeQuadLoad>().OpaquePattern();
	}

	public void SnapCloseVSScreen()
	{
		BubbleManager.Instance.DismissMessages();
		if (this.multiplayerDialog != null)
		{
			this.DestroyMultiplayerDialog();
			this.screenBlackout.SetActive(false);
			this.LoadingScreenBackground.gameObject.SetActive(false);
		}
	}

	public void ForceEndLoadingEffects()
	{
	    m_canvas.gameObject.SetActive(true);
	    m_animator.Play("FadeOut");
	    IsFading = false;
	    //this.screenBlackout.GetComponent<FadeQuadLoad>().ForceEndFade();
	}

	public void EnableScreenBlackout()
	{
		this.screenBlackout.SetActive(true);
	}

	public void DisableScreenBlackout()
	{
		this.screenBlackout.SetActive(false);
	}

	public void TriggerLoadingPanel(string text)
	{
		this.DestroyEventDialog();
		this.eventDialog = (GameObject)Instantiate(this.EventDialogPrefab);
	    this.eventDialog.transform.SetParent(base.gameObject.transform, false);
        LoadingPanel component = this.eventDialog.GetComponent<LoadingPanel>();
        component.SetUpForSmall(text);
		this.IsShowingLoadingPanel = true;
	}

	public void ClearLoadingPanel()
	{
		this.DestroyEventDialog();
		this.IsShowingLoadingPanel = false;
	}

	public void DestroyEventDialog()
	{
		if (this.eventDialog != null)
		{
			this.eventDialog.gameObject.SetActive(false);
			Destroy(this.eventDialog);
			this.eventDialog = null;
		}
	}

	public void DestroyMultiplayerDialog()
	{
		if (this.multiplayerDialog != null)
		{
			this.multiplayerDialog.SetActive(false);
			Destroy(this.multiplayerDialog);
		}
	}

    public static void Setactive(bool value)
    {
        if (Instance.m_canvas.gameObject.activeInHierarchy != value)
            Instance.m_canvas.gameObject.SetActive(value);

        if (Instance.m_animator.gameObject.activeInHierarchy != value)
            Instance.m_animator.gameObject.SetActive(value);
    }

    public static void Fadein(Action onCompleted=null)
    {
        Setactive(true);
        Instance.m_animator.Play(fadeinAnimation);
        Instance.StartCoroutine(Instance.CheckAnimationEnd(cyclingAnimation,0.5F, onCompleted));
    }

    public static void Fadeout(Action onCompleted=null)
    {
        Setactive(true);
        Instance.m_animator.Play(fadeoutAnimation);
        Instance.StartCoroutine(Instance.CheckAnimationEnd(fadeoutAnimation,1,onCompleted));
    }


    private IEnumerator CheckAnimationEnd(string animName,float time = 1,Action onCompleted=null)
    {
        IsFading = true;
        var endStateReached = false;
        while (!endStateReached)
        {
            if (m_animator.isActiveAndEnabled && !m_animator.IsInTransition(0))
            {
                var stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
                endStateReached =
                    stateInfo.IsName(animName)
                    && stateInfo.normalizedTime >= time;
            }

            yield return 0;
        }
        IsFading = false;
        if (onCompleted != null)
        {
            onCompleted.Invoke();
        }
    }

    protected override void Destroy()
    {
        Destroy(gameObject);
    }
}
