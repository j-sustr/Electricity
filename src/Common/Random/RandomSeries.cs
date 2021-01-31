using System.Collections.Generic;

namespace Common.Random
{
    public class RandomSeries
    {
        private System.Random _random;

        private float _next;

        public RandomSeries(float start = 0, int? seed = null)
        {
            _next = start;

            if (seed is int valueOfSeed)
            {
                _random = new System.Random(valueOfSeed);
            }
            else
            {
                _random = new System.Random();
            }
        }

        public float Next()
        {
            float value = _next;
            _next += (float)_random.NextDouble() * 2 - 1;
            return value;
        }
    }
}