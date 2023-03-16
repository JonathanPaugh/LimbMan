using UnityEngine;

namespace Jape
{
    public class Todo : SystemData
    {
        protected new static string Path => IO.JoinPath(SystemPath, "Todos");

        public string label;
        [Multiline(9)] public string text;
    }
}