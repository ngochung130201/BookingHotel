using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.News;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface INewsService
    {
        Task<PaginatedResult<NewsResponse>> GetPagination(NewRequest request);

        Task<Result<NewsDto>> GetById(int id);

        Task<IResult> Add(NewsDto request);

        Task<IResult> Update(NewsDto request);

        Task<IResult> Delete(int id);

        Task<IResult> ChangeStatusAsync(int id);

        Task<IResult> ChangeHotAsync(int id);
    }

    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsService> _logger;

        public NewsService(ApplicationDbContext dbContext, IMapper mapper, ILogger<NewsService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<NewsResponse>> GetPagination(NewRequest request)
        {
            var query = from n in _dbContext.News
                        where !n.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || n.Title!.ToLower().Contains(request.Keyword!.ToLower()))
                        select new NewsResponse
                        {
                            Id = n.Id,
                            Title = n.Title,
                            Thumbnail = n.Thumbnail,
                            Content = n.Content,
                            Status = n.Status,
                            Hot = n.Hot,
                            CreatedBy = n.CreatedBy,
                            CreatedOn = n.CreatedOn
                        };

            var totalRecord = query.Count();

            var newsList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<NewsResponse>>(newsList);

            return PaginatedResult<NewsResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<NewsDto>> GetById(int id)
        {
            var news = await _dbContext.News.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<NewsDto>(news);

            return await Result<NewsDto>.SuccessAsync(result ?? new NewsDto());
        }

        public async Task<IResult> Add(NewsDto request)
        {
            try
            {
                var result = _mapper.Map<Entities.News>(request);

                await _dbContext.News.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(NewsDto request)
        {
            try
            {
                var news = await _dbContext.News.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (news == null) return await Result.FailAsync(MessageConstants.NotFound);

                var updateNews = _mapper.Map(request, news);

                _dbContext.News.Update(updateNews);
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
                var result = await _dbContext.News.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.News.Update(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa: {Id}", id);
                return await Result.FailAsync(MessageConstants.DeleteFail);
            }
        }

        public async Task<IResult> ChangeStatusAsync(int id)
        {
            var news = await _dbContext.News.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (news == null) return await Result.FailAsync(MessageConstants.NotFound);

            news.Status = !news.Status;
            _dbContext.News.Update(news);
            await _dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public async Task<IResult> ChangeHotAsync(int id)
        {
            var news = await _dbContext.News.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (news == null) return await Result.FailAsync(MessageConstants.NotFound);

            news.Hot = !news.Hot;
            _dbContext.News.Update(news);
            await _dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}
