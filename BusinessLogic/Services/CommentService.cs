using AutoMapper;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Comment;
using System.Linq.Dynamic.Core;
using BusinessLogic.Services.Common;
using BusinessLogic.Wrapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Entities;

namespace BusinessLogic.Services
{
    public interface ICommentService
    {
        Task<PaginatedResult<CommentResponse>> GetPagination(CommentRequest request);

        Task<Result<CommentDto>> GetById(int id);

        Task<IResult> Add(CommentDto request);

        Task<IResult> Update(CommentDto request);

        Task<IResult> Delete(int id);
    }

    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;
        private readonly ICurrentUserService _currentUserService;


        public CommentService(ApplicationDbContext dbContext, IMapper mapper, ILogger<BookingService> logger, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<CommentResponse>> GetPagination(CommentRequest request)
        {
            var query = from c in _dbContext.Comment
                        join u in _dbContext.Users on c.UserId equals u.Id
                        where !c.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || u.FullName!.ToLower().Contains(request.Keyword!.ToLower())
                            && (!c.RoomId.HasValue && !c.NewId.HasValue) || (c.RoomId! == request.RoomId || c.NewId == request.NewId))
                        select new CommentResponse
                        {
                            Id = c.Id,
                            Content = c.Content,
                            FullName = u.FullName,
                            Avatar = u.AvatarUrl,
                            CreatedOn = c.CreatedOn
                        };

            var totalRecord = query.Count();

            var commentList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<CommentResponse>>(commentList);

            return PaginatedResult<CommentResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<CommentDto>> GetById(int id)
        {
            var comment = await _dbContext.Comment.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<CommentDto>(comment);

            return await Result<CommentDto>.SuccessAsync(result ?? new CommentDto());
        }

        public async Task<IResult> Add(CommentDto request)
        {
            try
            {
                var userId = _currentUserService.UserId;
                request.UserId = userId;

                var result = _mapper.Map<Comment>(request);
 
                await _dbContext.Comment.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(CommentDto request)
        {
            try
            {
                var comment = await _dbContext.Comment.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (comment == null) return await Result.FailAsync(MessageConstants.NotFound);

                var updateComment = _mapper.Map(request, comment);

                _dbContext.Comment.Update(updateComment);
                await _dbContext.SaveChangesAsync();

                return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi update: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.UpdateFail);
            }
        }

        public async Task<IResult> Delete(int id)
        {
            try
            {
                var result = await _dbContext.Comment.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
                var reply = await _dbContext.ReplyComment.Where(x => !x.IsDeleted && x.CommentId == id).ToListAsync();

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);
                result.IsDeleted = true;
                _dbContext.Comment.Update(result);

                foreach (var comment in reply)
                {
                    comment.IsDeleted = true;
                    _dbContext.ReplyComment.Update(comment);
                }

                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa: {Id}", id);
                return await Result.FailAsync(MessageConstants.DeleteFail);
            }
        }
    }
}
