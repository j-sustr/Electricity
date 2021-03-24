using System;
using KMB.DataSource;
using Electricity.Application.Common.Mappings;

namespace Electricity.Application.Common.Models.Dtos
{
    public class GroupDto : IMapFrom<Group>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}