namespace Jape
{
    public static class NetSideExt
    {
        public static NetSide Opposite(this NetSide side)
        {
            switch (side)
            {
                case NetSide.Clientside: default: return NetSide.Serverside;
                case NetSide.Serverside: return NetSide.Clientside;
            }
        }
    }

    public enum NetSide
    {
        Clientside,
        Serverside
    }
}