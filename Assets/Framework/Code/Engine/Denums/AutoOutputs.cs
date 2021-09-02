using System;

// DONT MODIFY DIRECTLY //

namespace Jape
{

public enum AutoOutputs 
{ 
OnInit = 1, 
OnFirst = 2, 
OnFrame = 3, 
OnTick = 4 
};

[Flags]
public enum AutoOutputsFlags
{ 
OnInit = 1, 
OnFirst = 2, 
OnFrame = 4, 
OnTick = 8 
};

}