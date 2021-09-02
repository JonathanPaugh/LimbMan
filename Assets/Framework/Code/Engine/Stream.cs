using UnityEngine;

namespace Jape
{
	public abstract class Stream
    {
        public enum Mode { None, Reading, Writing }

        protected Mode mode = Mode.None;

        public bool IsReading() => mode == Mode.Reading;
        public bool IsWriting() => mode == Mode.Writing;

        public void StartReading()
        {
            if (IsReading() || IsWriting()) { this.Log().Response("Stream is already running"); return; }
            mode = Mode.Reading;
        }

        public void StartWriting()
        {
            if (IsReading() || IsWriting()) { this.Log().Response("Stream is already running"); return; }
            mode = Mode.Writing;
        }

        public abstract void Stop();
    }
}