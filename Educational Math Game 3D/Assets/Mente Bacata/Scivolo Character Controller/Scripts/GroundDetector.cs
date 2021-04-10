//#define MB_DEBUG

using UnityEngine;
using MenteBacata.ScivoloCharacterController.Internal;
using static MenteBacata.ScivoloCharacterController.Internal.Math;
using static MenteBacata.ScivoloCharacterController.Internal.CapsuleSweepTester;
using static MenteBacata.ScivoloCharacterController.Internal.FloorAbovePointChecker;
using static MenteBacata.ScivoloCharacterController.Internal.ShapeCaster;

namespace MenteBacata.ScivoloCharacterController
{
    [RequireComponent(typeof(CharacterCapsule), typeof(CharacterMover))]
    public class GroundDetector : MonoBehaviour
    {
        [SerializeField]
        [Min(0f)]
        [Tooltip("Small tolerance distance so that ground is detected even if the capsule is not directly touching it but just close enough.")]
        private float tolerance = 0.05f;

        // Spacing between raycasts in relation to the capsule radius. Ray spacing is measured as the distance of the ray origins from 
        // the center of the triangle they form.
        private const float raySpacingOverRadius = 0.6f;

        // Offset of the rays center from an edge in relation to their spacing.
        private const float raysCenterOffsetOverSpacing = 1f;

        private const float extraRaycastDistance = 1f;

        private LayerMask collisionMask;

        private CharacterMover mover;

        private CharacterCapsule capsule;

        private Vector3 upDirection;

        private float minFloorUp;


        private void Awake()
        {
            collisionMask = gameObject.CollisionMask();
            capsule = GetComponent<CharacterCapsule>();
            mover = GetComponent<CharacterMover>();
        }

        /// <summary>
        /// Detects ground below the capsule bottom and retrieves useful info.
        /// </summary>
        public bool DetectGround(out GroundInfo groundInfo)
        {
            upDirection = capsule.UpDirection;
            minFloorUp = Mathf.Cos(Mathf.Deg2Rad * mover.maxFloorAngle);

            if (!CheckGroundContactWithCapsuleBottom(out RaycastHit capsuleBottomHit, out bool isOnFloor))
            {
                groundInfo = default;
                return false;
            }

            if (CheckFloorAbovePoint(capsuleBottomHit.point, capsule.Radius, capsule.Height, minFloorUp, upDirection, collisionMask, capsule.Collider, out RaycastHit upperFloorHit))
            {
                if (isOnFloor)
                {
                    groundInfo = new GroundInfo(
                        capsuleBottomHit.point,
                        // Gets the normal of the flattest floor.
                        Dot(upperFloorHit.normal - capsuleBottomHit.normal, upDirection) > 0f ? upperFloorHit.normal : capsuleBottomHit.normal,
                        capsuleBottomHit.normal,
                        capsuleBottomHit.collider,
                        true);
                }
                else
                {
                    // It can be on the edge of a climbable step, so it casts some rays to check if there is floor below.
                    groundInfo = new GroundInfo(
                        capsuleBottomHit.point,
                        upperFloorHit.normal,
                        capsuleBottomHit.normal,
                        capsuleBottomHit.collider,
                        IsEdgeOfClimbableStep(capsuleBottomHit.point, Cross(upperFloorHit.normal, capsuleBottomHit.normal)));
                }
            }
            else
            {
                groundInfo = new GroundInfo(capsuleBottomHit.point, capsuleBottomHit.normal, capsuleBottomHit.normal, capsuleBottomHit.collider, isOnFloor);
            }

            return true;
        }
        
        private static Vector3 NormalFromThreePoints(in Vector3 point1, in Vector3 point2, in Vector3 point3)
        {
            return Normalized(Cross(point1 - point2, point1 - point3));
        }

        private static void ComputeTriangleVertices(in Vector3 center, in Vector3 forward, in Vector3 right, float size,
            out Vector3 a, out Vector3 b, out Vector3 c)
        {
            const float cos60 = 0.5f;
            const float sen60 = 0.866f;

            a = size * (sen60 * right + -cos60 * forward) + center;
            b = size * (-sen60 * right + -cos60 * forward) + center;
            c = size * forward + center;
        }

        private bool CheckGroundContactWithCapsuleBottom(out RaycastHit hit, out bool isOnFloor)
        {
            float contactOffset = mover.contactOffset;
            float capsuleRadius = capsule.Radius;
            Vector3 capsuleLowerCenter = capsule.LowerHemisphereCenter;
            Vector3 capsuleUpperCenter = capsule.UpperHemisphereCenter;

            // First sweeps the capsule down a bit like it is moving down.
            if (!CapsuleSweepTestWithBuffer(capsuleLowerCenter, capsuleUpperCenter, capsuleRadius, -upDirection, tolerance, contactOffset, collisionMask, capsule.Collider, out hit))
            {
                isOnFloor = false;
                return false;
            }

            if (Dot(hit.normal, upDirection) > minFloorUp)
            {
                isOnFloor = true;
                return true;
            }

            // If it hits something but it's not floor, it does a sphere cast along the surface of the first contact for a small 
            // distance to retrieve a further possible contact.

            // Sphere radius is slightly smaller then capsule radius so that the cast starts in a safe position without overlap.
            float sphereRadius = capsuleRadius - contactOffset;

            if (sphereRadius < epsilon)
            {
                isOnFloor = false;
                return true;
            }

            Vector3 sphereOrigin = hit.point + capsuleRadius * hit.normal;
            Vector3 sphereCastDirection = -Normalized(ProjectOnPlane(upDirection, hit.normal));
            
            // Max distance is calculated so that distance along vertical direction is equal to tolerance.
            float sphereCastMaxDistance = tolerance / -Dot(sphereCastDirection, upDirection);

            if (SphereCast(sphereOrigin, sphereRadius, sphereCastDirection, sphereCastMaxDistance, collisionMask, capsule.Collider, out RaycastHit sphereHit))
            {
                // If the new hit point is lower than the previous...
                if (Dot(sphereHit.point - hit.point, upDirection) < 0f)
                {
                    hit = sphereHit;

                    if (Dot(sphereHit.normal, upDirection) > minFloorUp)
                    {
                        isOnFloor = true;
                        return true;
                    }
                }
            }

            isOnFloor = false;
            return true;
        }

        private bool IsEdgeOfClimbableStep(in Vector3 pointOnEdge, in Vector3 edgeParallel)
        {
            Vector3 edgeForward = Normalized(Cross(edgeParallel, upDirection));
            float raysSpacing = raySpacingOverRadius * capsule.Radius;
            Vector3 raysCenter = pointOnEdge + (raysCenterOffsetOverSpacing * raysSpacing) * edgeForward;

            return CheckFloorWithRaycasts(raysCenter, raysSpacing, mover.maxStepHeight + tolerance, edgeForward);
        }

        // Checks for floor by casting down rays from a group of 4 points, which are the center and the vertices of a triangle.
        private bool CheckFloorWithRaycasts(in Vector3 center, float spacing, float maxFloorDistance, in Vector3 forward)
        {
            // It should be greater than maxFloorDistance, this because even if a point is not on floor could still be used for further 
            // evaluations.
            float maxDistance = maxFloorDistance + extraRaycastDistance;

            Vector3 right = Cross(upDirection, forward);

            // Raycast down from the center O.
            bool oResult = RayCast(center, -upDirection, maxDistance, collisionMask, capsule.Collider, out RaycastHit oHit);

            if (oResult && oHit.distance < maxFloorDistance && Dot(oHit.normal, upDirection) > minFloorUp)
            {
                return true;
            }

            // If it didn't find floor with the first cast, it builds the triangle ABC centered at the origin and casts a ray down from 
            // each vertex.
            ComputeTriangleVertices(center, forward, right, spacing, out Vector3 a, out Vector3 b, out Vector3 c);

            // Raycast down from vertex A.
            bool aResult = RayCast(a, -upDirection, maxDistance, collisionMask, capsule.Collider, out RaycastHit aHit);

            if (aResult && aHit.distance < maxFloorDistance && Dot(aHit.normal, upDirection) > minFloorUp)
            {
                return true;
            }

            // Raycast down from vertex B.
            bool bResult = RayCast(b, -upDirection, maxDistance, collisionMask, capsule.Collider, out RaycastHit bHit);

            if (bResult && bHit.distance < maxFloorDistance && Dot(bHit.normal, upDirection) > minFloorUp)
            {
                return true;
            }

            // Raycast down from vertex C.
            bool cResult = RayCast(c, -upDirection, maxDistance, collisionMask, capsule.Collider, out RaycastHit cHit);

            if (cResult && cHit.distance < maxFloorDistance && Dot(cHit.normal, upDirection) > minFloorUp)
            {
                return true;
            }

            // If it didn't detect floor from single hit points, it groups them into triangles and check their normals.

            // ABC
            if (aResult && bResult && cResult)
            {
                if (aHit.distance < maxFloorDistance || bHit.distance < maxFloorDistance || cHit.distance < maxFloorDistance)
                {
                    if (Dot(NormalFromThreePoints(aHit.point, bHit.point, cHit.point), upDirection) > minFloorUp)
                    {
                        return true;
                    }
                }
            }

            // OAB
            if (oResult && aResult && bResult)
            {
                if (oHit.distance < maxFloorDistance || aHit.distance < maxFloorDistance || bHit.distance < maxFloorDistance)
                {
                    if (Dot(NormalFromThreePoints(oHit.point, aHit.point, bHit.point), upDirection) > minFloorUp)
                    {
                        return true;
                    }
                }
            }

            // OBC
            if (oResult && bResult && cResult)
            {
                if (oHit.distance < maxFloorDistance || bHit.distance < maxFloorDistance || cHit.distance < maxFloorDistance)
                {
                    if (Dot(NormalFromThreePoints(oHit.point, bHit.point, cHit.point), upDirection) > minFloorUp)
                    {
                        return true;
                    }
                }
            }

            // OCA
            if (oResult && cResult && aResult)
            {
                if (oHit.distance < maxFloorDistance || cHit.distance < maxFloorDistance || aHit.distance < maxFloorDistance)
                {
                    if (Dot(NormalFromThreePoints(oHit.point, cHit.point, aHit.point), upDirection) > minFloorUp)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
