namespace Jape
{
    public abstract class SettingsData : SystemData
    {
        protected new static string Path => IO.JoinPath(SystemPath, "Settings");
    }
}