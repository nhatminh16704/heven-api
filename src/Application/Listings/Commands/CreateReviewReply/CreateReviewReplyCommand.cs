using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Application.Common.Security;
using Heven.Api.Domain.Constants;
using Heven.Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Listings.Commands.CreateReviewReply;

[Authorize(Roles = Roles.Host)]
public record CreateReviewReplyCommand : IRequest<int>
{
    public int ListingId { get; init; }
    public int ReviewId { get; init; }
    public string? Comment { get; init; }
}

public class CreateReviewReplyCommandHandler : IRequestHandler<CreateReviewReplyCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateReviewReplyCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateReviewReplyCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra xem Review có tồn tại và thuộc về ListingId truyển vào không
        var review = await _context.Reviews
            .Include(r => r.Listing)
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId && r.ListingId == request.ListingId, cancellationToken);

        if (review == null)
        {
            throw new NotFoundException(nameof(Review), request.ReviewId.ToString());
        }

        // 2. Chống hack thao tác: Đảm bảo User hiện tại đang đăng nhập PHẢI là chủ phòng của cái Listing đó
        if (review.Listing.HostId != _user.Id)
        {
            throw new ForbiddenAccessException();
        }

        // 3. Tiến hành tạo Review Reply
        var reply = new ReviewReply
        {
            ReviewId = request.ReviewId,
            AuthorId = _user.Id!,
            Comment = request.Comment!
        };

        _context.ReviewReplies.Add(reply);

        await _context.SaveChangesAsync(cancellationToken);

        return reply.Id;
    }
}
