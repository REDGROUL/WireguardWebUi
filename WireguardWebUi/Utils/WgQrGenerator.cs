using ZXing;
using ZXing.Common;
using SkiaSharp;
using System.IO;

public class WgQrGenerator
{

    // Генерация QR-кода на основе конфигурации клиента
    public byte[] GenerateQrCode(string clientConfig)
    {
        var qrWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions
            {
                Height = 300, // Высота изображения
                Width = 300,  // Ширина изображения
                Margin = 1    // Отступы
            }
        };

        // Создание QR-кода из конфигурации WireGuard
        var pixelData = qrWriter.Write(clientConfig);

        // Используем SkiaSharp для создания изображения
        using (var image = new SKBitmap(pixelData.Width, pixelData.Height))
        {
            // Переносим пиксельные данные в SKBitmap
            for (int y = 0; y < pixelData.Height; y++)
            {
                for (int x = 0; x < pixelData.Width; x++)
                {
                    var colorValue = pixelData.Pixels[(y * pixelData.Width + x) * 4];
                    var color = colorValue == 0 ? SKColors.Black : SKColors.White;
                    image.SetPixel(x, y, color);
                }
            }

            // Сохраняем изображение в поток памяти в формате PNG
            using (var memoryStream = new MemoryStream())
            {
                using (var imageEncoded = SKImage.FromBitmap(image))
                {
                    using (var data = imageEncoded.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        data.SaveTo(memoryStream);
                    }
                }

                // Возвращаем изображение в виде массива байтов
                return memoryStream.ToArray();
            }
        }
    }
}
