using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution
{
    internal class GetPowerFactorDistributionQueryValidator : AbstractValidator<GetPowerFactorDistributionQuery>
    {
        public GetPowerFactorDistributionQueryValidator()
        {
            RuleFor(x => x.Interval1)
               .NotNull()
               .WithMessage("Interval1 is required.");
        }
    }
}