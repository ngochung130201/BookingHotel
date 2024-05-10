using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Booking;
using BusinessLogic.Dtos.CheckInOut;
using BusinessLogic.Dtos.Comment;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface ICheckInOutService
    {
        Task<PaginatedResult<CheckInOutResponse>> GetPagination(CheckInOutRequest request);

        Task<Result<CheckInOutResponse>> GetById(int id);

        Task<IResult> Update(CheckInOutResponse request);
    }

    public class CheckInOutService : ICheckInOutService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CheckInOutService> _logger;
        private readonly IMapper _mapper;

        public CheckInOutService(ApplicationDbContext dbContext, IMapper mapper, ILogger<CheckInOutService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<CheckInOutResponse>> GetPagination(CheckInOutRequest request)
        {
            var query = from r in _dbContext.Rooms.Where(r => !r.IsDeleted && (!request.StatusRoom.HasValue || request.StatusRoom == r.StatusRoom))
                        join rt in _dbContext.RoomTypes.Where(x => !x.IsDeleted) on r.RoomTypeId equals rt.Id
                        where string.IsNullOrEmpty(request.Keyword)
                            || r.Name.ToLower().Contains(request.Keyword.ToLower())
                            || rt.Name.ToLower().Contains(request.Keyword.ToLower())
                            || r.RoomCode!.ToLower().Contains(request.Keyword.ToLower())
                        select new CheckInOutResponse
                        {
                            Id = r.Id,
                            Name = r.Name,
                            RoomTypes = rt != null ? rt.Name : null,
                            Location = r.Location,
                            StatusRoom = r.StatusRoom,
                            CreatedOn = r.CreatedOn,
                        };

            var totalRecord = query.Count();

            var roomsList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<CheckInOutResponse>>(roomsList);

            return PaginatedResult<CheckInOutResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<CheckInOutResponse>> GetById(int id)
        {
            var comment = await _dbContext.Rooms.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<CheckInOutResponse>(comment);

            return await Result<CheckInOutResponse>.SuccessAsync(result ?? new CheckInOutResponse());
        }

        public async Task<IResult> Update(CheckInOutResponse request)
        {
            try
            {
                var room = await _dbContext.Rooms.Where(b => !b.IsDeleted && b.Id == request.Id).FirstOrDefaultAsync();
                if (room == null) { return await Result.FailAsync(MessageConstants.NotFound); };

                room.StatusRoom = request.StatusRoom;

                _dbContext.Rooms.Update(room);
                await _dbContext.SaveChangesAsync();

                return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi update: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.UpdateFail);
            }

        }
    }
}
