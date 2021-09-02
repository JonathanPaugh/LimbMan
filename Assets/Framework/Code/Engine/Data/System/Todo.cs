using UnityEngine;

namespace Jape
{
    public class Todo : SystemData
    {
        protected new static string Path => "System/Resources/Todos";

        public string label;
        [Multiline(9)] public string text;
    }
}