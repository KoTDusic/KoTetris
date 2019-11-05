using System;

namespace GameLogic
{
    public static class RandomHelper
    {
        private static readonly Random Random = new Random();

        public static int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }
    }
}