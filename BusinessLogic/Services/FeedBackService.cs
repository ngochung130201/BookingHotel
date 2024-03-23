using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.FeedBacks;
using BusinessLogic.Dtos.News;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface IFeedBackService
    {
        Task<PaginatedResult<FeedBacksResponse>> GetPagination(FeedBacksRequest request);

        Task<Result<FeedBacksDto>> GetById(int id);

        Task<IResult> Add(FeedBacksDto request);

        Task<IResult> Update(FeedBacksDto request);

        Task<IResult> Delete(int id);
    }

    public class FeedBackService : IFeedBackService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<FeedBackService> _logger;

        public FeedBackService(ApplicationDbContext dbContext, IMapper mapper, ILogger<FeedBackService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<FeedBacksResponse>> GetPagination(FeedBacksRequest request)
        {
            var query = from f in _dbContext.FeedBacks
                        where !f.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || f.Title!.ToLower().Contains(request.Keyword!.ToLower()))
                        select new FeedBacksResponse
                        {
                            Id = f.Id,
                            Title = f.Title,
                            Content = f.Content,
                            Email = f.Email,
                            Reply = f.Reply,
                            ReplyBy = f.ReplyBy,
                            CreatedBy = f.CreatedBy,
                            CreatedOn = f.CreatedOn
                        };

            var totalRecord = query.Count();

            var feedBacksList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<FeedBacksResponse>>(feedBacksList);

            return PaginatedResult<FeedBacksResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<FeedBacksDto>> GetById(int id)
        {
            var feedBacks = await _dbContext.FeedBacks.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<FeedBacksDto>(feedBacks);

            return await Result<FeedBacksDto>.SuccessAsync(result ?? new FeedBacksDto());
        }

        public async Task<IResult> Add(FeedBacksDto request)
        {
            try
            {
                var result = _mapper.Map<FeedBacks>(request);

                await _dbContext.FeedBacks.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(FeedBacksDto request)
        {
            try
            {
                var feedBacks = await _dbContext.FeedBacks.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (feedBacks == null) return await Result.FailAsync(MessageConstants.NotFound);

                var updateFeedBacks = _mapper.Map(request, feedBacks);

                _dbContext.FeedBacks.Update(updateFeedBacks);
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
                var result = await _dbContext.FeedBacks.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.FeedBacks.Update(result);
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
