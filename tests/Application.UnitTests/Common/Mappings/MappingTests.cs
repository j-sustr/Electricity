using AutoMapper;
using Electricity.Application.Common.Mappings;
using NUnit.Framework;
using System;

namespace Electricity.Application.UnitTests.Common.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }
    }
}