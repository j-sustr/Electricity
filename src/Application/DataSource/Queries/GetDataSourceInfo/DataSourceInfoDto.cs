using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.DataSource.Queries.GetDataSourceInfo
{
    public class DataSourceInfoDto
    {
        public DateTime? MinDatetime { get; set; }
        public DateTime? MaxDatetime { get; set; }
    }
}