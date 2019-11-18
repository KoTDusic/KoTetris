using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLogic.Figures
{
    /// <summary>
    /// изогнутая налево
    /// </summary>
    public class FigureZ : IFigure
    {
        private static readonly IReadOnlyList<Point> Geometry = FigureHelper.ParseFigureString(
            "***",
            "oo*",
            "*oo");

        public IReadOnlyList<Point> GetGeometry()
        {
            return Geometry;
        }
    }
}