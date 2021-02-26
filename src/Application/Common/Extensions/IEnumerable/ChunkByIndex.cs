using System;
using System.Collections.Generic;

namespace Electricity.Application.Common.Extensions
{
    public static class LINQExtension
    {
        // https://github.com/mariusschulz/ExtraLINQ#chunk

        public static IEnumerable<TSource[]> ChunkByIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, int, int> indexResolver)
        {
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
                yield break;
            var element = enumerator.Current;
            int prevIndex = indexResolver(element, 0);
            for (int i = 0; i < prevIndex; i++)
            {
                yield return null;
            }
            var buffer = new List<TSource>();
            buffer.Add(element);
            for (int i = 0; true; i++)
            {
                if (!enumerator.MoveNext())
                {
                    if (buffer.Count > 0)
                    {
                        yield return buffer.ToArray();
                    }
                    yield break;
                }

                element = enumerator.Current;
                int index = indexResolver(element, i);
                if (index != prevIndex)
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                    if (index != prevIndex + 1)
                    {
                        int j = prevIndex + 1;
                        do
                        {
                            yield return null;
                            j++;
                        } while (j < index);
                    }
                }
                else
                {
                    buffer.Add(element);
                }
                prevIndex = index;
            }
        }
    }
}