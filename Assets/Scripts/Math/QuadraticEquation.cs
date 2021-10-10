using System;
using System.Collections.Generic;
using System.Text;

namespace MathHelp
{
    class QuadraticEquation
    {
        public static Tuple<float, float> SolveQuadratic(float a, float b, float c)
        {
            float sqrtpart = b * b - 4 * a * c;
            float x, x1, x2;

            if (sqrtpart > 0)
            {
                x1 = (float)(-b + System.Math.Sqrt(sqrtpart)) / (2 * a);
                x2 = (float)(-b - System.Math.Sqrt(sqrtpart)) / (2 * a);

                return Tuple.Create(x1, x2);
            }

            else if (sqrtpart < 0)
            {
                return Tuple.Create(float.NaN, float.NaN);
            }

            else
            {
                x = (-b) / (2 * a);

                return Tuple.Create(x, float.NaN);
            }
        }
    }
}