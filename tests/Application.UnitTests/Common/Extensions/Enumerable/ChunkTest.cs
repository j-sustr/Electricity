using NUnit.Framework;
using System.Collections.Generic;
using Electricity.Application.Common.Extensions;
using FluentAssertions;

namespace Electricity.Application.UnitTests.Common.Extensions.Enumerable
{
    internal class ChunkTest
    {
        public class IntTestData
        {
            public int ChunkSize { get; set; }
            public int ChunkOffset { get; set; }

            public IEnumerable<int> Source { get; set; }
            public IEnumerable<int[]> ExpectedResult { get; set; }
        }

        public static IntTestData[] _intTestData = new[]{
            new IntTestData(){
                ChunkSize = 3,
                ChunkOffset = 2,
                Source = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 },
                ExpectedResult = new int[][]
                {
                    new int[] { 1, 2, },
                    new int[] { 3, 4 ,5 },
                    new int[] { 6, 7 ,8 },
                    new int[] { 9, 10 , 11 },
                    new int[] { 12 },
                }
            },
        };

        [Test]
        public void ShouldChunkArrayOfInts([ValueSource("_intTestData")] IntTestData data)
        {
            data.Source.Chunk(data.ChunkSize, data.ChunkOffset).Should().BeEquivalentTo(data.ExpectedResult);
        }
    }
}
