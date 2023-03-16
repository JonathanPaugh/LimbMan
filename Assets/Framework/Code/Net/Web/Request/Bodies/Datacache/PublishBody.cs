namespace JapeNet
{
    public partial class Request
    {
        public static partial class Datacache
        {
            public class PublishBody : DatacacheBody
            {
                public string channel;
                public string value;

                public PublishBody(string channel, string value) : base("publish")
                {
                    this.channel = channel;
                    this.value = value;
                }
            }
        }
    }
}