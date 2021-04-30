using UnityEngine;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    public static class Vector3Extensions
    {
        public static bool IsShorterThan(in this Vector3 v, float length)
        {
            return Math.Dot(v, v) < length * length;
        }

        public static bool IsShorterThan(in this Vector3 v, in Vector3 w)
        {
            return Math.Dot(v, v) < Math.Dot(w, w);
        }

        public static bool IsLongerThan(in this Vector3 v, float length)
        {
            return Math.Dot(v, v) > length * length;
        }

        public static bool IsLongerThan(in this Vector3 v, in Vector3 w)
        {
            return Math.Dot(v, v) > Math.Dot(w, w);
        }
    }
}
