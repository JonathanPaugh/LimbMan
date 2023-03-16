using System;
using Jape;

namespace JapeNet
{
	public static partial class Net
    {
        private static NetMode Mode => NetManager.GetMode();

        public static void Connect(NetMode mode, Action success, Action error) { NetManager.Connect(mode, 0, success, error); }
        public static void Connect(NetMode mode, int timeout = 0) { NetManager.Connect(mode, timeout, null, null); }
        public static void Disconnect() { NetManager.Disconnect(); }

        private static void OfflineAccessError()
        {
            Log.Write("Cannot call this command in offline mode");
        }
    }
}