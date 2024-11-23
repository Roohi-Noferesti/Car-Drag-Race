using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Z2HInitialisation : MonoBehaviour
{
	public bool Complete;

	public bool okToGoBig;

	private List<Z2HIDInfo> initialisers;

	public static GameObject persistence;

	private bool initialising;

	public static Z2HInitialisation _instance;

	public static float timeScale
	{
		get;
		private set;
	}

	public static float fixedDeltaTime
	{
		get;
		private set;
	}

	public static Z2HInitialisation Instance
	{
		get
		{
			if (_instance == null)
			{
				persistence = new GameObject("Persistence");
				DontDestroyOnLoad(persistence);
                _instance = persistence.AddComponent<Z2HInitialisation>();
			}
			return _instance;
		}
	}

	private void OnApplicationFocus(bool isFocussed)
	{
		BasePlatform.ActivePlatform.CallOnApplicationFocus(isFocussed);
	}

	private void OnApplicationQuit()
	{
		BasePlatform.ActivePlatform.CallOnApplicationQuit();
	}

	public bool Init()
	{
		return this.Init(false);
	}

	public bool Init(bool asyncronous)
    {
#if GT_DEBUG_LOGGING
        LoadDebugger();
#endif

		if (this.initialising)
		{
			return false;
		}
		this.initialising = true;
		this.Complete = false;
		fixedDeltaTime = Time.fixedDeltaTime;
		timeScale = Time.timeScale;
		BasePlatform.ActivePlatform.InitialisationPreProcess();
		this.initialisers = Z2HInitialisers.initialisers;
		if (asyncronous)
		{
			this.RunInitialisationAsync();
		}
		else
		{
			this.RunInitialisation();
		}
		return true;
	}

#if GT_DEBUG_LOGGING
    private void LoadDebugger()
    {
        var configs = Resources.Load<GTLogConfiguration>("GTLogConfiguration");
        GTDebug.Initialise(configs);
    }
#endif

	private void RunInitialisation()
	{
        foreach (Z2HIDInfo current in this.initialisers)
		{
			using (MemoryScope.Start(current.name, "Z2HInitialisation"))
			{
				using (TimingScope.Start(current.name, "Z2HInitialisation"))
				{
					current.Process();
				}
			}
		}
	}

	private void RunInitialisationAsync()
	{
		base.StartCoroutine(this.RunInitAsyncCoroutine());
	}

    //[DebuggerHidden]
	private IEnumerator RunInitAsyncCoroutine()
	{
//		Debug.Log("number of initializers : "+this.initialisers.Count);
        foreach (Z2HIDInfo current in this.initialisers)
        {
//	        Debug.Log("initializing : "+current.name);
            using (MemoryScope.Start(current.name, "Z2HInitialisation"))
            {
                using (TimingScope.Start(current.name, "Z2HInitialisation"))
                {
                    current.Process();
                    yield return 0;
                }
            }
        }
	}
}
