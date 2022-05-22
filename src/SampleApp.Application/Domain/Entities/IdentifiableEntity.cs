namespace SampleApp.Application.Domain.Entities;

public class IdentifiableEntity
{
    public Guid Id { get; protected set; }

    public IdentifiableEntity()
    {
        Id = Guid.NewGuid();
    }
}
