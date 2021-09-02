namespace Jape
{
    public static class Direction
    {
        public enum Cardinal { None=0, Up=1, Right=2, Down=3, Left=4 };
        public enum Horizontal { None=0, Left=-1, Right=1 };
        public enum Vertical { None=0, Down=-1, Up=1 };
        public enum Relative { None=0, Backward=-1, Forward=1 }

        public static Cardinal ToCardinal(this Horizontal direction)
        {
            switch (direction)
            {
                default: return Cardinal.None;
                case Horizontal.Right: return Cardinal.Right;
                case Horizontal.Left: return Cardinal.Left;
            }
        }

        public static Cardinal ToCardinal(this Vertical direction)
        {
            switch (direction)
            {
                default: return Cardinal.None;
                case Vertical.Up: return Cardinal.Up;
                case Vertical.Down: return Cardinal.Down;
            }
        }

        public static Horizontal ToHorizontal(this Cardinal direction)
        {
            switch (direction)
            {
                default: return Horizontal.None;
                case Cardinal.Right: return Horizontal.Right;
                case Cardinal.Left: return Horizontal.Left;
            }
        }

        public static Vertical ToVertical(this Cardinal direction)
        {
            switch (direction)
            {
                default: return Vertical.None;
                case Cardinal.Up: return Vertical.Up;
                case Cardinal.Down: return Vertical.Down;
            }
        }
    }
}
