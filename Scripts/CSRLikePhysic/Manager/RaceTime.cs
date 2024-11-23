using TMPro;
using UnityEngine;

public class RaceTime : MonoBehaviour
{
	public float currentTime;

	private float lastTime;

	private TextMeshProUGUI raceText;

	public void Reset()
	{
		this.currentTime = 0f;
		this.lastTime = 0f;
		this.raceText = base.gameObject.GetComponent<TextMeshProUGUI>();
		this.raceText.text = string.Format("{0:0.00}", this.currentTime);
	}

	private void Update()
	{
		if (this.currentTime != this.lastTime)
		{
			this.lastTime = this.currentTime;
			this.raceText.text = string.Format("{0:0.00}", this.currentTime);
		}
	}
}
