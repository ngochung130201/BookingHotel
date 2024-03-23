using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.CostOverrun;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface ICostBookingService
    {
        Task<PaginatedResult<CostOverrunResponse>> GetPagination(CostOverrunRequest request);

        Task<Result<CostOverrunDto>> GetById(int id);

        Task<IResult> Add(CostOverrunDto request);

        Task<IResult> Update(CostOverrunDto request);

        Task<IResult> Delete(int id);
    }

    public class CostBookingService : ICostBookingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CostBookingService> _logger;

        public CostBookingService(ApplicationDbContext dbContext, IMapper mapper, ILogger<CostBookingService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<CostOverrunResponse>> GetPagination(CostOverrunRequest request)
        {
            var query = from c in _dbContext.CostOverrun
                        where !c.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || c.Name!.ToLower().Contains(request.Keyword!.ToLower()))
                        select new CostOverrunResponse
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Price = c.Price,
                            Image = c.Image,
                            CreatedBy = c.CreatedBy,
                            CreatedOn = c.CreatedOn
                        };

            var totalRecord = query.Count();

            var newsList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<CostOverrunResponse>>(newsList);

            return PaginatedResult<CostOverrunResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<CostOverrunDto>> GetById(int id)
        {
            var costOverrun = await _dbContext.CostOverrun.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<CostOverrunDto>(costOverrun);

            return await Result<CostOverrunDto>.SuccessAsync(result ?? new CostOverrunDto());
        }

        public async Task<IResult> Add(CostOverrunDto request)
        {
            try
            {
                var result = _mapper.Map<CostOverrun>(request);

                await _dbContext.CostOverrun.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(CostOverrunDto request)
        {
            try
            {
                var costOverrun = await _dbContext.CostOverrun.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (costOverrun == null) return await Result.FailAsync(MessageConstants.NotFound);

                var updateCostOverrun = _mapper.Map(request, costOverrun);

                _dbContext.CostOverrun.Update(updateCostOverrun);
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
                var result = await _dbContext.CostOverrun.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.CostOverrun.Update(result);
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
