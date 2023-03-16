namespace JapeNet
{
    public partial class Response
    {
        public class InfoBody : Body
        {
            public string[] ips;
            public string[] domains;

            public int getServerCount()
            {
                switch (Jape.Game.IsWeb)
                {
                    default: return domains.Length;
                    case false: return ips.Length;
                }
            }

            public string[] getHosts()
            {
                switch (Jape.Game.IsWeb)
                {
                    default: return domains;
                    case false: return ips;
                }
            }
        }
    }
}