using UnityEngine;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    public static class MovementSurfaceHelper
    {
        // Minimum upward component of the surface normal for it to be considered a steep slope.
        private const float minSteepSlopeUp = 0.173f;

        // Minimum upward component of the surface normal for it to be considered a vertical wall.
        private const float minWallUp = -minSteepSlopeUp;

        // Minimum upward component of the surface normal for it to be considered a sloped ceiling.
        private const float minSlopedCeilingUp = -0.99f;

        public static MovementSurface GetMovementSurface(in Vector3 normal, in Vector3 upDirection, float minFloorUp)
        {
            float upComponent = Math.Dot(normal, upDirection);

            if (upComponent > minFloorUp)
            {
                return MovementSurface.Floor;
            }
            else if (upComponent > minSteepSlopeUp)
            {
                return MovementSurface.SteepSlope;
            }
            else if (upComponent > minWallUp)
            {
                return MovementSurface.Wall;
            }
            else if (upComponent > minSlopedCeilingUp)
            {
                return MovementSurface.SlopedCeiling;
            }
            else
            {
                return MovementSurface.FlatCeiling;
            }
        }
    }
}
