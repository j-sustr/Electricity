using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Extensions
{
    public static partial class LINQExtension
    {
        // https://github.com/mariusschulz/ExtraLINQ#chunk

        public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size, int offset = 0)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), nameof(size) + " must be positive.");

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), nameof(offset) + " must be positive or 0.");

            return ChunkIterator(source, size, offset);
        }

        private static IEnumerable<TSource[]> ChunkIterator<TSource>(IEnumerable<TSource> source, int size, int offset)
        {
            if (offset > 0)
            {
                yield return source.Take(offset).ToArray();
                source = source.Skip(offset);
            }

            TSource[] currentChunk = null;
            int currentIndex = 0;

            foreach (var element in source)
            {
                currentChunk = currentChunk ?? new TSource[size];
                currentChunk[currentIndex++] = element;

                if (currentIndex == size)
                {
                    yield return currentChunk;
                    currentIndex = 0;
                    currentChunk = null;
                }
            }

            // Do we have an incomplete chunk of remaining elements?
            if (currentChunk != null)
            {
                // This last chunk is incomplete, otherwise it would've been returned already.
                // Thus, we have to create a new, shorter array to hold the remaining elements.
                var lastChunk = new TSource[currentIndex];
                Array.Copy(currentChunk, lastChunk, currentIndex);

                yield return lastChunk;
            }
        }
    }
}
