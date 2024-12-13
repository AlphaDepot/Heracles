
using Application.Common.Responses;
using Application.Infrastructure.Logging;
using Application.Infrastructure.Validation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.UnitTest.Infrastructure;

 [TestFixture]
    public class FluentValidationBehaviorTests
    {
        private Mock<IValidator<TestRequest>> _validatorMock;
        private Mock<IAppLogger<FluentValidationBehavior<TestRequest, Result>>> _loggerMock;
        private FluentValidationBehavior<TestRequest, Result> _behavior;

        [SetUp]
        public void SetUp()
        {
            _validatorMock = new Mock<IValidator<TestRequest>>();
            _loggerMock = new Mock<IAppLogger<FluentValidationBehavior<TestRequest, Result>>>();
            _behavior = new FluentValidationBehavior<TestRequest, Result>(new[] { _validatorMock.Object }, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnNext_WhenNoValidationErrors()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<Result>>();
            next.Setup(n => n()).ReturnsAsync(Result.Success());

            _validatorMock.Setup(v => v.Validate(It.IsAny<TestRequest>())).Returns(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess);
            next.Verify(n => n(), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnValidationErrors_WhenValidationFails()
        {
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<Result>>();

            var validationFailures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Property", "Error message")
            };

            _validatorMock.Setup(v => v.Validate(It.IsAny<TestRequest>())).Returns(new FluentValidation.Results.ValidationResult(validationFailures));

            // Act
            var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

            // Assert
            Assert.That(result.IsFailure);
            Assert.That(result.Errors, Has.Length.EqualTo(1));
            Assert.That(result.Errors.First().Description, Is.EqualTo("Error message"));
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

            next.Verify(n => n(), Times.Never);
        }

        public class TestRequest : IRequest<Result>
        {
        }

    }


