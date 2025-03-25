using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinimalApis.Commands.RegisterProfile;
using MinimalApis.Commands.RequestOtp;
using MinimalApis.Database;
using MinimalApis.Domain;
using MinimalApis.Domain.Accounts;
using Moq;

namespace MinimalApis.Test;

public class CommandHandlerTest
{
    private readonly ServiceProvider serviceProvider;
    public CommandHandlerTest()
    {
        var services = new ServiceCollection();
        services.AddDbContext<EFContext>(options => {
            options.UseInMemoryDatabase("db_inMemory");
        })
            .AddScoped<IDbContext, EFContext>();
        serviceProvider = services.BuildServiceProvider();

        var dbContext = serviceProvider.GetService<IDbContext>();
        dbContext?
        .Accounts
        .Add(new Account { Name = "John Smith", Email = "john.smith@mailinator.com", Phone = "123456789", Status = AccountStatus.Active });
        dbContext?.SaveChanges();
    }

    [Fact]
    public async Task RegisterCommandHandlerTest()
    {
        //arrange
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var request = new RegisterProfileCommand
        {
            Name = "Test",
            Email = "test@mailinator.com",
            Phone = "123456789"
        };
        var validationResult = new ValidationResult(new List<ValidationFailure> { });
        var validator = new Mock<RegisterProfileCommandValidator>();
        validator
            .Setup(x => x.Validate(It.IsAny<ValidationContext<RegisterProfileCommand>>()))
            .Returns(validationResult);

        using var scope = serviceProvider.CreateScope();
        IDbContext dbContext = scope.ServiceProvider.GetRequiredService<EFContext>();
        var handler = new RegisterProfileCommandHandler(validator.Object, dbContext);

        //act
        var result = await handler.Handle(request, cts.Token);

        //assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Code);
    }

    [Fact]
    public async Task RequestOtpCommandHandlerTest()
    {
        //arrange
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var request = new RequestOtpCommand
        {
            Phone = "123456789"
        };
        var validationResult = new ValidationResult(new List<ValidationFailure> { });
        var validator = new Mock<RequestOtpCommandValidator>();
        validator
            .Setup(x => x.Validate(It.IsAny<ValidationContext<RequestOtpCommand>>()))
            .Returns(validationResult);

        using var scope = serviceProvider.CreateScope();
        IDbContext dbContext = scope.ServiceProvider.GetRequiredService<EFContext>();
        var handler = new RequestOtpCommandHandler(validator.Object, dbContext);

        //act
        var result = await handler.Handle(request, cts.Token);

        //assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Code);
        Assert.NotNull(result.Data);
        Assert.NotEqual($"{Guid.Empty}", result.Data!.Token);
    }
}