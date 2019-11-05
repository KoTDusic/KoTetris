using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameLogic
{
    public class Button
    {
        private readonly string _text;
        private SpriteFont _font;
        private Rectangle _container;
        public bool Hovered { get; private set; }
        public bool Pressed { get; private set; }


        public Button(string text)
        {
            _text = text;
        }
        
        public void Initialize(ContentManager content)
        {
            _font = content.Load<SpriteFont>("MainFont");
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle viewportBounds)
        {
            _container = viewportBounds;
            spriteBatch.Draw(EmptyTexture.Texture, viewportBounds, Color.Black);
            spriteBatch.Draw(EmptyTexture.Texture, viewportBounds.GetInnerBorder(2, 2), Color.White);
            spriteBatch.DrawStringInCenter(_font, _text, viewportBounds, Color.Black);
        }

        public void Update(MouseState mouse)
        {
            Hovered = _container.Contains(mouse.Position);
            Pressed = Hovered && mouse.LeftButton == ButtonState.Pressed;
        }
    }
}