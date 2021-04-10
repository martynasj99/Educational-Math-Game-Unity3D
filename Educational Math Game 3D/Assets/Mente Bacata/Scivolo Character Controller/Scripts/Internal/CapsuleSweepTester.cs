//#define MB_DEBUG

using UnityEngine;
using static MenteBacata.ScivoloCharacterController.Internal.ShapeCaster;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    public static class CapsuleSweepTester
    {
        private const float extraDistanceOverBuffer = 8f;

        /// <summary>
        /// It sweeps the capsule along the given direction. The returned hit distance accounts for the buffer distance as if the
        /// capsule stopped at a buffer distance from the hit point surface. Buffer distance is not guaranteed be kept if the
        /// capsule starts inside the buffer distance or the angle between the direction and the hit normal is too small.
        /// </summary>
        public static bool CapsuleSweepTestWithBuffer(in Vector3 lowerCenter, in Vector3 upperCenter, float radius, in Vector3 direction, 
            float maxDistance, float buffer, LayerMask collisionMask, Collider colliderToIgnore, out RaycastHit hit)
        {
            float extraDistance = extraDistanceOverBuffer * buffer;

            if (CapsuleCast(lowerCenter, upperCenter, radius, direction, maxDistance + extraDistance, collisionMask, colliderToIgnore, out hit))
            {
                float dot = Math.Dot(direction, hit.normal);

                if (dot >= 0f)
                {
                    hit.distance -= extraDistance;
                }
                else
                {
                    hit.distance -= Mathf.Min(-buffer / dot, extraDistance);
                }

                if (hit.distance >= maxDistance)
                {
                    return false;
                }

                if (hit.distance < 0f)
                    hit.distance = 0f;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
