using System;

namespace TenseWatcher.Model
{
    public class GameState
    {
        public TileType[,] Map { get; private set; }
        public int PlayerX { get; private set; }
        public int PlayerY { get; private set; }
        public int WatcherX { get; private set; }
        public int WatcherY { get; private set; }
        public int Anxiety { get; private set; }
        public int ArtefactsCollected { get; private set; }
        public bool GameOver { get; private set; }
        public bool Victory { get; private set; }

        private int _playerTurns;
        private const int MaxAnxiety = 100;
        private const int ArtefactsToWin = 3;
        private readonly int _width;
        private readonly int _height;

        public GameState(int width = 10, int height = 10)
        {
            _width = width;
            _height = height;
            Map = new TileType[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Map[x, y] = TileType.Empty;

            PlayerX = 0;
            PlayerY = 0;
            Map[PlayerX, PlayerY] = TileType.Player;

            WatcherX = width - 1;
            WatcherY = height - 1;
            Map[WatcherX, WatcherY] = TileType.Watcher;

            Map[2, 2] = TileType.Artefact;
            Map[5, 5] = TileType.Artefact;
            Map[8, 3] = TileType.Artefact;

            Map[3, 3] = TileType.LightZone;
            Map[6, 7] = TileType.LightZone;

            Map[9, 9] = TileType.Portal;

            Anxiety = 0;
            ArtefactsCollected = 0;
            GameOver = false;
            Victory = false;
            _playerTurns = 0;
        }

        public bool TryMovePlayer(int dx, int dy)
        {
            if (GameOver || Victory) return false;

            int newX = PlayerX + dx;
            int newY = PlayerY + dy;

            if (newX < 0 || newX >= _width || newY < 0 || newY >= _height)
                return false;

            if (Map[newX, newY] == TileType.Watcher)
            {
                IncreaseAnxiety(25);
                if (!GameOver) MoveWatcher();
                return false;
            }

            Map[PlayerX, PlayerY] = TileType.Empty;
            PlayerX = newX;
            PlayerY = newY;

            if (Map[PlayerX, PlayerY] == TileType.Artefact)
            {
                ArtefactsCollected++;
                Map[PlayerX, PlayerY] = TileType.Empty;
            }
            else if (Map[PlayerX, PlayerY] == TileType.LightZone)
            {
                Anxiety = Math.Max(0, Anxiety - 10);
            }
            else if (Map[PlayerX, PlayerY] == TileType.Portal && ArtefactsCollected >= ArtefactsToWin)
            {
                Victory = true;
                return true;
            }

            Map[PlayerX, PlayerY] = TileType.Player;

            if (ArtefactsCollected >= ArtefactsToWin && Map[PlayerX, PlayerY] == TileType.Portal)
            {
                Victory = true;
                return true;
            }

            _playerTurns++;
            if (_playerTurns % 2 == 0)
            {
                MoveWatcher();
            }

            return true;
        }

        private void MoveWatcher()
        {
            if (GameOver || Victory) return;

            int dx = 0, dy = 0;

            if (WatcherX < PlayerX) dx = 1;
            else if (WatcherX > PlayerX) dx = -1;
            else if (WatcherY < PlayerY) dy = 1;
            else if (WatcherY > PlayerY) dy = -1;

            int newX = WatcherX + dx;
            int newY = WatcherY + dy;

            if (newX < 0 || newX >= _width || newY < 0 || newY >= _height)
                return;

            if (Map[newX, newY] == TileType.Player)
            {
                IncreaseAnxiety(25);
                return;
            }

            Map[WatcherX, WatcherY] = TileType.Empty;
            WatcherX = newX;
            WatcherY = newY;
            Map[WatcherX, WatcherY] = TileType.Watcher;
        }

        private void IncreaseAnxiety(int amount)
        {
            Anxiety += amount;
            if (Anxiety >= MaxAnxiety)
            {
                Anxiety = MaxAnxiety;
                GameOver = true;
            }
        }
    }
}