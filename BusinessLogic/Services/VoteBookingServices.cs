using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.VoteBooking;
using BusinessLogic.Entities;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface IVoteBookingServices
    {
        Task<PaginatedResult<VoteBookingResponse>> GetPagination(VoteBookingRequest request);

        Task<Result<VoteBookingDto>> GetById(int id);

        Task<IResult> Add(VoteBookingDto request);

        Task<IResult> Update(VoteBookingDto request);

        Task<IResult> Delete(int id);
    }

    public class VoteBookingServices : IVoteBookingServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<VoteBookingServices> _logger;

        public VoteBookingServices(ApplicationDbContext dbContext, IMapper mapper, ILogger<VoteBookingServices> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<VoteBookingResponse>> GetPagination(VoteBookingRequest request)
        {
            var query = from v in _dbContext.VoteBooking
                        join b in _dbContext.Bookings.Where(b => !b.IsDeleted) on v.BookingId equals b.Id
                        join u in _dbContext.Users.Where(u => !u.IsDeleted) on b.UserId equals u.Id
                        where !v.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || v.Title!.ToLower().Contains(request.Keyword!.ToLower()))
                        select new VoteBookingResponse
                        {
                            Id = v.Id,
                            Title = v.Title,
                            Star = v.Star,
                            Comment = v.Comment,
                            Status = v.Status,
                            BookingCode = b != null ? b.BookingCode : null,
                            BookingId = v.BookingId,
                            FullName = u != null ? u.FullName : null,
                            CreatedBy = v.CreatedBy,
                            CreatedOn = v.CreatedOn
                        };

            var totalRecord = query.Count();

            var voteBookingList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<VoteBookingResponse>>(voteBookingList);

            return PaginatedResult<VoteBookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<VoteBookingDto>> GetById(int id)
        {
            var voteBooking = await _dbContext.VoteBooking.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            var result = _mapper.Map<VoteBookingDto>(voteBooking);

            return await Result<VoteBookingDto>.SuccessAsync(result ?? new VoteBookingDto());
        }

        public async Task<IResult> Add(VoteBookingDto request)
        {
            try
            {
                var result = _mapper.Map<VoteBooking>(request);

                await _dbContext.VoteBooking.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }

        public async Task<IResult> Update(VoteBookingDto request)
        {
            try
            {
                var voteBooking = await _dbContext.VoteBooking.Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

                if (voteBooking == null) return await Result.FailAsync(MessageConstants.NotFound);

                var updateVoteBooking = _mapper.Map(request, voteBooking);

                _dbContext.VoteBooking.Update(updateVoteBooking);
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
                var result = await _dbContext.VoteBooking.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.VoteBooking.Update(result);
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
