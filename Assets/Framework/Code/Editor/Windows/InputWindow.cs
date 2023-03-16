using System;
using Sirenix.OdinInspector;

namespace JapeEditor
{
    public class InputWindow : Window
    {
        protected override string Title => titleLabel;

        protected override Display DisplayMode => Display.Popup;

        protected override bool AutoHeight => true;

        private Action<string> action;

        private string titleLabel;
        private string buttonLabel;

        [ShowInInspector]
        [HideLabel]
        private string input;

        [PropertySpace(8)]

        [ShowInInspector]
        [LabelText("$" + nameof(buttonLabel))]
        [Button(ButtonSizes.Large)]
        private void Execute()
        {
            action(input);
            Close();
        }

        public static InputWindow Call(Action<string> action, string title, string buttonLabel, string input)
        {
            InputWindow window = Open<InputWindow>();
            window.titleLabel = title;
            window.buttonLabel = buttonLabel;
            window.input = input;
            window.action = action;
            window.UpdateTitle();
            return window;
        }
    }
}