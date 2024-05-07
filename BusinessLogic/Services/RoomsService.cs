using AutoMapper;
using BusinessLogic.Constants.Messages;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Rooms;
using BusinessLogic.Dtos.RoomTypes;
using BusinessLogic.Entities;
using BusinessLogic.Helpers;
using BusinessLogic.Services.Common;
using BusinessLogic.Wrapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
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

        Task<IResult> ImportData(IFormFile file);

        Task<byte[]?> CreateTemplate();

        byte[]? ExportExcel(RoomsRequest request);
    }

    public class RoomsService : IRoomsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomsService> _logger;
        private readonly IExcelService _excelService;

        public RoomsService(ApplicationDbContext dbContext, IMapper mapper, ILogger<RoomsService> logger, IExcelService excelService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _excelService = excelService;
        }

        public async Task<PaginatedResult<RoomsResponse>> GetPagination(RoomsRequest request)
        {
            var query = from r in _dbContext.Rooms
                        join rt in _dbContext.RoomTypes.Where(x => !x.IsDeleted) on r.RoomTypeId equals rt.Id
                        where (!r.IsDeleted && r.Status == true && (string.IsNullOrEmpty(request.Keyword)
                            || r.Name.ToLower().Contains(request.Keyword.ToLower())
                            || rt.Name.ToLower().Contains(request.Keyword.ToLower())
                            || r.RoomCode!.ToLower().Contains(request.Keyword.ToLower()))
                            && (!request.RoomTypes.HasValue || request.RoomTypes == r.RoomTypeId))
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

        public async Task<IResult> ImportData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Result.Fail("Không có file hoặc file trống.");
            }

            var importResult = new ImportResult<Rooms>();

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    string[] columnHeaders = new string[] { "Tên thương hiệu (*)", "Mã thương hiệu cha", "Mô tả" };
                    if (!IsExcelTemplateHelper.IsExcelTemplate(stream, columnHeaders, 3))
                    {
                        return Result.Fail("File excel không đúng mẫu.");
                    }

                    short countRows = 0;

                    var excelResult = _excelService.ImportExcel(stream, row =>
                    {
                        countRows++;

                        // Bạn có thể thêm các điều kiện kiểm tra dữ liệu hợp lệ ở đây
                        if (string.IsNullOrWhiteSpace(row.Cell(1).GetValue<string>()))
                        {
                            importResult.Errors.Add(new ImportError
                            {
                                RowNumber = row.RowNumber(),
                                ErrorMessage = "Tên thương hiệu không được trống."
                            });
                        }
                        else if (row.Cell(1).GetValue<string>().Length > 255)
                        {
                            importResult.Errors.Add(new ImportError
                            {
                                RowNumber = row.RowNumber(),
                                ErrorMessage = "Tên thương hiệu không được nhập quá 255 ký tự."
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(row.Cell(2).GetValue<string>()))
                        {
                            if (!row.Cell(2).TryGetValue(out short valueIdF))
                            {
                                importResult.Errors.Add(new ImportError
                                {
                                    RowNumber = row.RowNumber(),
                                    ErrorMessage = "Mã thương hiệu cha không đúng định dạng."
                                });
                            }
                            else
                            {
                                var idF = _dbContext.Rooms.Any(x => !x.IsDeleted && x.Id == valueIdF);
                                if (!idF)
                                {
                                    importResult.Errors.Add(new ImportError
                                    {
                                        RowNumber = row.RowNumber(),
                                        ErrorMessage = "Mã thương hiệu cha không tồn tại."
                                    });
                                }
                            }
                        }

                        // kiểm tra name mà trùng lặp trong file excel và show nội dung lỗi có thể hiện các dòng trùng nhau
                        if (importResult.SuccessEntities.Any(x => x.Name.ToLower() == row.Cell(1).GetValue<string>().ToLower()))
                        {
                            importResult.Errors.Add(new ImportError
                            {
                                RowNumber = row.RowNumber(),
                                ErrorMessage = "Tên thương hiệu trùng lặp trong file excel."
                            });
                        }
                        // kiểm tra name mà trùng lặp trong database
                        if (_dbContext.Rooms.Any(x => !x.IsDeleted && x.Name.ToLower() == row.Cell(1).GetValue<string>().ToLower()))
                        {
                            importResult.Errors.Add(new ImportError
                            {
                                RowNumber = row.RowNumber(),
                                ErrorMessage = "Tên thương hiệu trùng lặp trong cơ sở dữ liệu."
                            });
                        }
                        return new Rooms
                        {
                            Name = row.Cell(1).GetValue<string>(),
                   
                            Description = row.Cell(3).GetValue<string>(),
                        };
                    });

                    if (countRows == 0)
                    {
                        return Result.Fail("Không có dữ liệu trong file excel.");
                    }

                    // Kiểm tra xem có lỗi nào không
                    if (importResult.Errors.Any())
                    {
                        // Trả về danh sách lỗi nếu có
                        return Result.Fail(importResult.Errors.Select(e => $"Dòng {e.RowNumber}: {e.ErrorMessage}").ToList());
                    }

                    // Nếu không có lỗi, lưu danh sách thành công vào cơ sở dữ liệu
                    await _dbContext.Rooms.AddRangeAsync(excelResult.SuccessEntities);
                    await _dbContext.SaveChangesAsync();

                    return Result.Success("Dữ liệu đã được import thành công.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi import dữ liệu");
                return Result.Fail("Lỗi khi import dữ liệu.");
            }
        }

        public async Task<byte[]?> CreateTemplate()
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Rooms");

                    // Thiết lập các tiêu đề cột
                    worksheet.Cell(1, 1).Value = "Tên thương hiệu (*)";
                    worksheet.Cell(1, 2).Value = "Mã thương hiệu cha";
                    worksheet.Cell(1, 3).Value = "Mô tả";

                    // Thiết lập kiểu dữ liệu của cột
                    worksheet.Column(1).Style.NumberFormat.Format = "@";
                    worksheet.Column(2).Style.NumberFormat.Format = "0";
                    worksheet.Column(3).Style.NumberFormat.Format = "@";

                    // Auto adjust column width
                    worksheet.Columns().AdjustToContents();

                    // Bôi màu tiêu đề màu xanh chỉ bôi màu các ô có tiêu đề
                    worksheet.Range("A1:C1").Style.Fill.BackgroundColor = XLColor.AliceBlue;

                    // Freeze header
                    worksheet.SheetView.FreezeRows(1);
                    // Tạo sheet thứ 2 để mô tả cách sử dụng
                    var worksheet2 = workbook.Worksheets.Add("Hướng dẫn");
                    worksheet2.Cell(1, 1).Value = "Hướng dẫn sử dụng";
                    worksheet2.Cell(2, 1).Value = "Cột 1: Tên thương hiệu (Bắt buộc)";
                    worksheet2.Cell(3, 1).Value = "Cột 2: Mã thương hiệu cha (Nếu không có thương hiệu cha thì để trống) ";
                    worksheet2.Cell(4, 1).Value = "Cột 3: Mô tả";

                    var brands = await _dbContext.Rooms.Where(x => !x.IsDeleted).Select(n => new RoomsResponse()
                    {
                        Name = n.Name,
                    }).ToListAsync();

                    worksheet2.Cell(1, 3).Value = "Mã loại thương hiệu cha (Cột 2)";
                    worksheet2.Cell(1, 4).Value = "Tên loại thương hiệu cha";

                    if (brands.Any())
                    {
                        short i = 2;
                        foreach (var item in brands)
                        {
                            worksheet2.Cell(i, 4).Value = item.Name;
                            i++;
                        }
                    }

                    worksheet2.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();  // Trả về mảng byte của file
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo file mẫu");
                return null;
            }
        }

        public byte[]? ExportExcel(RoomsRequest request)
        {
            try
            {
                var query = from b in _dbContext.Rooms
                            where !b.IsDeleted
                            select new RoomsResponse 
                            {
                                Id = b.Id,
                                Name = b.Name,
                                Description = b.Description,
                                CreatedBy = b.CreatedBy,
                                CreatedOn = b.CreatedOn
                            };
                var brands = query.OrderBy(request.OrderBy).ToList();
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Brands");
                    worksheet.Cell(1, 1).Value = "Số";
                    worksheet.Cell(1, 2).Value = "Mã thương hiệu";
                    worksheet.Cell(1, 3).Value = "Tên thương hiệu";
                    worksheet.Cell(1, 4).Value = "Mã thương hiệu cha";
                    worksheet.Cell(1, 5).Value = "Tên thương hiệu cha";
                    worksheet.Cell(1, 6).Value = "Mô tả";
                    worksheet.Cell(1, 7).Value = "Người tạo";
                    worksheet.Cell(1, 8).Value = "Ngày tạo";

                    // Thiết lập kiểu dữ liệu của cột
                    worksheet.Column(1).Style.NumberFormat.Format = "0";
                    worksheet.Column(2).Style.NumberFormat.Format = "0";
                    worksheet.Column(3).Style.NumberFormat.Format = "@";
                    worksheet.Column(4).Style.NumberFormat.Format = "0";
                    worksheet.Column(5).Style.NumberFormat.Format = "@";
                    worksheet.Column(6).Style.NumberFormat.Format = "@";
                    worksheet.Column(7).Style.NumberFormat.Format = "@";
                    worksheet.Column(8).Style.NumberFormat.Format = "dd/MM/yyyy HH:mm:ss";

                    // Bôi màu tiêu đề màu xanh chỉ bôi màu các ô có tiêu đề
                    worksheet.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.AliceBlue;

                    for (int i = 0; i < brands.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = i + 1;
                        worksheet.Cell(i + 2, 2).Value = brands[i].Id;
                        worksheet.Cell(i + 2, 3).Value = brands[i].Name;
                        worksheet.Cell(i + 2, 6).Value = brands[i].Description;
                        worksheet.Cell(i + 2, 7).Value = brands[i].CreatedBy;
                        worksheet.Cell(i + 2, 8).Value = brands[i].CreatedOn;
                    }

                    // số căn phải của từng cột, chữ căn trái của từng cột, ngày giờ căn giữa
                    worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Column(5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Column(6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Column(7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Column(8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

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
