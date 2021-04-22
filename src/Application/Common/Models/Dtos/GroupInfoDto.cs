using Electricity.Application.Common.Mappings;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class GroupInfoDto : IMapFrom<GroupInfo>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public ArchiveInfoDto[] Archives { get; set; }
        public GroupInfoDto[] Subgroups { get; set; }
    }
}
