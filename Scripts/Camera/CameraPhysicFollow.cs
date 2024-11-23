using UnityEngine;

public class CameraPhysicFollow : MonoBehaviour
{
#if(UNITY_EDITOR)
    [SerializeField]
    private bool m_debug = true;
#endif
    public Transform Player;
    public Transform Opponent;
    private Camera m_camera;
    private Vector3 m_lookPosition;
    private float m_fov;
    private float m_fovTime;
    //private float m_vel;
    [SerializeField] private Vector3 m_distanceToTarget;
    [SerializeField] private Vector3 m_lookOffset;
    //[SerializeField] private float _smoothTime = 3;
    [SerializeField] private float _maxLeftAngle = 30;
    [SerializeField] private float _maxRightAngle = 30;
    [SerializeField] private bool m_angled = true;
    [SerializeField] private AnimationCurve m_fovCurve;
    [SerializeField] private float m_fovDuration = 1;
    [SerializeField] private float m_shakeSize = 1;
    [SerializeField] private float m_shakeSpeed = 3;

    public float shakeSize
    {
        set { m_shakeSize = value; }
    }

    void Awake()
    {
        m_camera = GetComponent<Camera>();
        m_fov = m_camera.fieldOfView;
        m_fovTime = m_fovDuration;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (Player == null)
	        return;
	    var targetPosition = Player.position;
	    var angleDistance = Opponent != null ? getAngleDistance() : 0;

        //Debug.Log(angle);
	    targetPosition += m_distanceToTarget;
	    targetPosition.z += -angleDistance;
	    transform.position = targetPosition;

	    m_lookPosition = Player.position + m_lookOffset;
        transform.LookAt(m_lookPosition);


	    if (m_fovTime <m_fovDuration)
	    {
	        var value = m_fovCurve.Evaluate(m_fovTime/m_fovDuration);
	        m_camera.fieldOfView = m_fov*value;
	        m_fovTime += Time.deltaTime;
	    }

        updateShaking();
	}

    public void shiftCamera()
    {
        m_fovTime = 0;
    }

    private float getAngleDistance()
    {
        var vector =  Opponent.position - Player.position;
        vector.y = 0;
        vector.Normalize();

        var angle = Vector3.Angle(Vector3.left, vector);
        var sign = Mathf.Sign(vector.z);
        angle = Mathf.Min(angle, sign >= 0 ? _maxRightAngle : _maxLeftAngle);

        if (!m_angled)
            angle = 0;

        return sign*angle/20;
    }

    void updateShaking()
    {
        if (Mathf.Approximately(m_shakeSize, 0)) return;
        //float y  = Random.Range(-m_shakeSize, m_shakeSize);
        float y = Mathf.Sin(Time.time*m_shakeSpeed%(2*Mathf.PI))*m_shakeSize;
        //Debug.Log(y);

        var targetPosition = transform.position;
        targetPosition.y += y;

        transform.position = targetPosition;//Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * m_shakeSpeed);

    }

    public void setTarget(Transform player,Transform opponent)
    {
        Player = player;
        Opponent = opponent;
    }

#if(UNITY_EDITOR)
    void OnDrawGizmos()
    {
        if (Application.isPlaying && m_debug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_lookPosition, 0.2F);
        }
    }
#endif
}
