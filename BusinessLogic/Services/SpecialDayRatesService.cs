using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.SpecialDayRates;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface ISpecialDayRatesService
    {
        Task<PaginatedResult<SpecialDayRatesResponse>> GetPagination(SpecialDayRatesRequest request);

        Task<Result<SpecialDayRatesDto>> GetById(int id);

        Task<IResult> Add(SpecialDayRatesDto request);

        Task<IResult> Update(SpecialDayRatesDto request);

        Task<IResult> Delete(int id);
    }

    public class SpecialDayRatesService : ISpecialDayRatesService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<SpecialDayRatesService> _logger;

        public SpecialDayRatesService(ApplicationDbContext dbContext, IMapper mapper, ILogger<SpecialDayRatesService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<SpecialDayRatesResponse>> GetPagination(SpecialDayRatesRequest request)
        {
            var query = from s in _dbContext.PriceManager
                        where !s.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || s.Title!.ToLower().Contains(request.Keyword!.ToLower()))
                        select new SpecialDayRatesResponse
                        {
                            Id = s.Id,
                            Title = s.Title,
                            PercentDiscount = s.PercentDiscount,
                            SinceDay = s.SinceDay,
                            ToDay = s.ToDay,
                            Description = s.Description,
                            CreatedBy = s.CreatedBy,
                            CreatedOn = s.CreatedOn
                        };

            var totalRecord = query.Count();

            var newsList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<SpecialDayRatesResponse>>(newsList);

            return PaginatedResult<SpecialDayRatesResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<SpecialDayRatesDto>> GetById(int id)
        {
            var news = await _dbContext.PriceManager.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<SpecialDayRatesDto>(news);

            return await Result<SpecialDayRatesDto>.SuccessAsync(result ?? new SpecialDayRatesDto());
        }

        public async Task<IResult> Add(SpecialDayRatesDto request)
        {
            try
            {
                var result = _mapper.Map<PriceManager>(request);

                await _dbContext.PriceManager.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(SpecialDayRatesDto request)
        {
            try
            {
                var news = await _dbContext.PriceManager.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (news == null) return await Result.FailAsync(MessageConstants.NotFound);

                var updateSpecialDayRates = _mapper.Map(request, news);

                _dbContext.PriceManager.Update(updateSpecialDayRates);
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
                var result = await _dbContext.PriceManager.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.PriceManager.Update(result);
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
