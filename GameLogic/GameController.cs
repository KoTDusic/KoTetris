using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContentRuntimeLoader;
using GameLogic.Figures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameLogic
{
    public class GameController
    {
        private const long TicksInMs = 10000;
        private const long InputMaxSpeed = 60;
        private const int GameFieldWidth = 10;
        private const int GameFieldHeight = 20;
        private readonly Action _exitAction;
        private readonly Button _startGameButton;
        private readonly Button _exitButton;
        private readonly Button _restartButton;
        private readonly TextBlock _resultTextBlock;
        private readonly BlockType[,] _gameField = new BlockType[GameFieldHeight, GameFieldWidth];
        private long _oldButtonPressedGameTime = 0;
        private int msTimeout = 80;
        private int _lines;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private GameState _gameState = GameState.MainMenu;
        private Texture2D _blockTexture;
        private Effect _effect;
        private Song _tetrisThemeSong;
        private bool _delayComplited;

        private bool LeftPressed { get; set; }
        private bool RightPressed { get; set; }
        private bool BottomPressed { get; set; }
        private bool UpPressed { get; set; }

        public GameController(Action exitAction)
        {
            _exitAction = exitAction;
            _startGameButton = new Button("Start Game");
            _restartButton = new Button("Restart");
            _exitButton = new Button("Exit");
            _resultTextBlock = new TextBlock();
            MediaPlayer.IsRepeating = true;
        }

        public void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            using (var stream = File.Open(ContentRuntime.BlockTexture, FileMode.Open))
            {
                _blockTexture = Texture2D.FromStream(graphicsDevice, stream);
            }

            _tetrisThemeSong = Song.FromUri(null, new Uri(ContentRuntime.TetrisTheme));
            //_effect = content.Load<Effect>("Shader");
            _startGameButton.Initialize(content);
            _restartButton.Initialize(content);
            _exitButton.Initialize(content);
            _resultTextBlock.Initialize(content);
        }

        public void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
        {
            switch (_gameState)
            {
                case GameState.MainMenu:
                {
                    _startGameButton.Update(mouse);
                    _exitButton.Update(mouse);
                    if (_startGameButton.Pressed || keyboard.IsKeyDown(Keys.Enter))
                    {
                        StartGame();
                    }

                    if (_exitButton.Pressed)
                    {
                        ExitGame();
                    }

                    if (IsSomeButtonHovered())
                    {
                        Mouse.SetCursor(MouseCursor.Hand);
                    }
                    else
                    {
                        Mouse.SetCursor(MouseCursor.Arrow);
                    }

                    break;
                }
                case GameState.Game:
                {
                    LeftPressed = keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A);
                    RightPressed = keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D);
                    BottomPressed = keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S);
                    UpPressed = keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W);
                    
                    if(UpPressed)
                    {
                        if (InvalidatePressSpeed(gameTime)) TryRotate();
                    }
                    
                    if (LeftPressed)
                    {
                        if (InvalidatePressSpeed(gameTime)) TryMoveLeft();
                    }

                    if (RightPressed)
                    {
                        if (InvalidatePressSpeed(gameTime)) TryMoveRight();
                    }
                    
                    if (BottomPressed)
                    {
                        if (InvalidatePressSpeed(gameTime))
                        {
                            TryMoveDown();
                            _delayComplited = false;
                        }
                    }
                    
                    if (!_delayComplited)
                    {
                        break;
                    }
                    
                    if (HasFallingFigure)
                    {
                        ProcessMoving();
                    }
                    else
                    {
                        GenerateFigure();
                    }

                    _delayComplited = false;
                    break;
                }
                case GameState.GameOver:
                {
                    _exitButton.Update(mouse);
                    _restartButton.Update(mouse);
                    if (_exitButton.Pressed)
                    {
                        ExitGame();
                    }

                    if (_restartButton.Pressed)
                    {
                        StartGame();
                    }

                    break;
                }
            }
        }

        private void TryRotate()
        {
            //Not implemented
        }

        private void ExitGame()
        {
            _exitAction();
        }

        private void StartGame()
        {
            _lines = 0;
            DisplayScore(0);
            ClearField();
            _gameState = GameState.Game;
            MediaPlayer.Play(_tetrisThemeSong);
            GameCycle(_cts.Token).Forget();
        }

        private void StopGame()
        {
            MediaPlayer.Stop();
            _gameState = GameState.GameOver;
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }

        private async Task GameCycle(CancellationToken ctsToken)
        {
            while (!ctsToken.IsCancellationRequested)
            {
                await Task.Delay(msTimeout, ctsToken).ConfigureAwait(false);
                _delayComplited = true;
            }
        }

        private void GenerateFigure()
        {
            IFigure figure = null;
            switch (RandomHelper.Next(0, 6))
            {
                //квадрат
                case 0:
                {
                    figure = new FigureI();
                    break;
                }
                case 1:
                {
                    figure = new FigureJ();
                    break;
                }
                case 2:
                {
                    figure = new FigureL();
                    break;
                }
                case 3:
                {
                    figure = new FigureO();
                    break;
                }
                case 4:
                {
                    figure = new FigureS();
                    break;
                }
                case 5:
                {
                    figure = new FigureT();
                    break;
                }
                case 6:
                {
                    figure = new FigureZ();
                    break;
                }
            }
            
            foreach (var pixel in figure.GetGeometry())
            {
                if (_gameField[pixel.X, pixel.Y] != BlockType.Air)
                {
                    StopGame();
                }

                _gameField[pixel.X, pixel.Y] = BlockType.FallingBlock;
            }
        }

        private void ProcessMoving()
        {
            if (!TryMoveDown())
            {
                FreezeFallingBlock();
                int lines;
                do
                {
                    lines = TryFinishLines();
                    if (lines == 0)
                    {
                        continue;
                    }

                    _lines += lines;
                    DisplayScore(_lines);
                } while (lines > 0);
            }
        }

        private void DisplayScore(int value)
        {
            _resultTextBlock.Text = "Score: " + value;
        }

        private int TryFinishLines()
        {
            var lines = 0;
            var counter = 0;
            for (var i = GameFieldHeight - 1; i >= 0; i--)
            {
                for (var j = 0; j < GameFieldWidth; j++)
                {
                    if (_gameField[i, j] == BlockType.Block)
                    {
                        counter++;
                    }

                    if (_gameField[i, j] == BlockType.Air)
                    {
                        counter = 0;
                        break;
                    }
                }

                if (counter == GameFieldWidth)
                {
                    lines++;
                    counter = 0;
                    for (var j = 0; j < GameFieldWidth; j++)
                    {
                        for (var k = i; k > 1; k--)
                        {
                            _gameField[k, j] = _gameField[k - 1, j];
                        }

                        _gameField[0, j] = BlockType.Air;
                    }
                }
            }

            return lines;
        }

        private bool InvalidatePressSpeed(GameTime gameTime)
        {
            var difference = gameTime.TotalGameTime.Ticks - _oldButtonPressedGameTime;
            if (difference < InputMaxSpeed * TicksInMs)
            {
                return false;
            }
            
            _oldButtonPressedGameTime = gameTime.TotalGameTime.Ticks;
            return true;

        }

        private bool TryMoveRight()
        {
            var list = new List<Point>();
            for (var i = 0; i < GameFieldHeight; i++)
            {
                for (var j = 0; j < GameFieldWidth; j++)
                {
                    if (_gameField[i, j] != BlockType.FallingBlock)
                    {
                        continue;
                    }

                    if (j + 1 >= GameFieldWidth)
                    {
                        return false;
                    }

                    if (_gameField[i, j + 1] == BlockType.Block)
                    {
                        return false;
                    }

                    list.Add(new Point(i, j));
                }
            }

            foreach (var point in list)
            {
                _gameField[point.X, point.Y] = BlockType.Air;
            }

            foreach (var point in list)
            {
                _gameField[point.X, point.Y + 1] = BlockType.FallingBlock;
            }

            return true;
        }

        private bool TryMoveLeft()
        {
            var list = new List<Point>();
            for (var i = 0; i < GameFieldHeight; i++)
            {
                for (var j = 0; j < GameFieldWidth; j++)
                {
                    if (_gameField[i, j] != BlockType.FallingBlock)
                    {
                        continue;
                    }

                    if (j - 1 < 0)
                    {
                        return false;
                    }

                    if (_gameField[i, j - 1] == BlockType.Block)
                    {
                        return false;
                    }

                    list.Add(new Point(i, j));
                }
            }

            foreach (var point in list)
            {
                _gameField[point.X, point.Y] = BlockType.Air;
            }

            foreach (var point in list)
            {
                _gameField[point.X, point.Y - 1] = BlockType.FallingBlock;
            }

            return true;
        }

        private bool TryMoveDown()
        {
            var list = new List<Point>();
            for (var i = 0; i < GameFieldHeight; i++)
            {
                for (var j = 0; j < GameFieldWidth; j++)
                {
                    if (_gameField[i, j] != BlockType.FallingBlock)
                    {
                        continue;
                    }

                    if (i + 1 >= GameFieldHeight)
                    {
                        return false;
                    }

                    if (_gameField[i + 1, j] == BlockType.Block)
                    {
                        return false;
                    }

                    list.Add(new Point(i, j));
                }
            }

            foreach (var point in list)
            {
                _gameField[point.X, point.Y] = BlockType.Air;
            }

            foreach (var point in list)
            {
                _gameField[point.X + 1, point.Y] = BlockType.FallingBlock;
            }

            return true;
        }

        private void ClearField()
        {
            for (var i = 0; i < GameFieldHeight; i++)
            {
                for (var j = 0; j < GameFieldWidth; j++)
                {
                    _gameField[i, j] = BlockType.Air;
                }
            }
        }

        private void FreezeFallingBlock()
        {
            for (var i = 0; i < GameFieldHeight; i++)
            {
                for (var j = 0; j < GameFieldWidth; j++)
                {
                    if (_gameField[i, j] == BlockType.FallingBlock)
                    {
                        _gameField[i, j] = BlockType.Block;
                    }
                }
            }
        }


        private bool HasFallingFigure =>
            _gameField.OfType<BlockType>().Any(blockType => blockType == BlockType.FallingBlock);

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle viewportBounds)
        {
            switch (_gameState)
            {
                case GameState.MainMenu:
                {
                    var startButtonRect = viewportBounds
                        .GetTopLeftPart(viewportBounds.Width, viewportBounds.Height / 3)
                        .GetAlignedToCenterBorder(300, 30);
                    var exitButtonRect = viewportBounds
                        .GetBotLeftPart(viewportBounds.Width, viewportBounds.Height / 3)
                        .GetAlignedToCenterBorder(300, 30);
                    _startGameButton.Draw(spriteBatch, startButtonRect);
                    _exitButton.Draw(spriteBatch, exitButtonRect);
                    break;
                }
                case GameState.Game:
                {
                    var gamePart = viewportBounds.GetTopLeftPart(
                        viewportBounds.Width / 3 * 2,
                        viewportBounds.Height);
                    var scorePart = viewportBounds.GetTopRightPart(300, 30);
                    var tileHeight = gamePart.Height / GameFieldHeight;
                    var tileWidth = gamePart.Width / GameFieldWidth;
                    var tileSize = Math.Min(tileWidth, tileHeight);
                    gamePart = new Rectangle(0, 0,
                        tileSize * GameFieldWidth,
                        tileSize * GameFieldHeight);
                    spriteBatch.Draw(EmptyTexture.Texture, gamePart, Color.Black);
                    for (var i = 0; i < GameFieldHeight; i++)
                    {
                        for (var j = 0; j < GameFieldWidth; j++)
                        {
                            switch (_gameField[i, j])
                            {
                                case BlockType.Air:
                                {
                                    break;
                                }
                                case BlockType.FallingBlock:
                                case BlockType.Block:
                                {
                                    var blockRectangle = new Rectangle(
                                        gamePart.X + j * tileSize,
                                        gamePart.Y + i * tileSize,
                                        tileSize,
                                        tileSize);
                                     //_effect.CurrentTechnique.Passes[0].Apply();
                                    spriteBatch.Draw(_blockTexture, blockRectangle, Color.White);
                                    break;
                                }
                            }
                        }
                    }

                    spriteBatch.End();
                    spriteBatch.Begin();
                    _resultTextBlock.Draw(spriteBatch, scorePart);

                    break;
                }
                case GameState.GameOver:
                {
                    var restartButtonRect = viewportBounds
                        .GetTopLeftPart(viewportBounds.Width, viewportBounds.Height / 3)
                        .GetAlignedToCenterBorder(300, 30);
                    var scoreTextRect = viewportBounds.GetAlignedToCenterBorder(300, 30);
                    var exitButtonRect = viewportBounds
                        .GetBotLeftPart(viewportBounds.Width, viewportBounds.Height / 3)
                        .GetAlignedToCenterBorder(300, 30);
                    _restartButton.Draw(spriteBatch, restartButtonRect);
                    _resultTextBlock.Draw(spriteBatch, scoreTextRect);
                    _exitButton.Draw(spriteBatch, exitButtonRect);
                    break;
                }
            }
        }

        private bool IsSomeButtonHovered()
        {
            switch (_gameState)
            {
                case GameState.MainMenu:
                {
                    return _startGameButton.Hovered || _exitButton.Hovered;
                }
                case GameState.GameOver:
                {
                    return _restartButton.Hovered || _exitButton.Hovered;
                }
                default:
                {
                    return false;
                }
            }
        }
    }
}