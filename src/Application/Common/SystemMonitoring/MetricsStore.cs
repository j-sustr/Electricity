using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Electricity.Application.Common.Interfaces;

namespace Electricity.Application.Common.SystemMonitoring
{
    public class MetricRecord
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public object Value { get; set; }
        public object Info { get; set; }
    }

    public class MetricQuery
    {
        public Guid UserId { get; set; }
        public Regex Name { get; set; }
        public DateTime MinDateTime { get; set; }
        public DateTime MaxDateTime { get; set; }

        public int MaxCount { get; set; }
    }

    public class MetricsStore
    {
        private readonly IUserProvider _userProvider;
        private List<MetricRecord> records = new List<MetricRecord>();

        public MetricsStore(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        private IEnumerable<MetricRecord> GetRecords(MetricQuery query)
        {
            return records.Where(r =>
            {
                return true;
            });
        }

        public void AddRecord(string name, object value, object info = null)
        {
            var user = _userProvider.GetUser();

            records.Add(new MetricRecord
            {
                UserId = user.Id,
                Name = name,
                Time = DateTime.Now,
                Value = value,
                Info = info
            });
        }
    }
}