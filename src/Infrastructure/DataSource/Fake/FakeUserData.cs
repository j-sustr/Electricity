using KMB.DataSource;
using System;
using System.Collections.Generic;

namespace Electricity.Infrastructure.DataSource.Fake
{
    public class FakeUserData
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<GroupInfo> Groups { get; set; }

        public GroupInfo GroupTree {
            get {
                return new GroupInfo
                {
                    ID = UserId,
                    Name = Username,
                    Subgroups = Groups
                };
            }
        }

        public void SetRange(DateRange range)
        {
            void SetRangeOnGroup(DateRange range, GroupInfo g)
            {
                if (g == null) return;
                if (g.Archives != null)
                {
                    foreach (var archive in g.Archives)
                    {
                        if (archive != null)
                        {
                            archive.Range = range;
                        }
                    }
                }
                if (g.Subgroups != null)
                {
                    foreach (var subgroup in g.Subgroups)
                    {
                        SetRangeOnGroup(range, subgroup);
                    }
                }
            }

            foreach (var grp in Groups)
            {
                SetRangeOnGroup(range, grp);
            }
        }
    }
}
