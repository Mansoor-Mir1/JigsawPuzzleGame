namespace Jigsaw
{
    public class Tile
    {
        // ── Properties ──────────────────────────────────────────────

        // Style 3 - anyone can READ, only Tile class can WRITE
        // This NEVER changes after the tile is created
        public int CorrectIndex { get; private set; }

        // Style 2 - anyone can READ and WRITE
        // This changes every time the tile moves
        public int CurrentIndex { get; set; }

        // Style 3 - anyone can READ, only Tile class can WRITE
        // This NEVER changes after creation
        public bool IsEmpty { get; private set; }

        // Style 2 - anyone can READ and WRITE
        // Board class will assign the button from UI
        public Button TileButton { get; set; }

        // Style 2 - anyone can READ and WRITE
        // ImageSlicer will assign the image later
        public Image? TileImage { get; set; }


        // ── Constructor ─────────────────────────────────────────────

        public Tile(int correctIndex, bool isEmpty = false)
        {
            CorrectIndex = correctIndex;   // set once, never changes
            CurrentIndex = correctIndex;   // starts in correct position
            IsEmpty = isEmpty;             // set once, never changes
            TileImage = null;              // no image yet
            TileButton = new Button();     // create button immediately
        }


        // ── Methods ─────────────────────────────────────────────────

        // Returns true if this tile is in its correct position
        public bool IsInCorrectPosition()
        {
            return CurrentIndex == CorrectIndex;
        }

        // Moves this tile to a new position in the grid
        public void MoveTo(int newIndex)
        {
            CurrentIndex = newIndex;
        }
    }
}