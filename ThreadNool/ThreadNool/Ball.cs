//Dahlberg, Simon och Sahlin, Jesper 2014-01-08

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ThreadNool
{
    /// <summary>
    /// This class represents a ball on the pool table.
    /// </summary>
    class Ball
    {
        int radius;
        Vector2 position, direction, velocity;
        float falloff = 0.997f;
        Texture2D texture;
        Color color, initialColor;
        bool selected = false;
        bool isActive = true;
        Task task;
        List<Ball> balls;
        Object resourceLock = new Object();

        public readonly float mass = 1.0f; //Ignore
        public int Radius { get { return radius; } }
        public Vector2 Position
        {
            get
            {
                lock (resourceLock)
                {
                    return position;
                }
            }
            set
            {
                lock (resourceLock)
                {
                    position = value;
                }
            }
        }
        public Vector2 Velocity
        {
            get
            {
                lock (resourceLock)
                {
                    return velocity;
                }
            }
            set
            {
                lock (resourceLock)
                {
                    direction = velocity;
                    direction.Normalize();
                    velocity = value;
                }
            }
        }

        public Vector2 Direction
        {
            get
            {
                lock (resourceLock)
                {
                    return direction;
                }
            }
            set
            {
                lock (resourceLock)
                {
                    direction = value;
                }
            }
        }

        /// <summary>
        /// The only Ball constructor, creates and initializes a ball object.
        /// </summary>
        /// <param name="initPos">The initial position of the ball.</param>
        /// <param name="color">The initial color of the ball.</param>
        /// <param name="balls">A reference to the list holding all of the balls.</param>
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

        /// <summary>
        /// Returns the center of the ball
        /// </summary>
        /// <returns>- '' -</returns>
        public Vector2 GetCenter()
        {
            Vector2 pos = Position;
            return new Vector2(pos.X + radius, pos.Y + radius);
        }

        /// <summary>
        /// Updates the ball. If the ball is not already selected it checks if the mouse was clicked inside of the ball, if so the state of the ball and the colour is changed to indicate selection
        /// Else the ball was previously selected and the direction where the ball is supposed to go is calculated. Lastly the direction is added on to the balls position.
        /// </summary>
        /// <param name="clickPos">The point where the mouse was clicked</param>
        public void Update(Point clickPos)
        {
            if (isActive)
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
        }

        /// <summary>
        /// Checks if a mouse click was inside the bounds of the ball
        /// </summary>
        /// <param name="clickPos">The point where the mouse was clicked.</param>
        /// <returns>True if the mouse click was inside of the balls bounds, false otherwise.</returns>
        public bool ClickedOn(Point clickPos)
        {
            if (clickPos.X != -1 && clickPos.Y != -1)
            {
                Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, 36, 36);
                return bounds.Contains(clickPos);
            }
            return false;
        }

        /// <summary>
        /// If not already started, starts the task which performs the collision-handling
        /// </summary>
        public void StartBallThread()
        {
            if (task.Status != TaskStatus.Running && task.Status != TaskStatus.WaitingToRun && task.Status != TaskStatus.WaitingForActivation)
            {
                task.Start();
                Debug.WriteLine("Task " + task.Id + " has started");
            }

        }

        /// <summary>
        /// Sets the velocity of the ball.
        /// </summary>
        /// <param name="direction">The direction of the velocity</param>
        /// <param name="force">The force applied to the velocity</param>
        public void SetVelocity(Vector2 direction, float force)
        {
            lock (resourceLock)
            {
                this.direction = direction;
                direction.Normalize();
                velocity = direction * force;
            }
        }

        /// <summary>
        /// Sets the velocity of the ball from an already existing velocity.
        /// </summary>
        /// <param name="velocity">The velocity which to mimic.</param>
        public void SetVelocity(Vector2 velocity)
        {
            lock (resourceLock)
            {
                direction = velocity;
                direction.Normalize();
                this.velocity = velocity;
            }
        }

        /// <summary>
        /// Knocks the ball in the direction and magnitude of the provided vector.
        /// Starts the collision-deteection task.
        /// </summary>
        /// <param name="vel">Velocity to knock the ball with</param>
        public void Puff(Vector2 vel)
        {
            SetVelocity(vel);
            StartBallThread();
        }

        /// <summary>
        /// Represents work for a task to do.
        /// The work consists of checking and handling collisions for a ball.
        /// There are two types of collisions the method resolves: circle-to-circle and
        /// circle-to-rectangle.
        /// If and when the ball has a velocity lower than a threshold value the work
        /// is considered done and the loop stops and releases the task.
        /// </summary>
        private void Work()
        {
            bool working = true;
            while (working && velocity.Length() > 0.0001f)
            {
                if (Table.CollidedWithHole(this))
                {
                    working = false;
                    isActive = false;
                    position.X = -55;
                    position.Y = -55;
                    break;
                }
                foreach (Ball b in balls)
                {
                    if (b != this)
                    {
                        if (BallManager.CheckCollision(this, b))
                        {
                            Vector2 responseVel = HandleBallCollision(b);

                            float distance = Vector2.Distance(Position, b.Position);
                            float overlap = (Radius * 2) - distance;
                            Vector2 dir = b.Position - Position;
                            dir.Normalize();
                            while (BallManager.CheckCollision(this, b))
                            {
                                position -= dir * overlap;
                            }

                            b.Puff(responseVel);
                        }
                    }
                }
                Rectangle? wall = Table.CollidedWithBorder(this);
                if (wall != null)
                {
                    Debug.WriteLine("NEIN");
                    HandleWallCollision(wall ?? Rectangle.Empty);
                }
            }
            //SetVelocity(Vector2.Zero, 0);
            working = false;
            task = new Task(new Action(Work));
        }

        /// <summary>
        /// Circle-to-circle collision handling
        /// </summary>
        /// <param name="b">The ball this ball has collided with</param>
        /// <returns>A new velocity wich is the velocity the ball 'b' should use after the collision has been resolved</returns>
        private Vector2 HandleBallCollision(Ball b)
        {
            Vector2 otherVel = b.Velocity;
            Vector2 tempVel = Velocity;
            if (otherVel.X == 0 && otherVel.Y == 0)
            {
                otherVel.X = -tempVel.X * 0.001f;
                otherVel.Y = -tempVel.Y * 0.001f;
            }
            float newVelX1 = (tempVel.X * (mass - b.mass) + (2 * b.mass * otherVel.X)) / (mass + b.mass);
            float newVelY1 = (tempVel.Y * (mass - b.mass) + (2 * b.mass * otherVel.Y)) / (mass + b.mass);
            float newVelX2 = (otherVel.X * (b.mass - mass) + (2 * mass * tempVel.X)) / (mass + b.mass);
            float newVelY2 = (otherVel.Y * (b.mass - mass) + (2 * mass * tempVel.Y)) / (mass + b.mass);

            SetVelocity(new Vector2(newVelX1, newVelY1));
            return new Vector2(newVelX2, newVelY2);

        }

        /// <summary>
        /// Circle-to-rectangle collision handling
        /// </summary>
        /// <param name="wall">A rectangle to responde to</param>
        private void HandleWallCollision(Rectangle wall)
        {
            Vector2 tempPos = Position;
            Vector2 tempVel = Velocity;

            float closestX = MathHelper.Clamp(GetCenter().X, wall.Left, wall.Right);
            float closestY = MathHelper.Clamp(GetCenter().Y, wall.Top, wall.Bottom);

            if (closestY == GetCenter().Y)
            {
                float Overlap = Math.Abs(GetCenter().X - closestX);
                //Korrigerar om cirkeln är i väggen
                if (Overlap < radius)
                {
                    float OverlapCorrection = GetCenter().X - closestX;
                    if (OverlapCorrection > 0)
                    {
                        tempPos.X += radius - OverlapCorrection;
                    }
                    else
                    {
                        tempPos.X += (OverlapCorrection + radius) * -1;
                    }                        
                }
            }
            else
            {
                float Overlap = Math.Abs(GetCenter().Y - closestY);
                //Korrigerar om cirkeln är i väggen
                if (Overlap < radius)
                {
                    float OverlapCorrection = GetCenter().Y - closestY;
                    if (OverlapCorrection > 0)
                    {
                        tempPos.Y += radius - OverlapCorrection;
                    }
                    else
                    {
                        tempPos.Y += (OverlapCorrection + radius) * -1;
                    }
                }
               
            }
            SetVelocity(tempVel);
        }

        /// <summary>
        /// Draws the ball onto the screen
        /// </summary>
        /// <param name="sb">The SpriteBatch object used for drawing.</param>
        public void Draw(SpriteBatch sb)
        {
            if (isActive)
                sb.Draw(texture, position, color);
        }
    }
}
