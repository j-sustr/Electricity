using Electricity.Application.Common.Interfaces;
using System;

namespace Electricity.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
