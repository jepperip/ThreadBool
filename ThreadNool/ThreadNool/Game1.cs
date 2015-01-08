//Dahlberg, Simon och Sahlin, Jesper 2014-01-08

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace ThreadNool
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static readonly int ScreenWidth = 800, ScreenHeight = (int)(ScreenWidth * 0.54795f);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Ball> balls;
        MouseState previousMouseState;
        MouseState currentMouseState;
        Ball currentlySelectedBall;
        bool firstClick = true;

        public static Texture2D BallTexture, TableTexture, Pixel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            //graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
            base.Initialize();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            BallTexture = Content.Load<Texture2D>("ball");
            TableTexture = Content.Load<Texture2D>("PoolTableReferenceTop");
            Pixel = Content.Load<Texture2D>("pixel");
            balls = new List<Ball>();
            balls.Add(new Ball(new Vector2(80, 120), Color.Red));
            balls.Add(new Ball(new Vector2(80, 160), Color.Red));
            balls.Add(new Ball(new Vector2(80, 200), Color.Red));
            balls.Add(new Ball(new Vector2(80, 240), Color.Red));


            balls.Add(new Ball(new Vector2(500, 120), Color.Blue));
            balls.Add(new Ball(new Vector2(500, 160), Color.Blue));
            balls.Add(new Ball(new Vector2(500, 200), Color.Blue));
            balls.Add(new Ball(new Vector2(500, 240), Color.Blue));

            Table.Setup();
           
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            //Point går inte att nulla, så vi får sätta det till något skitvärde och istället kolla så att man klickar innanför spelplanen typ
            Point clickPos = new Point(-1,-1);
            if(previousMouseState.LeftButton == ButtonState.Released && 
                currentMouseState.LeftButton == ButtonState.Pressed)
            {
                clickPos.X = Mouse.GetState().X;
                clickPos.Y = Mouse.GetState().Y;
                if (firstClick)
                {
                    foreach(Ball b in balls)
                    {
                        if (b.ClickedOn(clickPos))
                            currentlySelectedBall = b;
                    }
                }
                else
                {
                    if(currentlySelectedBall != null)
                    {
                        Vector2 newDir = new Vector2(clickPos.X - currentlySelectedBall.GetCenter().X, clickPos.Y - currentlySelectedBall.GetCenter().Y);
                        newDir.Normalize();
                        currentlySelectedBall.Direction = newDir;
                        Thread t1 = new Thread(currentlySelectedBall.MoveOnThread);
                        t1.Start();
                        currentlySelectedBall = null;
                    }
                }
                firstClick = !firstClick;
            }

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            foreach (Ball b1 in balls)
            {
                b1.Update(clickPos);
                //foreach (Ball b2 in balls)
                //{
                //    if (b1 != b2 && BallManager.CheckCollision(b1, b2))
                //    {
                //        bool collision = true;
                //    }

                //}
                //if (Table.CollidedWithBorder(b1))
                //{
                //    bool collision = true;
                //}
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            Table.Draw(spriteBatch);
            foreach (Ball ball in balls)
                ball.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
