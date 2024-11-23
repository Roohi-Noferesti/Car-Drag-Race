using System.Collections;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
	public enum Scene
	{
        None,
		Frontend,
		Race,
		Loading,
	    Splash
	}

	private enum LoaderState
	{
		Inactive,
		LoadRequested,
		LoadStarted,
		LoadingIntermission,
		CleaningResources,
		LoadingLevel,
		WaitingForSceneToProgress
	}

	private LoaderState state;

	private Scene currentScene;
    private bool m_useLoadingScreen;

    public static SceneLoadManager Instance
	{
		get;
		private set;
	}

	public Scene SceneToLoad
	{
		get;
		private set;
	}

	public Scene LastScene
	{
		get;
		private set;
	}

	public Scene CurrentScene
	{
		get
		{
			if (this.state != LoaderState.Inactive)
			{
				return Scene.Loading;
			}
			return this.currentScene;
		}
		private set
		{
			this.currentScene = value;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	    var sceneName = SceneManager.GetActiveScene().name;
	    SceneToLoad = this.LastScene = (sceneName == "Frontend" || sceneName == "Splash") ? Scene.Frontend : Scene.None;
		this.IdentifyCurrentScene();
	}

	private void IdentifyCurrentScene()
	{
	    var activeScenename = SceneManager.GetActiveScene();
		if (activeScenename.name == "Frontend")
		{
			this.CurrentScene = Scene.Frontend;
		}
        else if (activeScenename.name == "Splash")
		{
			this.CurrentScene = Scene.Splash;
		}
        else if (activeScenename.name == "Race" ||
                 activeScenename.name == "RaceMedium" ||
                 activeScenename.name == "RaceLow" ||
                 activeScenename.name == "Race_QuarterMile" ||
                 activeScenename.name == "Race_HalfMile" ||
                 activeScenename.name == "Race_QuarterMile_Day" ||
                 activeScenename.name == "Race_HalfMile_Day" ||
                 activeScenename.name == "Race_QuarterMile_Day_Cloudy" ||
                 activeScenename.name == "Race_HalfMile_Day_Cloudy" ||
                 activeScenename.name == "Race_QuarterMile_Online_Day" ||
                 activeScenename.name == "Race_HalfMile_Online_Day")
        {
            this.CurrentScene = Scene.Race;
        }
	}

	public void RequestScene(Scene zSceneToLoad)
	{
		if (this.state != LoaderState.Inactive)
		{
			return;
		}
		this.LastScene = this.SceneToLoad;
		this.SceneToLoad = zSceneToLoad;
		this.state = LoaderState.LoadRequested;
		this.DebugOutput("Request scene");
	}

    public void LoadRequestedScene(bool useLoadingSCreen = true)
	{
        m_useLoadingScreen = useLoadingSCreen;
		if (this.state != LoaderState.LoadRequested)
		{
			return;
		}
		base.StartCoroutine(this.LoaderProcess());
		this.DebugOutput("Load requested scene");
	}

	public void LoadScene(Scene zSceneToLoad,bool useLoadingSCreen = true)
	{
		if (this.state != LoaderState.Inactive)
		{
			return;
		}
		this.RequestScene(zSceneToLoad);
        this.LoadRequestedScene(useLoadingSCreen);
	}

	private void DebugOutput(string zId)
	{
	}

    private IEnumerator LoaderProcess()
    {
        yield return StartCoroutine(StartLoadingScreen());

        if (m_useLoadingScreen)
        {
            while (!LoadingScreenManager.Instance.IsReadyToLoad())
            {
                yield return new WaitForFixedUpdate();
            }

            while (LoadingScreenManager.Instance.IsFading)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        yield return StartCoroutine(CleanResources());

        yield return StartCoroutine(LoadNextScene());

        if (state == LoaderState.LoadingLevel)
        {
            state = LoaderState.WaitingForSceneToProgress;
        }
    }

    public void CompleteLoadingScene()
	{
        if (m_useLoadingScreen)
            LoadingScreenManager.Instance.StopLoadingEffects();
		this.state = LoaderState.Inactive;

        //if (SceneToLoad == Scene.Race && ScreenManager.Instance.CurrentScreen != ScreenID.VSDummy)
        //{
        //    ScreenManager.Instance.PushScreen(ScreenID.Dummy);
        //}
	}

	private IEnumerator StartLoadingScreen()
	{
        state = LoaderState.LoadStarted;
	    if (m_useLoadingScreen)
	    {
	        LoadingScreenManager.Instance.StartLoadingEffects();
	        while (LoadingScreenManager.Instance.IsFading)
	        {
	            yield return 0;
	        }
	        yield return new WaitForEndOfFrame();
	    }
	}

    private IEnumerator LoadIntermission()
    {
        state = LoaderState.LoadingIntermission;
        yield return SceneManager.LoadSceneAsync("Intermission");
    }

    private IEnumerator CleanResources()
    {
        state = LoaderState.CleaningResources;
        yield return Resources.UnloadUnusedAssets();
    }

    private IEnumerator LoadNextScene()
    {
        CurrentScene = SceneToLoad;
        state = LoaderState.LoadingLevel;
        var sceneName = GetSceneName();
//        Debug.Log("Loading scene : "+sceneName);
        var async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
//	        Debug.Log("Loading percentage : "+async.progress);
            yield return 0;
        }
    }


    private string GetSceneName()
    {
	    string result;
	    if (this.SceneToLoad == Scene.Race)
	    {
		    var raceTrack = RaceEventInfo.Instance.GetRaceTrack();
		    if (raceTrack == RaceEventInfo.RaceTrack.Day)
		    {
			    result = RaceEventInfo.Instance.CurrentEvent.IsHalfMile ? "Race_HalfMile_Day" : "Race_QuarterMile_Day";
		    }
		    else if (raceTrack == RaceEventInfo.RaceTrack.CloudyDay)
		    {
			    result = RaceEventInfo.Instance.CurrentEvent.IsHalfMile ? "Race_HalfMile_Day_Cloudy" : "Race_QuarterMile_Day_Cloudy";
		    }
		    else if (raceTrack == RaceEventInfo.RaceTrack.Airport)
		    {
			    result = RaceEventInfo.Instance.CurrentEvent.IsHalfMile ? "Race_HalfMile_Online_Day" : "Race_QuarterMile_Online_Day";
		    }
		    else
		    {
			    result = RaceEventInfo.Instance.CurrentEvent.IsHalfMile ? "Race_HalfMile" : "Race_QuarterMile";
		    }
           
	    }
	    else
	    {
		    result = this.SceneToLoad.ToString();
	    }
	    return result;
	}
}
