using System.Collections.Generic;

namespace Common.Random
{
    public class RandomSeries
    {
        private System.Random _random;

        public RandomSeries(int seed)
        {
            _random = new System.Random(seed);
        }

        public IEnumerable<float> NextFloat(int length, float start = 0)
        {
            float value = start;
            for (int i = 0; i < length; i++)
            {
                yield return value;
                value += (float)_random.NextDouble() * 2 - 1;
            }
        }
    }
}