using Jape;

namespace JapeNet
{
	public static partial class Net
    {
        public static partial class Client
        {
            public static void Invoke(string value, params object[] args)
            {
                if (Mode.IsClient)
                {
                    JapeNet.Client.Client.Send.Invoke(value, args);
                }
                else if (Mode.IsServer)
                {
                    ServerAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            private static void ServerAccessError()
            {
                Log.Write("Server cannot call client commands");
            }
        }
    }
}