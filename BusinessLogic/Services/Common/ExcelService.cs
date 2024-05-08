using BusinessLogic.Wrapper;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Services.Common
{
    public interface IExcelService
    {
        ImportResult<T> ImportExcel<T>(Stream stream, Func<IXLRow, T> convertFunction) where T : class, new();
    }


    public class ExcelService: IExcelService
    {
        private readonly ILogger<ExcelService> _logger;

        // Inject ILogger vào constructor
        public ExcelService(ILogger<ExcelService> logger)
        {
            _logger = logger;
        }

        public ImportResult<T> ImportExcel<T>(Stream stream, Func<IXLRow, T> convertFunction) where T : class, new()
        {
            var result = new ImportResult<T>();
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed();
                foreach (var row in rows.Skip(1))
                {
                    try
                    {
                        var entity = convertFunction(row);
                        if (entity != null)
                        {
                            result.SuccessEntities.Add(entity);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi chuyển đổi dữ liệu từ Excel tại dòng {RowNumber}", row.RowNumber());
                        result.Errors.Add(new ImportError
                        {
                            RowNumber = row.RowNumber(),
                            ErrorMessage = ex.Message
                        });
                    }
                }
            }
            return result;
        }
    }
}

