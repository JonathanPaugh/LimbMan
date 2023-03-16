using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntWorldPush : WorldEntity
    {
        [SerializeField]
        private ForceMode mode;

        [SerializeField]
        private float force;

        [SerializeField]
        private bool useMass;

        [SerializeField]
        private Dimension frontAxis = Dimension.Z;

        protected override void StayAction(GameObject gameObject)
        {
            if (!gameObject.HasRigidbody()) { return; }
            switch(frontAxis)
            {
                case Dimension.X:
                    
                    gameObject.ApplyForce(transform.right * force, mode, false);
                    break;
                case Dimension.Y:
                    gameObject.ApplyForce(transform.up * force, mode, false);
                    break;
                case Dimension.Z:
                    gameObject.ApplyForce(transform.forward * force, mode, false);
                    break;
            }
        }
    }
}