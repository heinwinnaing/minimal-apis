using MinimalApis.Commands.RegisterProfile;

namespace MinimalApis.Test;

public class CommandValidationTest
{
    [Fact]
    public void RegisterProfileCommandValidatorTest()
    {
        //arrange
        var request = new RegisterProfileCommand 
        {
            Name = "Test",
            Email = "test@mailinator.com",
            Phone = "123456789"
        };
        var validator = new RegisterProfileCommandValidator();

        //act
        var validationResult = validator.Validate(request);

        //asert
        Assert.NotNull(validationResult);
        Assert.True(validationResult.IsValid);
    }
}
