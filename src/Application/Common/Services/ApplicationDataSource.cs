using System;
using System.Linq;
using KMB.DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Exceptions;
using System.Collections.Generic;

namespace Electricity.Application.Common.Services
{
    public class ApplicationDataSource : IDisposable, IGroupService, IQuantityService, ITableCollection, IAuthenticationService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDataSourceManager _dataSourceManager;

        private KMB.DataSource.DataSource _dataSource;
        private IDisposable _connection;
        private IDisposable _transaction;

        public ApplicationDataSource(
            ICurrentUserService currentUserService,
            IDataSourceManager dataSourceManager)
        {
            _currentUserService = currentUserService;
            _dataSourceManager = dataSourceManager;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }

        public Guid Login(string username, string password)
        {
            InitializeOperation();

            return _dataSource.Login(username, password, _connection, _transaction);
        }

        public KMB.DataSource.DataSource GetDataSource(out IDisposable connection, out IDisposable transaction)
        {
            InitializeOperation();

            connection = _connection;
            transaction = _transaction;

            return _dataSource;
        }

        public List<SmpMeasNameDB> GetRecords()
        {
            AssertUserLoggedIn();
            InitializeOperation();

            return _dataSource.GetRecords(_connection, _transaction);
        }

        public Group[] GetUserGroups()
        {
            AssertUserLoggedIn();
            InitializeOperation();

            var groups = _dataSource.GetUserGroups(GetUserGuid(), _connection, _transaction);
            return groups.ToArray();
        }

        public GroupTreeNode GetUserGroupTree()
        {
            AssertUserLoggedIn();
            InitializeOperation();

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
            InitializeOperation();

            return _dataSource.GetQuantities(groupId, arch, range, _connection, _transaction);
        }

        public ITable GetTable(Guid groupId, byte arch)
        {
            AssertUserLoggedIn();
            InitializeOperation();

            return new DataSourceTableReader(this._dataSource, groupId, arch, _connection, _transaction);
        }

        public Interval GetInterval(Guid? groupId, byte arch)
        {
            AssertUserLoggedIn();
            InitializeOperation();

            if (groupId == null)
            {
                var groups = GetUserGroups().ToList();
                if (groups[0].Name == "UserData")
                    groups.RemoveAt(0);

                if (groups.Count == 0)
                    return null;

                groupId = groups[0].ID;
            }

            var reader = new DataSourceTableReader(this._dataSource, (Guid)groupId, arch, _connection, _transaction);

            return reader.GetInterval();
        }

        public Group GetGroupById(string id)
        {
            AssertUserLoggedIn();
            InitializeOperation();

            if (!Guid.TryParse(id, out var guid))
            {
                return null;
            }

            var groups = _dataSource.GetUserGroups(GetUserGuid(), _connection, _transaction);

            return groups.Find(g => g.ID == guid);
        }

        private void InitializeOperation()
        {
            _dataSource = _dataSourceManager.GetDataSource();
            _connection = _dataSource.NewConnection();
            _transaction = _dataSource.BeginTransaction(_connection);
        }

        private Guid GetUserGuid()
        {
            var userId = _currentUserService.UserId;
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

        private void AssertUserLoggedIn()
        {
            if (_currentUserService.UserId == null)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}