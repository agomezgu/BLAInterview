using BLAInterview.Domain.Common;

namespace BLAInterview.Domain.Tasks;

public sealed class Task : BaseAuditableEntity
{
    private Task(string title, string ownerId)
    {
        Title = title;
        Created = DateTimeOffset.UtcNow;
        CreatedBy = ownerId;
    }

    public string Title { get; }

   
}
