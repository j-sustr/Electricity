using Electricity.Application.Common.Extensions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class DataSourceGroupReader
    {
        private readonly KMB.DataSource.DataSource _dataSource;
        private IDisposable _connection;
        private IDisposable _transaction;

        public DataSourceGroupReader(
            KMB.DataSource.DataSource dataSource,
            IDisposable connection,
            IDisposable transaction
            )
        {
            _dataSource = dataSource;
            _connection = connection;
            _transaction = transaction;
        }

        public GroupInfo GetGroupInfo(Guid id)
        {
            var group = _dataSource.GetGroupInfos(id, new InfoFilter {
                RecurseSubgroups = false,
                IDisGroup = true,
            }, _connection, _transaction, out List<User> _);

            return group;
        }

        public GroupInfo GetUserGroupInfoTree(Guid userId)
        {
            return _dataSource.GetGroupInfos(userId, new InfoFilter() { 
                IDisGroup = false 
            }, _connection, _transaction, out List<User> _);
        }

        public GroupInfo[] GetUserRecordGroupInfos(Guid userId)
        {
            GroupInfo groupInfo = _dataSource.GetGroupInfos(userId, new InfoFilter() { 
                IDisGroup = false 
            }, _connection, _transaction, out List<User> _);

            return groupInfo.GetUserRecordGroupInfos();
        }

        public Group[] GetUserGroups(Guid userId)
        {
            var groups = _dataSource.GetUserGroups(userId, _connection, _transaction);
            return groups.ToArray();
        }
    }
}