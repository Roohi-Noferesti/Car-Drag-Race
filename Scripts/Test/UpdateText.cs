using UnityEngine;
using System.Collections;
using I2.Loc;
using TMPro;

public class UpdateText : MonoBehaviour
{

    private bool m_active;
    private TextMeshProUGUI text;
	// Use this for initialization
	IEnumerator Start ()
	{
	    LocalizationManager.CurrentLanguage = "Persian";
	    text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = 625.ToString();
	    while (true)
	    {
	        yield return new WaitForSeconds(Random.Range(0.5f, 1));
	        m_active = !m_active;
            //text.gameObject.SetActive(m_active);
	    }
	}

    private bool m_open;
    [SerializeField] private Animator m_animator;
	
	// Update is called once per frame
	void Update ()
	{
        //text.text = 125.ToString();

	    if (Input.GetMouseButtonDown(0))
	    {
	        m_open = !m_open;
            m_animator.Play(m_open?"open":"close");
	    }
	}
}
