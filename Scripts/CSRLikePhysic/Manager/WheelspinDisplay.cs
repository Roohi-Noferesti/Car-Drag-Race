using UnityEngine;
using UnityEngine.UI;

public class WheelspinDisplay : MonoBehaviour
{
	public bool isWheelSpinning;

	public float flickerSpeed;

	private bool flickering;

	private float flickerTimer;

	private bool currentWheelspinState;

	private bool lastWheelspinState;

	private Image WheelspinSprite;

    private Color wheelSpinColor;

	public void Reset()
	{
		this.flickering = false;
		this.currentWheelspinState = false;
		this.lastWheelspinState = false;
		this.WheelspinSprite = base.GetComponent<Image>();
        //this.WheelspinSprite.PlayAnim(0, 0);
        //this.WheelspinSprite.PauseAnim();
	}

	private void Start()
	{
	    wheelSpinColor = WheelspinSprite.color;
	}

	private void Update()
	{
		if (PauseGame.isGamePaused)
		{
			return;
		}
		if (this.isWheelSpinning && !this.flickering)
		{
			this.flickering = true;
		}
		if (this.flickering)
		{
			this.flickerTimer += Time.deltaTime;
			if (this.flickerTimer > this.flickerSpeed)
			{
				this.flickerTimer = 0f;
				this.currentWheelspinState = !this.currentWheelspinState;
			}
			if (!this.isWheelSpinning)
			{
				this.flickering = false;
				this.currentWheelspinState = false;
			}
		}
		if (this.currentWheelspinState != this.lastWheelspinState)
		{
			this.lastWheelspinState = this.currentWheelspinState;
			if (this.currentWheelspinState)
			{
			    this.WheelspinSprite.color = new Color(wheelSpinColor.r, wheelSpinColor.g, wheelSpinColor.b, 1);
			    //this.WheelspinSprite.PlayAnim(0, 1);
			    //this.WheelspinSprite.PauseAnim();
			}
			else
			{
                this.WheelspinSprite.color = new Color(wheelSpinColor.r, wheelSpinColor.g, wheelSpinColor.b, 0.28f);
                //this.WheelspinSprite.PlayAnim(0, 0);
                //this.WheelspinSprite.PauseAnim();
			}
		}
	}
}
