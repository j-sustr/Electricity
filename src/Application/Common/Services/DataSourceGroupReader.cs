using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.Common.Services
{
    public class GroupReader
    {
        private readonly KMB.DataSource.DataSource _dataSource;
        private IDisposable _connection;
        private IDisposable _transaction;

        public GroupReader(
            KMB.DataSource.DataSource dataSource,
            IDisposable connection,
            IDisposable transaction
            )
        {
            _dataSource = dataSource;
            _connection = connection;
            _transaction = transaction;
        }

        public Group GetGroupById(Guid id, Guid userId)
        {
            var groups = _dataSource.GetUserGroups(userId, _connection, _transaction);

            return groups.Find(g => g.ID == id);
        }

        public GroupInfo GetGroupInfo(Guid id, InfoFilter infoFilter)
        {
            return _dataSource.GetGroupInfos(id, infoFilter, _connection, _transaction);
        }

        public Group[] GetUserGroups(Guid userId)
        {
            var groups = _dataSource.GetUserGroups(userId, _connection, _transaction);
            return groups.ToArray();
        }

        public GroupTreeNode GetUserGroupTree(Guid userId)
        {
            var userGroups = _dataSource.GetUserGroups(userId, _connection, _transaction);
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
    }
}