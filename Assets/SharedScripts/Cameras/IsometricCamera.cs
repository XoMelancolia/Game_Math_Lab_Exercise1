using Cinemachine;
using UnityEngine;

namespace GameMath.Cameras
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class IsometricCamera : MonoBehaviour
    {
        public enum CameraMotion { Discrete, Smooth }
        public enum UpdateMethods { Update, LateUpdate, FixedUpdate }
        
        [SerializeField] private Transform anchor;
        [SerializeField] private float panningSpeed = 10f;
        
        [SerializeField] private float initialAngle = 315f;
        [SerializeField] private CameraMotion rotateMotion = CameraMotion.Smooth;
        [SerializeField] private float rotateSmoothTime = 0.1f;
        
        [SerializeField] private float zoomMinDistance = 5f;
        [SerializeField] private float zoomMaxDistance = 50f;
        [SerializeField] private CameraMotion zoomMotion = CameraMotion.Smooth;
        [SerializeField] private float zoomDiscreteDistance = 1f;
        [Range(0.1f, 1f)]
        [SerializeField] private float zoomSensitive = 0.1f;
        [Range(0.1f, 1f)]
        [SerializeField] private float zoomSmoothTime = 0.2f;
        
        [SerializeField] private UpdateMethods updateMethod;
        
        private CinemachineVirtualCamera virtualCamera;
        private Camera renderCamera;
        
        private float currentRotateVelocity;
        private float currentRotationY;
        private float currentZoomDelta;
        private float currentZoomVelocity;
        private float targetRotationY;
        private float targetZoomDistance;
        
        public Transform Anchor => anchor;
        public Camera RenderCamera => renderCamera;
        
        public float PanningSpeed
        {
            get => panningSpeed;
            set => panningSpeed = Mathf.Clamp(value, 0, float.MaxValue);
        }
        
        public float InitialAngle
        {
            get => initialAngle;
            set => initialAngle = Mathf.Clamp(value, 0, 360);
        }
        
        public CameraMotion RotateMotion
        {
            get => rotateMotion;
            set => rotateMotion = value;
        }
        
        public float RotateSmoothTime
        {
            get => rotateSmoothTime;
            set => rotateSmoothTime = Mathf.Clamp01(value);
        }
        
        public float ZoomMinDistance
        {
            get => zoomMinDistance;
            set => zoomMinDistance = Mathf.Clamp(value, 0f, zoomMaxDistance - 1);
        }
        
        public float ZoomMaxDistance
        {
            get => zoomMaxDistance;
            set => zoomMaxDistance = Mathf.Clamp(value, zoomMinDistance + 1, float.MaxValue);
        }
        
        public CameraMotion ZoomMotion
        {
            get => zoomMotion;
            set => zoomMotion = value;
        }
        
        public float ZoomDiscreteDistance
        {
            get => zoomDiscreteDistance;
            set => zoomDiscreteDistance = Mathf.Clamp(value, 0.1f, float.MaxValue);
        }
        
        public float ZoomSensitive
        {
            get => zoomSensitive;
            set => zoomSensitive = Mathf.Clamp01(value);
        }
        
        public float ZoomSmoothTime
        {
            get => zoomSmoothTime;
            set => zoomSmoothTime = Mathf.Clamp01(value);
        }
        
        public UpdateMethods UpdateMethod
        {
            get => updateMethod;
            set => updateMethod = value;
        }

        public int Priority
        {
            get => virtualCamera.Priority;
            set => virtualCamera.Priority = value;
        }
        
        public float OrthographicSize => virtualCamera.m_Lens.OrthographicSize;
        
        public Vector3 CameraUp => renderCamera.transform.up;
        public Vector3 CameraRight => renderCamera.transform.right;
        public Vector3 CameraForward => renderCamera.transform.forward;
        public Vector3 CameraPosition => renderCamera.transform.position;
        public Quaternion CameraOrientation => renderCamera.transform.rotation;
        
        private void Awake()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            virtualCamera.m_Lens.Orthographic = true;
            virtualCamera.Follow = anchor;

            if (virtualCamera.GetCinemachineComponent<CinemachineTransposer>() == null)
            {
                var transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
                transposer.m_FollowOffset = Vector3.zero;
                transposer.m_XDamping = 0;
                transposer.m_YDamping = 0;
                transposer.m_ZDamping = 0;
            }
            
            transform.eulerAngles = new Vector3(30, initialAngle, 0);
            
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            targetRotationY = initialAngle;
            targetZoomDistance = virtualCamera.m_Lens.OrthographicSize;
            currentRotationY = initialAngle;
        }

        private void Start()
        {
            renderCamera = CinemachineCore.Instance.FindPotentialTargetBrain(virtualCamera).OutputCamera;
        }

        private void Update()
        {
            if (updateMethod == UpdateMethods.Update)
                UpdaterCameraState();
        }

        private void LateUpdate()
        {
            if (updateMethod == UpdateMethods.LateUpdate)
                UpdaterCameraState();
        }
        
        private void FixedUpdate()
        {
            if (updateMethod == UpdateMethods.FixedUpdate)
                UpdaterCameraState();
        }
        
        protected void UpdaterCameraState()
        {
            HandleZoom();
            HandleRotation();
        }

        private void HandleZoom()
        {
            if (currentZoomDelta != 0f)
            {
                currentZoomDelta = Mathf.Clamp(currentZoomDelta, -1, 1);
                targetZoomDistance = zoomMotion switch
                {
                    CameraMotion.Discrete => Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize - currentZoomDelta * zoomDiscreteDistance, zoomMinDistance, zoomMaxDistance),
                    CameraMotion.Smooth => Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize - currentZoomDelta * zoomSensitive * 10, zoomMinDistance, zoomMaxDistance),
                    _ => targetZoomDistance
                };
            }

            virtualCamera.m_Lens.OrthographicSize = zoomMotion switch
            {
                CameraMotion.Discrete => targetZoomDistance,
                CameraMotion.Smooth => Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, targetZoomDistance, ref currentZoomVelocity, zoomSmoothTime),
                _ => virtualCamera.m_Lens.OrthographicSize
            };
        }

        private void HandleRotation()
        {
            currentRotationY = rotateMotion switch
            {
                CameraMotion.Discrete => targetRotationY,
                CameraMotion.Smooth   => Mathf.SmoothDamp(currentRotationY, targetRotationY, ref currentRotateVelocity, rotateSmoothTime),
                _ => currentRotationY
            };

            if (Mathf.Abs(currentRotationY - targetRotationY) < Mathf.Epsilon)
                currentRotationY = targetRotationY;

            if (currentRotationY > 360)
            {
                currentRotationY %= 360;
                targetRotationY  %= 360;
            }
        
            virtualCamera.transform.rotation = Quaternion.Euler(virtualCamera.transform.eulerAngles.x, currentRotationY, virtualCamera.transform.eulerAngles.z);
            virtualCamera.Follow.rotation    = Quaternion.Euler(0, currentRotationY, 0);
        }

        public void RotateClockwise()
        {
            targetRotationY += 90f;
        }
        
        public void RotateCounterClockwise()
        {
            targetRotationY -= 90f;
        }

        public void Zoom(float zoomDelta)
        {
            currentZoomDelta = zoomDelta;
        }
        
        public void Move(float relativeX, float relativeZ)
        {
            var moveVector = Quaternion.Euler(0, virtualCamera.Follow.eulerAngles.y, 0) * new Vector3(relativeX, 0, relativeZ);
            virtualCamera.Follow.position += moveVector.normalized * Time.deltaTime * panningSpeed;
        }
    }
}