namespace MinimalApis.Domain.Base;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public BaseEntity() => Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
}
