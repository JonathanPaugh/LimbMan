using Jape;

namespace Game
{
	public class Game : Jape.Game
    {
        public static Player Player => GameManager.Instance.Player;
        public static UI UI => GameManager.Instance.UI;
        public static GameManager.SpeedTimer Timer => GameManager.Instance.Timer;

        public static GameSettings Settings => Framework.Settings<GameSettings>();

        public new static void Pause()
        {
            if (pauseTimescale > -1)
            {
                Log.Warning("Game is already paused");
                return;
            }

            Timer.Pause();
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
            Timer.Resume();
        }
    }
}