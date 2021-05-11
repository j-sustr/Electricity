using Electricity.Application.Common.Utils;
using Electricity.Infrastructure.DataSource.Util;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Infrastructure.DataSource.Fake
{
    using static ArchiveInfoUtil;
    using static GroupInfoUtil;

    public static class FakeData
    {
        public static FakeUserData[] GetUsers()
        {
            var users = GetUserDataArray();
            var range = GetRange();
            foreach (var user in users) {
                user.SetRange(range);
            }
            return users;
        }


        static DateRange GetRange()
        {
            var start = DateTime.SpecifyKind(new DateTime(2021, 1, 1), DateTimeKind.Local);
            var end = DateTime.SpecifyKind(new DateTime(2021, 3, 1), DateTimeKind.Local);
            return new DateRange(start.ToUniversalTime(), end.ToUniversalTime());
        }

        static FakeUserData[] GetUserDataArray() {
            var user1 = new FakeUserData
            {
                UserId = GuidUtil.IntToGuid(1),
                Username = "TestUser1",
                Password = "1",
                Groups = new List<GroupInfo>
                {
                    new GroupInfo
                    {
                        ID = GuidUtil.IntToGuid(0),
                        Name = "UserData",
                        Subgroups = new List<GroupInfo>(),
                    },
                    new GroupInfo
                    {
                        ID = GuidUtil.IntToGuid(2),
                        Name = "Oddeleni",
                        Subgroups = new List<GroupInfo>
                        {
                            new GroupInfo
                            {
                                ID = GuidUtil.IntToGuid(3),
                                Name = "Mistnost101",
                                Archives = new ArchiveInfo[] {
                                    CreateArchiveInfo(0, 0, null, null),
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    CreateArchiveInfo(6, 0, null, null),
                                },
                                Subgroups = new List<GroupInfo>(),
                            },
                            new GroupInfo
                            {
                                ID = GuidUtil.IntToGuid(4),
                                Name = "Mistnost201",
                                Archives = new ArchiveInfo[] {
                                    CreateArchiveInfo(0, 0, null, null),
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    CreateArchiveInfo(6, 0, null, null),
                                },
                                Subgroups = new List<GroupInfo>(),
                            }
                        }
                    }
                }
            };
            var user2 = new FakeUserData
            {
                UserId = GuidUtil.IntToGuid(100),
                Username = "Karel",
                Password = "2",
                Groups = new List<GroupInfo> {
                    new GroupInfo
                    {
                        ID = GuidUtil.IntToGuid(200),
                        Name = "UserData",
                        Subgroups = new List<GroupInfo>(),
                    },
                    new GroupInfo
                    {
                        ID = GuidUtil.IntToGuid(300),
                        Name = "KarlovoMereni",
                        Archives = new ArchiveInfo[] {
                            CreateArchiveInfo(0, 0, null, null),
                            null,
                            null,
                            null,
                            null,
                            null,
                            CreateArchiveInfo(6, 0, null, null),
                        },
                        Subgroups = new List<GroupInfo>(),
                    },
                    new GroupInfo
                    {
                        ID = GuidUtil.IntToGuid(400),
                        Name = "KarlovoDruheMereni",
                        Archives = new ArchiveInfo[] {
                            CreateArchiveInfo(0, 0, null, null),
                            null,
                            null,
                            null,
                            null,
                            null,
                            CreateArchiveInfo(6, 0, null, null),
                        },
                        Subgroups = new List<GroupInfo>(),
                    },
                    new GroupInfo
                    {
                        ID = GuidUtil.IntToGuid(500),
                        Name = "KarlovoTretiMereni",
                        Archives = new ArchiveInfo[] {
                            CreateArchiveInfo(0, 0, null, null),
                            null,
                            null,
                            null,
                            null,
                            null,
                            CreateArchiveInfo(6, 0, null, null),
                        },
                        Subgroups = new List<GroupInfo>(),
                    }
                },
            };

            return new FakeUserData[] { user1, user2 };
        }
    }
}
