using UnityEngine;
using UnityEngine.UI;

public class ShiftPaddleAnimator : MonoBehaviour
{
	public Image PaddleL;

    public Image PaddleR;

    //public Transform PaddleRArrow;

	public Button PaddleLButton;

    public Button PaddleRButton;

	public float FadeTime;

	public float FadeAlpha;

	private float TimerL;

	private float TimerR;

	private bool PlayingL;

	private bool PlayingR;

    private Color PaddleColor;

    private float PaddleAlpha;

    private Color m_initialiPaddleColor;

	private bool IsFadeAppropriate
	{
	    get
	    {
	        return RaceEventInfo.Instance.CurrentEvent ==
	               GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.Tutorial;
	    }
	}

	public bool IsGearUpActive
	{
		get
		{
			return !this.IsFadeAppropriate || this.PaddleRButton.enabled;
		}
	}

	public bool IsGearDownActive
	{
		get
		{
			return !this.IsFadeAppropriate || this.PaddleLButton.interactable;
		}
	}

	public void ActivateGearDown()
	{
		this.PlayingL = true;
	}

	public void ActivateGearUp()
	{
		this.PlayingR = true;
	}

	private void Start()
	{
        m_initialiPaddleColor = PaddleR.color;
        ReFade();
	}

    public void ReFade()
    {
        TimerL = 0;
        TimerR = 0;
        PlayingL = false;
        PlayingL = false;
        PaddleColor = m_initialiPaddleColor;
        PaddleAlpha = m_initialiPaddleColor.a;
        if (this.IsFadeAppropriate)
        {
            this.PaddleLButton.interactable = false;
            this.PaddleRButton.interactable = false;
        }
        else
        {
            base.gameObject.SetActive(false);
        }
    }

	private void Update()
	{
		if (this.IsFadeAppropriate)
		{
			if (this.PlayingL)
			{
				this.TimerL = Mathf.Min(this.TimerL + Time.deltaTime / this.FadeTime, 1f);
				if (this.TimerL == 1f)
				{
                    this.PaddleLButton.interactable = true;
					this.PlayingL = false;
				}
			}
			if (this.PlayingR)
			{
				this.TimerR = Mathf.Min(this.TimerR + Time.deltaTime / this.FadeTime, 1f);
				if (this.TimerR == 1f)
				{
                    this.PaddleRButton.interactable = true;
					this.PlayingR = false;
				}
			}
			Color color = PaddleColor;
			color.a = Mathf.Lerp(this.FadeAlpha, PaddleAlpha, this.TimerL);
			if (this.PaddleL.color != color)
			{
				this.PaddleL.color= color;
			}
            color.a = Mathf.Lerp(this.FadeAlpha, PaddleAlpha, this.TimerR);
            if (this.PaddleR.color != color)
			{
				this.PaddleR.color= color;
                //this.PaddleRArrow.GetComponent<Image>().color= white;
			}
		}
	}
}
