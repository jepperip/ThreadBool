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
        Vector2 position;
        Texture2D texture;
        Color color;

        public int Radius { get { return radius; } }
        public Vector2 Position { get { return position; } }
        public Ball(Vector2 initPos, Color color)
        {
            radius = 16;
            position = initPos;
            this.color = color;
            texture = Game1.BallTexture;
        }

        public Vector2 GetCenter()
        {
            return new Vector2(position.X + radius, position.Y + radius);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, color);
        }
    }
}
