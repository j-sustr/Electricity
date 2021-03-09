using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PowerFactor.Queries.GetPowerFactorDistribution
{
    public class GetPowerFactorDistributionQueryValidator : AbstractValidator<GetPowerFactorDistributionQuery>
    {
        public GetPowerFactorDistributionQueryValidator()
        {
            RuleFor(x => x.GroupId)
               .NotEmpty()
               .WithMessage("GroupId is required.");

            RuleFor(x => x.Interval1)
               .NotNull()
               .WithMessage("Interval1 is required.");

            RuleFor(x => x.Phases)
               .NotNull().WithMessage("Phases are required.")
               .DependentRules(() =>
               {
                   RuleFor(x => x.Phases.ToArray()).NotEmpty()
                        .WithMessage("At least one phase must be selected.");
               });
                
        }
    }
}