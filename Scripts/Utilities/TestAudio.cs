using System.Text;
using Fabric;
using LitJson;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    [SerializeField] private string eventName;
    private bool m_isPlaying;

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetMouseButtonDown(0))
        {
            //if (!m_isPlaying)
            //{
            //    Fabric.SoundPack.Engine.Start(eventName);
            //}
            //else
            //{
            //    Fabric.SoundPack.Engine.Stop(eventName);
            //}
            //m_isPlaying = !m_isPlaying;
            EventManager.Instance.PostEvent(eventName, EventAction.PlaySound, null, null);
        }

        //if (m_isPlaying)
        //{
        //    var rpm = Input.GetAxis("Vertical");
        //    var load = Input.GetAxis("Horizontal");
        //    Fabric.SoundPack.Engine.SetRPM(eventName, rpm * 6500);
        //    Fabric.SoundPack.Engine.SetLoad(eventName, load);
        //    Debug.Log(m_isPlaying + "   " + rpm + "   " + load);
        //}
	}
}