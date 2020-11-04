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
            if (parts.Length == 2)
            {
                parts[1] = parts[1].Trim().TrimEnd(']');
            }
            var propName = parts[0].Trim();
            var unit = parts.Length > 1 ? parts[1] : null;

            return new Quantity(propName, unit);
        }
    }
}