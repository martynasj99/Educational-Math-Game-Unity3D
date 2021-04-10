using UnityEngine;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    public static class GizmosUtility
    {
        public readonly static Color defaultColliderColor = new Color(0.568f, 0.956f, 0.545f, 0.752f);

        public static void DrawCircle(Vector3 center, Vector3 normal, float radius, int resolution = 9)
        {
            Vector3 forwardRadius = Vector3.Slerp(normal, -normal, .5f);
            Vector3 rightRadius = Vector3.Cross(normal, forwardRadius);

            forwardRadius *= radius;
            rightRadius *= radius;

            float a = 0f;
            float da = Mathf.PI / (2f * resolution);
            float cosa = 1f;
            float sena = 0f;

            Vector3 p1 = center + cosa * forwardRadius + sena * rightRadius;
            Vector3 p2 = center - cosa * forwardRadius + sena * rightRadius;
            Vector3 p3 = center + cosa * forwardRadius - sena * rightRadius;
            Vector3 p4 = center - cosa * forwardRadius - sena * rightRadius;

            for (int i = 0; i < resolution; i++)
            {
                a += da;
                cosa = Mathf.Cos(a);
                sena = Mathf.Sin(a);

                Vector3 q1 = center + cosa * forwardRadius + sena * rightRadius;
                Vector3 q2 = center - cosa * forwardRadius + sena * rightRadius;
                Vector3 q3 = center + cosa * forwardRadius - sena * rightRadius;
                Vector3 q4 = center - cosa * forwardRadius - sena * rightRadius;

                Gizmos.DrawLine(p1, q1);
                Gizmos.DrawLine(p2, q2);
                Gizmos.DrawLine(p3, q3);
                Gizmos.DrawLine(p4, q4);

                p1 = q1; p2 = q2; p3 = q3; p4 = q4;
            }
        }

        public static void DrawWireCapsule(Vector3 lowerCenter, Vector3 upperCenter, float radius, int resolution = 9)
        {
            int iterations = 4 * resolution;

            Vector3 upDirection = (upperCenter - lowerCenter).normalized;

            if (upDirection.magnitude == 0f)
                upDirection = Vector3.up;

            Quaternion rot = Quaternion.FromToRotation(Vector3.up, upDirection);
            Vector3 upRadius = radius * upDirection;
            Vector3 rightRadius = radius * (rot * Vector3.right);
            Vector3 forwardRadius = radius * (rot * Vector3.forward);

            float a = 0f;
            float cosa = Mathf.Cos(a);
            float sena = Mathf.Sin(a);
            float deltaA = 2f * Mathf.PI / iterations;
            
            Vector3 vRF = cosa * rightRadius + sena * forwardRadius;
            Vector3 vRU = cosa * rightRadius + sena * upRadius;
            Vector3 vFU = cosa * forwardRadius + sena * upRadius;

            for (int i = 0; i < iterations; i++)
            {
                // Every quarter of iterations.
                if (4 * i % iterations == 0)
                    Gizmos.DrawLine(upperCenter + vRF, lowerCenter + vRF);
                
                a += deltaA;
                cosa = Mathf.Cos(a);
                sena = Mathf.Sin(a);

                Vector3 uRF = cosa * rightRadius + sena * forwardRadius;
                Vector3 uRU = cosa * rightRadius + sena * upRadius;
                Vector3 uFU = cosa * forwardRadius + sena * upRadius;

                Gizmos.DrawLine(lowerCenter + vRF, lowerCenter + uRF);
                Gizmos.DrawLine(upperCenter + vRF, upperCenter + uRF);

                if (2 * i < iterations)
                {
                    Gizmos.DrawLine(upperCenter + vRU, upperCenter + uRU);
                    Gizmos.DrawLine(upperCenter + vFU, upperCenter + uFU);
                }
                else
                {
                    Gizmos.DrawLine(lowerCenter + vRU, lowerCenter + uRU);
                    Gizmos.DrawLine(lowerCenter + vFU, lowerCenter + uFU);
                }

                vRF = uRF;
                vRU = uRU;
                vFU = uFU;
            }
        }

        public static void DrawGrid(Vector3 center, Vector3 sideA, Vector3 sideB, int subdivisions = 1)
        {
            Vector3 o = center - 0.5f * (sideB + sideA);
            float subdInv = 1f / subdivisions;

            for (int i = 0; i < subdivisions + 1; i++)
            {
                Gizmos.DrawRay(o + i * subdInv * sideB, sideA);
                Gizmos.DrawRay(o + i * subdInv * sideA, sideB);
            }
        }
    } 
}
