using System;
using KingKodeStudio;
using UnityEngine;

public class ShowroomCameraManager : MonoBehaviour
{
	public static ShowroomCameraManager Instance;

	public GameObject cameraPivot;

	public Camera cameraFrontPose;

	public Camera cameraBackPose;

    public MeshFadeQude MeshFadeQuade;

	public static CameraAtBackDelegate CameraAtBackEvent;

	private bool pinchStarted;

	private float pinchStartDistance;

	private float lastPinchTime;

	private bool tapStarted;

	private float tapTimer;

	private Vector2 tapStartPos;

	private Vector2 tapLastPos;

	private float lastTapTime;

    public float BackTime = 1;

    public float FronTime = 1;

    //public ShowroomCamBack backCam
    //{
    //    get;
    //    private set;
    //}

    //public CamTransitionToFront backToFront
    //{
    //    get;
    //    private set;
    //}

    //public ShowroomCamFront frontCam
    //{
    //    get;
    //    private set;
    //}

    //public CamTransitionToBack frontToBack
    //{
    //    get;
    //    private set;
    //}



    private bool m_transitionToFront;
    private bool m_transitionToBack;
    private bool m_backCamEnabled;
    private bool m_frontCamEnabled;
    private float startTime;

    public bool IsZoomedIn
	{
		get;
		set;
	}

	private void Awake()
	{
		ShowroomCameraManager.Instance = this;
        //this.backCam = base.GetComponent<ShowroomCamBack>();
        //this.backCam.enabled = true;
        //this.backCam.pivot = this.cameraPivot;
        //this.backCam.frontPose = this.cameraFrontPose;
        //this.backCam.backPose = this.cameraBackPose;
        //this.frontCam = base.GetComponent<ShowroomCamFront>();
        //this.frontCam.enabled = false;
        //this.frontCam.pivot = this.cameraPivot;
        //this.frontCam.frontPose = this.cameraFrontPose;
        //this.frontCam.backPose = this.cameraBackPose;
        //this.backToFront = base.GetComponent<CamTransitionToFront>();
        //this.backToFront.enabled = false;
        //this.backToFront.pivot = this.cameraPivot;
        //this.backToFront.frontPose = this.cameraFrontPose;
        //this.backToFront.backPose = this.cameraBackPose;
        //this.frontToBack = base.GetComponent<CamTransitionToBack>();
        //this.frontToBack.enabled = false;
        //this.frontToBack.pivot = this.cameraPivot;
        //this.frontToBack.frontPose = this.cameraFrontPose;
        //this.frontToBack.backPose = this.cameraBackPose;
	}

	public void TransitionToFront()
	{
        //this.backCam.enabled = false;
        //this.frontCam.enabled = false;
        //this.backToFront.StartTransition();
        //this.frontToBack.enabled = false;

	    m_backCamEnabled = false;
	    m_frontCamEnabled = false;
	    m_transitionToBack = false;

        if (m_transitionToFront)
        {
            return;
        }

        startTime = Time.time;
        m_transitionToFront = true;
        MeshFadeQuade.FadeTo(new Color(0, 0, 0, 0), FronTime, EnableFront);
	}

	public bool IsTransitioningToFront()
	{
        //return this.backToFront.enabled;
	    return m_transitionToFront;
	}

	public bool IsFrontCamEnabled()
	{
        //return this.frontCam.enabled;
	    return m_frontCamEnabled;
	}

	public void EnableFront()
	{
        //this.backCam.enabled = false;
        //this.frontCam.enabled = true;
        //this.backToFront.enabled = false;
        //this.frontToBack.enabled = false;

        m_backCamEnabled = false;
        m_frontCamEnabled = true;
        m_transitionToBack = false;
        m_transitionToFront = false;
	}

	public void TransitionToBack()
	{
        //if (this.frontToBack.enabled)
        //{
        //    return;
        //}
        //this.backCam.enabled = false;
        //this.frontCam.enabled = false;
        //this.backToFront.enabled = false;
        //this.frontToBack.StartTransition();

        m_backCamEnabled = false;
        m_frontCamEnabled = false;
        m_transitionToFront = false;

	    if (m_transitionToBack)
	    {
	        return;
	    }

	    startTime = Time.time;
	    m_transitionToBack = true;
        MeshFadeQuade.FadeTo(new Color(0,0,0,1),BackTime, EnableBack);
	}

	public bool IsTransitioningToBack()
	{
        //return this.frontToBack.enabled;
	    return m_transitionToBack;
	}

	public bool IsBackCamEnabled()
	{
        //return this.backCam.enabled;
	    return m_backCamEnabled;
	}

	public void EnableBack()
	{
        //if (this.backCam == null)
        //{
        //    return;
        //}
        //this.backCam.enabled = true;
        //this.frontCam.enabled = false;
        //this.backToFront.enabled = false;
        //this.frontToBack.enabled = false;

	    m_backCamEnabled = true;
	    m_frontCamEnabled = false;
	    m_transitionToBack = false;
	    m_transitionToFront = false;
		if (ShowroomCameraManager.CameraAtBackEvent != null)
		{
			ShowroomCameraManager.CameraAtBackEvent();
		}
	}

	public void ResetToBack()
	{
        //base.transform.localPosition = this.cameraBackPose.transform.position;
        //base.transform.LookAt(this.cameraPivot.transform, Vector3.up);
        //base.GetComponent<Camera>().fieldOfView = this.cameraBackPose.fieldOfView;

	    MeshFadeQuade.SetToBlack();
		this.EnableBack();
	}

	public void StopTransitionsForScreenshots()
	{
        //this.backCam.enabled = false;
        //this.frontCam.enabled = false;
        //this.backToFront.enabled = false;
        //this.frontToBack.enabled = false;
	}

	private void Update()
	{
		this.UpdateCameraRect();
		this.CheckForPinch();
		this.CheckForTap();
	}

	private void CheckForPinch()
	{
		if (Input.touches.Length == 2)
		{
			if (!this.pinchStarted)
			{
				this.pinchStarted = true;
				this.pinchStartDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
			}
			else
			{
				float num = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
				float num2 = Mathf.Abs(num - this.pinchStartDistance);
				if (num2 > 30f)
				{
					this.pinchStarted = false;
					this.lastPinchTime = Time.time;
					if (num < this.pinchStartDistance)
					{
						this.SwitchToUnZoomed();
					}
					else
					{
						this.SwitchToZoomed();
					}
				}
			}
		}
		else
		{
			this.pinchStarted = false;
		}
	}

	private void CheckForTap()
	{
		if (this.lastPinchTime + 0.6f > Time.time || this.lastTapTime + 0.6f > Time.time)
		{
			return;
		}
		bool flag = false;
		Vector2 vector = Vector2.zero;
		if (Input.touches.Length == 1)
		{
			vector = Input.touches[0].position;
			Vector2 point = new Vector2(vector.x / (float)Screen.width, vector.y / (float)BaseDevice.ActiveDevice.GetScreenHeight());
			if (Camera.main.rect.Contains(point) && !PopUpManager.Instance.isShowingPopUp)
			{
                //ZHUDScreen activeScreen = (ZHUDScreen) ScreenManager.Instance.ActiveScreen;
                //if (activeScreen.TappableRegion.Contains(point))
                //{
                //    flag = true;
                //}
			}
		}
		if (flag && !this.tapStarted)
		{
			this.tapStarted = true;
			this.tapTimer = 0f;
			this.tapStartPos = (this.tapLastPos = vector);
		}
		else if (flag && this.tapStarted)
		{
			this.tapLastPos = vector;
			this.tapTimer += Time.deltaTime;
			return;
		}
		float num = Vector2.Distance(this.tapLastPos, this.tapStartPos);
		if (this.tapStarted && !flag && this.tapTimer < 0.2f && num < 15f)
		{
			this.tapStarted = false;
			this.lastTapTime = Time.time;
			if (!this.IsZoomedIn)
			{
				this.SwitchToZoomed();
			}
			else
			{
				this.SwitchToUnZoomed();
			}
		}
		else if (this.tapStarted && !flag)
		{
			this.tapStarted = false;
		}
	}

	public void SwitchToZoomed()
	{
		if (this.IsZoomedIn)
		{
			return;
		}
		this.IsZoomedIn = true;
		this.lastTapTime = Time.time;
        //ScreenManager.Instance.PushScreen(ScreenID.ShowroomFreeCam);
	}

	public void SwitchToUnZoomed()
	{
		if (!this.IsZoomedIn)
		{
			return;
		}
		this.lastTapTime = Time.time;
		this.IsZoomedIn = false;
        //ScreenManager.Instance.PopScreen();
	}

	private void UpdateCameraRect()
	{
        //if (CommonUI.Instance == null)
        //{
        //    return;
        //}
        //float num = ScreenManager.Instance.GetHeightOfCurrentScreensUI() / GUICamera.Instance.ScreenHeight;
        //float num2 = 0f;
        //NavigationBar navBar = CommonUI.Instance.NavBar;
        //if (navBar.enabled)
        //{
        //    num2 = (CommonUI.Instance.NavBar.Background.height - 0.04f) / GUICamera.Instance.ScreenHeight;
        //}
        //Camera.main.rect = new Rect(0f, num, 1f, 1f - (num + num2));
	}

    public double GetTransitionTime()
    {
        return Time.time - this.startTime;
    }
}
