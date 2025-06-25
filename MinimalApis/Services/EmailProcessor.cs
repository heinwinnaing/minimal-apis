namespace MinimalApis.Services;

public interface IEmailService
{
    Task Send(string toMail, string subject, string contents, CancellationToken ctn = default);
}
public class EmailProcessor : IEmailService
{
    public async Task Send(string toMail, string subject, string contents, CancellationToken ctn = default)
    {
        await Task.Delay(1000, ctn);
        await Console.Out.WriteLineAsync($"Email has been successfully sent out to {toMail}.");
    }
}
