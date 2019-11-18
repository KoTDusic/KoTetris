using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLogic.Figures
{
    /// <summary>
    /// Палка
    /// </summary>
    public class FigureI : IFigure
    {
        private static readonly IReadOnlyList<Point> Geometry = FigureHelper.ParseFigureString(
            "o***",
            "o***",
            "o***",
            "o***");

        public IReadOnlyList<Point> GetGeometry()
        {
            return Geometry;
        }
    }
}