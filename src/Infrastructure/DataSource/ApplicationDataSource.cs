using System;
using System.Collections.Generic;
using System.Linq;
using KMB.DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;

namespace Electricity.Infrastructure.DataSource
{
    public class ApplicationDataSource : IDisposable, IGroupService, IQuantityService, ITableCollection, IAuthenticationService
    {
        private readonly ICurrentUserService _currentUserService;

        private readonly Tenant _tenant;

        private readonly KMB.DataSource.DataSource _dataSource;

        private readonly IDisposable _connection;
        private readonly IDisposable _transaction;

        public ApplicationDataSource(
            ICurrentUserService currentUserService,
            IDataSourceManager dsManager,
            ITenantProvider tenantProvider)
        {
            _currentUserService = currentUserService;
            _tenant = tenantProvider.GetTenant();

            if (_dataSource == null)
            {
                _tenant.DataSourceId = dsManager.CreateDataSource(_tenant.DataSourceConfig);

                _dataSource = dsManager.GetDataSource((Guid)_tenant.DataSourceId);
            }

            _connection = _dataSource.NewConnection();
            _transaction = _dataSource.BeginTransaction(_connection);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }

        public Group[] GetUserGroups()
        {
            return _dataSource.GetUserGroups(GetUserGuid(), _connection, _transaction).ToArray();
        }

        public GroupTreeNode GetUserGroupTree()
        {
            var userGroups = _dataSource.GetUserGroups(GetUserGuid(), _connection, _transaction);
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

        public Quantity[] GetQuantities(Guid groupId, byte arch, DateRange range)
        {
            return _dataSource.GetQuantities(groupId, arch, range, _connection, _transaction);
        }

        public ITable GetTable(Guid groupId, byte arch)
        {
            return new DataSourceTableReader(this._dataSource, groupId, arch, _connection, _transaction);
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

            var reader = new DataSourceTableReader(this._dataSource, (Guid)groupId, arch, _connection, _transaction);

            return reader.GetInterval();
        }

        public Guid Login(string username, string password)
        {
            return _dataSource.Login(username, password, _connection, _transaction);
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
            var groups = _dataSource.GetGroups(rootId, _connection, _transaction);
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

            var groups = _dataSource.GetUserGroups(GetUserGuid(), _connection, _transaction);

            return groups.Find(g => g.ID == guid);
        }
    }
}