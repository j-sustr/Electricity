using NUnit.Framework;
using System.Collections.Generic;
using Electricity.Application.Common.Extensions;
using FluentAssertions;
using System;

namespace Electricity.Application.UnitTests.Common.Extensions.Enumerable
{
    internal class ChunkByIndexTest
    {
        public class IntTestData
        {
            public IEnumerable<int> Source { get; set; }
            public IEnumerable<int[]> ExpectedResult { get; set; }
        }

        private static IntTestData[] _intTestData = new[]{
            new IntTestData(){
                Source = new int[] {
                    0,0,0,0,0,
                    1,1,1,
                    2,2,2,2,2,
                    3,3,3,3,
                    4,4,4,
                },
                ExpectedResult = new int[][]
                {
                    new int[] {
                        0,0,0,0,0,
                    },
                    new int[] {
                        1,1,1,
                    },
                    new int[] {
                        2,2,2,2,2,
                    },
                    new int[] {
                        3,3,3,3,
                    },
                    new int[] {
                        4,4,4,
                    },
                }
            },
            new IntTestData(){
                Source = new int[] {
                    2,2,2,2,2,
                    3,3,3,3,
                    4,4,4,
                },
                ExpectedResult = new int[][]
                {
                    null,
                    null,
                    new int[] {
                        2,2,2,2,2,
                    },
                    new int[] {
                        3,3,3,3,
                    },
                    new int[] {
                        4,4,4,
                    },
                }
            },
        };

        [Test]
        public void ShouldChunkArrayOfInts([ValueSource("_intTestData")] IntTestData data)
        {
            data.Source.ChunkByIndex((x, i) => x).Should().BeEquivalentTo(data.ExpectedResult);
        }

        public class TupleTestData
        {
            public IEnumerable<Tuple<int, int>> Source { get; set; }
            public IEnumerable<Tuple<int, int>[]> ExpectedResult { get; set; }
        }

        private static TupleTestData[] _tupleTestData = new[]{
            new TupleTestData(){
                Source = new Tuple<int, int>[] {
                    Tuple.Create(0, 1),
                    Tuple.Create(0, 2),
                    Tuple.Create(0, 3),
                    Tuple.Create(0, 4),
                    Tuple.Create(1, 5),
                    Tuple.Create(1, 6),
                    Tuple.Create(1, 7),
                    Tuple.Create(2, 8),
                    Tuple.Create(2, 9),
                    Tuple.Create(2, 10),
                    Tuple.Create(2, 11),
                    Tuple.Create(2, 12),
                    Tuple.Create(4, 13),
                    Tuple.Create(4, 14),
                    Tuple.Create(4, 15),
                },
                ExpectedResult = new Tuple<int, int>[][]
                {
                    new Tuple<int, int>[] {
                        Tuple.Create(0, 1),
                        Tuple.Create(0, 2),
                        Tuple.Create(0, 3),
                        Tuple.Create(0, 4),
                    },
                    new Tuple<int, int>[] {
                        Tuple.Create(1, 5),
                        Tuple.Create(1, 6),
                        Tuple.Create(1, 7),
                    },
                    new Tuple<int, int>[] {
                        Tuple.Create(2, 8),
                        Tuple.Create(2, 9),
                        Tuple.Create(2, 10),
                        Tuple.Create(2, 11),
                    },
                    null,
                    new Tuple<int, int>[] {
                        Tuple.Create(4, 13),
                        Tuple.Create(4, 14),
                        Tuple.Create(4, 15),
                    },
                }
            },
        };

        [Test]
        public void ShouldChunkArrayOfTuples([ValueSource("_tupleTestData")] TupleTestData data)
        {
            data.Source.ChunkByIndex((x, i) => x.Item1).Should().BeEquivalentTo(data.ExpectedResult);
        }
    }
}