using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThreadNool
{
    class Ball
    {
        int radius;
        Vector2 position, direction;
        Texture2D texture;
        Color color, initialColor;
        bool selected = false;

        public int Radius { get { return radius; } }
        public Vector2 Position { get { return position; } }
        public Ball(Vector2 initPos, Color color)
        {
            radius = 16;
            position = initPos;
            this.color = initialColor = color;
            texture = Game1.BallTexture;
            direction = Vector2.Zero;
        }

        public Vector2 GetCenter()
        {
            return new Vector2(position.X + radius, position.Y + radius);
        }

        public void Update(Point clickPos)
        {
            if (clickPos.X != -1 && clickPos.Y != -1)
            {
                if (!selected)
                {
                    Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, 36, 36);
                    if (bounds.Contains(clickPos))
                    {
                        selected = true;
                        color = Color.White;
                    }
                }
                else
                {
                    selected = false;
                    color = initialColor;
                    direction = new Vector2(clickPos.X - GetCenter().X, clickPos.Y - GetCenter().Y);
                    direction.Normalize();
                }
            }

            position += direction * 3;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, color);
        }
    }
}
