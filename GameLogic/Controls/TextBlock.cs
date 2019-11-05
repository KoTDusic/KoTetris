using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameLogic
{
    public class TextBlock
    {
        public string Text { get; set; } = string.Empty;
        private SpriteFont _font;


        public TextBlock()
        {
        }

        public void Initialize(ContentManager content)
        {
            _font = content.Load<SpriteFont>("MainFont");
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle viewportBounds)
        {
            spriteBatch.Draw(EmptyTexture.Texture, viewportBounds, Color.White);
            spriteBatch.DrawStringInCenter(_font, Text, viewportBounds, Color.Black);
        }
    }
}