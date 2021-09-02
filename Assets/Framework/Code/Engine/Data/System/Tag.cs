namespace Jape
{
    public class Tag : SystemData
    {
        protected new static string Path => "System/Resources/Tags";
        public static Tag Find(string name) { return Find<Tag>(name); }
    }
}