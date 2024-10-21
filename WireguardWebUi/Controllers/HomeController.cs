using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WireguardWebUi.Models;
using WireguardWebUi.Utils;

namespace WireguardWebUi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private  WgUtils _wgUtils;
    private  IConfiguration _conf;
    private  WgQrGenerator _wgQrGenerator;

    public HomeController(ILogger<HomeController> logger, WgUtils wgUtils, WgQrGenerator qrGenerator, IConfiguration configuration)
    {
        _logger = logger;
        _wgUtils = wgUtils;
        _wgQrGenerator = qrGenerator;
        _conf = configuration;
    }

    public string? Index()
    {
        return _wgUtils.GetStatus();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpGet("generate-qr")]
    public IActionResult GenerateWireGuardQrCode()
    {
        string client = _wgUtils.GenerateClientConfig("sss");
       Console.WriteLine("sss");

        // Генерация QR-кода из конфигурации
        byte[] qrCodeImage = _wgQrGenerator.GenerateQrCode(client);


        _logger.Log(LogLevel.Information, qrCodeImage.ToString());
        // Возвращаем QR-код в формате PNG
        return File(qrCodeImage, "image/png");
    }
    
}