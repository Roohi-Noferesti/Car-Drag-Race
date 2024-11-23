using System.Collections;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GTSystemOrderAsync : MonoBehaviour
{
	public static GTSystemOrderAsync _instance;

	public static GTSystemOrderAsync Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("SystemOrderAsync");
				Z2HInitialisers.Persist(gameObject);
				gameObject.AddComponent<GTSystemOrderAsync>();
				_instance = gameObject.GetComponent<GTSystemOrderAsync>();
			}
			return _instance;
		}
	}

	public void TriggerAsynchActions()
	{
		StartCoroutine(RunStartUpAsynchronousGameSystems());
	}

    private IEnumerator RunStartUpAsynchronousGameSystems()
    {
	   // Debug.Log("RunStartUpAsynchronousGameSystems");
        int launch = -1;

        //Debug.Log("number of actions : " + GTSystemOrder.AsynchActions.Count);

        foreach (var asynchAction in GTSystemOrder.AsynchActions)
        {
            while (!Z2HInitialisation.Instance.okToGoBig && ((GTSystemOrder.CountAsynchSystemsReady() + 2) > launch))
            {
                yield return new WaitForEndOfFrame();
            }

            while (AssetDatabaseClient.Instance.IsCorrupted)
            {
                yield return new WaitForEndOfFrame();
            }
            asynchAction();
            launch++;
            //Debug.Log("async action number : "+launch);
        }

        //Debug.Log("checking system ready");

        while (!GTSystemOrder.AreAsyncGameSystemsReady())
        {
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("System is ready");

        if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Race)
        {
            while (ScreenManager.Instance == null ||
                   (ScreenManager.Instance.CurrentScreen != ScreenID.Splash && ScreenManager.Instance.CurrentScreen != ScreenID.Language))
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //Debug.Log("CompleteGameSystemStartup ");
        GTSystemOrder.CompleteGameSystemStartup();
    }
}
