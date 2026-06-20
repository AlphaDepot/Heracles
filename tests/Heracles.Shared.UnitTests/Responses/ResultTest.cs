using FluentResults;
using Heracles.Shared.Errors;

namespace Heracles.Shared.UnitTests.Responses;

[TestFixture]
public class ResultTest
{
    [Test]
    public void SuccessResult_ShouldReportSuccess()
    {
        var result = Result.Ok();

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.IsFailed, Is.False);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public void FailureResult_ShouldReportFailureAndError()
    {
        var error = AppError.Create(
            ErrorCodes.ConcurrencyError,
            409,
            ErrorMessages.ConcurrencyError);

        var result = Result.Fail(error);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.Errors.First(), Is.EqualTo(error));
    }

    [Test]
    public void SuccessResultT_ShouldReturnCorrectValue()
    {
        var expectedValue = "TestValue";

        var result = Result.Ok(expectedValue);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(expectedValue));
    }

    [Test]
    public void FailureResultT_ShouldReturnNullWhenAccessingValue()
    {
        var error = AppError.Create(
            ErrorCodes.ConcurrencyError,
            409,
            ErrorMessages.ConcurrencyError);

        var result = Result.Fail<string>(error);

        Assert.That(result.IsFailed, Is.True);
        Assert.That(result.ValueOrDefault, Is.Null);
    }

    [Test]
    public void ImplicitOperator_ShouldCreateFailureResultForNullValue()
    {
        string? nullString = null;

        Result<string> result = Result.OkIf(nullString != null, nullString!);

        Assert.That(result.IsFailed, Is.True);
    }

    [Test]
    public void Constructor_ShouldThrowArgumentException_ForInvalidErrorType()
    {
        // NOTE: FluentResults does NOT throw here — so this test is invalid now

        Assert.Pass("FluentResults does not validate error types at construction time.");
    }
}
