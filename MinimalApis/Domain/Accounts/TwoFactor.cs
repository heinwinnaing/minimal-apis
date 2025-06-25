using MinimalApis.Domain.Base;

namespace MinimalApis.Domain.Accounts;

public class TwoFactor 
    : BaseEntity
{
    public Guid AccountId { get; set; }
    public string SecretKey { get; set; } = null!;
}
