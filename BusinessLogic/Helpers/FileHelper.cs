using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ImageMagick;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Helpers
{
    public class FileHelper
    {
        public static string ConvertFileBase64ToWebp(string folder, string base64, bool isConvertToWebp)
        {
            try
            {
                // check folder exist
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{folder}");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var data = base64.Split(',');
                if (data.Length > 1 && IsBase64Webp(data[0]))
                {
                    return SaveFileBase64ToWebp($"{folder}", data[1]);
                }

                var fileName = $"{Guid.NewGuid()}.{GetFileExtension(base64)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{folder}", fileName);
                //Ensure file is not in use by another process before proceeding
                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    var bytes = Convert.FromBase64String(data[1]);
                    stream.Write(bytes, 0, bytes.Length);
                }

                if (isConvertToWebp)
                {
                    var webpFile = $"{folder}/{Path.GetFileNameWithoutExtension(fileName)}.webp";
                    var webpPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot", webpFile);
                    using var image = new MagickImage(filePath);
                    image.Write(webpPath);
                    //Delete the file to avoid storing image files
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    return $"/{webpFile}";
                }
                return $"/{folder}/{fileName}";
            }
            catch (Exception e)
            {
                throw new InvalidOleVariantTypeException(e.Message);
            }
        }
        public static string SaveFileBase64ToWebp(string folder, string base64)
        {
            try
            {
                var fileName = $"{Guid.NewGuid()}.webp";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{folder}", fileName);
                //Ensure file is not in use by another process before proceeding
                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    var bytes = Convert.FromBase64String(base64);
                    stream.Write(bytes, 0, bytes.Length);
                }
                return $"{folder}/{fileName}";
            }
            catch (Exception e)
            {
                throw new InvalidOleVariantTypeException(e.Message);
            }
        }

        public static bool IsBase64Webp(string base64)
        {
            return base64.Contains("data:image/webp;base64,");
        }

        public static string UploadFileToServer(string folder, string hotelId, IFormFile file)
        {
            try
            {
                // check folder exist
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{folder}/{hotelId}");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{folder}/{hotelId}", fileName);
                //Ensure file is not in use by another process before proceeding
                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    file.CopyTo(stream);
                }
                return $"{folder}/{hotelId}/{fileName}";
            }
            catch (Exception e)
            {
                throw new InvalidOleVariantTypeException(e.Message);
            }
        }

        private static string GetFileExtension(string base64)
        {
            var data = base64.Split(',');
            var extension = data[0].Split('/')[1].Split(';')[0];
            return extension.Contains("svg") ? "svg" : extension;
        }

        public static bool ValidatePassword(string password)
        {
            if (password.Length < 9)
                return false;

            var uppercaseRegex = new Regex(@"[A-Z]");
            var lowercaseRegex = new Regex(@"[a-z]");
            var numberRegex = new Regex(@"[0-9]");
            var specialCharRegex = new Regex(@"[@$!%*?&]");

            if (!uppercaseRegex.IsMatch(password) || !lowercaseRegex.IsMatch(password) ||
                !numberRegex.IsMatch(password) || !specialCharRegex.IsMatch(password))
                return false;

            return true;
        }
    }
}
