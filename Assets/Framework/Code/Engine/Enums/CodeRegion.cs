using System;

namespace Jape
{
    public enum CodeRegion { Engine = 1, Editor = 2, Net = 3 }
    [Flags] public enum CodeRegionFlags { Engine = 1, Editor = 2, Net = 4 }
}