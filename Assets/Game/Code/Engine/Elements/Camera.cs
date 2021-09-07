using UnityEngine;
using Jape;
using Time = Jape.Time;

namespace Game
{
    [DisallowMultipleComponent]
    public class Camera : Jape.Camera
    {
        public Collider2D CameraBounds;

        [Space(16)]

        public Global CameraBasic;

        [Space(16)]

        public Vector2 CameraOffset;

        [Space(8)]

        public Vector2 CameraCatchup;

        [Space(16)]

        [Range(0, 100)] 
        public float CameraSmoothing;

        [Space(16)]

        public float CameraDrag;

        [Space(16)]

        public float CameraFalling;

        [Range(0, 100)] 
        public float CameraFallingThreshold;

        [Space(16)]

        public float CameraSpeed;
        public float CameraZoomSpeed;
        public float CameraZoom;
        public float CameraScale;

        private Player CameraTarget;

        private float TargetMaxSpeed;
        private bool TargetDescending;

        private float CameraZoomCurrent;

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

            if ((bool)CameraBasic.GetValue())
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

            // Calculate Camera Smoothing
            float SmoothingTime = ((100f - CameraSmoothing) / 100f) + 0.1f;

            // Calculate Camera Drag
            float Drag = CameraTarget.rigidbody.velocity.x / TargetMaxSpeed;
            Drag *= CameraDrag * 5;

            // Calculate Camera Catchup
            float catchupX = Mathf.Abs(((transform.position.x + CameraOffset.x) - Drag) - CameraTarget.transform.position.x);
            catchupX /= CameraZoom;
            catchupX = Math.Rescale(catchupX, 0, 1, 1, CameraCatchup.x);

            float catchupY = Mathf.Abs((transform.position.y + CameraOffset.y) - CameraTarget.transform.position.y);
            catchupY /= CameraZoom;
            catchupY = Math.Rescale(catchupY, 0, 1, 1, CameraCatchup.y);

            // Calculate Camera Falling
            float FallingSpeed = 1;
            float FallingOffset = 0;

            if (TargetDescending && (Mathf.Abs(CameraTarget.rigidbody.velocity.y) / Movement.MaxFallSpeed) > (CameraFallingThreshold / 100)) {
                FallingSpeed = 5;
                FallingOffset = -CameraFalling;
            }

            // Calculate True Offset For X & Y
            Vector2 TrueOffset;
            TrueOffset.x = CameraOffset.x + Drag;
            TrueOffset.y = CameraOffset.y + FallingOffset;

            // Calculate Camera Velocity For X & Y
            Vector2 Velocity;
            Velocity.x = CameraSpeed * 5;
            Velocity.x *= catchupX;

            Velocity.y = CameraSpeed * 5;
            Velocity.y *= catchupY;
            Velocity.y *= FallingSpeed;
            Velocity.y *= 2;

            // Calculate Next Camera Position
            CameraPositionNext.x = Mathf.MoveTowards(transform.position.x, CameraTarget.transform.position.x + TrueOffset.x, Velocity.x * Time.FrameInterval());
            CameraPositionNext.y = Mathf.MoveTowards(transform.position.y, CameraTarget.transform.position.y + TrueOffset.y, Velocity.y * Time.FrameInterval());

            // Apply Camera Position & Zoom
            CameraPositionNext = Vector3.Lerp(CameraPositionCurrent, CameraPositionNext, SmoothingTime);
            if (CameraBounds != null) {
                if (CameraBounds.bounds.Contains(new Vector2(CameraPositionNext.x, CameraPositionNext.y))) {
                    transform.position = CameraPositionNext;
                } else {
                    transform.position = new Vector3(CameraBounds.bounds.ClosestPoint(CameraPositionCurrent).x,
                                                     CameraBounds.bounds.ClosestPoint(CameraPositionCurrent).y,
                                                     CameraPositionCurrent.z);
                }
            } else {
                transform.position = CameraPositionNext;
            }

            float ZoomSpeed = CameraZoomSpeed * 50;



            // Calculate Camera Zoom
            float NextYZoom = Mathf.Abs(CameraTarget.rigidbody.velocity.y) / Movement.MaxFallSpeed;
            NextYZoom = NextYZoom < 0.4f ? 0 : Math.Rescale(NextYZoom, 0.4f, 1, 0, 1);

            float CameraZoomNext = (Mathf.Abs(CameraTarget.rigidbody.velocity.x / TargetMaxSpeed) 
                                   + NextYZoom)
                                   / 2;

            CameraZoomNext = Math.Rescale(CameraZoomNext, 0, 1, 1, CameraScale);
            CameraZoomNext *= CameraZoom;

            CameraZoomNext = Mathf.MoveTowards(camera.orthographicSize, CameraZoomNext, Time.FrameInterval() * ZoomSpeed);
            CameraZoomNext = Mathf.Lerp(CameraZoomCurrent, CameraZoomNext, SmoothingTime);

            // Set Zoom
            camera.orthographicSize = CameraZoomNext;
            CameraZoomCurrent = CameraZoomNext;
        }
    }
}