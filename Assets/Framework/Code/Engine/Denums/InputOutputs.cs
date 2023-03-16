using System;

// DONT MODIFY DIRECTLY //

namespace Jape
{

public enum InputOutputs 
{ 
OnPress = 1, 
OnRelease = 2, 
OnHoldFrame = 3, 
OnHoldTick = 4 
};

[Flags]
public enum InputOutputsFlags
{ 
OnPress = 1, 
OnRelease = 2, 
OnHoldFrame = 4, 
OnHoldTick = 8 
};

}