using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jape
{
	public static class VectorExt
    {
        public static Vector2 SetX(this Vector2 vector, float x) { return new Vector2(x, vector.y); }
        public static Vector2 SetY(this Vector2 vector, float y) { return new Vector2(vector.x, y); }

        public static Vector2 Direction(this Vector2 origin, Vector2 target) { return (target - origin).normalized; }
        public static Vector2 Abs(this Vector2 vector) { return new Vector2(vector.x.Abs(), vector.y.Abs()); }

        public static Vector2 Inverse(this Vector2 vector)
        {
            return new Vector2(-vector.x, 
                               -vector.y);
        }

        public static Vector2 Average(this IEnumerable<Vector2> vectors)
        {
            float x = vectors.Select(v => v.x).Average();
            float y = vectors.Select(v => v.y).Average();
            return new Vector2(x, y);
        }

        public static Vector3 SetX(this Vector3 vector, float x) { return new Vector3(x, vector.y, vector.z); }
        public static Vector3 SetY(this Vector3 vector, float y) { return new Vector3(vector.x, y, vector.z); }
        public static Vector3 SetZ(this Vector3 vector, float z) { return new Vector3(vector.x, vector.y, z); }

        public static Vector3 Direction(this Vector3 origin, Vector3 target) { return (target - origin).normalized; }
        public static Vector3 Abs(this Vector3 vector) { return new Vector3(vector.x.Abs(), vector.y.Abs(), vector.z.Abs()); }

        public static Vector3 Inverse(this Vector3 vector)
        {
            return new Vector3(-vector.x, 
                               -vector.y, 
                               -vector.z);
        }

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            float x = vectors.Select(v => v.x).Average();
            float y = vectors.Select(v => v.y).Average();
            float z = vectors.Select(v => v.z).Average();
            return new Vector3(x, y, z);
        }
    }
}