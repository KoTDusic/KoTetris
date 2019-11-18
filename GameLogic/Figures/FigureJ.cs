using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLogic.Figures
{
    /// <summary>
    /// Буква L налево
    /// </summary>
    public class FigureJ:IFigure
    {
        private static readonly IReadOnlyList<Point> Geometry = FigureHelper.ParseFigureString(
            "*o*",
            "*o*",
            "oo*");

        public IReadOnlyList<Point> GetGeometry()
        {
            return Geometry;
        }
    }
}