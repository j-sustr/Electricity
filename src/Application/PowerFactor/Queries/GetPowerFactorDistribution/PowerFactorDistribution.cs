using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Models.Quantities;
using Electricity.Application.Common.Services;
using Electricity.Application.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution
{
    public static class PowerFactorDistribution
    {
        public static MainQuantity[] GetQuantities(Phase[] phases)
        {
            var quanities = new List<MainQuantity>();

            foreach (var p in phases)
            {
                quanities.Add(new MainQuantity
                {
                    Type = MainQuantityType.CosFi,
                    Phase = p
                });
            }

            return quanities.ToArray();
        }

        public static int[] BinValues(float[] values, float[] thresholds)
        {
            int n = thresholds.Length + 1;
            var bins = new int[n];
            Array.Fill(bins, 0);

            foreach (var v in values)
            {
                int i = 0;
                for (; i < (n - 2); i++)
                {
                    if (v < thresholds[i])
                    {
                        bins[i] += 1;
                        break;
                    }
                }

                // last threshold is inclusive
                if (v <= thresholds[i])
                {
                    bins[i] += 1;
                    continue;
                }

                bins[i + 1] += 1;
            }

            return bins;
        }

        public static Tuple<BinRange, int>[] CreateDistributionTuples(int[] bins, float[] thresholds)
        {
            var items = new List<Tuple<BinRange, int>>();

            items.Add(Tuple.Create(new BinRange
            {
                Start = null,
                End = thresholds[0]
            }, bins[0]));

            int i = 1;
            for (; i < thresholds.Length; i++)
            {
                items.Add(Tuple.Create(new BinRange
                {
                    Start = thresholds[i - 1],
                    End = thresholds[i]
                }, bins[i]));
            }

            items.Add(Tuple.Create(new BinRange
            {
                Start = null,
                End = thresholds[0]
            }, bins[i]));

            return items.ToArray();
        }



        // --- DEAD CODE ---

        static string CreateBinKey(float start, float end)
        {
            const float RES = 0.001f;
            return start.ToString("0.000") + "-" + (end - RES).ToString("0.000");
        }
    }
}
