using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jape
{
    public class Input : SystemData
    {
        protected new static string Path => "System/Resources/Inputs";

        [SerializeField]
        private InputActionMap map = new InputActionMap();

        [NonSerialized]
        private List<Action> actions;

        public InputActionMap GetMap()
        {
            return map;
        }

        public Action GetAction(string name)
        {
            return actions.FirstOrDefault(a => a.Input.name == name);
        }

        public void Enable()
        {
            if (actions == null)
            {
                actions = new List<Action>();
                foreach (InputAction inputAction in map)
                {
                    actions.Add(new Action(inputAction));
                }
            }

            foreach (Action action in actions)
            {
                action.Enable();
            }
        }

        public void Disable()
        {
            foreach (Action action in actions)
            {
                action.Disable();
            }
        }

        [Serializable]
        public class Action
        {
            [SerializeField]
            private InputAction input;
            public InputAction Input => input;

            private bool active;

            private object lastValue;

            private bool tapped;
            private bool pressed;
            private bool pressedFirst;

            public Action(InputAction input) { this.input = input; }

            private bool HasInteractions() { return string.IsNullOrEmpty(Input.interactions); }

            public bool Idle() { return Input.phase == InputActionPhase.Waiting; }
            public bool Started() { return Input.phase == InputActionPhase.Started; }
            public bool Canceled() { return Input.phase == InputActionPhase.Canceled; }
            public bool Performed() { return Input.phase == InputActionPhase.Performed; }

            public bool IsPress() { return Input.interactions.Contains("Press"); }
            public bool IsHold() { return Input.interactions.Contains("Hold"); }
            public bool IsTap() { return Input.interactions.Contains("Tap"); }

            public object Read() { return Input.ReadValueAsObject(); }
            public T Read<T>() where T : struct { return Input.ReadValue<T>(); }

            private void OnPerform(InputAction.CallbackContext value)
            {
                if (IsTap()) { tapped = true; }
                if (IsPress()) { pressed = true; pressedFirst = true; }
            }

            public void Enable()
            {
                Input.Enable();
                StartStream();
                Input.performed += OnPerform;
            }

            public void Disable()
            {
                Input.performed -= OnPerform;
                StopStream();
                Input.Disable();
            }

            private void StartStream()
            {
                if (active) { this.Log().Response("Stream is already running"); }
                active = true;

                InitStream();
            }

            private void StopStream()
            {
                if (!active) { this.Log().Response("Stream is not running"); }
                active = false;
            }

            private void InitStream()
            {
                tapped = default;
                lastValue = default;
            }

            public object Stream()
            {
                if (!active) { this.Log().Response("Stream is not running"); }
                if (tapped) { tapped = false; return lastValue; }
                lastValue = Read();
                if (!Started() && !Performed()) { return null; }
                if (!Performed() && !HasInteractions()) { return Read().GetType().GetDefaultValue(); }
                return Read();
            }

            private T? ValueStream<T>() where T : struct
            {
                if (!active) { this.Log().Response("Stream is not running"); }
                if (tapped) { tapped = false; return (T)lastValue; }
                lastValue = Read<T>();
                if (!Started() && !Performed()) { return null; }
                if (!Performed() && !HasInteractions()) { return (T?)typeof(T).GetDefaultValue(); }
                return Read<T>();
            }

            public bool? ButtonStream()
            {
                float? value = ValueStream<float>();

                if (IsPress())
                {
                    if (pressed)
                    {
                        if (value == null)
                        {
                            pressed = false;
                            return null;
                        }
                        
                        if (pressedFirst)
                        {
                            pressedFirst = false;
                            return true;
                        }

                        return false;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (value == null)
                {
                    return null;
                }

                if (value > 0)
                {
                    return true;
                }

                return false;
            }

            public float? AxisStream()
            {
                if (IsHold()) { this.Log().Warning(HoldWarning); }
                return ValueStream<float>();
            }

            public Vector2? DirectionalStream()
            {
                if (IsHold()) { this.Log().Warning(HoldWarning); }
                return ValueStream<Vector2>();
            }

            private const string HoldWarning = "Unable to use hold interaction with composite input";
        }
    }
}