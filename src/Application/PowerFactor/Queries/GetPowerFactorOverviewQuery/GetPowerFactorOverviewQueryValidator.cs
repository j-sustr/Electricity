using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery
{
    public class GetPowerFactorOverviewQueryValidator : AbstractValidator<GetPowerFactorOverviewQuery>
    {
        public GetPowerFactorOverviewQueryValidator()
        {
            RuleFor(x => x.Intervals)
               .NotNull()
               .NotEmpty().WithMessage("Intervals are required.");
        }
    }
}