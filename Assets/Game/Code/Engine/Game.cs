using Jape;

namespace Game
{
	public class Game : Jape.Game
    {
        public new static void Pause()
        {
            if (pauseTimescale > -1)
            {
                Log.Warning("Game is already paused");
                return;
            }

            GameManager.Instance.Timer.Pause();
            Jape.Game.Pause();
        }

        public new static void Resume()
        {
            if (pauseTimescale < 0)
            {
                Log.Warning("Game is not paused");
                return;
            }

            Jape.Game.Resume();
            GameManager.Instance.Timer.Resume();
        }
    }
}