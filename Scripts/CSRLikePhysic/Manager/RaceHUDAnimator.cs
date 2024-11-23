using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RaceHUDAnimator : MonoBehaviour
{
    [SerializeField] private Animator m_hudAnimator;

	public GameObject nitrousButton;

	public GameObject recordButton;

	public GameObject throttleButton;

	public GameObject gearUpButton;

	public GameObject gearDownButton;

	public Button gearDownButtonUI;

	public GameObject gearChangeDevice;

	public GameObject wheelspinDevice;

	public ShiftPaddleAnimator ShiftPaddleAnims;

    public GameObject GaugeRoot;

	private bool incomingHUD;

	private bool outgoingHUD;

	private bool HUDIsIn;

    private Canvas m_canvas;

    void Start()
    {
        m_canvas = GetComponentInParent<Canvas>();
    }

	public void Reset()
	{
		this.incomingHUD = false;
		this.outgoingHUD = false;
		this.HUDIsIn = false;
		this.ResetHUD(true);
	}

	public void IntroduceHUD()
	{
		this.incomingHUD = true;
	    m_hudAnimator.Play("In");
        StartCoroutine(_checkAnimationEnd("In"));
	}

	public bool HUDHasBeenIntroduced()
	{
		return this.incomingHUD || this.HUDIsIn || this.outgoingHUD;
	}

	public void DismissHUD()
	{
		this.outgoingHUD = true;
        m_hudAnimator.Play("Out");
        StartCoroutine(_checkAnimationEnd("Out"));
	}

	public void DismissThrottle()
	{
        m_hudAnimator.Play("DismisThrottle");
	}

	public void RemoveHUD()
	{
	}

	public void ResetHUD(bool offscreen)
	{
		this.EnableGearDownPaddle(false);
		this.incomingHUD = (this.outgoingHUD = false);
        //m_throttleAnimator.Play("Idle");
	}

	public bool IsAnimating()
	{
		return this.incomingHUD || this.outgoingHUD;
	}

	public bool GetHUDIsInPositionOnscreen()
	{
		return this.HUDIsIn;
	}

	public Vector3 GetNOSButtonPosition()
	{
	    return this.nitrousButton.transform.rectTransform().GetUpperPoint();
	}

	public Vector3 GetRecordButtonPosition()
	{
        return this.recordButton.transform.position;
	}

	public Vector3 GetThrottleButtonPosition()
	{
	    var height = throttleButton.GetComponent<RectTransform>().rect.height*m_canvas.transform.localScale.y*
	                 throttleButton.transform.localScale.y;
	    return this.throttleButton.transform.position + (Vector3.up*height/2);
	}

	public Vector3 GetGearUpButtonPosition()
	{
        var height = gearUpButton.GetComponent<RectTransform>().rect.height * m_canvas.transform.localScale.y *
                     gearUpButton.transform.localScale.y;
        return this.gearUpButton.transform.position + (Vector3.up * height / 2);
	}

	public Vector3 GetEmbeddedGearChangeDevicePosition()
	{
		Vector3 position = this.gearChangeDevice.transform.position;
		position.z -= 0.5f;
		return position;
	}

	public Vector3 GetWheelSpinPosition()
	{
        return this.wheelspinDevice.transform.position;
	}

	public void EnableGearDownPaddle(bool zState)
	{
		this.gearDownButton.GetComponent<Button>().enabled = zState;
	}

	public void SetThrottleHightlight(bool zState)
	{
		Vector3 position = this.throttleButton.transform.position;
		if (zState)
		{
			position.z = -0.5f;
		}
		else
		{
			position.z = 0f;
		}
		this.throttleButton.transform.position = position;
	}

	public void SetNOSHightlight(bool zState)
	{
		Vector3 position = this.nitrousButton.transform.position;
		if (zState)
		{
			position.z = -0.5f;
		}
		else
		{
			position.z = 0f;
		}
		this.nitrousButton.transform.position = position;
	}

	public void SetRecordHightlight(bool zState)
	{
		Vector3 position = this.recordButton.transform.position;
		if (zState)
		{
			position.z = -0.5f;
		}
		else
		{
			position.z = 0f;
		}
		this.recordButton.transform.position = position;
	}

	public void SetGearUpHightlight(bool zState)
	{
		Vector3 position = this.gearUpButton.transform.position;
		if (zState)
		{
			position.z = -0.5f;
		}
		else
		{
			position.z = 0f;
		}
		this.gearUpButton.transform.position = position;
	}

    private IEnumerator _checkAnimationEnd(string animName)
    {
        var endStateReached = false;
        while (!endStateReached)
        {
            if (!m_hudAnimator.IsInTransition(0))
            {
                var stateInfo = m_hudAnimator.GetCurrentAnimatorStateInfo(0);
                endStateReached = stateInfo.IsName("Idle") || (stateInfo.IsName(animName)
                    && stateInfo.normalizedTime >= .98F);
            }
            yield return 0;
        }

        yield return new WaitForEndOfFrame();
        if (incomingHUD)
        {
            incomingHUD = false;
            HUDIsIn = true;
        }
        if (outgoingHUD)
        {
            outgoingHUD = false;
            HUDIsIn = false;
        }
    }
}
