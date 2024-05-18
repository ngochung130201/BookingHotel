using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Entities;
using BusinessLogic.Request;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface IRoomsImageService
    {
        Task<PaginatedResult<RoomImagesResponse>> GetPagination(RoomImageRequest request);

        Task<IResult> Add(RoomImageDto request);

        Task<IResult> Delete(int id);
    }

    public class RoomsImageService : IRoomsImageService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RoomsImageService> _logger;
        private readonly IMapper _mapper;

        public RoomsImageService(ApplicationDbContext dbContext, ILogger<RoomsImageService> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<RoomImagesResponse>> GetPagination(RoomImageRequest request)
        {
            var query = from ri in _dbContext.RoomImages
                        join r in _dbContext.Rooms.Where(r => !r.IsDeleted) on ri.RoomId equals r.Id
                        where !ri.IsDeleted && r.Id == request.RoomId
                        select new RoomImagesResponse
                        {
                            Id = ri.Id,
                            UrlImage = ri.UrlImage,
                            CreatedOn = ri.CreatedOn,
                        };

            var totalRecord = query.Count();

            var roomsImageList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<RoomImagesResponse>>(roomsImageList);

            return PaginatedResult<RoomImagesResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<IResult> Add(RoomImageDto request)
        {
            try
            {
                var result = _mapper.Map<RoomImages>(request);

                await _dbContext.RoomImages.AddAsync(result);
                await _dbContext.SaveChangesAsync();

                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Delete(int id)
        {
            try
            {
                var result = await _dbContext.RoomImages.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.RoomImages.Update(result);
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

    public class RoomImageRequest : RequestParameter
    {
        public int? RoomId { get; set; }
    }

    public class RoomImageDto 
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? UrlImage { get; set; }
    }
    public class RoomImagesResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? UrlImage { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
