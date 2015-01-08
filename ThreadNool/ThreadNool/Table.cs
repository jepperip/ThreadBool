//Dahlberg, Simon och Sahlin, Jesper 2014-01-08

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ThreadNool
{
    static class Table
    {
        static readonly bool drawHitboxes = true;
        static Texture2D texture;
        static Rectangle drawRectangle;
        static List<Rectangle> holes = new List<Rectangle>();
        static List<Rectangle> borders = new List<Rectangle>();
        static Rectangle topLeft, topRight, bottomLeft, bottomRight, left, right, cornerTopLeft, cornerTopRight, cornerBottomLeft, cornerBottomRight, centerTop, centerBottom;
        static readonly int cornerSize = (int)(Game1.ScreenWidth * 0.08951);
        static readonly int centerWidth = (int)(Game1.ScreenWidth * 0.0505115089514066);
        static readonly int centerHeight = (int)(Game1.ScreenWidth * 0.0351662404092072);
        static readonly int centerPosition = (int)(Game1.ScreenWidth / 2 - centerWidth / 2);
        static readonly int borderWidth = (int)(Game1.ScreenWidth * 0.0485933503836317); 
       
        /// <summary>
        /// Creates the table
        /// </summary>
       
        public static void Setup()
        {
            texture = Game1.TableTexture;

            drawRectangle = new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight);
            CreateHoles();
            CreateBorders();
        }

        /// <summary>
        /// Creates the objects that represents the tables holes
        /// </summary>
        private static void CreateHoles()
        {
            cornerTopLeft = new Rectangle(0, 0, cornerSize, cornerSize);
            cornerTopRight = new Rectangle(Game1.ScreenWidth - cornerSize, 0, cornerSize, cornerSize);
            centerTop = new Rectangle(centerPosition, 0, centerWidth, centerHeight);
            cornerBottomLeft = new Rectangle(0, Game1.ScreenHeight - cornerSize, cornerSize, cornerSize);
            cornerBottomRight = new Rectangle(Game1.ScreenWidth - cornerSize, Game1.ScreenHeight - cornerSize, cornerSize, cornerSize);
            centerBottom = new Rectangle(centerPosition, Game1.ScreenHeight - centerHeight, centerWidth, centerHeight);

            holes.Add(cornerTopLeft);
            holes.Add(centerTop);
            holes.Add(cornerTopRight);

            holes.Add(cornerBottomLeft);
            holes.Add(cornerBottomRight);
            holes.Add(centerBottom);
        }

        /// <summary>
        /// Creates the objects that represents the tables borders
        /// </summary>
        private static void CreateBorders()
        {
            left = new Rectangle(0, cornerTopLeft.Bottom, borderWidth, cornerBottomLeft.Top - cornerTopLeft.Bottom);
            right = new Rectangle(Game1.ScreenWidth - borderWidth, cornerTopRight.Bottom, borderWidth, cornerBottomRight.Top - cornerTopRight.Bottom);
            topLeft = new Rectangle(cornerTopLeft.Right, 0, centerTop.Left - cornerTopLeft.Right, borderWidth);
            topRight = new Rectangle(centerTop.Right, 0, cornerBottomRight.Left - centerTop.Right, borderWidth);
            bottomLeft = new Rectangle(cornerTopLeft.Right, Game1.ScreenHeight - borderWidth, centerTop.Left - cornerTopLeft.Right, borderWidth);
            bottomRight = new Rectangle(centerTop.Right, Game1.ScreenHeight - borderWidth, cornerBottomRight.Left - centerTop.Right, borderWidth);

            borders.Add(left);
            borders.Add(right);
            borders.Add(topLeft);
            borders.Add(topRight);
            borders.Add(bottomLeft);
            borders.Add(bottomRight);
        }

        /// <summary>
        /// Returns true if a given ball has collided with any of the borders of the table
        /// </summary>
        /// <param name="b">The ball to test with</param>
        /// <returns>True if the ball has collided with any borders</returns>
        public static Rectangle? CollidedWithBorder(Ball b)
        {
            foreach (Rectangle r in borders)
            {
                if(Collision(b, r))
                {
                    return r;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if a given ball has collided with any of the holes of the table
        /// </summary>
        /// <param name="b">The ball to test with</param>
        /// <returns>True if the ball has collided with any holes</returns>
        public static bool CollidedWithHole(Ball b)
        {
            foreach (Rectangle r in holes)
            {
                if (Collision(b, r))
                {
                    Debug.WriteLine("HOOOOLE");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a ball(circle) has collided with a rectangle
        /// </summary>
        /// <param name="b">The circle</param>
        /// <param name="r">The rectangle</param>
        /// <returns>If the circle has collided with the rectangle</returns>
        private static bool Collision(Ball b, Rectangle r)
        {
            float closestX = MathHelper.Clamp(b.GetCenter().X, r.Left, r.Right);
            float closestY = MathHelper.Clamp(b.GetCenter().Y, r.Top, r.Bottom);
            float distanceX = b.GetCenter().X - closestX;
            float distanceY = b.GetCenter().Y - closestY;
            return distanceX * distanceX + distanceY * distanceY < b.Radius * b.Radius;
        }

        /// <summary>
        /// Draws the table. If the boolean drawHitboxes is true also draws hitboxes for the borders and the holes.
        /// </summary>
        /// <param name="s">The spritebatch which the table will be drawn on</param>
        public static void Draw(SpriteBatch s)
        {
            s.Draw(Game1.TableTexture, drawRectangle, Color.White);
            if(drawHitboxes)
            {
                foreach (Rectangle r in holes)
                {
                    s.Draw(Game1.Pixel, r, Color.Red);
                }
                foreach (Rectangle r in borders)
                {
                    s.Draw(Game1.Pixel, r, Color.PowderBlue);
                }
            }            
        }
    }
}
