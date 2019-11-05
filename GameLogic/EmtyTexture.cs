using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLogic
{
    public static class EmptyTexture
    {
        public static Texture2D Texture { get; private set; }

        public static void SetGraphic(GraphicsDevice graphics)
        {
            Texture = new Texture2D(graphics, 1, 1);
            Texture.SetData(new[] {Color.White});
        }
        
    }
}