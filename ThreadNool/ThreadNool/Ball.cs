//Dahlberg, Simon och Sahlin, Jesper 2014-01-08

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace ThreadNool
{
    class Ball
    {
        int radius;
        Vector2 position, direction;
        Texture2D texture;
        Color color, initialColor;
        bool selected = false;
        //public GameTime GameTime { private get; set; }

        public int Radius { get { return radius; } }
        public Vector2 Position { get { return position; } }

        public Vector2 Direction { get { return direction; } set { direction = value; } }
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

        /// <summary>
        /// Updates the ball. If the ball is not already selected it checks if the mouse was clicked inside of the ball, if so the state of the ball and the colour is changed to indicate selection
        /// Else the ball was previously selected and the direction where the ball is supposed to go is calculated. Lastly the direction is added on to the balls position.
        /// </summary>
        /// <param name="clickPos">The point where the mouse was clicked</param>
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
                    //direction = new Vector2(clickPos.X - GetCenter().X, clickPos.Y - GetCenter().Y);
                    //direction.Normalize();
                }
            }
        }

        public bool ClickedOn(Point clickPos)
        {
            if (clickPos.X != -1 && clickPos.Y != -1)
            {
                Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, 36, 36);
                return bounds.Contains(clickPos);
            }
            return false;
        }

        public void MoveOnThread()
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            int timer = 60000;
            int elapsed = watch.Elapsed.Milliseconds;
            float x = direction.X * 0.00005f;
            float y = direction.Y * 0.00005f;
            Vector2 move = new Vector2(x, y);
            while (true)
            {
                int elapsedGameTime = watch.Elapsed.Milliseconds - elapsed;

                if (elapsedGameTime > 5)
                {
                    
                    elapsed = elapsedGameTime;
                }
                lock (myLock)
                {
                    position += move;
                }
  
            }
        }

        public void Draw(SpriteBatch sb)
        {
            lock (myLock)
            {
                sb.Draw(texture, position, color);
            }
            
        }

        object myLock = new Object();

    }
}
