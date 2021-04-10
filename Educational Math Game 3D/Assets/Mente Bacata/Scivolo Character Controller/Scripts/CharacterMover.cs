//#define MB_DEBUG

using System.Collections.Generic;
using UnityEngine;
using MenteBacata.ScivoloCharacterController.Internal;
using static MenteBacata.ScivoloCharacterController.Internal.Math;
using static MenteBacata.ScivoloCharacterController.Internal.OverlapResolver;
using static MenteBacata.ScivoloCharacterController.Internal.FloorAbovePointChecker;
using static MenteBacata.ScivoloCharacterController.Internal.CapsuleSweepTester;
using static MenteBacata.ScivoloCharacterController.Internal.MovementSurfaceHelper;
using static MenteBacata.ScivoloCharacterController.Internal.GeometricTests;

namespace MenteBacata.ScivoloCharacterController
{
    [RequireComponent(typeof(CharacterCapsule))]
    public partial class CharacterMover : MonoBehaviour
    {
        [Tooltip("If true, it uses a dedicated movement method for walking or in general for any movement on feet. In walk mode it can climb steps, clamps to floor and prevents climbing steep slope.")]
        public bool isInWalkMode = true;

        [Range(25f, 75f)]
        [Tooltip("Maximum angle a slope can have in order to be considered floor.")]
        public float maxFloorAngle = 45f;

        [Min(0f)]
        [Tooltip("Maximum height of climbable step.")]
        public float maxStepHeight = 0.3f;

        [Tooltip("Allow the character to climb slopes which exceed the maximum floor angle. Only if not in walk mode.")]
        public bool canClimbSteepSlope = false;

        [Min(0f)]
        [Tooltip("Small distance padding from the collider surface within which contacts are detected.")]
        public float contactOffset = 0.01f;

        private const int maxMoveLoopIterations = 5;

        private const int maxStepDownLoopIterations = 2;

        // Maximum allowed move distance when starting with overlaps in relation to the length of the capsule radius.
        private const float maxMoveDistanceOverlapOverRadius = 0.5f;
        
        // Maximum distance between two contacts for them to be considered near in relation to the length of the capsule radius.
        private const float maxNearContactDistanceOverRadius = 0.05f;

        private MovementProjector movementProjector;

        private LayerMask collisionMask;

        private CharacterCapsule capsule;

        private List<MoveContact> moveContacts;

        // Minimum up component of the ground normal for it to be considered floor.
        private float minFloorUp;

        private float capsuleHeight;

        private float capsuleRadius;

        private Vector3 upDirection;

        private Vector3 toCapsuleLowerCenter;

        private Vector3 toCapsuleUpperCenter;
        
        private void Awake()
        {
            capsule = GetComponent<CharacterCapsule>();
            collisionMask = gameObject.CollisionMask();
        }

        /// <summary>
        /// Moves the character according to the movement settings trying to fulfill the desired movement as close as it can, if a 
        /// MoveContact list is provided it populates the list with all the contact informations it have found during the movement.
        /// </summary>
        public void Move(Vector3 desiredMovement, List<MoveContact> moveContacts = null)
        {
            Initialize(moveContacts);

            Vector3 currentPosition = transform.position;

            if (!TryResolveCapsuleOverlap(capsule, currentPosition, contactOffset, collisionMask, out currentPosition))
            {
                desiredMovement = Vector3.ClampMagnitude(desiredMovement, maxMoveDistanceOverlapOverRadius * capsuleRadius);
            }

            Vector3 beforeMovePosition = currentPosition;
            Move(ref currentPosition, desiredMovement);

            // If it ends up in a position where it can't resolve overlap, it reverts back to the position prior the move loop.
            // The only movement is given by the first overlap resolve.
            if (!TryResolveCapsuleOverlap(capsule, currentPosition, contactOffset, collisionMask, out currentPosition))
            {
                currentPosition = beforeMovePosition;
                moveContacts?.Clear();
            }

            transform.position = currentPosition;
        }
        
        private void Initialize(List<MoveContact> moveContacts)
        {
            maxFloorAngle = Mathf.Clamp(maxFloorAngle, 25f, 75f);
            minFloorUp = Mathf.Cos(Mathf.Deg2Rad * maxFloorAngle);
            capsuleHeight = capsule.Height;
            capsuleRadius = capsule.Radius;
            upDirection = capsule.UpDirection;
            toCapsuleLowerCenter = capsule.ToLowerHemisphereCenter;
            toCapsuleUpperCenter = capsule.ToUpperHemisphereCenter;
            movementProjector = new MovementProjector(upDirection, minFloorUp, !canClimbSteepSlope || isInWalkMode);
            this.moveContacts = moveContacts;
            this.moveContacts?.Clear();
        }

        private void Move(ref Vector3 position, in Vector3 desiredMovement)
        {
            if (isInWalkMode)
            {
                MoveForWalkMode(ref position, desiredMovement);
            }
            else
            {
                DoMoveLoop(ref position, desiredMovement, maxMoveLoopIterations);
            }
        }

        private void MoveForWalkMode(ref Vector3 position, Vector3 desiredMovement)
        {
            Vector3 initialPosition = position;
            bool clampToFloor = ShouldClampToFloor(desiredMovement);
            if (clampToFloor)
            {
                AdjustMovementForMaxFloorDescent(ref desiredMovement);
            }

            DoMoveLoop(ref position, desiredMovement, maxMoveLoopIterations);

            Vector3 movementMade = position - initialPosition;
            float distanceLeftDown = Mathf.Max(Dot(movementMade - desiredMovement, upDirection), 0f);
            float maxStepDownDistance = distanceLeftDown;

            if (clampToFloor)
            {
                float stepHeightLeft = Mathf.Max(maxStepHeight + Dot(movementMade, upDirection), 0f);
                maxStepDownDistance = Mathf.Max(maxStepDownDistance, stepHeightLeft);
            }

            DoStepDownLoop(ref position, maxStepDownDistance, distanceLeftDown, maxStepDownLoopIterations);
        }

        private bool ShouldClampToFloor(in Vector3 desiredMovement)
        {
            return Dot(desiredMovement, upDirection) < epsilon;
        }

        // Adjusts the vertical component of the desired movement so that it doesn't go above the floor at its max angle.
        private void AdjustMovementForMaxFloorDescent(ref Vector3 desiredMovement)
        {
            Vector3 desiredMovementHorizontal = ProjectOnPlane(desiredMovement, upDirection);
            float maxFloorDescent = Mathf.Tan(Mathf.Deg2Rad * maxFloorAngle) * Magnitude(desiredMovementHorizontal);

            if (Dot(desiredMovement, upDirection) > -maxFloorDescent)
            {
                desiredMovement = desiredMovementHorizontal - maxFloorDescent * upDirection;
            }
        }

        private void DoMoveLoop(ref Vector3 position, Vector3 desiredMovement, int maxIterations)
        {
            Vector3 initialPosition = position;
            Vector3 movementToMake = desiredMovement;

            ContactInfo currentContact = new ContactInfo();
            bool hasCurrentContact = false;

            for (int i = 0; i < maxIterations; i++)
            {
                if (IsCircaZero(movementToMake) || (isInWalkMode && IsCircaZero(ProjectOnPlane(movementToMake, upDirection))))
                {
                    break;
                }

                SweepCapsuleAndUpdateCurrentContact(ref position, movementToMake, ref hasCurrentContact, ref currentContact);

                movementToMake = (initialPosition - position) + desiredMovement;

                if (hasCurrentContact)
                {
                    if (isInWalkMode && IsClimbableStep(position, currentContact.position, currentContact.normal))
                    {
                        if (IsMovementHeadedTowardsSurface(movementToMake, currentContact.normal))
                        {
                            DoStepUp(ref position, maxStepHeight, out float steppedUpDistance);
                            hasCurrentContact = IsCircaZero(steppedUpDistance);
                        }

                        AdjustDesiredMovementForClimbableStep(ref desiredMovement, position - initialPosition);
                        movementToMake = (initialPosition - position) + desiredMovement;
                    }

                    if (hasCurrentContact)
                    {
                        HandleProjectionOnContactSurface(ref movementToMake, currentContact);
                    }
                }
            }
        }

        private bool IsMovementHeadedTowardsSurface(in Vector3 movement, in Vector3 surfaceNormal)
        {
            return Dot(ProjectOnPlane(movement, upDirection), surfaceNormal) < 0f;
        }

        // Adjusts the desired movement so that it is not lower than the movement made.
        private void AdjustDesiredMovementForClimbableStep(ref Vector3 desiredMovement, in Vector3 movementMade)
        {
            float heightDifference = Dot(movementMade - desiredMovement, upDirection);

            if (heightDifference < 0f)
                return;

            desiredMovement += heightDifference * upDirection;
        }

        private void DoStepUp(ref Vector3 position, float maxDistance, out float distanceMade)
        {
            if (maxDistance <= 0f)
            {
                distanceMade = 0f;
                return;
            }

            if (SweepTest(position, upDirection, maxDistance, out RaycastHit hit))
            {
                position += hit.distance * upDirection;
                distanceMade = hit.distance;
            }
            else
            {
                position += maxDistance * upDirection;
                distanceMade = maxDistance;
            }
        }

        // Iteratively slides down until it completes the movement or it reaches the floor. If at the end it hasn't reached the floor then
        // it goes down using at most the max non floor distance.
        private void DoStepDownLoop(ref Vector3 position, float maxFloorDistance, float maxNonFloorDistance, int maxIterations)
        {
            Vector3 initialPosition = position;
            Vector3 desiredMovement = -maxFloorDistance * upDirection;
            Vector3 movementToMake = desiredMovement;

            // D parameter for the equation of the horizontal plane at max non floor distance below the initial position.
            float maxNonFloorPlaneD = Dot(initialPosition, upDirection) - maxNonFloorDistance;

            ContactInfo currentContact = new ContactInfo();
            bool hasCurrentContact = false;

            bool hasFoundFloor = false;
            bool isPastMaxNonFloorDistance = false;
            Vector3 positionAtMaxNonFloorDistance = new Vector3();
            int contactsCountAtMaxNonFloorDistance = 0;

            for (int i = 0; i < maxIterations; i++)
            {
                if (IsCircaZero(movementToMake))
                {
                    break;
                }

                Vector3 positionBeforeSweep = position;

                SweepCapsuleAndUpdateCurrentContact(ref position, movementToMake, ref hasCurrentContact, ref currentContact);

                movementToMake = (initialPosition - position) + desiredMovement;

                if (!isPastMaxNonFloorDistance)
                {
                    if (CheckLineAndPlaneIntersection(positionBeforeSweep, position, upDirection, maxNonFloorPlaneD, out Vector3 intersectionPoint))
                    {
                        positionAtMaxNonFloorDistance = intersectionPoint;
                        isPastMaxNonFloorDistance = true;
                        contactsCountAtMaxNonFloorDistance = hasCurrentContact ? moveContacts.Count - 1 : moveContacts.Count;                        
                    }
                }

                if (hasCurrentContact)
                {
                    if (IsPointOnFloor(currentContact.position, currentContact.normal))
                    {
                        hasFoundFloor = true;
                        break;
                    }

                    HandleProjectionOnContactSurface(ref movementToMake, currentContact);
                }

                if (Dot(movementToMake, upDirection) > 0f)
                {
                    break;
                }
            }

            if (isPastMaxNonFloorDistance && !hasFoundFloor)
            {
                position = positionAtMaxNonFloorDistance;

                // Removes all contacts found past max non floor distance.
                if (moveContacts.Count > contactsCountAtMaxNonFloorDistance)
                    moveContacts.RemoveRange(contactsCountAtMaxNonFloorDistance, moveContacts.Count - contactsCountAtMaxNonFloorDistance);
            }
        }

        private bool IsPointOnFloor(in Vector3 point, in Vector3 normal)
        {
            return GetMovementSurface(normal, upDirection, minFloorUp) == MovementSurface.Floor ||
                CheckFloorAbovePoint(point, capsuleRadius, capsuleHeight, minFloorUp, upDirection, collisionMask, capsule.Collider, out _);
        }

        // Sweeps the capsule along the movement direction until it either finds a contact or completes the movement.
        private void SweepCapsuleAndUpdateCurrentContact(ref Vector3 position, in Vector3 movement, ref bool hasCurrentContact, ref ContactInfo currentContact)
        {
            MagnitudeAndDirection(movement, out float distance, out Vector3 direction);

            if (SweepTest(position, direction, distance, out RaycastHit hit))
            {
                moveContacts?.Add(new MoveContact(hit.point, hit.normal, hit.collider));

                position += hit.distance * direction;

                currentContact = new ContactInfo
                {
                    position = hit.point,
                    normal = hit.normal,
                    nearNormal = currentContact.normal,
                    hasNearNormal = hasCurrentContact && hit.distance < maxNearContactDistanceOverRadius * capsuleRadius,
                    fromDirection = direction
                };

                hasCurrentContact = true;
            }
            else
            {
                position += movement;

                hasCurrentContact = false;
            }
        }

        private bool SweepTest(in Vector3 position, in Vector3 direction, float maxDistance, out RaycastHit hit)
        {
            return CapsuleSweepTestWithBuffer(position + toCapsuleLowerCenter, position + toCapsuleUpperCenter, capsuleRadius, direction, 
                maxDistance, contactOffset, collisionMask, capsule.Collider, out hit);
        }
        
        // Check if a contact can be considered a step and if the character can climb it.
        private bool IsClimbableStep(in Vector3 position, in Vector3 contactPosition, in Vector3 contactNormal)
        {
            var surface = GetMovementSurface(contactNormal, upDirection, minFloorUp);

            if (surface != MovementSurface.SteepSlope && surface != MovementSurface.Wall)
                return false;

            float contactHeightFromCapsuleBottom = Dot((contactPosition - position) - toCapsuleLowerCenter, upDirection) + capsuleRadius;

            if (contactHeightFromCapsuleBottom >= maxStepHeight)
                return false;

            if (CheckFloorAbovePoint(contactPosition, capsuleRadius, capsuleHeight, minFloorUp, upDirection, collisionMask, capsule.Collider, out RaycastHit floorHit))
            {
                float floorHeightFromCapsuleBottom = Dot((floorHit.point - position) - toCapsuleLowerCenter, upDirection) + capsuleRadius;

                return floorHeightFromCapsuleBottom < maxStepHeight;
            }

            return false;
        }

        // Projects the movement on contact surface, but only if pointing into it. It handles the projection on the intersection of two 
        // surfaces if the contact has a near surface.
        private void HandleProjectionOnContactSurface(ref Vector3 movement, in ContactInfo contact)
        {
            Vector3 originalMovement = movement;

            // Only projects if movement points into the surface.
            if (Dot(originalMovement, contact.normal) < 0f)
            {
                movement = movementProjector.ProjectOnSurface(originalMovement, contact.normal);
            }

            if (!contact.hasNearNormal)
                return;

            // If then points into the near surface...
            if (Dot(movement, contact.nearNormal) < 0f)
            {
                // ...it projects the original movement on the intersection of the two surface.
                if (movementProjector.TryProjectOnSurfacesIntersection(originalMovement, contact.normal, contact.nearNormal, out Vector3 movementOnIntersection))
                {
                    movement = movementOnIntersection;
                }
            }
        }
        
        private struct ContactInfo
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector3 nearNormal;
            public bool hasNearNormal;
            public Vector3 fromDirection;
        }
    }
}
