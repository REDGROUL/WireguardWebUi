using QRCoder;
namespace WireguardWebUi.Utils.Linux;

public class WgQrGenerator ()
{
    public string GenerateBase64(string clientConfig)
    {
        using QRCodeGenerator qrGenerator = new QRCodeGenerator();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(clientConfig, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);
        return "data:image/png;base64,"+Convert.ToBase64String(qrCodeImage);
    }
}