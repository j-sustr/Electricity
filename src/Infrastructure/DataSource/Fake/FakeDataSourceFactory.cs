using KMB.DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Utils;
using System.Collections.Generic;
using Electricity.Infrastructure.DataSource.Abstractions;
using Electricity.Infrastructure.DataSource.Util;

namespace Electricity.Infrastructure.DataSource.Fake
{
    using static ArchiveInfoUtil;
    using static GroupInfoUtil;

    public class FakeDataSourceFactory : IDataSourceFactory
    {
        private int _seed;

        public string Username { get; set; }
        public string Password { get; set; }

        public GroupInfo GroupTree = new GroupInfo
        {
            ID = GuidUtil.IntToGuid(0),
            Name = "TestUser",
            Subgroups = new List<GroupInfo>
            {
                new GroupInfo
                {
                    ID = GuidUtil.IntToGuid(1),
                    Name = "Oddeleni",
                    Subgroups = new List<GroupInfo>
                    {
                        new GroupInfo
                        {
                            ID = GuidUtil.IntToGuid(2),
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
                            ID = GuidUtil.IntToGuid(3),
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
                },
                new GroupInfo
                {
                    ID = GuidUtil.IntToGuid(4),
                    Name = "UserData",
                    Subgroups = new List<GroupInfo>(),
                }
            }
        };

        public FakeDataSourceFactory(int seed)
        {
            _seed = seed;
        }

        public KMB.DataSource.DataSource CreateDataSource(DataSourceCreationParams creationParams)
        {
            var ds = new FakeDataSource(_seed);
            ds.GroupTree = GroupTree;
            return ds;
        }
    }
}