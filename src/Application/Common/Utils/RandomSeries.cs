using System;
using System.Collections.Generic;

namespace Electricity.Application.Common.Utils
{
    public class RandomSeries
    {
        private System.Random _random;

        public double Scale { get; set; } = 1;
        public double Min { get; set; } = double.MinValue;
        public double Max { get; set; } = double.MaxValue;

        public double[] Distribution { get; set; } = new double[] { 0 };
        public double MaxShift { get; set; } = 0;

        public bool Cumulative { get; set; } = false;
        public double Accumulator { get; set; } = 0;


        public RandomSeries(int? seed = null)
        {
            if (seed is int valueOfSeed)
            {
                _random = new System.Random(valueOfSeed);
            }
            else
            {
                _random = new System.Random();
            }
        }

        public double Next()
        {
            int randomIndex = _random.Next(0, Distribution.Length);
            double randomShift = (double)(_random.NextDouble() * 2 - 1) * MaxShift;
            double value = Distribution[randomIndex] + randomShift;
            value = Math.Clamp(value, Min, Max);
            value = Scale * value;

            if (Cumulative)
            {
                Accumulator += value;
                value = Accumulator;
            }

            return value;
        }
    }
}