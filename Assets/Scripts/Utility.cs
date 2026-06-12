using System.Collections.Generic;

namespace CursedKnight
{
    public static class Utility
    {
        public static void Shuffle<T>(List<T> list)
        {
            var random = new System.Random();
            var n = list.Count;

            for (var i = n - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                
                // Tuple Swap 
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
    }
}