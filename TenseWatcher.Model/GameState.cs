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
        public int TotalMoves { get; private set; }

        private int _playerTurns;
        private const int MaxAnxiety = 100;
        private const int ArtefactsToWin = 3;
        private readonly int _width;
        private readonly int _height;
        private TileType[,] _contentUnderWatcher;

        public GameState(int width = 10, int height = 10)
        {
            _width = width;
            _height = height;
            Map = new TileType[width, height];
            _contentUnderWatcher = new TileType[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Map[x, y] = TileType.Empty;
                    _contentUnderWatcher[x, y] = TileType.Empty;
                }
            }

            PlayerX = 0;
            PlayerY = 0;
            Map[PlayerX, PlayerY] = TileType.Player;

            WatcherX = width - 2;
            WatcherY = height - 1;
            Map[WatcherX, WatcherY] = TileType.Watcher;
            _contentUnderWatcher[WatcherX, WatcherY] = TileType.Empty;

            Map[width - 1, height - 1] = TileType.Portal;

            Map[2, 2] = TileType.Artefact;
            Map[5, 5] = TileType.Artefact;
            Map[8, 3] = TileType.Artefact;

            Map[3, 3] = TileType.LightZone;
            Map[6, 7] = TileType.LightZone;

            Anxiety = 0;
            ArtefactsCollected = 0;
            GameOver = false;
            Victory = false;
            _playerTurns = 0;
            TotalMoves = 0;
        }

        public bool TryMovePlayer(int dx, int dy)
        {
            if (GameOver || Victory) return false;

            TotalMoves++;

            int newX = PlayerX + dx;
            int newY = PlayerY + dy;

            if (newX < 0 || newX >= _width || newY < 0 || newY >= _height)
                return false;

            if (Map[newX, newY] == TileType.Watcher)
            {
                IncreaseAnxiety(35);
                if (!GameOver) MoveWatcher();
                return false;
            }

            TileType targetContent = Map[newX, newY];

            if (targetContent == TileType.Portal && ArtefactsCollected < ArtefactsToWin)
            {
                return false;
            }

            Map[PlayerX, PlayerY] = TileType.Empty;
            PlayerX = newX;
            PlayerY = newY;

            if (targetContent == TileType.Artefact)
            {
                ArtefactsCollected++;
            }
            else if (targetContent == TileType.LightZone)
            {
                Anxiety = Math.Max(0, Anxiety - 10);
                Map[PlayerX, PlayerY] = TileType.LightZone;
            }
            else if (targetContent == TileType.Portal && ArtefactsCollected >= ArtefactsToWin)
            {
                Victory = true;
                return true;
            }

            if (targetContent != TileType.LightZone)
            {
                Map[PlayerX, PlayerY] = TileType.Player;
            }
            else
            {
                Map[PlayerX, PlayerY] = TileType.Player;
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
                IncreaseAnxiety(35);
                return;
            }

            if (Map[WatcherX, WatcherY] == TileType.Watcher)
            {
                Map[WatcherX, WatcherY] = _contentUnderWatcher[WatcherX, WatcherY];
            }

            TileType targetContent = Map[newX, newY];
            _contentUnderWatcher[newX, newY] = targetContent;

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

        public int GetRequiredArtefacts() => ArtefactsToWin;
        public int GetMaxAnxiety() => MaxAnxiety;
    }
}