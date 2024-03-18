using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Admin.Controllers
{
    public class UploadController(IWebHostEnvironment hostingEnvironment) : AdminBaseController
    {
        /// <summary>
        /// upload image for ckeditor tool
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="ckEditorFuncNum"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task UploadImageForCkEditor(IList<IFormFile> upload, string ckEditorFuncNum)
        {
            DateTime now = DateTime.Now;
            if (upload.Count == 0)
            {
                await HttpContext.Response.WriteAsync("Image invalid.");
            }
            else
            {
                var file = upload[0];
                var filename = ContentDispositionHeaderValue
                                    .Parse(file.ContentDisposition)
                                    .FileName?.Trim('"');

                var imageFolder = $@"\uploaded\images\{now:yyyyMMdd}";

                string folder = hostingEnvironment.WebRootPath + imageFolder;

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filePath = Path.Combine(folder, filename!);
                await using (FileStream fs = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(fs);
                    fs.Flush();
                }
                await HttpContext.Response.WriteAsync("<script>window.parent.CKEDITOR.tools.callFunction(" + ckEditorFuncNum + ", '" + Path.Combine(imageFolder, filename!).Replace(@"\", @"/") + "');</script>");
            }
        }

        /// <summary>
        /// Upload image for form
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadImage()
        {
            DateTime now = DateTime.Now;
            var files = Request.Form.Files;
            if (files.Count == 0)
            {
                return new BadRequestObjectResult(files);
            }
            else
            {
                var file = files[0];
                var filename = ContentDispositionHeaderValue
                                    .Parse(file.ContentDisposition)
                                    .FileName?.Trim('"');

                var imageFolder = $@"\uploaded\images\{now:yyyyMMdd}";

                string folder = hostingEnvironment.WebRootPath + imageFolder;

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filePath = Path.Combine(folder, filename!);
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                return new OkObjectResult(Path.Combine(imageFolder, filename!).Replace(@"\", @"/"));
            }
        }
    }
}