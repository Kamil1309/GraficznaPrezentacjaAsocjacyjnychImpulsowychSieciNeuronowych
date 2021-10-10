using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MathHelp
{
    /// <summary>
    ///A class to calculate a line passing through two points
    ///</summary>
    class LineEquation
    {
        public float a { get; set; }
        public float b { get; set; }

        public LineEquation(float a = 0, float b = 0){
            this.a = a;
            this.b = b;
        }
        
        /// <summary>Method to count y(x) on the line described by the coefficients</summary>
        public float CountY(float x){
            float y = this.a * x + this.b;

            return y;
        }

        /// <summary>Method to count coefficients of line passing through two points described by input parameters</summary>
        public void CountCoefficients(float x1, float y1, float x2, float y2){
            this.a = (float)((y1 - y2)/(x1 - x2));
            this.b = y1 - (y1 - y2)/(x1 - x2)*x1;
        }
    }
}