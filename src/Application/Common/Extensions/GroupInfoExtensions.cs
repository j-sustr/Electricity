using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Extensions
{
    public static class GroupInfoExtensions
    {
        public static GroupInfo[] GetUserRecordGroupInfos(this GroupInfo g)
        {
            List<GroupInfo> recordGroups = new List<GroupInfo>();

            void AddRecordGroups(List<GroupInfo> list, GroupInfo gi)
            {
                if (gi.Archives != null)
                    recordGroups.Add(gi);

                List<GroupInfo> groups = gi.Subgroups;
                if (groups == null) return;

                foreach (var item in groups)
                    AddRecordGroups(list, item);

            }

            AddRecordGroups(recordGroups, g);

            return recordGroups.ToArray();
        }
    }
}
