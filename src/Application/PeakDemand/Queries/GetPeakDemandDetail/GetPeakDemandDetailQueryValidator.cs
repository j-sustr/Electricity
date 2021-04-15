using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electricity.Application.PeakDemand.Queries.GetPeakDemandDetail
{
    public class GetPeakDemandDetailQueryValidator : AbstractValidator<GetPeakDemandDetailQuery>
    {
        public GetPeakDemandDetailQueryValidator()
        {
            RuleFor(x => x.GroupId)
               .NotEmpty()
               .WithMessage("GroupId is required.");

            RuleFor(x => x.Interval1)
               .NotNull()
               .WithMessage("Interval1 is required.");
        }
    }
}
