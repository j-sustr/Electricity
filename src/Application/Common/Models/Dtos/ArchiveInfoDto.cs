using Electricity.Application.Common.Mappings;
using KMB.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class ArchiveInfoDto : IMapFrom<ArchiveInfo>
    {
        public byte Arch { get; set; }
        public long Count {get; set;}
        public DateRangeDto Range {get; set;}
        public DateRangeDto[] Intervals {get; set;}
        public string Name { get; set; }
    }
}
