using AutoMapper;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Booking;
using BusinessLogic.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace BusinessLogic.Services
{
    public interface IBookingService
    {
        Task<PaginatedResult<BookingResponse>> GetPagination(BookingRequest request);

        Task<Result<BookingDto>> GetById(int id);

        Task<IResult> Add(BookingDto request);

        Task<IResult> Update(BookingDto request);

        Task<IResult> Delete(int id);

        Task<IResult> ChangeStatusAsync(int id);
    }

    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;


        public BookingService(ApplicationDbContext dbContext, IMapper mapper, ILogger<BookingService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<BookingResponse>> GetPagination(BookingRequest request)
        {
            var query = from b in _dbContext.Bookings
                        join u in _dbContext.Users.Where(x => !x.IsDeleted) on b.UserId equals u.Id
                        join sb in _dbContext.SpecialDayBooking.Where(x => !x.IsDeleted) on b.Id equals sb.BookingId
                        join pm in _dbContext.PriceManager.Where(x => !x.IsDeleted) on sb.SpecialDayId equals pm.Id
                        where !b.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                            || u.FullName.ToLower().Contains(request.Keyword.ToLower())
                            && (request.Status == null || request.Status == b.Status))
                        select new BookingResponse
                        {
                            Id = b.Id,
                            FullName = u != null ? u.FullName : null,
                            BookingCode = b.BookingCode,
                            TransactionDate = b.TransactionDate,
                            CheckInDate = b.CheckInDate,
                            BookedRoomNumber = (from r in _dbContext.Rooms
                                               join br in _dbContext.BookingDetail.Where(x => !x.IsDeleted) on r.Id equals br.RoomId
                                               join rb in _dbContext.Bookings.Where(x => !x.IsDeleted) on br.BookingId equals rb.Id
                                               where !r.IsDeleted
                                               select r).Count(),
                            ServicesArising = (from c in _dbContext.CostOverrun
                                               join cb in _dbContext.CostBooking.Where(x => !x.IsDeleted) on c.Id equals cb.CostId
                                               join bc in _dbContext.Bookings.Where(x => !x.IsDeleted) on cb.BookingId equals bc.Id
                                               where !c.IsDeleted
                                               select c).Count(),
                            TotalAmount = b.TotalAmount,
                            Status = b.Status,
                        };

            var totalRecord = query.Count();

            var roomsList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<BookingResponse>>(roomsList);

            return PaginatedResult<BookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public Task<Result<BookingDto>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> Add(BookingDto request)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> Update(BookingDto request)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ChangeStatusAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
