using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

public static class QRCodeHelper
{
    public static dynamic GenerateQRCodeBase64(string code)
    {
        throw new NotImplementedException();
    }

    public static string GenerateQrCodeBase64Async(string url)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                bitMap.Save(ms, ImageFormat.Png);
                var result = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());

                return result;
            }
        }
    }
}
