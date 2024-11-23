using UnityEngine;
using UnityEngine.UI;

public class Tacho : MonoBehaviour
{
	public float maximumRevs;
    public float redlineRevs;

    public float currentRevs
    {
        set
        {
            if (m_gaugeProgress == null)
            {
                m_gaugeProgress = GetComponentInChildren<GaugeProgressBar>();
            }

            if (m_gaugeProgress != null)
                m_gaugeProgress.value = value;
        }
    }
    [SerializeField]
    private GaugeProgressBar m_gaugeProgress;
    private Image m_redlineImage;
    private int m_maxGaugeRevs;

    [SerializeField] private Transform m_gaugeParent;
    public float minimumGoodRevs;
    public float minimumPerfectRevs;
    public float maximumPerfectRevs;
    public bool showGreenLine;
    public float maximumGoodRevs;
    public float minimumBadRevs;
    public float maximumBadRevs;

    public void Reset()
	{
		this.currentRevs = 0f;
		this.CreateTachoNotchesAndNumbers();
		this.CreateTachoRedline();
	}

	private void CreateTachoNotchesAndNumbers()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/HUDElements/GuageLarge");
        GameObject original2 = (GameObject)Resources.Load("Prefabs/HUDElements/GuageSmall");

        m_maxGaugeRevs = maximumRevs > 8000 ? 10000 : 8000;
	    var gameObject2 = Instantiate(maximumRevs > 8000 ? original : original2);
        gameObject2.transform.SetParent(m_gaugeParent, false);
        gameObject2.transform.rectTransform().anchoredPosition = new Vector3(0, 83, 0);
        gameObject2.transform.SetAsFirstSibling();
	    m_gaugeProgress = gameObject2.GetComponent<GaugeProgressBar>();
        m_redlineImage = gameObject2.transform.Find("Redline").GetComponent<Image>();

	}

	private void CreateTachoRedline()
	{
        m_redlineImage.fillAmount = (m_maxGaugeRevs - maximumRevs + 500) / m_maxGaugeRevs;
	}
}
