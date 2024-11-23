using System.Collections;
using UnityEngine;



// this class with type of monobehaviour is only created to interact with android plugin
public class FortumoAndroiEvents : MonoBehaviour
{
    public static FortumoAndroiEvents Instance { get; private set; }
    public StoreManagerFortumo fortumoReference;
    public bool AnswerReturned = false;
    public bool ShouldAnswer = false;
    void Awake()
    {
        Instance = this;
    }


    // this will get called by android plugin
    public void OnPurchaseResult(string result)
    {

        UnityEngine.Debug.Log("fortumo result is"+ result);
        // check exactly what message is returned from android
        if (result.ToLower().Contains("completed"))
        {
            UnityEngine.Debug.Log("fortumo condition satisfied");
            fortumoReference.SuccessFulResult = true;
        }
        else
        {
            fortumoReference.SuccessFulResult = false;
            UnityEngine.Debug.Log("fortumo condition not satisfied");

        }
        fortumoReference.HasResult = true;
        fortumoReference.IsProcessingTransaction = false;
        AnswerReturned = true;
        ShouldAnswer = false;
    }



#if UNITY_EDITOR

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        Debug.Log("return to game pressed");
        OnPurchaseResult("ddd");
    }
#endif


    private void OnApplicationPause(bool paused)
    {
        Debug.Log("Fortumo application pause:::>>"+paused);

        if (paused)
        {
        }

        else
        {
            if (ShouldAnswer)
            {
                StartCoroutine(WaiterResult());
            }
        }
        
    }


    IEnumerator WaiterResult()
    {
        yield return new WaitForSeconds(2);
        AnswerReturned = true;
        fortumoReference.HasResult = true;
        fortumoReference.SuccessFulResult = false;
        fortumoReference.IsProcessingTransaction = false;
    }

}
