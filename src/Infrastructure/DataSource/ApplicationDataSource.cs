using System;
using System.Collections.Generic;
using System.Linq;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using DS = DataSource;

namespace Electricity.Infrastructure.DataSource
{
    public class ApplicationDataSource : IGroupService, IQuantityService, ITableCollection, IAuthenticationService
    {
        private readonly Tenant _tenant;

        private readonly DS.DataSource _dataSource;

        public ApplicationDataSource(IDataSourceManager dsManager, ITenantProvider tenantProvider)
        {
            _tenant = tenantProvider.GetTenant();

            _dataSource = dsManager.GetDataSource(_tenant.DataSourceId);

            if (_dataSource == null)
            {
                _tenant.DataSourceId = dsManager.CreateDataSource(_tenant.DataSourceConfig);

                _dataSource = dsManager.GetDataSource(_tenant.DataSourceId);
            }
        }

        public DS.Group[] GetUserGroups(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return null;
            }

            return _dataSource.GetUserGroups(userGuid).ToArray();
        }

        public GroupTreeNode GetUserGroupTree(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return null;
            }

            var userGroups = _dataSource.GetUserGroups(userGuid);
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

        public DS.Quantity[] GetQuantities(Guid groupId, byte arch, DS.DateRange range)
        {
            return _dataSource.GetQuantities(groupId, arch, range);
        }

        public ITable GetTable(Guid groupId, byte arch)
        {
            return new DataSourceTableReader(this._dataSource, groupId, arch);
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

        public Guid Login(string username, string password)
        {
            return _dataSource.Login(username, password);
        }
    }
}