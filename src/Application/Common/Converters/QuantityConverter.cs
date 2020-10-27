using System;
using System.ComponentModel;
using System.Globalization;
using DataSource;

namespace Electricity.Application.Common.Converters
{
    public class QuantityConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var casted = value as string;
            if (casted == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            string[] parts = casted.Split("[");
            var propName = parts[0];
            var unit = parts[1];

            return new Quantity(propName, unit);
        }
    }
}