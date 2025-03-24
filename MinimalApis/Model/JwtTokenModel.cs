namespace MinimalApis.Model;

public class JwtTokenModel
{
    public string Token { get; set; } = null!;
    public DateTime ExpiryIn { get; set; }
}
