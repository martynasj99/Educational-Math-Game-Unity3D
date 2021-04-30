using UnityEngine;
using static MenteBacata.ScivoloCharacterController.Internal.Math;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    public static class GeometricTests
    {
        /// <summary>
        /// Checks for the intersection between a line and a plane, plane is in the form: x dot n = d.
        /// </summary>
        public static bool CheckLineAndPlaneIntersection(in Vector3 lineStart, in Vector3 lineEnd, in Vector3 planeNormal, float planeD, out Vector3 intersectionPoint)
        {
            float startDotNormal = Dot(lineStart, planeNormal);
            float endDotNormal = Dot(lineEnd, planeNormal);

            if (startDotNormal == planeD)
            {
                intersectionPoint = lineStart;
                return true;
            }
            else if (endDotNormal == planeD)
            {
                intersectionPoint = lineEnd;
                return true;
            }
            else if ((startDotNormal > planeD) ^ (endDotNormal > planeD))
            {
                float t = (planeD - startDotNormal) / (endDotNormal - startDotNormal);

                // Better be safe here...
                if (t <= 0f)
                    intersectionPoint = lineStart;
                else if (t >= 1f)
                    intersectionPoint = lineEnd;
                else
                    intersectionPoint = lineStart + t * (lineEnd - lineStart);

                return true;
            }

            intersectionPoint = default;
            return false;
        }
    }
}
