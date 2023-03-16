using UnityEngine;

namespace Jape
{
    [DisallowMultipleComponent]
    public class Anchor : Element
    {
        [SerializeField] private GameObject parent;
        [Space(8)]
        [SerializeField] private bool autoAnchor = false;
        [SerializeField] private bool autoDestroy = false;
        [Space(8)]
        [SerializeField] private bool anchorPosition = false;
        [SerializeField] private Vector3 positionOffset = Vector3.zero;
        [Space(8)]
        [SerializeField] private bool anchorRotation = false;
        [SerializeField] private Vector3 rotationOffset = Vector3.zero;
        [Space(8)]
        [SerializeField] private ScaleType scaleX = ScaleType.None;
        [SerializeField] private ScaleType scaleY = ScaleType.None;
        [SerializeField] private ScaleType scaleZ = ScaleType.None;
        [Space(8)]
        [SerializeField] private float scaleBase = 1;

        private Vector3 defaultScale;

        public enum ScaleType { None, Absolute, Relative, Mimic, MimicInverse };
        public enum ScaleAxis { X, Y, Z };

        public GameObject Parent
        {
            get => parent;
            set 
            {
                parent = value;
                Process();
            }
        }

        protected override void Init()
        {
            defaultScale = transform.localScale;
            Process();
        }

        protected override void Tick() { Process(); }

        private void Process()
        {
            AutoAnchor();
            AutoDestroy();
            Reposition();
        }

        private void AutoAnchor()
        {
            if (!autoAnchor || parent != null) { return; }
            if (transform.parent == null) { return; }
            Parent = transform.parent.gameObject;
        }

        private void AutoDestroy()
        {
            if (!autoDestroy || parent != null) { return; }
            Destroy(gameObject);
        }

        private void Reposition()
        {
            if (Parent == null) { return; }

            if (anchorPosition) 
            { 
                transform.position = new Vector3(Parent.transform.position.x + positionOffset.x, 
                                                 Parent.transform.position.y + positionOffset.y, 
                                                 Parent.transform.position.z + positionOffset.z);

            }

            if (anchorRotation) 
            { 
                transform.rotation = Quaternion.Euler(Parent.transform.eulerAngles.x + rotationOffset.x, 
                                                      Parent.transform.eulerAngles.y + rotationOffset.y, 
                                                      Parent.transform.eulerAngles.z + rotationOffset.z);
            }

            transform.localScale = new Vector3(SetScale(scaleX, ScaleAxis.X), 
                                               SetScale(scaleY, ScaleAxis.Y), 
                                               SetScale(scaleZ, ScaleAxis.Z));
        }
        
        private float SetScale(ScaleType scale, ScaleAxis axis)
        {
            float objectDefaultScale = 0;
            float objectScale = 0;
            float anchorScale = 0;

            switch (axis) 
            {
                case ScaleAxis.X:
                    objectDefaultScale = defaultScale.x;
                    objectScale = transform.localScale.x;
                    anchorScale = Parent.transform.lossyScale.x;
                    break;

                case ScaleAxis.Y:
                    objectDefaultScale = defaultScale.y;
                    objectScale = transform.localScale.y;
                    anchorScale = Parent.transform.lossyScale.y;
                    break;

                case ScaleAxis.Z:
                    objectDefaultScale = defaultScale.z;
                    objectScale = transform.localScale.z;
                    anchorScale = Parent.transform.lossyScale.z;
                    break;
            }

            switch (scale) 
            {
                case ScaleType.Mimic:
                    return Math.Mimic(objectScale, anchorScale);

                case ScaleType.MimicInverse:
                    return -Math.Mimic(objectScale, anchorScale);

                case ScaleType.Absolute:
                    return anchorScale;

                case ScaleType.Relative:
                    return Math.Rescale(anchorScale, -scaleBase.Abs(), scaleBase.Abs(), -objectDefaultScale.Abs(), objectDefaultScale.Abs());

                default:
                    return objectScale;
            }

        }
    }
}
