using UnityEngine;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    /// <summary>
    /// Check if given capsule is overlapping other colliders taking into account a small overlap margin.
    /// Returns true if any collider overlaps the capsule.
    /// </summary>
    public static class OverlapChecker
    {
        private const float overlapMargin = 0.001f;

        private readonly static Collider[] colliders = new Collider[2];

        /// <summary>
        /// Checks if capsule overlaps other colliders, returns true if it does, false otherwise.
        /// </summary>
        public static bool CheckCapsuleOverlap(in Vector3 lowerCenter, in Vector3 upperCenter, float radius, LayerMask collisionMask, Collider colliderToIgnore)
        {
            int colliderCount = Physics.OverlapCapsuleNonAlloc(lowerCenter, upperCenter, radius + overlapMargin, colliders, collisionMask, QueryTriggerInteraction.Ignore);

            return colliderCount > 1 || (colliderCount == 1 && colliders[0] != colliderToIgnore);
        }
    }
}
