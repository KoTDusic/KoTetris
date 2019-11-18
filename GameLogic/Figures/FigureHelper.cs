using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLogic.Figures
{
    public class FigureHelper
    {
        public static IReadOnlyList<Point> ParseFigureString(params string[] figureStrings)
        {
            var result = new List<Point>();
            for (var i = 0; i < figureStrings.Length; i++)
            {
                var figureString = figureStrings[i];
                for (var j = 0; j < figureString.Length; j++)
                {
                    if (figureString[figureString.Length - 1 - j] == 'o')
                    {
                        result.Add(new Point(i, j));
                    }
                }
            }

            return result;
        }
    }
}