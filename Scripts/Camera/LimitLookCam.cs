using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Cameras
{
    public class LimitLookCam : MonoBehaviour,ITargetTracker
    {
        [SerializeField] private Transform m_pivot;
        [SerializeField] private Transform m_target;
        [SerializeField] private float m_turnSpeed = 4;
        [SerializeField] private float m_rotationLerpSpeed = 1;
        [SerializeField] private float m_maxTurnSpeed = 20;
        [SerializeField] private float m_cameraSpeed = 4;
        [SerializeField] private float m_cameraMaxSpeed = 1;
        [SerializeField] private float m_cameraLerpSpeed = 1;
        [SerializeField] private float m_cameraLimitX = 4;
        [SerializeField] private float m_cameraLimitY = 4;
        [SerializeField] private float m_cameraDistanceZ = 4;
        [SerializeField] private float m_cameraHeight = 3;
        [SerializeField] private float m_initAngle = 55;
		[SerializeField] private float m_Ypos_Offset = 0;
        //[SerializeField] private float tt = 0;

        public static LimitLookCam Instance { get; private set; }


        public Transform Target
        {
            get
            {
                return m_target;
            }
            set { m_target = value; }
        }

        public void ResetTransform()
        {
            m_rotationY = m_pivot.localEulerAngles.y;
            m_cameraPositionX = m_targetCameraPositionX = m_pivot.localPosition.x;
            m_cameraPositionY = m_targetCameraPositionY = m_pivot.localPosition.y + m_cameraHeight - 1.3f;
            m_rotationY = m_targetRotationY = m_initAngle;
            m_xInput = 0;
            m_yInput = 0;
        }


        private float m_xInput;
        private float m_yInput;
        private float m_rotationY;
        private float m_targetRotationY;
        private float m_cameraPositionX;
        private float m_cameraPositionY;
        private float m_targetCameraPositionX;
        private float m_targetCameraPositionY;

        void Start()
        {
            Instance = this;

            m_rotationY = m_pivot.localEulerAngles.y;
            m_cameraPositionX = m_pivot.localPosition.x;
            m_cameraPositionY = m_targetCameraPositionY = m_pivot.localPosition.y + m_cameraHeight;
            m_rotationY = m_targetRotationY = m_initAngle;

            ResetTransform();
        }

        void OnDestroy()
        {
            Instance = null;
        }

        void Update()
        {
            m_xInput = CrossPlatformInputManager.GetAxis("Mouse X");
            m_yInput = CrossPlatformInputManager.GetAxis("Mouse Y");

            if (m_target == null)
                return;

            HandleRotation();
            UpdateCameraPosition();
        }

        void HandleRotation()
        {
            var turn = LimitSpeed(m_turnSpeed * -m_xInput, m_maxTurnSpeed);
            m_targetRotationY += turn;
            m_rotationY = Mathf.Lerp(m_rotationY, m_targetRotationY, Time.deltaTime * m_rotationLerpSpeed);
            m_target.localRotation = Quaternion.Euler(0, m_rotationY, 0);
        }

        void UpdateCameraPosition()
        {
            var camSpeedX = LimitSpeed(m_cameraSpeed * m_xInput, m_cameraMaxSpeed);
            m_targetCameraPositionX += camSpeedX;
            m_targetCameraPositionX = Mathf.Clamp(m_targetCameraPositionX, -m_cameraLimitX + m_pivot.localPosition.x,
                m_cameraLimitX + m_pivot.localPosition.x);

            var camSpeedY = LimitSpeed(m_cameraSpeed * m_yInput, m_cameraMaxSpeed);
            m_targetCameraPositionY += camSpeedY;
            m_targetCameraPositionY = Mathf.Clamp(m_targetCameraPositionY,
                m_pivot.localPosition.y + m_cameraHeight - m_cameraLimitY,
                m_pivot.localPosition.y + m_cameraHeight + m_cameraLimitY);


            var m_cameraPositionZ = m_pivot.localPosition.z + m_cameraDistanceZ;

            m_cameraPositionX = Mathf.Lerp(m_cameraPositionX, m_targetCameraPositionX, Time.deltaTime * m_cameraLerpSpeed);
			m_cameraPositionY =Mathf.Lerp(m_cameraPositionY, m_targetCameraPositionY, Time.deltaTime * m_cameraLerpSpeed);

            transform.localPosition = new Vector3(m_cameraPositionX, m_cameraPositionY, m_cameraPositionZ);
            transform.localPosition = m_pivot.localPosition - (transform.localPosition - m_pivot.localPosition).normalized * m_cameraDistanceZ;

			Vector3 NEW_m_pivot = new Vector3 (m_pivot.transform.position.x, m_pivot.transform.position.y + m_Ypos_Offset, m_pivot.transform.position.z);
            transform.LookAt(NEW_m_pivot);
        }



        private float LimitSpeed(float speed,float maxSpeed)
        {
            if (Mathf.Abs(speed) > maxSpeed)
            {
                return Mathf.Sign(speed) * maxSpeed;
            }
            return speed;
        }
    }
}
