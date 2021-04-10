using Electricity.Application.Common.Interfaces;
using Moq;

namespace Electricity.Application.UnitTests.Common.Behaviours
{
    public class RequestLoggerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserService;


        public RequestLoggerTests()
        {
            _currentUserService = new Mock<ICurrentUserService>();
        }
    }
}
