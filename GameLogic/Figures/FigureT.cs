using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLogic.Figures
{
    /// <summary>
    /// буква Т
    /// </summary>
    public class FigureT : IFigure
    {
        private static readonly IReadOnlyList<Point> Geometry = FigureHelper.ParseFigureString(
            "***",
            "*o*",
            "ooo");

        public IReadOnlyList<Point> GetGeometry()
        {
            return Geometry;
        }
    }
}