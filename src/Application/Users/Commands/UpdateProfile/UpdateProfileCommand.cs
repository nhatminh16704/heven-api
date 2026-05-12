using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Users.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? AvatarUrl { get; init; }
    public string? Bio { get; init; }
    public int? CityId { get; init; }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public UpdateProfileCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException();
        }

        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile == null)
        {
            profile = new UserProfile
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                AvatarUrl = request.AvatarUrl,
                Bio = request.Bio,
                CityId = request.CityId
            };
            _context.UserProfiles.Add(profile);
        }
        else
        {
            profile.FirstName = request.FirstName;
            profile.LastName = request.LastName;
            profile.DateOfBirth = request.DateOfBirth;
            profile.AvatarUrl = request.AvatarUrl;
            profile.Bio = request.Bio;
            profile.CityId = request.CityId;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
