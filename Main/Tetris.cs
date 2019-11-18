using System.Reflection;
using GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Main
{
    public class Tetris : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameController _gameController;
        private MouseState _mouse;
        private KeyboardState _keyboard;

        public Tetris()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600
            };
            
            Window.AllowUserResizing = true;
            Window.Title = "KoTetris";
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            EmptyTexture.SetGraphic(GraphicsDevice);
            _gameController = new GameController(Exit);
            _gameController.Initialize(Content, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _mouse = Mouse.GetState();
            _keyboard = Keyboard.GetState();
            _gameController.Update(gameTime, _mouse, _keyboard);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _gameController.Draw(gameTime,_spriteBatch, GraphicsDevice.Viewport.Bounds);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}