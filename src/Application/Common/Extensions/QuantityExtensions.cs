using Ardalis.GuardClauses;
using DataSource;

namespace Electricity.Application.Common.Extensions
{
    public static class QuantityExtensions
    {
        public static Quantity FromString(string value)
        {
            Guard.Against.NullOrWhiteSpace(value, nameof(value));

            string[] parts = value.Split('[');
            parts[1] = parts[1].TrimEnd(']');
            var propName = parts[0];
            var unit = parts[1];

            return new Quantity(propName, unit);
        }
    }
}