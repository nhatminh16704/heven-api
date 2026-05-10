using Heven.Api.Domain.Entities;

namespace Heven.Api.Domain.Events;

public class ListingUpdatedEvent : BaseEvent
{
    public ListingUpdatedEvent(Listing item)
    {
        Item = item;
    }

    public Listing Item { get; }
}