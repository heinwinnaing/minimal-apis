using Microsoft.AspNetCore.Mvc.Testing;
using MinimalApis.Commands.RegisterProfile;
using MinimalApis.Commands.RequestOtp;
using MinimalApis.Endpoints.Authentication;
using MinimalApis.Model;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;

namespace MinimalApis.Test;
public class EndpointTest
{
    #region #member-data
    public static TheoryData<HttpStatusCode, RegisterProfileCommand> RegisterMemberData => new()
    {
        { HttpStatusCode.OK, new RegisterProfileCommand { Name = "Test 111", Email = "test@mailinator.com", Phone = "123456789" } },
        { HttpStatusCode.BadRequest, new RegisterProfileCommand { Name = "Test 111", Email = "test", Phone = "123456789" } }
    };
    public static TheoryData<HttpStatusCode, RequestOtpCommand> RequestOtpMemberData = new()
    {
        { HttpStatusCode.OK, new RequestOtpCommand { Phone = "123456789" } },
        { HttpStatusCode.BadRequest, new RequestOtpCommand { Phone = "" } },
    };
    public static TheoryData<HttpStatusCode, object> VerifyOtpMemberData = new()
    {
        { HttpStatusCode.BadRequest, new { token = $"{Guid.Empty}", otpCode = "123123" } }
    };
    #endregion

    [Theory]
    [MemberData(nameof(RegisterMemberData))]
    public async Task RegisterProfileEndpointTest(HttpStatusCode expected, RegisterProfileCommand payload)
    {
        //arrange
        await using var app = new WebApplicationFactory<RegisterProfile>();
        using var httpClient = app.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Idempotency-Key", $"{Guid.NewGuid()}");

        //action
        var result = await httpClient.PostAsJsonAsync("/v1/auth/register", payload);
        //result.EnsureSuccessStatusCode();

        //assert
        Assert.NotNull(result);
        Assert.Equal(expected, result.StatusCode);
    }

    [Theory]
    [MemberData(nameof(RequestOtpMemberData))]
    public async Task RequestOtpEndpointTest(HttpStatusCode expected, RequestOtpCommand payload)
    {
        //arrange
        await using var app = new WebApplicationFactory<RequestOtp>();
        using var httpClient = app.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Idempotency-Key", $"{Guid.NewGuid()}");

        //act
        var result = await httpClient.PostAsJsonAsync("/v1/auth/request-otp", payload);

        //assert
        Assert.NotNull(result);
        Assert.Equal(expected, result.StatusCode);
    }

    [Theory]
    [MemberData(nameof(VerifyOtpMemberData))]
    public async Task VerifyOtpEndpointTest(HttpStatusCode expected, object payload)
    {
        //arrange
        var response = ResultModel.Success();
        await using var app = new WebApplicationFactory<VerifyOtp>();
        using HttpClient httpClient = app.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Idempotency-Key", $"{Guid.NewGuid()}");
        
        HttpContent httpContent = new StringContent(
            content: System.Text.Json.JsonSerializer.Serialize(payload), 
            encoding: Encoding.UTF8, 
            mediaType: MediaTypeNames.Application.Json);

        //act
        var result = await httpClient.PostAsync("/v1/auth/verify-otp", httpContent);

        //assert
        Assert.NotNull(result);
        Assert.Equal(expected, result.StatusCode);
    }
}
