using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThreadNool
{
    class Table
    {
        public Table()
        {
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch s)
        {
            s.Draw(Game1.TableTexture, Vector2.Zero, Color.White);
        }

    }
}
