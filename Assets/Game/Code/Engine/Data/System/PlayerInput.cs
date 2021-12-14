using Sirenix.OdinInspector;
using Input = Jape.Input;

namespace Game
{
    public class PlayerInput : Input
    {
        [PropertyOrder(-1)]
        [PropertySpace(0, 8)]
        public bool useTouchControls;

        public TouchControls TouchControls => Game.UI.TouchControls;

        public float? HorizontalStream()
        {
            switch (Jape.Game.IsWeb)
            {
                case true: return GetAction("HorizontalGL").AxisStream();
                case false: return GetAction("Horizontal").AxisStream();
            }
        }

        public float? VerticalStream()
        {
            switch (Jape.Game.IsWeb)
            {
                case true: return GetAction("VerticalGL").AxisStream();
                case false: return GetAction("Vertical").AxisStream();
            }
        }
    }
}