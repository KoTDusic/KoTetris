using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLogic.Figures
{
    /// <summary>
    /// изогнутая направо
    /// </summary>
    public class FigureS : IFigure
    {
        private static readonly IReadOnlyList<Point> Geometry = FigureHelper.ParseFigureString(
            "***",
            "*oo",
            "oo*");

        public IReadOnlyList<Point> GetGeometry()
        {
            return Geometry;
        }
    }
}