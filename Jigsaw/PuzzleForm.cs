namespace Jigsaw
{
    public partial class PuzzleForm : Form
    {
        // ── Fields ──────────────────────────────────────────────────

        // The game engine that controls everything
        private GameEngine engine;

        // The image slicer that cuts image into 9 pieces
        private ImageSlicer? slicer;

        // List of all 9 PictureBoxes from UI (pbTile0 to pbTile8)
        private List<PictureBox> tilePictureBoxes;


        // ── Constructor ─────────────────────────────────────────────

        public PuzzleForm()
        {
            InitializeComponent();

            // Create the game engine
            engine = new GameEngine();

            // Subscribe to events
            engine.TimerTicked += OnTimerTicked;
            engine.GameWon += OnGameWon;

            // Put all 9 PictureBoxes into a list — order matters!
            // Must match grid index 0 to 8
            tilePictureBoxes = new List<PictureBox>
            {
                pbTile0, pbTile1, pbTile2,
                pbTile3, pbTile4, pbTile5,
                pbTile6, pbTile7, pbTile8
            };

            // Attach click event to each PictureBox
            for (int i = 0; i < tilePictureBoxes.Count; i++)
            {
                // We need to capture i for each iteration
                int index = i;
                tilePictureBoxes[i].Click += (sender, e) => OnTileClicked(index);
            }

            // Disable shuffle and resume until image is loaded
            btnShuffle.Enabled = false;
            btnResume.Enabled = false;
        }


        // ── Event Handlers ───────────────────────────────────────────

        // Called when player clicks Load Image button
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            // Open file dialog to pick an image
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Puzzle Image";
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Load the selected image
                    Image loadedImage = Image.FromFile(dialog.FileName);

                    // Slice the image into 9 pieces
                    slicer = new ImageSlicer(loadedImage);

                    // Show original image in pbOriginal
                    pbOriginal.Image = loadedImage;
                    pbOriginal.SizeMode = PictureBoxSizeMode.Zoom;

                    // Assign sliced images to tiles
                    AssignImagesToTiles();

                    // Enable shuffle button now that image is loaded
                    btnShuffle.Enabled = true;
                    btnResume.Enabled = false;

                    // Update UI
                    lblMoves.Text = "Moves: 0";
                    lblTimer.Text = "Time: 00:00";
                }
            }
        }

        // Called when player clicks Shuffle button
        private void btnShuffle_Click(object sender, EventArgs e)
        {
            if (slicer == null) return;

            // Start a new game
            engine.StartGame();

            // Update tile PictureBoxes to match shuffled board
            RefreshBoard();

            // Update UI
            lblMoves.Text = "Moves: 0";
            lblTimer.Text = "Time: 00:00";

            btnResume.Enabled = false;
        }

        // Called when player clicks Resume button
        private void btnResume_Click(object sender, EventArgs e)
        {
            engine.ResumeGame();
            btnResume.Enabled = false;
        }

        // Called when player clicks Exit button
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Stop everything cleanly before closing
            engine.StopGame();
            Application.Exit();
        }

        // Called when player clicks a tile PictureBox
        private void OnTileClicked(int index)
        {
            // Try to move the tile
            bool moved = engine.TryMove(index);

            if (moved)
            {
                // Refresh board visuals
                RefreshBoard();

                // Update move counter label
                lblMoves.Text = $"Moves: {engine.MoveCount}";
            }
        }

        // Called every second by GameEngine timer event
        private void OnTimerTicked(object? sender, EventArgs e)
        {
            // Update timer label
            lblTimer.Text = $"Time: {engine.GetTimeString()}";
        }

        // Called when GameEngine fires GameWon event
        private void OnGameWon(object? sender, EventArgs e)
        {
            // Show winning message with stats
            MessageBox.Show(
                $"Congratulations! You solved the puzzle!\n\n" +
                $"Moves : {engine.MoveCount}\n" +
                $"Time  : {engine.GetTimeString()}",
                "Puzzle Solved!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }


        // ── Helper Methods ───────────────────────────────────────────

        // Assigns sliced images to tiles in Board
        // Assigns sliced images to tiles in Board
        private void AssignImagesToTiles()
        {
            if (slicer == null) return;

            // Assign each image piece to tile based on CorrectIndex
            // CorrectIndex tells us which image piece belongs to this tile
            for (int i = 0; i < engine.Board.Tiles.Count; i++)
            {
                Tile tile = engine.Board.Tiles[i];

                if (tile.IsEmpty)
                    tile.TileImage = null;
                else
                    tile.TileImage = slicer.Pieces[tile.CorrectIndex];
            }

            RefreshBoard();
        }

        // Updates all PictureBoxes to match current Board state
        private void RefreshBoard()
        {
            for (int i = 0; i < tilePictureBoxes.Count; i++)
            {
                // Find which tile is sitting at position i
                Tile tile = engine.Board.Tiles[i];

                if (tile.IsEmpty)
                {
                    // Empty tile - gray background
                    tilePictureBoxes[i].Image = null;
                    tilePictureBoxes[i].BackColor = Color.LightGray;
                }
                else
                {
                    // Show tile image based on its CorrectIndex
                    // CorrectIndex tells us which piece of the image to show
                    tilePictureBoxes[i].Image = slicer!.Pieces[tile.CorrectIndex];
                    tilePictureBoxes[i].BackColor = Color.White;
                    tilePictureBoxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }
    }
}