using System;
using System.Drawing;

namespace Mosaique.Core
{
    public static class GridLayout
    {
        /// <summary>Calcule les cases dans l'ordre de lecture, de gauche à droite puis de haut en bas.</summary>
        public static Rectangle[] Calculate(Rectangle frame, int windowCount)
        {
            if (windowCount < 0 || windowCount > 4)
                throw new ArgumentOutOfRangeException(nameof(windowCount));

            if (windowCount == 0)
                return Array.Empty<Rectangle>();

            if (windowCount == 1)
                return new[] { frame };

            // Les cases à droite et en bas absorbent le pixel restant si la dimension est impaire.
            int leftWidth = frame.Width / 2;
            int rightWidth = frame.Width - leftWidth;
            int topHeight = windowCount == 2 ? frame.Height : frame.Height / 2;
            var cells = new Rectangle[windowCount];
            cells[0] = new Rectangle(frame.X, frame.Y, leftWidth, topHeight);
            cells[1] = new Rectangle(frame.X + leftWidth, frame.Y, rightWidth, topHeight);

            if (windowCount == 2)
                return cells;

            int bottomY = frame.Y + topHeight;
            int bottomHeight = frame.Height - topHeight;
            cells[2] = new Rectangle(frame.X, bottomY,
                windowCount == 3 ? frame.Width : leftWidth, bottomHeight);

            if (windowCount == 4)
                cells[3] = new Rectangle(frame.X + leftWidth, bottomY, rightWidth, bottomHeight);

            return cells;
        }

        /// <summary>Agrandit une case visible pour inclure les bordures invisibles fournies.</summary>
        public static Rectangle CompensateBorders(Rectangle cell, int left, int top, int right, int bottom) =>
            new Rectangle(cell.X - left, cell.Y - top,
                cell.Width + left + right, cell.Height + top + bottom);
    }
}
