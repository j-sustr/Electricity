using System;
using Electricity.Application.Common.Enums;
using Finbuckle.MultiTenant;

namespace Electricity.Application.Common.Models
{
    public class Tenant : ITenantInfo
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        public Guid DataSourceId { get; set; }
        public DataSourceType DataSourceType { get; set; }
        public DBConnectionParams DBConnectionParams { get; set; }
        public string CEAFileName { get; set; }

        public DataSourceCreationParams DataSourceCreationParams
        {
            get
            {
                return new DataSourceCreationParams
                {
                    DataSourceType = DataSourceType,
                    CEAFileName = CEAFileName,
                    DBConnectionParams = DBConnectionParams
                };
            }
        }
    }
}