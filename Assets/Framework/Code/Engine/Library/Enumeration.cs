using System;

namespace Jape
{
	public static class Enumeration
    {
        public static void Repeat(int count, Action action)
        {
            if (count < 1) { return; }
            for (int i = 0; i < count; i++) { action(); }
        }
    }
}