namespace BusinessLogic.Wrapper
{
    public class ImportResult<T> where T : class // Thêm ràng buộc class để T không phải là kiểu giá trị
    {
        public List<T> SuccessEntities { get; set; } = new List<T>();
        public List<ImportError> Errors { get; set; } = new List<ImportError>();
    }


    public class ImportError
    {
        public int RowNumber { get; set; }
        public required string ErrorMessage { get; set; }
    }
}