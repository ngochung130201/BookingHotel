using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Booking;
using BusinessLogic.Entities;
using BusinessLogic.Services.Common;
using BusinessLogic.Wrapper;
using ClosedXML.Excel;
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

        byte[]? ExportExcel(BookingRequest request);
    }

    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;
        private readonly ICurrentUserService _currentUserService;

        public BookingService(ApplicationDbContext dbContext, IMapper mapper, ILogger<BookingService> logger, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<BookingResponse>> GetPagination(BookingRequest request)
        {
            var query = from b in _dbContext.Bookings.Where(b => !b.IsDeleted && (request.UserId == null || b.UserId == request.UserId))
                        join u in _dbContext.Users.Where(x => !x.IsDeleted) on b.UserId equals u.Id
                        join sb in _dbContext.SpecialDayBooking.Where(x => !x.IsDeleted) on b.Id equals sb.BookingId
                        join pm in _dbContext.PriceManager.Where(x => !x.IsDeleted) on sb.SpecialDayId equals pm.Id
                        where (string.IsNullOrEmpty(request.Keyword)
                            || u.FullName.ToLower().Contains(request.Keyword.ToLower())
                            || b.Status!.HasValue && request.Status != null && request.Status == b.Status)
                        select new BookingResponse
                        {
                            Id = b.Id,
                            FullName = u != null ? u.FullName : null,
                            BookingCode = b.BookingCode,
                            TransactionDate = b.TransactionDate,
                            CheckInDate = b.CheckInDate,
                            CheckOutDate = b.CheckOutDate,
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
                            CreatedOn = b.CreatedOn,
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
                                 where !b.IsDeleted 
                                 select new BookingDto()
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
                                                             Image = r.Thumbnail,
                                                             Name = r.Name,
                                                             Price = r.Price,
                                                         }).ToList(),
                                     CostBookingDto = (from cb in _dbContext.CostBooking
                                                       join c in _dbContext.CostOverrun.Where(x => !x.IsDeleted) on cb.CostId equals c.Id
                                                       where !cb.IsDeleted && cb.BookingId == b.Id
                                                       select new CostBookingDto()
                                                       {
                                                           CostId = c.Id,
                                                           Image = c.Image,
                                                           Name = c.Name,
                                                           Price = c.Price,
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
            var userId = _currentUserService.UserId;

            request.UserId = userId;
            request.TransactionDate = DateTime.Now;

            var result = _mapper.Map<Bookings>(request);

            var percentDiscount = await (from sb in _dbContext.SpecialDayBooking
                                         join pm in _dbContext.PriceManager.Where(x => !x.IsDeleted) on sb.SpecialDayId equals pm.Id
                                         where !sb.IsDeleted && (pm.SinceDay.AddHours(7) >= request.CheckInDate && pm.ToDay.AddHours(7) >= request.CheckOutDate)
                                         select new PriceManager() { PercentDiscount = pm.PercentDiscount, Id = pm.Id }).FirstOrDefaultAsync();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
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
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi tạo mới: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }  

        public async Task<IResult> Update(BookingDto request)
        {
            var booking = await _dbContext.Bookings.Where(b => !b.IsDeleted && b.Id == request.Id).FirstOrDefaultAsync();
            if (booking == null) { return await Result.FailAsync(MessageConstants.NotFound); };

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (request.RoomId != null)
                {
                    foreach (var id in request.RoomId)
                    {
                        var roomsPrice = await (from r in _dbContext.Rooms.Where(x => !x.IsDeleted && x.Id == id) select r.Price).FirstOrDefaultAsync();
                        booking.TotalAmount += roomsPrice;
                    }
                }

                await transaction.CommitAsync();
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Lỗi khi cập nhật: {Id}", request.Id);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        public async Task<IResult> Delete(int id)
        {
            try
            {
                var result = await _dbContext.Bookings.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

                if (result == null) return await Result.FailAsync(MessageConstants.NotFound);

                result.IsDeleted = true;
                _dbContext.Bookings.Update(result);
                await _dbContext.SaveChangesAsync();
                return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa: {Id}", id);
                return await Result.FailAsync(MessageConstants.DeleteFail);
            }
        }

        public async Task<IResult> ChangeStatusAsync(int id)
        {
            var booking = await _dbContext.Bookings.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (booking == null) return await Result.FailAsync(MessageConstants.NotFound);

            booking.Status = 1;   

            _dbContext.Bookings.Update(booking);
            await _dbContext.SaveChangesAsync();
            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }

        public byte[]? ExportExcel(BookingRequest request)
        {
            try
            {
                var query = from b in _dbContext.Bookings.Where(b => !b.IsDeleted && (request.UserId == null || b.UserId == request.UserId))
                            join u in _dbContext.Users.Where(x => !x.IsDeleted) on b.UserId equals u.Id
                            join sb in _dbContext.SpecialDayBooking.Where(x => !x.IsDeleted) on b.Id equals sb.BookingId
                            join pm in _dbContext.PriceManager.Where(x => !x.IsDeleted) on sb.SpecialDayId equals pm.Id
                            where (string.IsNullOrEmpty(request.Keyword)
                                || u.FullName.ToLower().Contains(request.Keyword.ToLower())
                                || b.Status!.HasValue && request.Status != null && request.Status == b.Status)
                            select new BookingResponse
                            {
                                Id = b.Id,
                                FullName = u != null ? u.FullName : null,
                                BookingCode = b.BookingCode,
                                TransactionDate = b.TransactionDate,
                                CheckInDate = b.CheckInDate,
                                CheckOutDate = b.CheckOutDate,
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
                                CreatedOn = b.CreatedOn,
                            };
                var bookings = query.OrderBy(request.OrderBy).ToList();
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Rooms");
                    worksheet.Cell(1, 1).Value = "Số";
                    worksheet.Cell(1, 2).Value = "Khách hàng";
                    worksheet.Cell(1, 3).Value = "Mã đặt phòng";
                    worksheet.Cell(1, 4).Value = "Ngày nhận phòng";
                    worksheet.Cell(1, 5).Value = "Ngày trả Phòng";
                    worksheet.Cell(1, 6).Value = "Số phòng";
                    worksheet.Cell(1, 7).Value = "Dịch vụ";
                    worksheet.Cell(1, 8).Value = "Tổng hóa đơn";
                    worksheet.Cell(1, 9).Value = "Ngày đặt phòng";

                    // Thiết lập kiểu dữ liệu của cột
                    worksheet.Column(1).Style.NumberFormat.Format = "0";
                    worksheet.Column(2).Style.NumberFormat.Format = "@";
                    worksheet.Column(3).Style.NumberFormat.Format = "@";
                    worksheet.Column(4).Style.NumberFormat.Format = "@";
                    worksheet.Column(5).Style.NumberFormat.Format = "@";
                    worksheet.Column(6).Style.NumberFormat.Format = "0";
                    worksheet.Column(7).Style.NumberFormat.Format = "0";
                    worksheet.Column(8).Style.NumberFormat.Format = "0.00";
                    worksheet.Column(9).Style.NumberFormat.Format = "@";

                    // Bôi màu tiêu đề màu xanh chỉ bôi màu các ô có tiêu đề
                    worksheet.Range("A1:I1").Style.Fill.BackgroundColor = XLColor.AliceBlue;

                    for (int i = 0; i < bookings.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = i + 1;
                        worksheet.Cell(i + 2, 2).Value = bookings[i].FullName;
                        worksheet.Cell(i + 2, 3).Value = bookings[i].BookingCode;
                        worksheet.Cell(i + 2, 4).Value = bookings[i].CheckInDate;
                        worksheet.Cell(i + 2, 5).Value = bookings[i].CheckOutDate;
                        worksheet.Cell(i + 2, 6).Value = bookings[i].BookedRoomNumber;
                        worksheet.Cell(i + 2, 7).Value = bookings[i].ServicesArising;
                        worksheet.Cell(i + 2, 8).Value = bookings[i].TotalAmount;
                        worksheet.Cell(i + 2, 9).Value = bookings[i].TransactionDate;
                    }

                    // số căn phải của từng cột, chữ căn trái của từng cột, ngày giờ căn giữa
                    worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Column(5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Column(6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column(7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column(8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column(9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xuất file excel");
                return null;
            }
        }
    }
}