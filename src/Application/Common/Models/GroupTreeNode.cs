using DataSource;
using Electricity.Application.Common.Models.Dtos;

namespace Electricity.Application.Common.Models
{
    public class GroupTreeNode
    {
        public Group Group { get; set; }
        public GroupTreeNode[] Nodes { get; set; }
    }
}