using Electricity.Application.Common.Models.Quantities;
using NUnit.Framework;
using Electricity.Application.Common.Enums;
using FluentAssertions;

namespace Electricity.Application.UnitTests.Common.Models
{
    class ElectricityMeterQuantityTests
    {
        private static ElectricityMeterQuantity[] _quantities = new[]{
            new ElectricityMeterQuantity(ElectricityMeterQuantityType.ActiveEnergy, Phase.Main),
            new ElectricityMeterQuantity(ElectricityMeterQuantityType.ActiveEnergy, Phase.L1),
            new ElectricityMeterQuantity(ElectricityMeterQuantityType.ReactiveEnergyC, Phase.L2),
            new ElectricityMeterQuantity(ElectricityMeterQuantityType.ReactiveEnergyL, Phase.L2),
        };

        [Test]
        public void FromQuantityConversionTest([ValueSource("_quantities")] ElectricityMeterQuantity emQuantity)
        {
            var quant = emQuantity.ToQuantity();
            bool success = ElectricityMeterQuantity.TryCreateFromQuantity(quant, out var result);
            result.Should().BeEquivalentTo(emQuantity);
        }
    }
}
