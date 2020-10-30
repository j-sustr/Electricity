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
        private readonly ICurrentUserService _currentUser;
        private List<MetricRecord> records = new List<MetricRecord>();

        public MetricsStore(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
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
            var userId = _currentUser.UserId;

            records.Add(new MetricRecord
            {
                UserId = userId ?? Guid.Empty,
                Name = name,
                Time = DateTime.Now,
                Value = value,
                Info = info
            });
        }
    }
}