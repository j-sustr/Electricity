using Electricity.Application.Common.Enums;
using Electricity.Application.Common.Models.Quantities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Application.UnitTests.Common.Models
{
    class MainQuantityTests
    {
        private static MainQuantity[] _quantities = new[]{
            new MainQuantity(MainQuantityType.PAvg, Phase.Main),
            new MainQuantity(MainQuantityType.PAvg, Phase.L1),
            new MainQuantity(MainQuantityType.PAvg, Phase.L2),
            new MainQuantity(MainQuantityType.CosFi, Phase.Main),
            new MainQuantity(MainQuantityType.CosFi, Phase.L1),
            new MainQuantity(MainQuantityType.CosFi, Phase.L2),
        };

        [Test]
        public void FromQuantityConversionTest([ValueSource("_quantities")] MainQuantity emQuantity)
        {
            var quant = emQuantity.ToQuantity();
            bool success = MainQuantity.TryCreateFromQuantity(quant, out var result);
            result.Should().BeEquivalentTo(emQuantity);
        }
    }
}
