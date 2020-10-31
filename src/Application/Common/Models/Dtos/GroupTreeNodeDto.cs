using Electricity.Application.Common.Mappings;

namespace Electricity.Application.Common.Models.Dtos
{
    public class GroupTreeNodeDto : IMapFrom<GroupTreeNode>
    {
        public GroupDto Group { get; set; }
        public GroupTreeNodeDto[] Nodes { get; set; }
    }
}