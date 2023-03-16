using System;

// DONT MODIFY DIRECTLY //

namespace Jape
{

public enum BaseOutputs 
{ 
None = 0, 
OnValue = 1, 
OnGet = 2, 
OnTrigger = 3, 
OnTouch = 4, 
OnStay = 5, 
OnLeave = 6, 
OnEnable = 7, 
OnDisable = 8, 
OnDestroy = 9, 
OnLaunch1 = 10, 
OnLaunch2 = 11, 
OnLaunch3 = 12, 
OnLaunch4 = 13 
};

[Flags]
public enum BaseOutputsFlags
{ 
None = 0, 
OnValue = 1, 
OnGet = 2, 
OnTrigger = 4, 
OnTouch = 8, 
OnStay = 16, 
OnLeave = 32, 
OnEnable = 64, 
OnDisable = 128, 
OnDestroy = 256, 
OnLaunch1 = 512, 
OnLaunch2 = 1024, 
OnLaunch3 = 2048, 
OnLaunch4 = 4096 
};

}