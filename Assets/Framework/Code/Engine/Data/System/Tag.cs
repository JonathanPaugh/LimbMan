namespace Jape
{
    public class Tag : SystemData
    {
        protected new static string Path => IO.JoinPath(SystemPath, "Tags");
        public static Tag Find(string name) { return Find<Tag>(name); }
    }
}