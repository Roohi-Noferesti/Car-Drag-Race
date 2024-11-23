using UnityEngine;

public class GearChangeLight : MonoBehaviour
{
	public bool turnOffPreviousLight;

	public bool isOn;

	public bool shouldFlash;

	public float flickerSpeed;

	private bool flickering;

	private float flickerTimer;

	private bool displayLight;

	public void Reset()
	{
		this.flickering = false;
		this.displayLight = false;
		this.flickerTimer = 0f;
		base.GetComponent<Renderer>().enabled = false;
	}

	private void Update()
	{
		if (PauseGame.isGamePaused)
		{
			return;
		}
		if (this.isOn)
		{
			if (this.shouldFlash)
			{
				if (!this.flickering)
				{
					this.flickering = true;
				}
			}
			else
			{
				this.displayLight = true;
				this.flickering = false;
				this.flickerTimer = 0f;
			}
			if (this.flickering)
			{
				this.flickerTimer += Time.deltaTime;
				if (this.flickerTimer > this.flickerSpeed)
				{
					this.flickerTimer = 0f;
					this.displayLight = !this.displayLight;
				}
			}
			if (this.displayLight && !base.GetComponent<Renderer>().enabled)
			{
				base.GetComponent<Renderer>().enabled = true;
			}
			else if (!this.displayLight && base.GetComponent<Renderer>().enabled)
			{
				base.GetComponent<Renderer>().enabled = false;
			}
		}
		else if (base.GetComponent<Renderer>().enabled)
		{
			base.GetComponent<Renderer>().enabled = false;
			this.flickering = false;
			this.displayLight = false;
		}
	}
}
