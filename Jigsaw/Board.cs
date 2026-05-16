namespace Jigsaw
{
    public class Board
    {
        // ── Constants ───────────────────────────────────────────────

        private const int GridSize = 3;    // 3x3 grid
        private const int TotalTiles = 9;  // 9 tiles total (including empty)


        // ── Properties ──────────────────────────────────────────────

        // Style  - The list of all 9 tiles
        // Index 0 to 8 represents position in the grid
        public List<Tile> Tiles { get; set; }

        // Style  - The current index of the empty tile
        // Board tracks this so we don't search for it every time
        public int EmptyIndex { get; private set; }


        // ── Constructor ─────────────────────────────────────────────

        public Board()
        {
            Tiles = new List<Tile>();
            CreateTiles();
        }


        // ── Methods ─────────────────────────────────────────────────

        // Creates all 9 tiles in correct order
        // private - only called inside constructor
        private void CreateTiles()
        {
            Tiles.Clear();

            for (int i = 0; i < TotalTiles; i++)
            {
                // Last tile (index 8) is the empty tile
                bool isEmpty = (i == 8);
                Tiles.Add(new Tile(i, isEmpty));
            }

            // Empty tile starts at index 8 (bottom-right)
            EmptyIndex = 8;
        }

        // Checks if a tile at given index can move into the empty space
        // A tile can move only if it is directly adjacent to empty tile
        public bool IsValidMove(int tileIndex)
        {
            // Get row and column of the tile we want to move
            int tileRow = tileIndex / GridSize;
            int tileCol = tileIndex % GridSize;

            // Get row and column of the empty tile
            int emptyRow = EmptyIndex / GridSize;
            int emptyCol = EmptyIndex % GridSize;

            // Calculate the difference between positions
            int rowDiff = Math.Abs(tileRow - emptyRow);
            int colDiff = Math.Abs(tileCol - emptyCol);

            // Valid move = tile is exactly 1 step away (up/down/left/right)
            // rowDiff + colDiff == 1 means directly adjacent
            // Examples:
            // tile at index 5, empty at index 8 → not adjacent ❌
            // tile at index 7, empty at index 8 → adjacent     ✅
            // tile at index 5, empty at index 8 → not adjacent ❌
            return (rowDiff + colDiff) == 1;
        }

        // Moves a tile into the empty space
        // Call IsValidMove first before calling this
        public void MoveTile(int tileIndex)
        {
            // Swap CurrentIndex of the clicked tile and the empty tile
            Tiles[tileIndex].MoveTo(EmptyIndex);
            Tiles[EmptyIndex].MoveTo(tileIndex);

            // Swap their positions in the list
            Tile temp = Tiles[tileIndex];
            Tiles[tileIndex] = Tiles[EmptyIndex];
            Tiles[EmptyIndex] = temp;

            // Update empty tile index to where the clicked tile was
            EmptyIndex = tileIndex;
        }

        // Checks if all tiles are in their correct positions
        public bool IsSolved()
        {
            foreach (Tile tile in Tiles)
            {
                if (!tile.IsInCorrectPosition())
                    return false;
            }
            return true;
        }

        // Shuffles the puzzle by making many random valid moves
        public void Shuffle()
        {
            Random random = new Random();

            // Make 200 random valid moves
            // This guarantees puzzle is always solvable
            for (int i = 0; i < 200; i++)
            {
                // Find all tiles that can currently move
                List<int> validMoves = new List<int>();

                for (int j = 0; j < TotalTiles; j++)
                {
                    if (IsValidMove(j))
                        validMoves.Add(j);
                }

                // Pick a random valid move and apply it
                int randomIndex = random.Next(validMoves.Count);
                MoveTile(validMoves[randomIndex]);
            }
        }

        // Resets the board back to solved state
        public void Reset()
        {
            CreateTiles();
        }
    }
}