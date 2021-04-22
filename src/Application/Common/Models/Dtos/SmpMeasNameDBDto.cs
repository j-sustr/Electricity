using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.Common.Models.Dtos
{
    public class SmpMeasNameDBDto
    {
        public static bool ShowUserNames { get; set; }
        public int Id { get; set; }

        public string measName { get; set; }
        public string UserName { get; set; }
        public string MeasurementPathNoAlias { get; set; }
        public string MeasurementPath { get; set; }
        public string RecordAndObject { get; set; }
        public string MeasurementPathFull { get; set; }
        public string Alias { get; set; }
    }
}