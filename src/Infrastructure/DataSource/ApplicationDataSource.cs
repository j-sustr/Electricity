using System;
using System.Linq;
using KMB.DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;

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
            ITenantProvider tenantProvider,
            IDataSourceManager dsManager,
            IHttpContextAccessor accessor)
        {
            var t = accessor.HttpContext.Session.GetString("__tenant__");

            _currentUserService = currentUserService;
            _tenant = tenantProvider.GetTenant();

            if (_tenant == null)
            {
                throw new UnknownTenantException();
            }

            _dataSource = dsManager.GetDataSource(_tenant.DataSourceId);

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

        public Guid Login(string username, string password)
        {
            return _dataSource.Login(username, password, _connection, _transaction);
        }

        public Group[] GetUserGroups()
        {
            AssertUserLoggedIn();

            return _dataSource.GetUserGroups(GetUserGuid(), _connection, _transaction).ToArray();
        }

        public GroupTreeNode GetUserGroupTree()
        {
            AssertUserLoggedIn();

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
            AssertUserLoggedIn();

            return _dataSource.GetQuantities(groupId, arch, range, _connection, _transaction);
        }

        public ITable GetTable(Guid groupId, byte arch)
        {
            AssertUserLoggedIn();

            return new DataSourceTableReader(this._dataSource, groupId, arch, _connection, _transaction);
        }

        public Interval GetInterval(Guid? groupId, byte arch)
        {
            AssertUserLoggedIn();

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

        public Group GetGroupById(string id)
        {
            AssertUserLoggedIn();

            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            var groups = _dataSource.GetUserGroups(GetUserGuid(), _connection, _transaction);

            return groups.Find(g => g.ID == guid);
        }

        private Guid GetUserGuid()
        {
            AssertUserLoggedIn();

            var userId = _currentUserService.UserId;
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new Exception("could not parse userId");
            }

            return userGuid;
        }

        private void ReadGroupTree(GroupTreeNode root, Guid rootId)
        {
            AssertUserLoggedIn();

            var groups = _dataSource.GetGroups(rootId, _connection, _transaction);
            root.Nodes = groups.Select(g =>
            {
                var node = new GroupTreeNode();
                node.Group = g;
                ReadGroupTree(node, g.ID);
                return node;
            }).ToArray();
        }

        private void AssertUserLoggedIn()
        {
            if (_currentUserService.UserId == null)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}