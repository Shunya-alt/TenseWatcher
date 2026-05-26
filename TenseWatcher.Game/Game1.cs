using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TenseWatcher.Model;

namespace TenseWatcher.Game
{
    public class MyGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState _gameState;
        private Texture2D _pixel;
        private SpriteFont _font;
        private int _cellSize = 50;

        private Texture2D _playerTex;
        private Texture2D _watcherTex;

        private bool _showGameOverMessage;
        private bool _showVictoryMessage;

        private KeyboardState _lastKeyboardState;
        private MouseState _lastMouseState;
        private Rectangle _restartButtonRect = new Rectangle(280, 250, 240, 40);
        private Rectangle _exitButtonRect = new Rectangle(290, 310, 220, 20);

        public MyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 10 * _cellSize + 200;
            _graphics.PreferredBackBufferHeight = 10 * _cellSize;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void NewGame()
        {
            _gameState = new GameState(10, 10);
            _showGameOverMessage = false;
            _showVictoryMessage = false;
        }

        protected override void Initialize()
        {
            NewGame();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            _font = Content.Load<SpriteFont>("Font");

            _playerTex = Content.Load<Texture2D>("Sprites/Joseph");
            _watcherTex = Content.Load<Texture2D>("Sprites/Cars");

            Color[] playerData = new Color[_playerTex.Width * _playerTex.Height];
            _playerTex.GetData(playerData);
            for (int i = 0; i < playerData.Length; i++)
            {
                if (playerData[i] == Color.Black)
                    playerData[i] = Color.Transparent;
            }
            _playerTex.SetData(playerData);

            Color[] watcherData = new Color[_watcherTex.Width * _watcherTex.Height];
            _watcherTex.GetData(watcherData);
            for (int i = 0; i < watcherData.Length; i++)
            {
                if (watcherData[i] == Color.Black)
                    watcherData[i] = Color.Transparent;
            }
            _watcherTex.SetData(watcherData);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            if (_showGameOverMessage || _showVictoryMessage)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePos = new Point(currentMouseState.X, currentMouseState.Y);
                    if (_restartButtonRect.Contains(mousePos))
                    {
                        NewGame();
                    }
                    else if (_exitButtonRect.Contains(mousePos))
                    {
                        Exit();
                    }
                }

                if (currentKeyboardState.IsKeyDown(Keys.R) && !_lastKeyboardState.IsKeyDown(Keys.R))
                {
                    NewGame();
                }
                if (currentKeyboardState.IsKeyDown(Keys.Escape))
                {
                    Exit();
                }

                _lastKeyboardState = currentKeyboardState;
                _lastMouseState = currentMouseState;
                base.Update(gameTime);
                return;
            }

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Up))
                _gameState.TryMovePlayer(0, -1);
            else if (state.IsKeyDown(Keys.Down))
                _gameState.TryMovePlayer(0, 1);
            else if (state.IsKeyDown(Keys.Left))
                _gameState.TryMovePlayer(-1, 0);
            else if (state.IsKeyDown(Keys.Right))
                _gameState.TryMovePlayer(1, 0);

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            if (_gameState.GameOver && !_showGameOverMessage)
            {
                _showGameOverMessage = true;
            }
            else if (_gameState.Victory && !_showVictoryMessage)
            {
                _showVictoryMessage = true;
            }

            this.Window.Title = $"TenseWatcher | Anxiety: {_gameState.Anxiety}% | Artefacts: {_gameState.ArtefactsCollected}/3";

            _lastKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Color backColor = Color.DarkGray;

                    switch (_gameState.Map[x, y])
                    {
                        case TileType.Artefact:
                            _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), Color.Gold);
                            break;
                        case TileType.LightZone:
                            _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), Color.LimeGreen);
                            break;
                        case TileType.Portal:
                            _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), Color.HotPink);
                            break;
                        case TileType.Player:
                            _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), backColor);
                            _spriteBatch.Draw(_playerTex, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), Color.White);
                            break;
                        case TileType.Watcher:
                            _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), backColor);
                            _spriteBatch.Draw(_watcherTex, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), Color.White);
                            break;
                        default:
                            _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), backColor);
                            break;
                    }
                }
            }

            int panelX = 10 * _cellSize + 10;
            int panelY = 20;

            _spriteBatch.Draw(_pixel, new Rectangle(panelX, panelY, 180, 30), Color.DarkGray);
            int anxietyWidth = (int)(180 * _gameState.Anxiety / 100.0f);
            _spriteBatch.Draw(_pixel, new Rectangle(panelX, panelY, anxietyWidth, 30), Color.Red);
            _spriteBatch.DrawString(_font, $"Anxiety: {_gameState.Anxiety}%", new Vector2(panelX + 10, panelY + 8), Color.White);

            panelY += 50;
            _spriteBatch.Draw(_pixel, new Rectangle(panelX, panelY, 180, 50), Color.DarkSlateGray);
            _spriteBatch.DrawString(_font, $"Artefacts: {_gameState.ArtefactsCollected}/3", new Vector2(panelX + 10, panelY + 15), Color.Gold);

            panelY += 60;
            _spriteBatch.Draw(_pixel, new Rectangle(panelX, panelY, 180, 50), Color.DarkSlateGray);
            _spriteBatch.DrawString(_font, "Controls:", new Vector2(panelX + 10, panelY + 8), Color.White);
            _spriteBatch.DrawString(_font, "Arrow Keys", new Vector2(panelX + 10, panelY + 28), Color.LightGray);

            if (_showGameOverMessage)
            {
                _spriteBatch.Draw(_pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(0, 0, 0, 200));
                _spriteBatch.Draw(_pixel, new Rectangle(180, 150, 440, 200), Color.DarkRed);
                _spriteBatch.Draw(_pixel, new Rectangle(185, 155, 430, 190), Color.Red);

                Vector2 gameOverSize = _font.MeasureString("GAME OVER");
                _spriteBatch.DrawString(_font, "GAME OVER", new Vector2(400 - gameOverSize.X / 2, 175), Color.White);
                Vector2 reasonSize = _font.MeasureString("You were consumed by fear...");
                _spriteBatch.DrawString(_font, "You were consumed by fear...", new Vector2(400 - reasonSize.X / 2, 210), Color.LightGray);

                _spriteBatch.Draw(_pixel, _restartButtonRect, Color.DarkGray);
                _spriteBatch.Draw(_pixel, new Rectangle(_restartButtonRect.X + 5, _restartButtonRect.Y + 5, _restartButtonRect.Width - 10, _restartButtonRect.Height - 10), Color.Gray);
                Vector2 restartSize = _font.MeasureString("RESTART (R)");
                _spriteBatch.DrawString(_font, "RESTART (R)", new Vector2(400 - restartSize.X / 2, 258), Color.White);

                _spriteBatch.Draw(_pixel, _exitButtonRect, Color.DarkGray);
                Vector2 exitSize = _font.MeasureString("EXIT (ESC)");
                _spriteBatch.DrawString(_font, "EXIT (ESC)", new Vector2(400 - exitSize.X / 2, 313), Color.White);
            }
            else if (_showVictoryMessage)
            {
                _spriteBatch.Draw(_pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(0, 0, 0, 200));
                _spriteBatch.Draw(_pixel, new Rectangle(180, 150, 440, 200), Color.DarkGreen);
                _spriteBatch.Draw(_pixel, new Rectangle(185, 155, 430, 190), Color.Green);

                Vector2 victorySize = _font.MeasureString("VICTORY!");
                _spriteBatch.DrawString(_font, "VICTORY!", new Vector2(400 - victorySize.X / 2, 175), Color.White);
                Vector2 congratsSize = _font.MeasureString("You escaped the Watcher!");
                _spriteBatch.DrawString(_font, "You escaped the Watcher!", new Vector2(400 - congratsSize.X / 2, 210), Color.LightGray);

                _spriteBatch.Draw(_pixel, _restartButtonRect, Color.DarkGray);
                _spriteBatch.Draw(_pixel, new Rectangle(_restartButtonRect.X + 5, _restartButtonRect.Y + 5, _restartButtonRect.Width - 10, _restartButtonRect.Height - 10), Color.Gray);
                Vector2 restartSize = _font.MeasureString("RESTART (R)");
                _spriteBatch.DrawString(_font, "RESTART (R)", new Vector2(400 - restartSize.X / 2, 258), Color.White);

                _spriteBatch.Draw(_pixel, _exitButtonRect, Color.DarkGray);
                Vector2 exitSize = _font.MeasureString("EXIT (ESC)");
                _spriteBatch.DrawString(_font, "EXIT (ESC)", new Vector2(400 - exitSize.X / 2, 313), Color.White);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}