using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface IRoomTypesService
    {
        Task<PaginatedResult<RoomTypesResponse>> GetPagination(RoomTypesRequest request);

        Task<Result<RoomTypesDto>> GetById(int id);

        Task<IResult> Add(RoomTypesDto request);

        Task<IResult> Update(RoomTypesDto request);

        Task<IResult> Delete(int id);
    }

    public class RoomTypesService : IRoomTypesService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomTypesService> _logger;


        public RoomTypesService(ApplicationDbContext dbContext, IMapper mapper, ILogger<RoomTypesService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<RoomTypesResponse>> GetPagination(RoomTypesRequest request)
        {
            var query = from rt in _dbContext.RoomTypes
                        where !rt.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || rt.Name.ToLower().Contains(request.Keyword.ToLower()))
                        select new RoomTypesResponse 
                        {
                            Id = rt.Id,
                            Name = rt.Name,
                            Star = rt.Star,
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

            var result = _mapper.Map<List<RoomTypesResponse>>(roomTypesList);

            return PaginatedResult<RoomTypesResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<RoomTypesDto>> GetById(int id)
        {
            var roomTypes = await _dbContext.RoomTypes.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<RoomTypesDto>(roomTypes);

            return await Result<RoomTypesDto>.SuccessAsync(result ?? new RoomTypesDto());
        }

        public async Task<IResult> Add(RoomTypesDto request)
        {
            try
            {
                var checkName = await _dbContext.RoomTypes.AnyAsync(x => !x.IsDeleted && x.Name.ToLower().Trim().Equals(request.Name.ToLower().Trim()));
                if (checkName)
                {
                    return await Result.FailAsync(string.Format(MessageConstants.CheckExists, "Tên loại phòng"));
                }

                var result = _mapper.Map<RoomTypes>(request);

                await _dbContext.RoomTypes.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(RoomTypesDto request)
        {
            try
            {
                var roomTypes = await _dbContext.RoomTypes.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (roomTypes == null) return await Result.FailAsync(MessageConstants.NotFound);

                // check duplicate name
                var checkName = await _dbContext.RoomTypes.AnyAsync(x => !x.IsDeleted && x.Id != request.Id && x.Name.ToLower().Trim().Equals(request.Name.ToLower().Trim()));
                if (checkName)
                {
                    return await Result.FailAsync(string.Format(MessageConstants.CheckExists, "Tên loại phòng"));
                }

                var updateRoomTypes = _mapper.Map(request, roomTypes);

                _dbContext.RoomTypes.Update(updateRoomTypes);
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
                var result = await _dbContext.RoomTypes.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.RoomTypes.Update(result);
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
