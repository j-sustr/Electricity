using Electricity.Application.Common.Enums;
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
        public static ElectricityMeterQuantity[] GetQuantities(Phase[] phases)
        {
            var emQuantityTypes = new ElectricityMeterQuantityType[] {
                    ElectricityMeterQuantityType.ActiveEnergy,
                    ElectricityMeterQuantityType.ReactiveEnergyL,
                    ElectricityMeterQuantityType.ReactiveEnergyC,
                };

            var quanities = new List<ElectricityMeterQuantity>();

            foreach (var qt in emQuantityTypes)
            {
                foreach (var p in phases)
                {
                    quanities.Add(new ElectricityMeterQuantity
                    {
                        Type = qt,
                        Phase = p
                    });
                }
            }

            return quanities.ToArray();
        }

        public static float[] CalcCosFiForPhase(ElectricityMeterRowsView emView, Phase phase)
        {
            var activeEnergy = emView.GetDifferenceInQuarterHours(new ElectricityMeterQuantity
            {
                Type = ElectricityMeterQuantityType.ActiveEnergy,
                Phase = phase,
            });
            var reactiveEnergyL = emView.GetDifferenceInQuarterHours(new ElectricityMeterQuantity
            {
                Type = ElectricityMeterQuantityType.ReactiveEnergyL,
                Phase = phase,
            });
            var reactiveEnergyC = emView.GetDifferenceInQuarterHours(new ElectricityMeterQuantity
            {
                Type = ElectricityMeterQuantityType.ReactiveEnergyC,
                Phase = phase,
            });

            var ep = activeEnergy.Values().ToArray();
            var eqL = reactiveEnergyL.Values().ToArray();
            var eqC = reactiveEnergyC.Values().ToArray();
            var cosFi = new float[ep.Length];

            for (int i = 0; i < ep.Length; i++)
            {
                cosFi[i] = ElectricityUtil.CalcCosFi(ep[i], eqL[i] - eqC[i]);
            }

            return cosFi;
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


        public static Dictionary<string, int> CalcGeneralDistribution(float[] cosFi)
        {
            var counter = new Dictionary<string, int>();
            counter.Add("0.950-1.000", 0);
            counter.Add("0.900-0.949", 0);
            counter.Add("0.800-0.899", 0);
            counter.Add("0.700-0.799", 0);
            counter.Add("0.600-0.699", 0);
            counter.Add("0.000-0.599", 0);
            counter.Add("outlier", 0);

            foreach (float value in cosFi)
            {
                float absVal = Math.Abs(value);
                switch (absVal)
                {
                    case var v when v <= 1 && v >= 0.95:
                        counter["0.950-1.000"] += 1;
                        break;
                    case var v when v >= 0.9:
                        counter["0.900-0.949"] += 1;
                        break;
                    case var v when v >= 0.8:
                        counter["0.800-0.899"] += 1;
                        break;
                    case var v when v >= 0.7:
                        counter["0.700-0.799"] += 1;
                        break;
                    case var v when v >= 0.6:
                        counter["0.600-0.699"] += 1;
                        break;
                    case var v when v >= 0:
                        counter["0.000-0.599"] += 1;
                        break;
                    default:
                        counter["outlier"] += 1;
                        break;
                }
            }

            return counter;
        }
    }
}
