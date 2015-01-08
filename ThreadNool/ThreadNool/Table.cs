using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThreadNool
{
    class Table
    {
        Texture2D texture;

        public Table()
        {
            texture = Game1.TableTexture;
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
