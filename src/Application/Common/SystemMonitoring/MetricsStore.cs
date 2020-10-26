using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Electricity.Application.Common.SystemMonitoring
{
    public class MetricRecord
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public object Value { get; set; }
        public object Info { get; set; }
    }

    public class MetricQuery
    {
        public Regex Name { get; set; }
        public DateTime MinDateTime { get; set; }
        public DateTime MaxDateTime { get; set; }

        public int MaxCount { get; set; }
    }

    public class MetricsStore
    {
        private List<MetricRecord> records = new List<MetricRecord>();

        private IEnumerable<MetricRecord> GetRecords(MetricQuery query)
        {
            return records.Where(r =>
            {
                return true;
            });
        }

        public void AddRecord(string name, object value, object info = null)
        {
            records.Add(new MetricRecord
            {
                Name = name,
                Time = DateTime.Now,
                Value = value,
                Info = info
            });
        }
    }
}