using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Booking;
using BusinessLogic.Entities;
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
                            || b.Status!.HasValue && request.Status != null && request.Status == b.Status)
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
                            Status = b.Status
                        };

            var totalRecord = query.Count();

            var roomsList = await query.OrderBy(request.OrderBy)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToListAsync();

            var result = _mapper.Map<List<BookingResponse>>(roomsList);

            return PaginatedResult<BookingResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<Result<BookingDto>> GetById(int id)
        {
            var booking = await (from b in _dbContext.Bookings 
                                 join u in _dbContext.Users.Where(x => !x.IsDeleted) on b.UserId equals u.Id
                                 join sb in _dbContext.SpecialDayBooking.Where(x => !x.IsDeleted) on b.Id equals sb.BookingId
                                 join pm in _dbContext.PriceManager.Where(x => !x.IsDeleted) on sb.SpecialDayId equals pm.Id
                                 where !b.IsDeleted select new BookingDto()
                                 {
                                     BookingCode = b.BookingCode,
                                     TransactionDate = b.TransactionDate,
                                     CheckInDate = b.CheckInDate,
                                     CheckOutDate = b.CheckOutDate,
                                     Status = b.Status,
                                     Adult = b.Adult,
                                     Kid = b.Kid,
                                     TotalAmount = b.TotalAmount,
                                     Payment = b.Payment,
                                     Message = b.Message,
                                     FullName = u != null ? u.FullName : null,
                                     BookingDetailDto = (from bd in _dbContext.BookingDetail
                                                         join r in _dbContext.Rooms.Where(x => !x.IsDeleted) on bd.RoomId equals r.Id
                                                         where !bd.IsDeleted && bd.BookingId == b.Id
                                                         select new BookingDetailDto()
                                                         {
                                                             RoomId = r.Id,
                                                             Name = r.Name,
                                                             Price = r.Price,
                                                         }).ToList(),
                                     CostBookingDto = (from cb in _dbContext.CostBooking
                                                       join c in _dbContext.CostOverrun.Where(x => !x.IsDeleted) on cb.CostId equals c.Id
                                                       where !cb.IsDeleted && cb.BookingId == b.Id
                                                       select new CostBookingDto()
                                                       {
                                                           CostId = c.Id,
                                                           Name = c.Name,
                                                           Price = cb.Price,
                                                       }).ToList(),
                                     SpecialDayBookingDto = new SpecialDayBookingDto()
                                     {
                                         Id = pm.Id,
                                         Title = pm.Title,
                                         PercentDiscount = pm.PercentDiscount,
                                         Description = pm.Description,
                                     }
                                 }).FirstOrDefaultAsync();

            var result = _mapper.Map<BookingDto>(booking);

            return await Result<BookingDto>.SuccessAsync(result ?? new BookingDto());
        }

        public async Task<IResult> Add(BookingDto request)
        {
            try
            {
                var result = _mapper.Map<Bookings>(request);

                var percentDiscount = await (from sb in _dbContext.SpecialDayBooking
                                               join pm in _dbContext.PriceManager.Where(x => !x.IsDeleted) on sb.SpecialDayId equals pm.Id
                                               where !sb.IsDeleted && (pm.Date.AddHours(7) >= request.CheckInDate && pm.Date.AddHours(7) <= request.CheckOutDate)
                                               select new PriceManager() { PercentDiscount = pm.PercentDiscount ,Id = pm.Id }).FirstOrDefaultAsync();

                foreach (var id in request.RoomId!)
                {
                    var roomsPrice = await (from r in _dbContext.Rooms.Where(x => !x.IsDeleted && x.Id == id) select r.Price).FirstOrDefaultAsync();
                    result.TotalAmount += roomsPrice;
                }

                result.TotalAmount = (decimal)(result.TotalAmount * (percentDiscount!.PercentDiscount!/100));

                result.DownPayment = (result.TotalAmount * (decimal)0.3);

                await _dbContext.Bookings.AddAsync(result);
                await _dbContext.SaveChangesAsync();
                if (request.RoomId != null)
                {
                    foreach (var id in request.RoomId)
                    {
                        var bookingDetail = new BookingDetail()
                        {
                            RoomId = id,
                            BookingId = result.Id,
                            Price = await (from r in _dbContext.Rooms.Where(x => !x.IsDeleted && x.Id == id) select r.Price).FirstOrDefaultAsync(),
                        };
                        await _dbContext.BookingDetail.AddAsync(bookingDetail);
                    }
                }
                if (percentDiscount != null)
                {
                    var SpecialDayBooking = new SpecialDayBooking()
                    {
                        BookingId = result.Id,
                        SpecialDayId = percentDiscount.Id,
                    };
                    await _dbContext.SpecialDayBooking.AddAsync(SpecialDayBooking);
                }

                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
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
