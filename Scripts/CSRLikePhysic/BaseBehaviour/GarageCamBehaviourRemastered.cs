using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GarageCamBehaviourRemastered : BaseBehaviour ,ITargetTracker
{
    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:

    // 	Camera Rig
    // 		Pivot
    // 			Camera

    [SerializeField]
    private Transform m_lookTransform;
    //[SerializeField]
    //private float m_MoveSpeed = 1f;                      // How fast the rig will move to keep up with the target's position.
    [Range(0f, 10f)]
    [SerializeField]
    private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
    [SerializeField]
    private float m_TurnSmoothing = 0.1f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
    [SerializeField]
    private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
    [SerializeField]
    private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
    [SerializeField]
    private bool m_VerticalAutoReturn = false;           // set wether or not the vertical axis should auto return
    [SerializeField]
    private float m_sideCameraDistance = 5;
    [SerializeField]
    private float m_frontCameraDistance = 7;
    [SerializeField]
    private float m_topPivotDistance = 2;
    [SerializeField]
    private float m_bottomPivotDistance = 2;
    [SerializeField]
    private float m_topCameraAngle = 6;
    [SerializeField]
    private float m_bottomCameraAngle = 0;
    [SerializeField]
    private float m_halfDistance = 2;
    [SerializeField]
    private float m_fullDistance = 0;
    [SerializeField]
    private float m_halfHeight = 2;
    [SerializeField]
    private float m_fullHeight = 0;
    [SerializeField]
    private float m_initAngle = 45;
    [SerializeField]
    private float m_initTiltAngle = 0;

    [SerializeField] private float m_idleSpeed = 5;
    [SerializeField]
    protected Transform m_Target;            // The target object to follow
    [SerializeField]
    protected Transform m_Cam; // the transform of the camera

    [SerializeField] protected float m_fieldOfView = 35;

    private float m_LookAngle;                    // The rig's y axis rotation.
    private float m_TiltAngle;                    // The pivot's x axis rotation.
    private const float k_LookDistance = 300f;    // How far in front of the pivot the character's look target is.
    private Vector3 m_PivotEulers;
    private Quaternion m_PivotTargetRot;
    private Quaternion m_TransformTargetRot;
    private bool m_half = true;
    private float x;
    private float y;
    private float m_initFOV;

    protected Transform m_Pivot; // the point at which the camera pivots around
    protected Vector3 m_LastTargetPosition;
    private float m_lastUpdateTime;
    private float m_lastX;
    private float m_pivotX;

    public static GarageCamBehaviourRemastered Instance { get; private set; }

    protected virtual void Awake()
    {
        m_Pivot = m_Cam.parent;
        m_PivotEulers = m_Pivot.rotation.eulerAngles;
        m_pivotX = m_Pivot.transform.localPosition.x;

        m_PivotTargetRot = m_Pivot.localRotation;
        m_TransformTargetRot = transform.localRotation;
        m_half = true;
        ResetTransform();
        Instance = this;
    }

    void OnEnable()
    {
        m_Cam.transform.localPosition = Vector3.zero;
        m_Cam.transform.localRotation = Quaternion.identity;
    }

    public virtual void SetTarget(Transform newTransform)
    {
        m_Target = newTransform;
    }

    protected virtual void FollowTarget(float deltaTime)
    {
        //if (m_Target == null) return;
        // Move the rig towards target position.
        if (m_lookTransform!=null)
        transform.position = m_lookTransform.position;//Vector3.Lerp(transform.position, m_lookTransform.position, deltaTime * m_MoveSpeed);
    }


    private void HandleRotationMovement()
    {
        if (Time.timeScale < float.Epsilon)
            return;

        // Read the user input
        x = CrossPlatformInputManager.GetAxis("Mouse X");
        y = CrossPlatformInputManager.GetAxis("Mouse Y");

        if (Mathf.Abs(x) > 0)
        {
            m_lastX = x;
        }
        else
        {
            if (m_lastX == 0)
                m_lastX = 1;
            x = Mathf.Sign(m_lastX)*m_idleSpeed;
        }

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        m_LookAngle += x * m_TurnSpeed;

        // Rotate the rig (the root object) around Y axis only:
        m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);

        if (m_VerticalAutoReturn)
        {
            // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
            // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
            // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
            m_TiltAngle = y > 0 ? Mathf.Lerp(0, -m_TiltMin, y) : Mathf.Lerp(0, m_TiltMax, -y);
        }
        else
        {
            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            m_TiltAngle -= y * m_TurnSpeed;
            // and make sure the new value is within the tilt range
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
        }

        // Tilt input around X is applied to the pivot (the child of this object)
        m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);

        if (m_TurnSmoothing > 0)
        {
            m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
        }
        else
        {
            m_Pivot.localRotation = m_PivotTargetRot;
            transform.localRotation = m_TransformTargetRot;
        }
    }

    private void HandleCameraPosition()
    {
        var pivotForward = new Vector3(m_Pivot.forward.x, 0, m_Pivot.forward.z);
        var angle = Vector3.Angle(pivotForward, Vector3.forward);
        if (angle > 90)
            angle = 180 - angle;
        var t = Mathf.InverseLerp(90, 0, angle);

        var distance = -Mathf.Lerp(m_sideCameraDistance, m_frontCameraDistance, t);

        var camLocPos = m_Cam.localPosition;
        camLocPos.z = distance;
        m_Cam.localPosition = camLocPos;
    }


    private void HandlePivotPosition()
    {
        var angle = m_Pivot.localRotation.eulerAngles.x;
        //Debug.Log(angle);
        if (angle > 300)
        {
            angle = angle - 360;
        }
        var t = Mathf.InverseLerp(-10, 30, angle);
        var distance = -Mathf.Lerp(m_bottomPivotDistance, m_topPivotDistance, t);

        //Set pivot Pos
        var pivotPos = m_Pivot.localPosition;
        pivotPos.z = distance + (m_half ? m_halfDistance : m_fullDistance);
        pivotPos.y = (m_half ? m_halfHeight : m_fullHeight);
        pivotPos.x = m_pivotX;
        m_Pivot.localPosition = pivotPos;


        //Set Camera Angle
        var cameraRot = m_Cam.localRotation.eulerAngles;
        cameraRot.x = Mathf.Lerp(m_topCameraAngle, m_bottomCameraAngle, t);
        m_Cam.localRotation = Quaternion.Euler(cameraRot);
        //Debug.Log(t + "   " + cameraRot.x);
    }

    public void SwitchScreen()
    {
        m_half = !m_half;
    }

    public Transform Target
    {
        get
        {
            return m_Target;
        }
        set { m_Target = value; }
    }

    public void ResetTransform()
    {
        m_LookAngle = m_initAngle;
        m_TiltAngle = m_initTiltAngle;
        transform.localRotation = m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);
        m_Pivot.localRotation = m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, 0, 0);
    }

    public override void OnActivate()
    {
        m_lookTransform = GarageCarVisualsSettings.Instance.carPlacementNode.transform;//GarageManager.Instance.SpawnPoint;
        var cameraEuler = Camera.main.transform.eulerAngles;
        var position = Camera.main.transform.position;
        m_LookAngle = cameraEuler.y;
        transform.localRotation = m_TransformTargetRot = Quaternion.Euler(0, cameraEuler.y, 0);
        m_Pivot.localRotation = m_PivotTargetRot = Quaternion.Euler(cameraEuler.x, 0, 0);
        m_Pivot.position = position;
    }

    public override void OnUpdate(ref CameraState zResult)
    {
        if (m_lastUpdateTime>0 && Time.time - m_lastUpdateTime > 0.5F)
        {
            //var cameraEuler = Camera.main.transform.eulerAngles;
            //var position = Camera.main.transform.position;
            //m_LookAngle = cameraEuler.y;
            //transform.localRotation = m_TransformTargetRot = Quaternion.Euler(0,cameraEuler.y,0);
            //m_Pivot.localRotation = m_PivotTargetRot = Quaternion.Euler(cameraEuler.x, 0, 0);
            //m_Pivot.position = position;
            //m_Pivot.position = position;
            //m_Cam.transform.position = position;
            //m_Pivot.LookAt(Target);
        }
        HandleRotationMovement();
        HandleCameraPosition();
        HandlePivotPosition();
        FollowTarget(Time.deltaTime);

        zResult.position = m_Cam.position;
        zResult.rotation = m_Cam.rotation;
        zResult.fov = m_fieldOfView;
        m_lastUpdateTime = Time.time;
    }
}
