using System.Collections.Generic;

namespace Common.Random
{
    public class RandomSeries
    {
        private System.Random _random;

        private float _next;

        public bool Cumulative { get; set; } = false;

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

            if (Cumulative)
            {
                _next += (float)_random.NextDouble();
            }
            else
            {
                _next += (float)_random.NextDouble() * 2 - 1;
            }

            return value;
        }
    }
}