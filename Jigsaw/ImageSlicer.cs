namespace Jigsaw
{
    public class ImageSlicer
    {
        // ── Constants ───────────────────────────────────────────────

        private const int GridSize = 3;          // 3x3 puzzle
        public const int TilePixelSize = 90;     // each tile = 90x90 px
        public const int FullImageSize = 270;    // full image = 270x270 px


        // ── Properties ──────────────────────────────────────────────

        // Style - anyone can READ, only ImageSlicer can WRITE
        // The original full image loaded by the player
        public Image OriginalImage { get; private set; }

        // Style - anyone can READ and WRITE
        // The 9 cropped pieces stored in a list
        public List<Image> Pieces { get; set; }

        // Style  - anyone can READ, only ImageSlicer can WRITE
        // Size of each tile piece (always 90)
        public int TileSize { get; private set; }


        // ── Constructor ─────────────────────────────────────────────

        public ImageSlicer(Image originalImage)
        {
            OriginalImage = originalImage;
            TileSize = TilePixelSize;       // always 90
            Pieces = new List<Image>();

            SliceImage();                   // cut image immediately
        }


        // ── Methods ─────────────────────────────────────────────────

        // Cuts the full image into 9 equal pieces
        // private - only called inside this class
        private void SliceImage()
        {
            // Clear any old pieces first
            Pieces.Clear();

            // Resize original image to exactly 270x270
            Bitmap resizedImage = ResizeImage(OriginalImage, FullImageSize, FullImageSize);

            // Loop through each row (0, 1, 2)
            for (int row = 0; row < GridSize; row++)
            {
                // Loop through each column (0, 1, 2)
                for (int col = 0; col < GridSize; col++)
                {
                    // Calculate where to crop in the full image
                    // row=0, col=0 → x=0,   y=0
                    // row=0, col=1 → x=90,  y=0
                    // row=1, col=0 → x=0,   y=90
                    int x = col * TileSize;
                    int y = row * TileSize;

                    // Create a blank 90x90 bitmap for this piece
                    Bitmap piece = new Bitmap(TileSize, TileSize);

                    using (Graphics g = Graphics.FromImage(piece))
                    {
                        // Where to crop FROM in the full image
                        Rectangle sourceRect = new Rectangle(x, y, TileSize, TileSize);

                        // Where to draw TO in our blank piece
                        Rectangle destRect = new Rectangle(0, 0, TileSize, TileSize);

                        // Draw the cropped portion onto our piece
                        g.DrawImage(resizedImage, destRect, sourceRect, GraphicsUnit.Pixel);
                    }

                    // Add piece to list
                    // Order: 0,1,2,3,4,5,6,7,8
                    Pieces.Add(piece);
                }
            }
        }

        // Resizes any image to exact size we need
        // private - only used inside this class
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            Bitmap resized = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(resized))
            {
                // High quality resizing
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Stretch original image to fill new size
                g.DrawImage(image, 0, 0, width, height);
            }

            return resized;
        }
    }
}