namespace Jape
{
	public static partial class Serializer
    {
        public static partial class Dynamic
        {
            public class SerialStream
            {
                private DataStream data = new();

                public void Stream<T>(ref T value)
                {
                    data.Stream(ref value);
                }

                public object[] ToArray()
                {
                    return data.ToArray();
                }

                internal void SetData(object[] data) => this.data.SetData(data);
                internal void StartReading() => data.StartReading();
                internal void StartWriting() => data.StartWriting();
                internal void Stop() => data.Stop();

                public static bool CanCatch(SerialStream stream) => stream.data.Count == 0;
                public static void OnRelease(SerialStream stream) => stream.SetData(null);
            }
        }
    }
}