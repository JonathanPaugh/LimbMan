using System;

// DONT MODIFY DIRECTLY //

namespace Jape
{

public enum CaseOutputs 
{ 
OnNone = 1, 
OnAny = 2, 
OnMatch = 3, 
OnCase1 = 4, 
OnCase2 = 5, 
OnCase3 = 6, 
OnCase4 = 7, 
OnCase5 = 8, 
OnCase6 = 9, 
OnCase7 = 10, 
OnCase8 = 11, 
OnCase9 = 12, 
OnCase10 = 13, 
OnCase11 = 14, 
OnCase12 = 15, 
OnCase13 = 16, 
OnCase14 = 17, 
OnCase15 = 18, 
OnCase16 = 19 
};

[Flags]
public enum CaseOutputsFlags
{ 
OnNone = 1, 
OnAny = 2, 
OnMatch = 4, 
OnCase1 = 8, 
OnCase2 = 16, 
OnCase3 = 32, 
OnCase4 = 64, 
OnCase5 = 128, 
OnCase6 = 256, 
OnCase7 = 512, 
OnCase8 = 1024, 
OnCase9 = 2048, 
OnCase10 = 4096, 
OnCase11 = 8192, 
OnCase12 = 16384, 
OnCase13 = 32768, 
OnCase14 = 65536, 
OnCase15 = 131072, 
OnCase16 = 262144 
};

}