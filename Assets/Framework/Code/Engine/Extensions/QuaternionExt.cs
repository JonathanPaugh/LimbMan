using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jape
{
	public static class QuaternionExt
    {
        public static Quaternion SetEulerX(this Quaternion quaternion, float x) { return Quaternion.Euler(x, quaternion.eulerAngles.y, quaternion.eulerAngles.z); }
        public static Quaternion SetEulerY(this Quaternion quaternion, float y) { return Quaternion.Euler(quaternion.eulerAngles.x, y, quaternion.eulerAngles.z); }
        public static Quaternion SetEulerZ(this Quaternion quaternion, float z) { return Quaternion.Euler(quaternion.eulerAngles.x, quaternion.eulerAngles.y, z); }

        public static Quaternion Inverse(this Quaternion quaternion)
        {
            return new Quaternion(-quaternion.x, 
                                  -quaternion.y, 
                                  -quaternion.z, 
                                  -quaternion.w);
        }
    }
}