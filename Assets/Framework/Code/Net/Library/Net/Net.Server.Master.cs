using System;

namespace JapeNet
{
	public static partial class Net
    {
        public static partial class Server
        {
            public static class Master
            {
                public static void ServerDestroy(Action response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Master.ServerDestroy()
                                  .Read(_ => response?.Invoke())
                                  .Error(error);
                }
            }
        }
    }
}