using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Advertisements;

/*
public class TestUnityAds : MonoBehaviour
{
    [SerializeField] private UnityAdProvider m_provider;
    [SerializeField] private Text m_text;
    void Start()
    {
        //m_provider.AdFinished += M_provider_AdFinished;
        //m_provider.Init();
        Advertisement.debugMode = true;
        Advertisement.Initialize("2903709",true);
    }


    private IEnumerator _showVideo(string adunit)
    {
        Debug.Log("start showing video");
        if (!Advertisement.IsReady(adunit))
        {
            Debug.Log("Advertisement is not ready");
            yield break;
        }

        Debug.Log("Advertisement is ready");
        var ad = Advertisement.GetPlacementState(adunit);

        if (ad == PlacementState.Ready)
        {
            Debug.Log("video is ready");
            Advertisement.Show(adunit);
            //Show(adunit);
        }
        else
        {
            Debug.Log("video is :" + ad);
        }

    }

    private void M_provider_AdFinished(IAdProvider sender, AdFinishedEventArgs e)
    {
        var text = "Video finished : " + e;
        Debug.Log(text);
        m_text.text = text;
    }

    public void Show(string adunit)
    {
        StartCoroutine(_showVideo(adunit));
        //Advertisement.Show(adunit);
        //var state = m_provider.PrecachingStatus("videoForGold");
        //if (state == AdPrecachingState.Precached)
        //{
        //    m_provider.Show("videoForGold");
        //}
        //else
        //{
        //    m_text.text = "Video not available now : "+ state;
        //}
    }
}
*/
