using Electricity.Application.Common.Enums;
using System;

namespace Electricity.Application.Common.Models
{
    public class ApplicationUser
    {
        public Guid Id { get; }

        public Guid TenantId { get; }

        public Guid CurrentDataSourceId { get; set; }

    }
}