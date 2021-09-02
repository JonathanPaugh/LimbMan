using UnityEngine;

namespace Jape
{
	public static class Random
    {
        public static UnityEngine.Random.State InitState()
        {
            UnityEngine.Random.InitState(Time.Unix());
            return UnityEngine.Random.state;
        }
        
        public static bool Chance(float percent) { return UnityEngine.Random.value * 100 <= Mathf.Clamp(percent, 0, 100); }

        /// <param name="min">Inclusive</param>
        /// <param name="max">Inclusive</param>
        public static float Float(float min, float max) { return UnityEngine.Random.Range(min, max); }

        /// <param name="min">Inclusive</param>
        /// <param name="max">Inclusive</param>
        public static int Int(int min, int max) { return UnityEngine.Random.Range(min, max + 1); }

        public static bool Bool() { return UnityEngine.Random.Range(0, 2) == 1; }

        public static int Sign()
        {
            if (UnityEngine.Random.Range(0, 2) == 1) { return 1; }
            return -1;
        }

        public static Vector3 Vector(Vector3 minVector, Vector3 maxVector)
        {
            return Vector(minVector.x, 
                          maxVector.x, 
                          minVector.y, 
                          maxVector.y, 
                          minVector.z, 
                          maxVector.z);
        }
        public static Vector3 Vector(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            float x = Float(minX, maxX);
            float y = Float(minY, maxY);
            float z = Float(minZ, maxZ);
            return new Vector3(x, y, z);
        }

        public static Vector3 Direction()
        {
            float x = Float(-1, 1);
            float y = Float(-1, 1);
            float z = Float(-1, 1);
            return new Vector3(x, y, z);
        }
        public static Quaternion Rotation(Quaternion minRotation, Quaternion maxRotation)
        {
            return Rotation(minRotation.eulerAngles.x, 
                            maxRotation.eulerAngles.x, 
                            minRotation.eulerAngles.y, 
                            maxRotation.eulerAngles.y, 
                            minRotation.eulerAngles.z, 
                            maxRotation.eulerAngles.z);
        }
        public static Quaternion Rotation(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            float x = Float(minX, maxX);
            float y = Float(minY, maxY);
            float z = Float(minZ, maxZ);
            return Quaternion.Euler(x, y, z);
        }
    }
}