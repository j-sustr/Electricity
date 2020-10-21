using Electricity.Application.Common.Enums;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models
{
    public class DBConnectionParams
    {
        public string Server { get; set; }
        public string DBName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class User
    {
        public Guid Id;

        public int CurrentDataSource { get; set; }
        public DBConnectionParams DBConnectionParams { get; set; }

        public DataSourceType DataSourceType { get; set; }
    }
}