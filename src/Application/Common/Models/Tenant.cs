using System;
using Electricity.Application.Common.Enums;

namespace Electricity.Application.Common.Models
{
    public class Tenant
    {
        private Guid Id { get; set; }

        public string Key { get; set; }
        public string Host { get; set; }
        public Guid DataSourceId { get; set; }

        public DataSourceType DataSourceType { get; set; }
        public DBConnectionParams DBConnectionParams { get; set; }
        public string CEAFileName { get; set; }

        public DataSourceConfig DataSourceConfig
        {
            get
            {
                return new DataSourceConfig
                {
                    DataSourceType = DataSourceType,
                    CEAFileName = CEAFileName,
                    DBConnectionParams = DBConnectionParams
                };
            }
        }
    }
}