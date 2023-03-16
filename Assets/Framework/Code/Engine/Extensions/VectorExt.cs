using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

        public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max)
        {
            return new Vector2(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y)
            );
        }

        public static Vector2 Clamp01(this Vector2 vector)
        {
            return new Vector2(
                Mathf.Clamp(vector.x, 0, 1),
                Mathf.Clamp(vector.y, 0, 1)
            );
        }

        public static Vector2 Rescale(this Vector2 vector, float oldMin, float oldMax, float newMin, float newMax)
        {
            return new Vector2(
                Math.Rescale(vector.x, oldMin, oldMax, newMin, newMax),
                Math.Rescale(vector.y, oldMin, oldMax, newMin, newMax)
            );
        }

        public static Vector2 Rescale(this Vector2 vector, Vector2 oldMin, Vector2 oldMax, Vector2 newMin, Vector2 newMax)
        {
            return new Vector2(
                Math.Rescale(vector.x, oldMin.x, oldMax.x, newMin.x, newMax.x),
                Math.Rescale(vector.y, oldMin.y, oldMax.y, newMin.y, newMax.y)
            );
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

        public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y),
                Mathf.Clamp(vector.z, min.z, max.z)
            );
        }

        public static Vector3 Clamp01(this Vector3 vector)
        {
            return new Vector3(
                Mathf.Clamp(vector.x, 0, 1),
                Mathf.Clamp(vector.y, 0, 1),
                Mathf.Clamp(vector.z, 0, 1)
            );
        }

        public static Vector3 Rescale(this Vector3 vector, float oldMin, float oldMax, float newMin, float newMax)
        {
            return new Vector3(
                Math.Rescale(vector.x, oldMin, oldMax, newMin, newMax),
                Math.Rescale(vector.y, oldMin, oldMax, newMin, newMax),
                Math.Rescale(vector.z, oldMin, oldMax, newMin, newMax)
            );
        }

        public static Vector3 Rescale(this Vector3 vector, Vector3 oldMin, Vector3 oldMax, Vector3 newMin, Vector3 newMax)
        {
            return new Vector3(
                Math.Rescale(vector.x, oldMin.x, oldMax.x, newMin.x, newMax.x),
                Math.Rescale(vector.y, oldMin.y, oldMax.y, newMin.y, newMax.y),
                Math.Rescale(vector.z, oldMin.z, oldMax.z, newMin.z, newMax.z)
            );
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