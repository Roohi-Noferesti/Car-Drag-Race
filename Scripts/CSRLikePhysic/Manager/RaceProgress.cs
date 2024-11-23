using UnityEngine;
using UnityEngine.UI;

public class RaceProgress : MonoBehaviour
{
	public float raceDistance;

	public float humanCarDistance;

	public float opponentCarDistance;

    [SerializeField]
    private Slider m_humanProgressbar;
    [SerializeField]
    private Slider m_opponentProgressbar;

	public void Reset()
	{
		//float num = 1f;
		this.DisplayOpponentProgess(false);
		this.UpdateProgressAll();
	}

	public void DisplayOpponentProgess(bool zState)
	{
	    m_opponentProgressbar.gameObject.SetActive(zState);
	}

	private void Update()
	{
		this.UpdateProgressAll();
	}

	private void UpdateProgressAll()
	{
		float zProgress = Mathf.Clamp(this.humanCarDistance / this.raceDistance, 0f, 1f);
		float zProgress2 = Mathf.Clamp(this.opponentCarDistance / this.raceDistance, 0f, 1f);
        m_humanProgressbar.value = zProgress;
        m_opponentProgressbar.value = zProgress2;
	}
}
