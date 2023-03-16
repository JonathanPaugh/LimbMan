namespace Jape
{
    public class Logger
    {
        private object instance;

        private bool active = true;
        private bool diagonstic;

        public Logger(object instance) { this.instance = instance; }

        public object On() { active = true; return instance; }
        public object Off() { active = false; return instance; }

        public object ToggleDiagnostics()
        {
            diagonstic = !diagonstic;
            return instance;
        }

        public object Response(object line)
        {
            if (!active) { return instance; }

            if (Game.IsBuild)
            {
                Log.Write($"{instance}: {line}");
            } 
            else
            {
                Log.Write($"<color=green><b>{instance}</b></color>: {line}");
            }

            return instance;
        }

        public object Warning(object line)
        {
            if (!active) { return instance; }

            if (Game.IsBuild)
            {
                Log.Warning($"{instance}: {line}");
            } 
            else
            {
                Log.Warning($"<color=orange><b>{instance}</b></color>: {line}");
            }

            return instance;
        }

        public object Diagnostic(object line)
        {
            if (!active) { return instance; }
            if (!diagonstic) { return instance; }

            if (Game.IsBuild)
            {
                Log.Write($"{instance}: {line}");
            } 
            else
            {
                Log.Write($"<color=purple><b>{instance}</b></color>: {line}");
            }

            return instance;
        }

        public object Value(object member, object value)
        {
            if (!active) { return instance; }

            if (Game.IsBuild)
            {
                Log.Write($"{instance}", $"{member}: {value}");
            } 
            else
            {
                Log.Write($"<color=green><b>{instance}</b></color>", $"{member}: <color=purple>{value}</color>");
            }

            return instance;
        }

        public bool HasInstance(object instance) { return this.instance.Equals(instance); }
    }
}