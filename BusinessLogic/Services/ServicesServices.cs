using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Service;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface IServicesServices
    {
        Task<PaginatedResult<ServiceResponse>> GetPagination(ServiceRequest request);

        Task<Result<ServiceDto>> GetById(int id);

        Task<IResult> Add(ServiceDto request);

        Task<IResult> Update(ServiceDto request);

        Task<IResult> Delete(int id);
    }

    public class ServicesServices : IServicesServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ServicesServices> _logger;

        public ServicesServices(ApplicationDbContext dbContext, IMapper mapper, ILogger<ServicesServices> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<ServiceResponse>> GetPagination(ServiceRequest request)
        {
            var query = from rt in _dbContext.Services
                        where !rt.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || rt.Name.ToLower().Contains(request.Keyword.ToLower()))
                        select new ServiceResponse
                        {
                            Id = rt.Id,
                            Name = rt.Name,
                            Image = rt.Image,
                            Description = rt.Description,
                            CreatedBy = rt.CreatedBy,
                            CreatedOn = rt.CreatedOn
                        };

            var totalRecord = query.Count();

            var roomTypesList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<ServiceResponse>>(roomTypesList);

            return PaginatedResult<ServiceResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<ServiceDto>> GetById(int id)
        {
            var roomTypes = await _dbContext.Services.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<ServiceDto>(roomTypes);

            return await Result<ServiceDto>.SuccessAsync(result ?? new ServiceDto());
        }

        public async Task<IResult> Add(ServiceDto request)
        {
            try
            {
                var checkName = await _dbContext.Services.AnyAsync(x => !x.IsDeleted && x.Name.ToLower().Trim().Equals(request.Name.ToLower().Trim()));
                if (checkName)
                {
                    return await Result.FailAsync(string.Format(MessageConstants.CheckExists, "Dịch vụ"));
                }

                var result = _mapper.Map<Entities.Services>(request);

                await _dbContext.Services.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(ServiceDto request)
        {
            try
            {
                var service = await _dbContext.Services.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (service == null) return await Result.FailAsync(MessageConstants.NotFound);

                // check duplicate name
                var checkName = await _dbContext.Services.AnyAsync(x => !x.IsDeleted && x.Id != request.Id && x.Name.ToLower().Trim().Equals(request.Name.ToLower().Trim()));
                if (checkName)
                {
                    return await Result.FailAsync(string.Format(MessageConstants.CheckExists, "Tên loại phòng"));
                }

                var updateService = _mapper.Map(request, service);

                _dbContext.Services.Update(updateService);
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
                var result = await _dbContext.Services.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.Services.Update(result);
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
