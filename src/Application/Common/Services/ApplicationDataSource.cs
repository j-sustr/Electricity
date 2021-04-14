using System;
using System.Linq;
using KMB.DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Application.Common.Exceptions;
using System.Collections.Generic;

namespace Electricity.Application.Common.Services
{
    public class ApplicationDataSource : IDisposable, IGroupRepository, IArchiveRepository, IAuthenticationService
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
            CheckUserLoggedIn();
            InitializeOperation();

            return _dataSource.GetRecords(_connection, _transaction);
        }

        public GroupInfo GetUserGroupInfoTree()
        {
            CheckUserLoggedIn();
            InitializeOperation();

            var gr = CreateGroupReader();
            return gr.GetUserGroupInfoTree(GetUserGuid());
        }

        public GroupInfo[] GetUserRecordGroupInfos()
        {
            CheckUserLoggedIn();
            InitializeOperation();

            var gr = CreateGroupReader();
            return gr.GetUserRecordGroupInfos(GetUserGuid());
        }

        public GroupInfo GetGroupInfo(string id)
        {
            CheckUserLoggedIn();
            InitializeOperation();

            var guid = ParseGuidFromString(id, nameof(id));

            var gr = CreateGroupReader();
            return gr.GetGroupInfo(guid);
        }

        public IArchive GetArchive(Guid groupId, byte arch)
        {
            CheckUserLoggedIn();
            InitializeOperation();

            return new ArchiveReader(this._dataSource, groupId, arch, _connection, _transaction);
        }

        private void InitializeOperation()
        {
            _dataSource = _dataSourceManager.GetDataSource();
            _connection = _dataSource.NewConnection();
            _transaction = _dataSource.BeginTransaction(_connection);
        }

        private DataSourceGroupReader CreateGroupReader()
        {
            return new DataSourceGroupReader(_dataSource, _connection, _transaction);
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

        private Guid ParseGuidFromString(string id, string name)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                throw new ArgumentException($"could not parse Guid", name);
            }
            return guid;
        }

        private void CheckUserLoggedIn()
        {
            if (_currentUserService.UserId == null)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}