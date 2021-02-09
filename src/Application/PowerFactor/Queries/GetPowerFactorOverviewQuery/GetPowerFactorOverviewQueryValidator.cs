using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorOverviewQuery
{
    public class GetPowerFactorOverviewQueryValidator : AbstractValidator<GetPowerFactorOverviewQuery>
    {
        public GetPowerFactorOverviewQueryValidator()
        {
            RuleFor(x => x.Intervals)
               .NotNull()
               .NotEmpty()
               .Must(collection => collection != null && collection.All(item => item != null))
               .WithMessage("Intervals are required.");
        }
    }
}