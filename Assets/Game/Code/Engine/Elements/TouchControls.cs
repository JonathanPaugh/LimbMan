using UnityEngine;
using Jape;

namespace Game
{
    public class TouchControls : Element
    {
        [SerializeField]
        private PlayerInput input;

        [SerializeField]
        private Joystick axis;
        public Joystick Axis => axis;

        private bool? jump;
        private bool? dodge;

        public bool Active
        {
            get
            {
                return input.useTouchControls;
            }

            set
            {
                gameObject.SetActive(value);
                input.useTouchControls = value;
            }
        }

        private bool? ButtonSteam(ref bool? button)
        {
            if (button == true)
            {
                button = false;
                return true;
            }

            return button;
        }

        public bool? JumpStream() => ButtonSteam(ref jump);
        public bool? DodgeStream() => ButtonSteam(ref dodge);

        public void PressJump() { jump = true; }
        public void ReleaseJump() { jump = null; }
        public void PressDodge() { dodge = true; }
        public void ReleaseDodge() { dodge = null; }
    }
}