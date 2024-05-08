using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Services
{
    public interface IRoomsImageService
    {
        Task<List<RoomImages>> GetPagination();

        Task<Result<RoomImages>> GetById(int id);

        Task<IResult> Add(RoomImages request);

        Task<IResult> Update(RoomImages request);

        Task<IResult> Delete(int id);
    }

    public class RoomsImageService : IRoomsImageService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RoomsImageService> _logger;

        public RoomsImageService(ApplicationDbContext dbContext, ILogger<RoomsImageService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<RoomImages>> GetPagination()
        {
            var query = await (from ri in _dbContext.RoomImages
                        join r in _dbContext.Rooms.Where(r => !r.IsDeleted) on ri.RoomId equals r.Id
                        where !ri.IsDeleted
                        select new RoomImages
                        {
                            Id = ri.Id,
                            UrlImage = ri.UrlImage,
                            CreatedBy = ri.CreatedBy,
                            CreatedOn = ri.CreatedOn,
                        }).ToListAsync();
            return query;
        }

        public Task<Result<RoomImages>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> Add(RoomImages request)
        {
            try
            {
                var roomImages = new RoomImages()
                {
                    RoomId = request.Id,
                    UrlImage = request.UrlImage,
                };
                await _dbContext.RoomImages.AddAsync(roomImages);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(RoomImages request)
        {
            try
            {
                var roomImages = await _dbContext.RoomImages.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (roomImages == null) return await Result.FailAsync(MessageConstants.NotFound);

                roomImages.UrlImage = request.UrlImage;

                _dbContext.RoomImages.Update(roomImages);
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
}
