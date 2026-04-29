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
        private int _cellSize = 50;

        public MyGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 500;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gameState = new GameState(10, 10);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
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
                    Color color = Color.Gray;
                    switch (_gameState.Map[x, y])
                    {
                        case TileType.Player:
                            color = Color.Blue;
                            break;
                        case TileType.Watcher:
                            color = Color.Red;
                            break;
                        case TileType.Artefact:
                            color = Color.Gold;
                            break;
                        case TileType.LightZone:
                            color = Color.LimeGreen;
                            break;
                        case TileType.Portal:
                            color = Color.HotPink;
                            break;
                        default:
                            color = Color.LightGray;
                            break;
                    }
                    _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), color);
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}