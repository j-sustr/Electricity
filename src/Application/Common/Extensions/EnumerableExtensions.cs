using System;
using System.Collections.Generic;
using System.Linq;

namespace Electricity.Application.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOfMin<TSource>(this IEnumerable<TSource> source) where TSource : IComparable
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var minValue = source.FirstOrDefault();
            int minIndex = -1;
            int index = -1;

            foreach (var value in source)
            {
                index++;

                if (value.CompareTo(minValue) < 0)
                {
                    minValue = value;
                    minIndex = index;
                }
            }

            if (index == -1)
            {
                throw new InvalidOperationException("Sequence was empty");
            }

            return minIndex;
        }

        public static int IndexOfMax<TSource>(this IEnumerable<TSource> source) where TSource : IComparable
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var maxValue = source.FirstOrDefault();
            int minIndex = -1;
            int index = -1;

            foreach (var value in source)
            {
                index++;

                if (value.CompareTo(maxValue) > 0)
                {
                    maxValue = value;
                    minIndex = index;
                }
            }

            if (index == -1)
            {
                throw new InvalidOperationException("Sequence was empty");
            }

            return minIndex;
        }

    }
}