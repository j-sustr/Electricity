using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Models;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Utils
{
    public static class ArchiveUtils
    {
        public static void MustHaveArchives(GroupInfo groupInfo, Arch[] archives)
        {
            var missingArchs = new List<Arch>();
            foreach (var arch in archives)
            {
                if (!HasArchive(groupInfo, arch))
                {
                    missingArchs.Add(arch);
                }
            }

            if (missingArchs.Count != 0)
            {
                throw new MissingArchivesException(missingArchs.ToArray());
            }
        }

        public static Interval MustGetSubintervalWithData(GroupInfo groupInfo, Interval interval, string intervalName, Arch[] archives)
        {
            if (interval == null) return null;

            var subinterval = GetRangeOverlapWithArchives(groupInfo, interval, archives);
            if (subinterval == null)
                throw new IntervalOutOfRangeException();

            return subinterval;
        }

        public static bool HasArchive(GroupInfo groupInfo, Arch arch)
        {
            try
            {
                return groupInfo.Archives[(int)arch] != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static Interval GetRangeOverlapWithMain(GroupInfo groupInfo, Interval interval)
        {
            return GetRangeOverlapWithArchives(groupInfo, interval, new Arch[] {
                Arch.Main
            });
        }

        public static Interval GetRangeOverlapWithElectrityMeter(GroupInfo groupInfo, Interval interval)
        {
            return GetRangeOverlapWithArchives(groupInfo, interval, new Arch[] {
                Arch.Main, Arch.ElectricityMeter
            });
        }

        public static Interval GetRangeOverlapWithArchives(GroupInfo groupInfo, Interval interval, Arch[] archives)
        {
            var overlap = interval;

            foreach (var arch in archives)
            {
                var range = GetRangeForArchive(groupInfo, arch);

                overlap = overlap.GetOverlap(range);
            }

            return overlap;
        }

        public static Interval GetRangeForArchive(GroupInfo groupInfo, Arch arch)
        {
            DateRange range = null;
            try
            {
                range = groupInfo.Archives[(int)arch].Range;
            }
            catch (Exception ex)
            {
                return null;
            }

            if (range == null) return null;

            return Interval.FromDateRange(range);
        }

        public static bool HasDataOnRange(GroupInfo groupInfo, Interval interval, Arch arch)
        {
            ArchiveInfo archInfo = null;
            try
            {
                archInfo = groupInfo.Archives[(int)arch];
            }
            catch (Exception ex)
            {
                return false;
            }

            if (archInfo == null)
                return false;

            var range = Interval.FromDateRange(archInfo.Range);

            var overlap = interval.GetOverlap(range);
            if (overlap == null)
            {
                return false;
            }
            return true;
        }


        public static string CreateMissingArchivesMessage(bool missingMain, bool missingEM)
        {
            string message = $"Missing archives: ";
            message += missingMain ? (nameof(Arch.Main) + ", ") : "";
            message += missingEM ? (nameof(Arch.ElectricityMeter) + ", ") : "";
            return message;
        }

        public static string CreateArchivesDoNotHaveDataOnRangeMessage(bool noDataMain, bool noDataEM)
        {
            string message = $"Archives have no data on specified range: ";
            message += noDataMain ? (nameof(Arch.Main) + ", ") : "";
            message += noDataEM ? (nameof(Arch.ElectricityMeter) + ", ") : "";
            return message;
        }
    }
}
