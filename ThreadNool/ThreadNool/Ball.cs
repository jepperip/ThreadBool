//Dahlberg, Simon och Sahlin, Jesper 2014-01-08

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadNool
{
    class Ball
    {
        int radius;
        Vector2 position, direction, velocity;
        float falloff = 0.997f;
        Texture2D texture;
        Color color, initialColor;
        bool selected = false;
        Task task;
        List<Ball> balls;
        public readonly float mass = 1.0f; //Ignore
        public int Radius { get { return radius; } }
        public Vector2 Position { get { return position; } }
        public Vector2 Velocity { get { return velocity; } }

        public Vector2 Direction { get { return direction; } set { direction = value; } }
        public Ball(Vector2 initPos, Color color, List<Ball> balls)
        {
            radius = 16;
            position = initPos;
            this.color = initialColor = color;
            texture = Game1.BallTexture;
            direction = Vector2.Zero;
            velocity = Vector2.Zero;
            task = new Task(new Action(Work));
            this.balls = balls;
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
            position += velocity;
            velocity *= falloff;

            
            
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
            if(task.Status != TaskStatus.Running && task.Status != TaskStatus.WaitingToRun && task.Status != TaskStatus.WaitingForActivation)
            {
                task.Start();
            }
            
            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();
            //int timer = 60000;
            
            //int elapsed = watch.Elapsed.Milliseconds;
            
            //while (true)
            //{
            //    int elapsedGameTime = watch.Elapsed.Milliseconds - elapsed;

            //    if (elapsedGameTime > 5)
            //    {
            //        //float x = direction.X * momentum;
            //        //float y = direction.Y * momentum;
            //        //Vector2 move = new Vector2(x, y);
            //        //momentum *= 0.9999999f;
            //        //elapsed = elapsedGameTime;
            //        ////position += move;
            //        //posX += x;
            //        //posY += y;

            //    } 
            //}
        }

        public void SetVelocity(Vector2 direction, float force)
        {
            velocity = direction * force;
        }

        public void SetVelocity(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public void Puff(Vector2 vel)
        {
            SetVelocity(vel);
            MoveOnThread();
        }

        private void Work()
        {
            bool working = true;
            while(velocity.Length() > 0.1f)
            {
                foreach (Ball b in balls)
                {
                    if(b != this)
                    {
                        if (BallManager.CheckCollision(this, b))
                        {
                            Vector2 responseVel = HandleBallCollision(b);
                            while (BallManager.CheckCollision(this, b))
                            {
                                position += velocity;
                            }
                            b.Puff(responseVel); 
                        } 
                    }      
                }

                if(Table.CollidedWithBorder(this))
                {
                    float oldX = velocity.X;
                    velocity.X = -velocity.Y;
                    velocity.Y = oldX;
                    //velocity *= -1;
                }
            }
            SetVelocity(Vector2.Zero, 0);
            working = false;
            task = new Task(new Action(Work));
        }

        /// <summary>
        /// Ball-to-Ball Collision
        /// </summary>
        public Vector2 HandleBallCollision(Ball b)
        {
            float newVelX1 = (Velocity.X * (mass - b.mass) + (2 * b.mass * b.Velocity.X)) / (mass + b.mass);
            float newVelY1 = (Velocity.Y * (mass - b.mass) + (2 * b.mass * b.Velocity.Y)) / (mass + b.mass);
            float newVelX2 = (b.Velocity.X * (b.mass - mass) + (2 * mass * Velocity.X)) / (mass + b.mass);
            float newVelY2 = (b.Velocity.Y * (b.mass - mass) + (2 * mass * Velocity.Y)) / (mass + b.mass);

            SetVelocity(new Vector2(newVelX1, newVelY1));
            return new Vector2(newVelX2, newVelY2);
        }

        ///// <summary>
        ///// Ball-to-Wall Collision
        ///// </summary>
        //public void HandleIt(Ball collisionBall, Wall wall, float closestX, float closestY)
        //{
        //    if (closestY == collisionBall.position.Y)
        //    {
        //        float Overlap = Math.Abs(collisionBall.position.X - closestX);
        //        //Korrigerar om cirkeln är i väggen
        //        if (Overlap < collisionBall.radius)
        //        {
        //            float OverlapCorrection = collisionBall.position.X - closestX;
        //            if (OverlapCorrection > 0)
        //            {
        //                collisionBall.position.X += collisionBall.radius - OverlapCorrection;
        //            }
        //            else
        //                collisionBall.position.X += (OverlapCorrection + collisionBall.radius) * -1;
        //        }
        //        float Y = collisionBall.Velocity.Y * wall.friction;
        //        float X = -collisionBall.Velocity.X * (wall.elasticity * collisionBall.elasticity);
        //        //V = -eu
        //        collisionBall.SetVelocity(new Vector2(X, Y));
        //    }
        //    else
        //    {
        //        float Overlap = Math.Abs(collisionBall.position.Y - closestY);
        //        //Korrigerar om cirkeln är i väggen
        //        if (Overlap < collisionBall.radius)
        //        {
        //            float OverlapCorrection = collisionBall.position.Y - closestY;
        //            if (OverlapCorrection > 0)
        //            {
        //                collisionBall.position.Y += collisionBall.radius - OverlapCorrection;
        //            }
        //            else
        //            {
        //                collisionBall.position.Y += (OverlapCorrection + collisionBall.radius) * -1;
        //            }
        //        }

        //        float X = collisionBall.Velocity.X * wall.friction;
        //        float Y = -collisionBall.Velocity.Y * (wall.elasticity * collisionBall.elasticity);
        //        //V = -eu
        //        collisionBall.SetVelocity(new Vector2(X, Y));

        //    }
        //    collisionBall.angleVelocity = collisionBall.Velocity.Length() / collisionBall.radius * Math.Sign(collisionBall.Velocity.X);
        //}

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, color);
            
        }

        object myLock = new Object();

    }
}
