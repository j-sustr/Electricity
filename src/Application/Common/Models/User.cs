using Electricity.Application.Common.Enums;
using System;

namespace Electricity.Application.Common.Models
{
    public class User
    {
        public Guid Id;

        public Guid CurrentDataSourceId { get; set; }
        public DBConnectionParams DBConnectionParams { get; set; }
        public string CEAFileName { get; set; }

        public DataSourceType DataSourceType { get; set; }


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