﻿//Dahlberg, Simon och Sahlin, Jesper 2014-01-08

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadNool
{
    static class BallManager
    {
        /// <summary>
        /// Checks for a collision between two balls.
        /// </summary>
        /// <param name="b1">The first ball</param>
        /// <param name="b2">The second ball.</param>
        /// <returns>True if a collision was found, false otherwise.</returns>
        public static bool CheckCollision(Ball b1, Ball b2)
        {
            double deltaX= b2.GetCenter().X - b1.GetCenter().X;
            deltaX *= deltaX;
            double deltaY= b2.GetCenter().Y - b1.GetCenter().Y;
            deltaY *= deltaY;
            double radiSum = b1.Radius + b2.Radius;
            radiSum *= radiSum;

            return (deltaX + deltaY <= radiSum);
        }
    }
}
