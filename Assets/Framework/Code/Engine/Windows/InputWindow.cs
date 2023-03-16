using System;

namespace Jape
{
    public class InputWindow
    {
        public static void Call(Action<string> action, string title, string buttonLabel = null, string input = null)
        {
            Member.Static(Assemblies.FrameworkEditor, nameof(InputWindow), nameof(Call)).Get(action, title, buttonLabel, input);
        }
    }
}