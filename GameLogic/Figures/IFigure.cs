using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLogic.Figures
{
    public interface IFigure
    {
        IReadOnlyList<Point> GetGeometry();
    }
}