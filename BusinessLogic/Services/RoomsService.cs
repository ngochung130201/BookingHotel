using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Rooms;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace BusinessLogic.Services
{
    public interface IRoomsService
    {
        Task<PaginatedResult<RoomsResponse>> GetPagination(RoomsRequest request);

        Task<Result<RoomsDto>> GetById(int id);

        Task<IResult> Add(RoomsDto request);

        Task<IResult> Update(RoomsDto request);

        Task<IResult> Delete(int id);

        Task<IResult<List<RoomTypesResponse>>> GetRoomTypesName();

        Task<IResult> ChangeStatusAsync(int id);
    }

    public class RoomsService : IRoomsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomsService> _logger;


        public RoomsService(ApplicationDbContext dbContext, IMapper mapper, ILogger<RoomsService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<RoomsResponse>> GetPagination(RoomsRequest request)
        {
            var query = from r in _dbContext.Rooms
                        join rt in _dbContext.RoomTypes.Where(x => !x.IsDeleted) on r.RoomTypeId equals rt.Id
                        where !r.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || r.Name.ToLower().Contains(request.Keyword.ToLower())
                            || rt.Name.ToLower().Contains(request.Keyword.ToLower())
                            || r.RoomCode!.ToLower().Contains(request.Keyword.ToLower()))
                        select new RoomsResponse
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Thumbnail = r.Thumbnail,
                            RoomTypeName = rt != null ? rt.Name : null,
                            RoomCode = r.RoomCode,
                            Price = r.Price,
                            Status = r.Status,
                            CreatedBy = r.CreatedBy,
                            CreatedOn = r.CreatedOn,
                        };

            var totalRecord = query.Count();

            var roomsList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<RoomsResponse>>(roomsList);

            return PaginatedResult<RoomsResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<RoomsDto>> GetById(int id)
        {
            var room = await _dbContext.Rooms.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<RoomsDto>(room);

            return await Result<RoomsDto>.SuccessAsync(result ?? new RoomsDto());
        }

        public async Task<IResult> Add(RoomsDto request)
        {
            try
            {
                var checkName = await _dbContext.Rooms.AnyAsync(x => !x.IsDeleted && x.Name.ToLower().Trim().Equals(request.Name.ToLower().Trim()));
                if (checkName)
                {
                    return await Result.FailAsync(string.Format(MessageConstants.CheckExists, "Tên phòng"));
                }

                var result = _mapper.Map<Rooms>(request);

                await _dbContext.Rooms.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(RoomsDto request)
        {
            try
            {
                var rooms = await _dbContext.Rooms.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (rooms == null) return await Result.FailAsync(MessageConstants.NotFound);

                // check duplicate name
                var checkName = await _dbContext.Rooms.AnyAsync(x => !x.IsDeleted && x.Id != request.Id && x.Name.ToLower().Trim().Equals(request.Name.ToLower().Trim()));
                if (checkName)
                {
                    return await Result.FailAsync(string.Format(MessageConstants.CheckExists, "Tên phòng"));
                }

                var updateRooms = _mapper.Map(request, rooms);

                _dbContext.Rooms.Update(updateRooms);
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
                var result = await _dbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.Rooms.Update(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa: {Id}", id);
                return await Result.FailAsync(MessageConstants.DeleteFail);
            }
        }

        public async Task<IResult<List<RoomTypesResponse>>> GetRoomTypesName()
        {
            var result = await _dbContext.RoomTypes.Where(x => !x.IsDeleted).Select(n => new RoomTypesResponse()
            {
                Id = n.Id,
                Name = n.Name,
            }).ToListAsync();

            if (result.Any())
            {
                return Result<List<RoomTypesResponse>>.Success(result);
            }
            return Result<List<RoomTypesResponse>>.Fail(MessageConstants.NotFound);
        }

        public async Task<IResult> ChangeStatusAsync(int id)
        {
            var room = await _dbContext.Rooms.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (room == null) return await Result.FailAsync(MessageConstants.NotFound);

            room.Status = !room.Status;
            _dbContext.Rooms.Update(room);
            await _dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}
