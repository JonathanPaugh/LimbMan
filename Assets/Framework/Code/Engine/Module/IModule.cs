namespace Jape
{
	public interface IModule<out T>
    {
        T Start();
        T ForceStart();
        T Stop();
        T Pause();
        T Resume();

        bool IsProcessing();
        bool IsRunning();
        bool IsPaused();
        bool IsComplete();
    }
}