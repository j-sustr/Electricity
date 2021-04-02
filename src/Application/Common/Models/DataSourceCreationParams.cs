using System;
using Electricity.Application.Common.Enums;

namespace Electricity.Application.Common.Models
{
    public class DBConnectionParams : ICloneable
    {
        public string Server { get; set; }
        public string DBName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class DataSourceCreationParams : ICloneable
    {
        public DataSourceType DataSourceType { get; set; }

        public DBConnectionParams DBConnectionParams { get; set; }

        public string CEAFileName { get; set; }

        public object Clone()
        {
            return new DataSourceCreationParams
            {
                DataSourceType = this.DataSourceType,
                DBConnectionParams = (DBConnectionParams)this.DBConnectionParams.Clone(),
                CEAFileName = this.CEAFileName,
            };
        }
    }
}