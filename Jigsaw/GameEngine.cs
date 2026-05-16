namespace Jigsaw
{
    public class GameEngine
    {
        // ── Constants ───────────────────────────────────────────────

        private const int TimerInterval = 1000;  // 1000ms = 1 second


        // ── Properties ──────────────────────────────────────────────

        // Style  - The board that holds all tiles
        public Board Board { get; private set; }

        // Style  - How many moves the player has made
        public int MoveCount { get; private set; }

        // Style  - How many seconds have passed
        public int ElapsedSeconds { get; private set; }

        // Style  - Is the game currently running?
        public bool IsRunning { get; private set; }

        // Style  - Is the game paused?
        public bool IsPaused { get; private set; }

        // Style  - Has the player won?
        public bool IsWon { get; private set; }

        // The timer that ticks every second
        // private - nobody outside needs to touch it
        private System.Windows.Forms.Timer gameTimer;

        // Event - fires every second to update UI timer label
        // PuzzleForm will subscribe to this event
        public event EventHandler? TimerTicked;

        // Event - fires when player wins
        // PuzzleForm will subscribe to this event
        public event EventHandler? GameWon;


        // ── Constructor ─────────────────────────────────────────────

        public GameEngine()
        {
            Board = new Board();
            MoveCount = 0;
            ElapsedSeconds = 0;
            IsRunning = false;
            IsPaused = false;
            IsWon = false;

            // Setup the timer
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = TimerInterval;
            gameTimer.Tick += OnTimerTick;
        }


        // ── Methods ─────────────────────────────────────────────────

        // Starts a new game - shuffles board and starts timer
        public void StartGame()
        {
            Board.Reset();
            Board.Shuffle();

            MoveCount = 0;
            ElapsedSeconds = 0;
            IsRunning = true;
            IsPaused = false;
            IsWon = false;

            gameTimer.Start();
        }

        // Pauses the game - stops timer
        public void PauseGame()
        {
            if (!IsRunning || IsPaused) return;

            IsPaused = true;
            gameTimer.Stop();
        }

        // Resumes the game - starts timer again
        public void ResumeGame()
        {
            if (!IsRunning || !IsPaused) return;

            IsPaused = false;
            gameTimer.Start();
        }

        // Stops the game completely
        public void StopGame()
        {
            IsRunning = false;
            IsPaused = false;
            gameTimer.Stop();
        }

        // Tries to move a tile at given index
        // Returns true if move was successful
        public bool TryMove(int tileIndex)
        {
            // Cannot move if game is not running or paused or won
            if (!IsRunning || IsPaused || IsWon) return false;

            // Check if this tile can move
            if (!Board.IsValidMove(tileIndex)) return false;

            // Move the tile
            Board.MoveTile(tileIndex);

            // Count this move
            MoveCount++;

            // Check if player has won
            if (Board.IsSolved())
            {
                IsWon = true;
                IsRunning = false;
                gameTimer.Stop();

                // Fire the GameWon event to notify PuzzleForm
                GameWon?.Invoke(this, EventArgs.Empty);
            }

            return true;
        }

        // Returns timer display string — e.g. "02:45"
        public string GetTimeString()
        {
            int minutes = ElapsedSeconds / 60;
            int seconds = ElapsedSeconds % 60;

            // {0:D2} means always show 2 digits — e.g. 5 becomes "05"
            return string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }

        // Called every second by the timer
        private void OnTimerTick(object? sender, EventArgs e)
        {
            ElapsedSeconds++;

            // Fire the TimerTicked event to notify PuzzleForm
            TimerTicked?.Invoke(this, EventArgs.Empty);
        }
    }
}