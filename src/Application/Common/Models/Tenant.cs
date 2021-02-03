using System;
using Electricity.Application.Common.Enums;
using Finbuckle.MultiTenant;

namespace Electricity.Application.Common.Models
{
    public class Tenant : ITenantInfo
    {
        public string Id { get; set; }

        public string Identifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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