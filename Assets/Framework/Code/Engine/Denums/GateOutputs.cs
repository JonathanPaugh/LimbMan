using System;

// DONT MODIFY DIRECTLY //

namespace Jape
{

public enum GateOutputs 
{ 
OnSuccess = 1, 
OnFail = 2 
};

[Flags]
public enum GateOutputsFlags
{ 
OnSuccess = 1, 
OnFail = 2 
};

}