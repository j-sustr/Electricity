using System;
using System.Linq;
using DataSource;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;

namespace Electricity.Application.Common.Services
{
    public class ApplicationDataSource : IGroupService, IQuantityService, IRowCollectionReader
    {
        private readonly User _user;

        private readonly DataSource.DataSource _dataSource;

        public ApplicationDataSource(IDataSourceManager dsManager, IUserProvider userProvider)
        {
            _user = userProvider.GetUser();
            _dataSource = dsManager.GetDataSource(_user.CurrentDataSourceId);

            if (_dataSource == null)
            {
                dsManager.CreateDataSource(_user.DataSourceConfig);
            }
        }

        public Group[] GetUserGroups(Guid userId)
        {
            return _dataSource.GetUserGroups(userId).ToArray();
        }

        public GroupTreeNode GetUserGroupTree(Guid userId)
        {
            var userGroups = _dataSource.GetUserGroups(userId);
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
            return _dataSource.GetQuantities(groupId, arch, range);
        }

        public RowCollection GetRows(Guid groupId, byte arch, DateRange range, Quantity[] quantities, uint aggregation, EEnergyAggType energyAggType = EEnergyAggType.Cumulative)
        {
            return _dataSource.GetRows(groupId, arch, range, quantities, aggregation, energyAggType);
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
    }
}