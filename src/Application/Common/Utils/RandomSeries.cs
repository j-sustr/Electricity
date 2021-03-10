using System.Collections.Generic;

namespace Electricity.Application.Common.Utils
{
    public class RandomSeries
    {
        private System.Random _random;

        private float _nextValue;

        public bool Positive { get; set; } = true;
        public bool Cumulative { get; set; } = false;

        public RandomSeries(float start = 0, int? seed = null)
        {
            _nextValue = start;

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
            float value = _nextValue;
            float next = 0;

            if (Positive)
            {
                next = (float)_random.NextDouble();
            }
            else
            {
                next = (float)(_random.NextDouble() * 2 - 1);
            }

            if (Cumulative)
            {
                _nextValue += next;
            }
            else
            {
                _nextValue = next;
            }

            return value;
        }
    }
}