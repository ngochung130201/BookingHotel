using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Comment;
using BusinessLogic.Entities;
using BusinessLogic.Services.Common;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface IReplyCommentService
    {
        Task<PaginatedResult<ReplyCommentResponse>> GetPagination(ReplyCommentRequest request);

        Task<Result<ReplyCommentDto>> GetById(int id);

        Task<IResult> Add(ReplyCommentDto request);

        Task<IResult> Update(ReplyCommentDto request);

        Task<IResult> Delete(int id);

    }

    public class ReplyCommentService : IReplyCommentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;
        private readonly ICurrentUserService _currentUserService;


        public ReplyCommentService(ApplicationDbContext dbContext, IMapper mapper, ILogger<BookingService> logger, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<ReplyCommentResponse>> GetPagination(ReplyCommentRequest request)
        {
            var query = from rl in _dbContext.ReplyComment.Where(rl => !rl.IsDeleted && (rl.CommentId == null|| rl.CommentId == request.CommentId))
                        join u in _dbContext.Users on rl.UserId equals u.Id
                        select new ReplyCommentResponse
                        {
                            Id = rl.Id,
                            Content = rl.Content,
                            FullName = u.FullName,
                            Avatar = u.AvatarUrl,
                            CreatedOn = rl.CreatedOn
                        };

            var totalRecord = query.Count();

            var commentList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<ReplyCommentResponse>>(commentList);

            return PaginatedResult<ReplyCommentResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<ReplyCommentDto>> GetById(int id)
        {
            var comment = await _dbContext.ReplyComment.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<ReplyCommentDto>(comment);

            return await Result<ReplyCommentDto>.SuccessAsync(result ?? new ReplyCommentDto());
        }

        public async Task<IResult> Add(ReplyCommentDto request)
        {
            try
            {
                var userId = _currentUserService.UserId;
                request.UserId = userId;

                var result = _mapper.Map<ReplyComment>(request);

                await _dbContext.ReplyComment.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(ReplyCommentDto request)
        {
            try
            {
                var comment = await _dbContext.ReplyComment.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (comment == null) return await Result.FailAsync(MessageConstants.NotFound);

                var updateComment = _mapper.Map(request, comment);

                _dbContext.ReplyComment.Update(updateComment);
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
                var result = await _dbContext.ReplyComment.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.ReplyComment.Update(result);

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
