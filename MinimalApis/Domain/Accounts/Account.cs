using MinimalApis.Domain.Base;

namespace MinimalApis.Domain.Accounts;

public class Account
    : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public AccountStatus? Status { get; set; } = AccountStatus.Active;
}

public enum AccountStatus
{
    Active, Suspended, Deleted
}