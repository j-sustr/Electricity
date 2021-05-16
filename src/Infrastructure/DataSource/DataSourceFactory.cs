using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Exceptions;
using Electricity.Application.Common.Interfaces;
using Electricity.Application.Common.Models;
using Electricity.Infrastructure.DataSource.Abstractions;
using Electricity.Infrastructure.DataSource.Fake;
using KMB.DataSource.DB;
using KMB.DataSource.File;
using System.Data.SqlClient;
using System.IO;

namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceFactory : IDataSourceFactory
    {
        public KMB.DataSource.DataSource CreateDataSource(DataSourceCreationParams creationParams)
        {
            KMB.DataSource.DataSource ds = null;
            if (creationParams.DataSourceType == DataSourceType.DB)
            {
                var db = creationParams.DBConnectionParams;
                if (db.Server == "fake-server" && db.DBName == "fake-db")
                {
                    return CreateFakeDataSource();
                }

                ds = new DBDataSource(db.Server, db.DBName, db.Username, db.Password);
                try
                {
                    var conn = ds.NewConnection();
                    conn.Dispose();
                }
                catch (SqlException)
                {
                    throw new NotFoundException("Specified DB is not available");
                }
            }
            else if (creationParams.DataSourceType == DataSourceType.File)
            {
                var filePath = creationParams.CEAFileName;
                if (!File.Exists(filePath))
                {
                    throw new NotFoundException("Specified File does not exist");
                }

                ds = new FileDataSource(filePath);
            }

            return ds;
        }

        public KMB.DataSource.DataSource CreateFakeDataSource()
        {
            var ds = new FakeDataSource(0);
            ds.Users = FakeData.GetUsers();
            return ds;
        }
    }
}