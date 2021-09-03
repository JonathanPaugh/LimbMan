using UnityEngine;
using Jape;
using Time = Jape.Time;

namespace Game
{
    [DisallowMultipleComponent]
    public class Camera : Jape.Camera
    {
        [Space(16)]

        public Vector2 CameraOffset;

        [Space(16)]

        [Range(0, 10)] public float CameraSmoothing;

        [Space(16)]

        public float CameraDrag;
        public float CameraCatchup;

        [Space(16)]

        [Range(0, 10)] public float CameraFalling;
        [Range(0, 100)] public float CameraFallingThreshold;

        [Space(16)]

        public float CameraSpeed;
        public float CameraZoom;
        public float CameraScale;
        public bool CameraBasic;

        [Space(16)]

        public Collider2D CameraBounds;

        private Player CameraTarget;

        private float TargetMaxSpeed;
        private bool TargetDescending;

        private float CameraZoomPrev;

        private Vector3 CameraPositionCurrent;
        private Vector3 CameraPositionNext;

        protected override void Init()
        {
            camera.orthographicSize = CameraZoom;
        }

        public void SetTarget(Player unit)
        {
            CameraTarget = unit;
        }

        protected override void Late()
        {
            Follow();
        }

        private void Follow()
        {
            if (CameraTarget == null) { return; }

            if (CameraBasic)
            {
                transform.position = new Vector3(CameraTarget.transform.position.x, CameraTarget.transform.position.y, -100); 
                return;
            }

            // Reference Component Variables
            TargetMaxSpeed = CameraTarget.movement.moveSpeed * Movement.MoveSpeedRatio;
                
            TargetDescending = CameraTarget.movement.Falling || CameraTarget.movement.Sliding;

            // Init Camera Positions
            CameraPositionCurrent = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            CameraPositionNext = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            // Calculate Camera Drag
            float Drag = CameraTarget.rigidbody.velocity.x / TargetMaxSpeed;
            Drag *= CameraDrag * 5;

            // Calculate Camera Smoothing
            float SmoothingTime = ((10 - CameraSmoothing) / 10) + 0.1f;

            // Calculate Camera Zoom
            float Zoom = Mathf.Abs(CameraTarget.rigidbody.velocity.x) / TargetMaxSpeed;
            Zoom = Math.Rescale(Zoom, 0, 1, 1, CameraScale);
            Zoom = CameraZoom * Zoom;
            Zoom = Mathf.Lerp(CameraZoomPrev, Zoom, SmoothingTime);

            // Calculate Camera Catchup
            float Catchup = Mathf.Abs(((transform.position.x + CameraOffset.x) - Drag) - CameraTarget.transform.position.x);
            Catchup /= CameraZoom;
            Catchup = Math.Rescale(Catchup, 0, 1, 1, CameraCatchup);

            // Calculate Camera Falling
            float FallingSpeed = 1;
            float FallingOffset = 0;

            if (TargetDescending && (Mathf.Abs(CameraTarget.rigidbody.velocity.y) / Movement.MaxFallSpeed) > (CameraFallingThreshold / 100)) {
                FallingSpeed = FallingOffset = ((CameraFalling / 8f) + 1) * ((Mathf.Abs(CameraTarget.rigidbody.velocity.y) / Movement.MaxFallSpeed) + 1);

                FallingSpeed *= Math.Rescale((10 - CameraFalling) / 10, 0, 1, 1, 2);

                FallingOffset *= -1;
                FallingOffset *= (CameraFalling / 3f);
            }

            // Calculate Camera Ascending
            float AscendingSpeed = 1;
            float AscendingOffset = 0;

            const float FastAscendThreshold = 15;
            if (CameraTarget.rigidbody.velocity.y > FastAscendThreshold)
            {
                AscendingSpeed = 2;
                AscendingOffset = 10;
            }

            // Calculate True Offset For X & Y
            Vector2 TrueOffset;
            TrueOffset.x = CameraOffset.x + Drag;
            TrueOffset.y = CameraOffset.y + FallingOffset + AscendingOffset;

            // Calculate Camera Velocity For X & Y
            Vector2 Velocity;
            Velocity.x = Velocity.y = CameraSpeed * 5;
            Velocity.x *= Catchup;
            Velocity.y *= FallingSpeed;
            Velocity.y *= AscendingSpeed;
            Velocity.y *= 2;

            // Calculate Next Camera Position
            CameraPositionNext.x = Mathf.MoveTowards(transform.position.x, CameraTarget.transform.position.x + TrueOffset.x, Velocity.x * Time.FrameInterval());
            CameraPositionNext.y = Mathf.MoveTowards(transform.position.y, CameraTarget.transform.position.y + TrueOffset.y, Velocity.y * Time.FrameInterval());

            // Apply Camera Position & Zoom
            Vector3 CameraPositionLerp = Vector3.Lerp(CameraPositionCurrent, CameraPositionNext, SmoothingTime);
            if (CameraBounds != null) {
                if (CameraBounds.bounds.Contains(new Vector2(CameraPositionLerp.x, CameraPositionLerp.y))) {
                    transform.position = CameraPositionLerp;
                } else {
                    transform.position = new Vector3(CameraBounds.bounds.ClosestPoint(CameraPositionCurrent).x,
                                                        CameraBounds.bounds.ClosestPoint(CameraPositionCurrent).y,
                                                        CameraPositionCurrent.z);
                }
            } else {
                transform.position = CameraPositionLerp;
            }

            // Set Zoom
            camera.orthographicSize = Zoom;

            // Update Variables For Next Calculation
            CameraZoomPrev = Zoom;
        }
    }
}