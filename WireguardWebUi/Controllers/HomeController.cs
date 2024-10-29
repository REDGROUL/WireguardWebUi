using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WireguardWebUi.Models;
using WireguardWebUi.Utils;
using WireguardWebUi.Utils.Linux;

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

    public IActionResult Index()
    {
        string clientConfig = _wgUtils.GenerateClientConfig("sometest");
        string serverConfig = _wgUtils.AddClientToServerConfig();
        string qr = _wgQrGenerator.GenerateBase64(clientConfig);

        ViewData["client"] = clientConfig;
        ViewData["serverConfig"] = serverConfig;
        ViewData["qr"] = qr;
        
        return View();
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
    
    
}