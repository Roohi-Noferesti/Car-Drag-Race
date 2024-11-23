using TMPro;
using UnityEngine;

public class GearDisplay : MonoBehaviour
{
	private TextMeshProUGUI gearText;

	public int lastGear;

	public int currentGear;

	public void Reset()
	{
		this.currentGear = 0;
		this.lastGear = -1;
        this.gearText = base.gameObject.GetComponent<TextMeshProUGUI>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.currentGear != this.lastGear)
		{
			this.lastGear = this.currentGear;
			this.gearText.text = string.Format("{0:0}", this.currentGear);
		}
	}
}
