using WireguardWebUi.Builders;

namespace WireguardWebUi.Utils.Linux;

public class WgUtils (CliUtil cliUtil, IConfiguration configuration, WgConfigsBuilder wgConfigsBuilder,IWebHostEnvironment environment)
{
    private CliUtil _cliUtil = cliUtil;
    private WgConfigsBuilder _wgConfigsBuilder = wgConfigsBuilder;
    private readonly IWebHostEnvironment _environment = environment;

    private readonly string? _wgInterface = configuration["WireguardPaths:WgInterface"];
    private readonly string? _wgConfigPath = configuration["WireguardPaths:WgConfigPath"];
    private readonly string? _serverIp = configuration["WireguardPaths:ServerIp"];
    private readonly string? _dns = configuration["WireguardPaths:DNS"];
    private readonly string? _clientIpPart = configuration["WireguardPaths:ClientIpPart"];
    private readonly ushort _serverPort = configuration.GetValue<ushort>("WireguardPaths:ServerPort");
    private readonly string _serverPublicKey = configuration["WireguardPaths:ServerPublicKey"];
    public string GeneratePrivateKey()
    {
        return cliUtil.RunCommand("wg genkey");
    }
    
    public string GeneratePresharedKey()
    {
        return cliUtil.RunCommand($"wg genpsk");
    }
    
    public string GeneratePublicKey(string privateKey)
    {
        return cliUtil.RunCommand($"echo "+privateKey+" | wg pubkey");
    }

    public string GetStatus()
    {
        return cliUtil.RunCommand("sudo wg show");
    }

    public ushort GetPaths()
    {
        return _serverPort;
    }
    
    
    public string GenerateClientConfig(string clientName)
    {

        Console.WriteLine(clientName);
        string privateKey = GeneratePrivateKey();
        string publicKey = GeneratePublicKey(privateKey);
        ushort clinetNum = GetLastClientIp();
        string newIpClient = _clientIpPart + clinetNum;
        
        _wgConfigsBuilder.clientPrivateKey = privateKey;
        _wgConfigsBuilder.clientPublicKey = publicKey;
        _wgConfigsBuilder.clientPresharedKey = GeneratePresharedKey();
        _wgConfigsBuilder.clientName = clientName;
        _wgConfigsBuilder.clientAddressPort = newIpClient;
        _wgConfigsBuilder.serverPublicKey = _serverPublicKey;
        _wgConfigsBuilder.serverIp = _serverIp;
        _wgConfigsBuilder.serverPort = _serverPort;
        _wgConfigsBuilder.clientNum = clinetNum;
        _wgConfigsBuilder.dns = _dns;
        string newClientConfig = _wgConfigsBuilder.BuildClientConfig();
        using StreamWriter sw = new StreamWriter($"{_environment.WebRootPath}/UserConfigs/{clientName}.conf");
        sw.Write(newClientConfig);
        return newClientConfig;
    }


    public ushort GetLastClientIp()
    {
        string data = _cliUtil.RunCommand("sudo wg show");
        var lines = data.Split('\n');
        var allowedIps = lines.Where(line => line.Trim().StartsWith("allowed ips", StringComparison.OrdinalIgnoreCase))
            .ToList();


        if (allowedIps.Any())
        {
            // Последняя строка с 'allowed ips'
            var lastAllowedIp = allowedIps.Last();

            // Извлечение IP-адреса
            var ipAddress = lastAllowedIp.Split(':')[1].Trim();
            ushort num = Convert.ToUInt16(ipAddress.Split(_clientIpPart)[1].Split('/')[0].Trim());
            num++;
            return num;
        }

        return 1;

    }
    
    
    public string AddClientToServerConfig()
    {
        string serverConfig = _wgConfigsBuilder.BuildServerConfig();
        using StreamWriter streamWriter = new("/etc/wireguard/wg0.conf", true);
        streamWriter.WriteLine(serverConfig);

        return serverConfig;
        
    }
    
    
}