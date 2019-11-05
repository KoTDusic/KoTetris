using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLogic
{
    public static class SpriteBatchExtensions
    {
        public static void DrawStringInCenter(this SpriteBatch spriteBatch, SpriteFont font, string text,
            Rectangle container, Color color)
        {
            var textSize = font.MeasureString(text);
            var xOffset = container.X + container.Width / 2 - textSize.X / 2;
            var yOffset = container.Y + container.Height / 2 - textSize.Y / 2;
            var textPos = new Vector2(xOffset, yOffset);
            spriteBatch.DrawString(font, text, textPos, color);
        }
    }
}