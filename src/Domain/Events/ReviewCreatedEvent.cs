using Heven.Api.Domain.Common;
using Heven.Api.Domain.Entities;

namespace Heven.Api.Domain.Events;

public class ReviewCreatedEvent : BaseEvent
{
    public ReviewCreatedEvent(Review item)
    {
        Item = item;
    }

    public Review Item { get; }
}
