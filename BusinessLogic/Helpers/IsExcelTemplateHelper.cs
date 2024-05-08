using ClosedXML.Excel;

namespace BusinessLogic.Helpers
{
    public class IsExcelTemplateHelper
    {
        public static bool IsExcelTemplate(Stream excelFileStream, string[] columnHeaders, short columnCount)
        {
            using (var workbook = new XLWorkbook(excelFileStream))
            {
                var worksheet = workbook.Worksheet(1);

                if (worksheet.ColumnsUsed().Count() != columnCount ||
                    !worksheet.FirstRow().CellsUsed().Select(c => c.Value.ToString()).SequenceEqual(columnHeaders))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
