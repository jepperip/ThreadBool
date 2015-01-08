using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadNool
{
    class Table
    {
        static readonly bool drawHitboxes = true;
        Texture2D texture;
        Rectangle drawRectangle;
        List<Rectangle> holes = new List<Rectangle>();
        List<Rectangle> borders = new List<Rectangle>();
        Rectangle topLeft, topRight, bottomLeft, bottomRight, left, right, cornerTopLeft, cornerTopRight, cornerBottomLeft, cornerBottomRight, centerTop, centerBottom;
        static readonly int cornerSize = (int)(Game1.ScreenWidth * 0.08951);
        static readonly int centerWidth = (int)(Game1.ScreenWidth * 0.0505115089514066);
        static readonly int centerHeight = (int)(Game1.ScreenWidth * 0.0351662404092072);
        static readonly int centerPosition = (int)(Game1.ScreenWidth / 2 - centerWidth / 2);
        static readonly int borderWidth = (int)(Game1.ScreenWidth * 0.0485933503836317);

        public Table()
        {
            texture = Game1.TableTexture;

            drawRectangle = new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight);
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

        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Draws the table. If the boolean drawHitboxes is true also draws hitboxes for the borders and the holes.
        /// </summary>
        /// <param name="s">The spritebatch wich the table will be drawn on</param>
        public void Draw(SpriteBatch s)
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
