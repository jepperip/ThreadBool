﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThreadNool
{
    class Table
    {
        Texture2D texture;
        Rectangle drawRectangle;

        public Table()
        {
            texture = Game1.TableTexture;
            drawRectangle = new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch s)
        {
            s.Draw(Game1.TableTexture, drawRectangle, Color.White);
        }

    }
}
