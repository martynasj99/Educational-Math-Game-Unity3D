//#define MB_DEBUG

using UnityEngine;
using static MenteBacata.ScivoloCharacterController.Internal.Math;

namespace MenteBacata.ScivoloCharacterController.Internal
{
    /*
     * Given a set of displacement vectors where each resolves an overlap with a single collider, this class provides a method
     * to calculate a single displacement vector which is a good match for all of them.
     * In practice it uses the steepest descent method to approximate the vector x that minimizes the sum of squared errors
     * given by the function: 
     * F(x) = 1/2 Sum (<x, direction_i> - distance_i)^2
     * Where direction_i and distance_i are respectively the direction and the distance of the i-th displacement vector of the 
     * given set.
     */
    public static class DisplacementOfBestFitCalculator
    {
        private const int maxIterations = 10;

        private static readonly float[] errors = new float[OverlapResolver.maxOverlaps];

        /// <summary>
        /// Gets an approximation of the displacement vector of best fit the given a set displacement vectors.
        /// </summary>
        public static Vector3 GetDisplacementOfBestFit(Vector3[] directions, float[] distances, int n)
        {
            Vector3 x = GetInitialGuess(directions, distances, n);

            for (int i = 0; i < maxIterations; i++)
            {
                PopulateErrors(x, directions, distances, n);

                var gradient = GetGradient(directions, n);

                if (IsCircaZero(gradient))
                    break;

                if (TryGetStep(gradient, directions, n, out Vector3 step))
                {
                    x += step;
                }
                else
                    break;
            }

            return x;
        }

        private static Vector3 GetGradientAt(in Vector3 x, Vector3[] directions, float[] distances, int n)
        {
            PopulateErrors(x, directions, distances, n);

            return GetGradient(directions, n);
        }

        private static Vector3 GetInitialGuess(Vector3[] directions, float[] distances, int n)
        {
            var x = new Vector3(0f, 0f, 0f);

            for (int i = 0; i < n; i++)
            {
                x += distances[i] * directions[i];
            }

            return x;
        }

        private static void PopulateErrors(in Vector3 x, Vector3[] directions, float[] distances, int n)
        {
            for (int i = 0; i < n; i++)
            {
                errors[i] = Dot(x, directions[i]) - distances[i];
            }
        }

        private static Vector3 GetGradient(Vector3[] directions, int n)
        {
            var d = new Vector3(0f, 0f, 0f);

            for (int i = 0; i < n; i++)
            {
                d += errors[i] * directions[i];
            }

            return d;
        }

        private static bool TryGetStep(in Vector3 gradient, Vector3[] directions, int n, out Vector3 step)
        {
            float num = 0f, denum = 0f;

            // It's not necessary to normalize, but if the gradient magnitude is small it could leads to very small denum.
            var gradientDirection = Normalized(gradient);

            for (int i = 0; i < n; i++)
            {
                float dot = Dot(gradientDirection, directions[i]);

                num += dot * errors[i];

                denum += dot * dot;
            }

            if (IsCircaZero(denum))
            {
                step = new Vector3();
                return false;
            }

            step = (-num / denum) * gradientDirection;
            return true;
        }
    }
}
