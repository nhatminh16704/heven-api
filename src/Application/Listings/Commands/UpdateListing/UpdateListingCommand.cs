using AutoMapper;
using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Heven.Api.Domain.Events;

namespace Heven.Api.Application.Listings.Commands.UpdateListing;

public record UpdateListingCommand : IRequest
{
    public int Id { get; init; }
    public int CategoryId { get; init; }

    public required string Title { get; init; }
    public string? Description { get; init; }
    public decimal PricePerNight { get; init; }
    public decimal CleaningFee { get; init; }
    public int MaxGuests { get; init; }
    public int Bedrooms { get; init; }
    public int Beds { get; init; }
    public int Bathrooms { get; init; }
    public string? PropertyType { get; init; }
    public bool InstantBook { get; init; }

    public required UpdateListingLocationDto Location { get; init; }
    public IReadOnlyCollection<int> AmenityIds { get; init; } = Array.Empty<int>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateListingCommand, Listing>()
                .ForMember(d => d.Id, opt => opt.Ignore())       
                .ForMember(d => d.Location, opt => opt.Ignore()) 
                .ForMember(d => d.Amenities, opt => opt.Ignore());
        }
    }
}

public class UpdateListingLocationDto
{
    public required string Address { get; init; }
    public int CityId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateListingLocationDto, Location>();
        }
    }
}

public class UpdateListingCommandHandler : IRequestHandler<UpdateListingCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _user;

    public UpdateListingCommandHandler(IApplicationDbContext context, IMapper mapper, IUser user)
    {
        _context = context;
        _mapper = mapper;
        _user = user;
    }

    public async Task Handle(UpdateListingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Listings
            .Include(l => l.Location)
            .Include(l => l.Amenities)
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Listing), request.Id.ToString());
        }

        // Phải là chủ của Listing mới được phép Update
        if (entity.HostId != _user.Id)
        {
            throw new ForbiddenAccessException();
        }

        // Cập nhật thông tin cơ bản bằng AutoMapper
        _mapper.Map(request, entity);

        // Cập nhật Location
        _mapper.Map(request.Location, entity.Location);

  
        // 1. Xóa những Amenity cũ không còn nằm trong request
        var amenitiesToRemove = entity.Amenities
            .Where(a => !request.AmenityIds.Contains(a.AmenityId))
            .ToList();
        foreach (var item in amenitiesToRemove)
        {
            entity.Amenities.Remove(item);
        }

        // 2. Thêm những Amenity mới chưa có trong list hiện tại
        var existingAmenityIds = entity.Amenities.Select(a => a.AmenityId).ToHashSet();
        var amenitiesToAdd = request.AmenityIds
            .Where(id => !existingAmenityIds.Contains(id))
            .ToList();
        foreach (var newId in amenitiesToAdd)
        {
            entity.Amenities.Add(new ListingAmenity { AmenityId = newId });
        }

        entity.AddDomainEvent(new ListingUpdatedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
