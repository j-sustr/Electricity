using System;
using Electricity.Application.Common.Enums;

namespace Electricity.Application.Common.Models
{
    [Serializable]
    public class DBConnectionParams
    {
        public string Server { get; set; }
        public string DBName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class DataSourceConfig
    {
        public DataSourceType DataSourceType { get; set; }

        public DBConnectionParams DBConnectionParams { get; set; }

        public string CEAFileName { get; set; }
    }
}