using UnityEngine;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    public static class ShapeCaster
    {
        private const int maxHitCount = 20;

        private readonly static RaycastHit[] hits = new RaycastHit[maxHitCount];

        public static bool RayCast(in Vector3 origin, in Vector3 direction, float maxDistance, LayerMask collisionMask, Collider colliderToIgnore, out RaycastHit hit)
        {
            return TryGetCloserHit(Physics.RaycastNonAlloc(origin, direction, hits, maxDistance, collisionMask, QueryTriggerInteraction.Ignore),
                                colliderToIgnore,
                                out hit,
                                out _);
        }

        public static bool RayCast(in Vector3 origin, in Vector3 direction, float maxDistance, LayerMask collisionMask, Collider colliderToIgnore, out RaycastHit hit, out bool hasInitialOverlap)
        {
            return TryGetCloserHit(Physics.RaycastNonAlloc(origin, direction, hits, maxDistance, collisionMask, QueryTriggerInteraction.Ignore),
                                colliderToIgnore,
                                out hit,
                                out hasInitialOverlap);
        }

        public static bool SphereCast(in Vector3 center, float radius, in Vector3 direction, float maxDistance, LayerMask collisionMask, Collider colliderToIgnore, out RaycastHit hit)
        {
            return TryGetCloserHit(Physics.SphereCastNonAlloc(center, radius, direction, hits, maxDistance, collisionMask, QueryTriggerInteraction.Ignore),
                                colliderToIgnore,
                                out hit,
                                out _);
        }

        public static bool SphereCast(in Vector3 center, float radius, in Vector3 direction, float maxDistance, LayerMask collisionMask, Collider colliderToIgnore, out RaycastHit hit, out bool hasInitialOverlap)
        {
            return TryGetCloserHit(Physics.SphereCastNonAlloc(center, radius, direction, hits, maxDistance, collisionMask, QueryTriggerInteraction.Ignore),
                                colliderToIgnore,
                                out hit,
                                out hasInitialOverlap);
        }

        public static bool CapsuleCast(in Vector3 lowerCenter, in Vector3 upperCenter, float radius, in Vector3 direction, float maxDistance, LayerMask collisionMask, Collider colliderToIgnore, out RaycastHit hit)
        {
            return TryGetCloserHit(Physics.CapsuleCastNonAlloc(lowerCenter, upperCenter, radius, direction, hits, maxDistance, collisionMask, QueryTriggerInteraction.Ignore),
                                colliderToIgnore,
                                out hit,
                                out _);
        }

        public static bool CapsuleCast(in Vector3 lowerCenter, in Vector3 upperCenter, float radius, in Vector3 direction, float maxDistance, LayerMask collisionMask, Collider colliderToIgnore, out RaycastHit hit, out bool hasInitialOverlap)
        {
            return TryGetCloserHit(Physics.CapsuleCastNonAlloc(lowerCenter, upperCenter, radius, direction, hits, maxDistance, collisionMask, QueryTriggerInteraction.Ignore),
                                colliderToIgnore,
                                out hit,
                                out hasInitialOverlap);
        }

        private static bool TryGetCloserHit(int hitCount, Collider colliderToIgnore, out RaycastHit hit, out bool foundZeroDistanceHit)
        {
            int closestHitIndex = -1;
            float closestHitDistance = float.MaxValue;
            foundZeroDistanceHit = false;

            for (int i = 0; i < hitCount; i++)
            {
                ref RaycastHit currentHit = ref hits[i];

                if (currentHit.collider == colliderToIgnore)
                    continue;

                if (currentHit.distance <= 0f)
                {
                    foundZeroDistanceHit = true;
                    continue;
                }

                if (currentHit.distance < closestHitDistance)
                {
                    closestHitDistance = currentHit.distance;
                    closestHitIndex = i;
                }
            }

            if (closestHitIndex < 0)
            {
                hit = default;
                return false;
            }

            hit = hits[closestHitIndex];
            return true;
        }
    }
}
