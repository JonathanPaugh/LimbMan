using System;

// DONT MODIFY DIRECTLY //

namespace Jape
{

public enum Primitive 
{ 
String = 1, 
Bool = 2, 
Float = 3, 
Int = 4 
};

[Flags]
public enum PrimitiveFlags
{ 
String = 1, 
Bool = 2, 
Float = 4, 
Int = 8 
};

}