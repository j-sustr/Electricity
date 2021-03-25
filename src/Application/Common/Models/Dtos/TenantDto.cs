using Electricity.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class TenantDto
    {
        public DataSourceType DataSourceType { get; set; }
        public DBConnectionParams DBConnectionParams { get; set; }
        public string CEAFileName { get; set; }
    }
}