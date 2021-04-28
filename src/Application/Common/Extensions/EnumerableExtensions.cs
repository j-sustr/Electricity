using System;
using System.Collections.Generic;
using System.Linq;

namespace Electricity.Application.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOfMax<TSource>(
            this IEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("Sequence is empty.");

                if (comparer == null)
                    comparer = Comparer<TSource>.Default;

                int index = 0, maxIndex = 0;
                var maxProjection = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    index++;
                    var projectedItem = enumerator.Current;

                    if (comparer.Compare(projectedItem, maxProjection) > 0)
                    {
                        maxIndex = index;
                        maxProjection = projectedItem;
                    }
                }
                return maxIndex;
            }
        }


    }
}