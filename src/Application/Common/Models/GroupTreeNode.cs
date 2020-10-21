using DataSource;

namespace Electricity.Application.Common.Models
{
    public class GroupTreeNode
    {
        public Group Group { get; set; }
        public GroupTreeNode[] Nodes { get; set; }
    }
}