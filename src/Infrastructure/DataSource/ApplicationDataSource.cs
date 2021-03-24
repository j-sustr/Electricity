using System;
using System.Collections.Generic;
using System.Linq;
using KMB.DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;

namespace Electricity.Infrastructure.DataSource
{
    public class ApplicationDataSource : IGroupService, IQuantityService, ITableCollection, IAuthenticationService
    {
        private readonly ICurrentUserService _currentUserService;

        private readonly Tenant _tenant;

        private readonly KMB.DataSource.DataSource _dataSource;

        public ApplicationDataSource(
            ICurrentUserService currentUserService,
            IDataSourceManager dsManager,
            ITenantProvider tenantProvider)
        {
            _currentUserService = currentUserService;
            _tenant = tenantProvider.GetTenant();

            _dataSource = dsManager.GetDataSource(_tenant.DataSourceId);

            if (_dataSource == null)
            {
                _tenant.DataSourceId = dsManager.CreateDataSource(_tenant.DataSourceConfig);

                _dataSource = dsManager.GetDataSource(_tenant.DataSourceId);
            }
        }

        public KMB.DataSource.Group[] GetUserGroups()
        {
            return _dataSource.GetUserGroups(GetUserGuid()).ToArray();
        }

        public GroupTreeNode GetUserGroupTree()
        {
            var userGroups = _dataSource.GetUserGroups(GetUserGuid());
            var root = new GroupTreeNode();
            root.Nodes = userGroups.Select(g =>
            {
                var node = new GroupTreeNode();
                node.Group = g;
                ReadGroupTree(node, g.ID);
                return node;
            }).ToArray();
            return root;
        }

        public KMB.DataSource.Quantity[] GetQuantities(Guid groupId, byte arch, KMB.DataSource.DateRange range)
        {
            return _dataSource.GetQuantities(groupId, arch, range);
        }

        public ITable GetTable(Guid groupId, byte arch)
        {
            return new DataSourceTableReader(this._dataSource, groupId, arch);
        }

        public Interval GetInterval(Guid? groupId, byte arch)
        {
            if (groupId == null)
            {
                var groups = GetUserGroups();
                if (groups.Length == 0)
                    return null;

                groupId = groups[0].ID;
            }

            var reader = new DataSourceTableReader(this._dataSource, (Guid)groupId, arch);

            return reader.GetInterval();
        }

        public Interval GetIntervalOverlap(Guid groupId, byte arch, Interval interval)
        {
            return interval;
        }

        public Guid Login(string username, string password)
        {
            return _dataSource.Login(username, password);
        }

        private Guid GetUserGuid()
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new Exception("could not parse userId");
            }

            return userGuid;
        }

        private void ReadGroupTree(GroupTreeNode root, Guid rootId)
        {
            var groups = _dataSource.GetGroups(rootId);
            root.Nodes = groups.Select(g =>
            {
                var node = new GroupTreeNode();
                node.Group = g;
                ReadGroupTree(node, g.ID);
                return node;
            }).ToArray();
        }

        public Group GetGroupById(string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            var groups = _dataSource.GetUserGroups(GetUserGuid());

            return groups.Find(g => g.ID == guid);
        }
    }
}