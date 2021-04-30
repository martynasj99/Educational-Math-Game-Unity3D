using UnityEngine;
using static MenteBacata.ScivoloCharacterController.Internal.Math;
using static MenteBacata.ScivoloCharacterController.Internal.MovementSurfaceHelper;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    public struct MovementProjector
    {
        // Minimum cross product squared magnitude between two normals for them to be considered parallel.
        private const float minCrossSqrMagnitude = 0.001f;

        private Vector3 upDirection;

        // Minimum upward component of the surface normal for it to be considered a floor.
        private float minFloorUp;

        private bool preventMovingUpSteepSlope;


        public MovementProjector(in Vector3 upDirection, float minFloorUp, bool preventMovingUpSteepSlope)
        {
            this.upDirection = upDirection;
            this.minFloorUp = minFloorUp;
            this.preventMovingUpSteepSlope = preventMovingUpSteepSlope;
        }

        /// <summary>
        /// It projects the movement on the surface based on its type and according to movement rules.
        /// </summary>
        public Vector3 ProjectOnSurface(in Vector3 movement, in Vector3 normal)
        {
            Vector3 projectedMovement;

            switch (GetMovementSurface(normal, upDirection, minFloorUp))
            {
                case MovementSurface.Floor:
                    projectedMovement = ProjectOnPlane(movement, normal, upDirection);
                    break;
                case MovementSurface.SteepSlope:
                case MovementSurface.Wall:
                    projectedMovement = ProjectOnPlane(movement, normal);

                    if (preventMovingUpSteepSlope && Dot(projectedMovement, upDirection) > 0f)
                    {
                        projectedMovement = ProjectOnVector(movement, Cross(normal, upDirection));
                    }
                    break;
                case MovementSurface.SlopedCeiling:
                case MovementSurface.FlatCeiling:
                default:
                    projectedMovement = ProjectOnPlane(movement, normal);
                    break;
            }

            return projectedMovement;
        }

        /// <summary>
        /// It projects the movement on the intersection of the two surfaces based on the combination of their types. 
        /// It returns false if the two normals are nearly parallel and it couldn't compute the projected vector.
        /// </summary>
        public bool TryProjectOnSurfacesIntersection(in Vector3 movement, in Vector3 normal1, in Vector3 normal2, out Vector3 projectedVector)
        {
            Vector3 intersectionDirection = Cross(normal1, normal2);

            if (intersectionDirection.sqrMagnitude < minCrossSqrMagnitude)
            {
                projectedVector = new Vector3();
                return false;
            }

            intersectionDirection = Normalized(intersectionDirection);

            var surface1 = GetMovementSurface(normal1, upDirection, minFloorUp);
            var surface2 = GetMovementSurface(normal2, upDirection, minFloorUp);

            if (surface1 == MovementSurface.Floor || surface2 == MovementSurface.Floor)
            {
                // It's not possible in general to keep the horizontal movement here, but projecting on the horizontal 
                // plane first helps.
                projectedVector = ProjectOnDirection(ProjectOnPlane(movement, upDirection), intersectionDirection);
            }
            else if (surface1 == MovementSurface.SteepSlope || surface2 == MovementSurface.SteepSlope
                || surface1 == MovementSurface.Wall || surface2 == MovementSurface.Wall)
            {
                projectedVector = ProjectOnDirection(movement, intersectionDirection);

                if (preventMovingUpSteepSlope && Dot(projectedVector, upDirection) > 0f)
                {
                    // If it points upward it simply suppresses the movement.
                    projectedVector = new Vector3(0f, 0f, 0f);
                }
            }
            else // If both surface are ceilings, no matter if flat or sloped.
            {
                projectedVector = ProjectOnDirection(movement, intersectionDirection);
            }

            return true;
        }
    }
}
