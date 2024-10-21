using System;
using System.Diagnostics;
using System.IO;

namespace WireguardWebUi.Utils;

public class WgUtils (CliUtil cliUtil, IConfiguration configuration)
{
    private CliUtil _cliUtil = cliUtil;

    private readonly string? _wgInterface = configuration["WireguardPaths:WgInterface"];
    private readonly string? _wgConfigPath = configuration["WireguardPaths:WgConfigPath"];
    private readonly string? _serverIp = configuration["WireguardPaths:ServerIp"];
    private readonly string? _dns = configuration["WireguardPaths:DNS"];
    private readonly string? _clientIpPart = configuration["WireguardPaths:ClientIpPart"];
    private readonly ushort _serverPort = configuration.GetValue<ushort>("WireguardPaths:ServerPort");
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
        return cliUtil.RunCommand("wg show");
    }

    public ushort GetPaths()
    {
        return _serverPort;
    }
    
    
    public string GenerateClientConfig(string clientName)
    {

        string privateKey = GeneratePrivateKey();
        
        Console.WriteLine(privateKey);
        
        string publicKey = GeneratePublicKey(privateKey);
        string presharedKey = GeneratePresharedKey();
        Console.WriteLine("pub:"+publicKey);
        ushort clientNum = GetLastClientIp();
        clientNum++;
        string clientConfig = $@"
[Interface]
PrivateKey = {privateKey}
Address = {_clientIpPart+clientNum},fd42:42:42::8/128
DNS = {_dns}

[Peer]
PublicKey = mfYMXZw3AD3zxWB7QBVTLzKXDT8CSTMbyjyuL5dFtkc=
PresharedKey = {presharedKey}
Endpoint = {_serverIp}:{_serverPort}
AllowedIPs = 0.0.0.0/0, ::0,fd42:42:42::8/128";
        clientConfig.Trim();
        Console.WriteLine(clientConfig);
        string clientConfigPath = $"{clientName}.conf";
        System.IO.File.WriteAllText(clientConfigPath, clientConfig);
        AddClientToServerConfig(clientName, publicKey, _clientIpPart + clientNum,presharedKey);
        
        return clientConfig;
    }


    public ushort GetLastClientIp()
    {
        string data = _cliUtil.RunCommand("wg show");
        var lines = data.Split('\n');
        var allowedIps = lines.Where(line => line.Trim().StartsWith("allowed ips", StringComparison.OrdinalIgnoreCase))
            .ToList();


        if (allowedIps.Any())
        {
            // Последняя строка с 'allowed ips'
            var lastAllowedIp = allowedIps.Last();

            // Извлечение IP-адреса
            var ipAddress = lastAllowedIp.Split(':')[1].Trim();
            
            return Convert.ToUInt16(ipAddress.Split(_clientIpPart)[1].Split('/')[0].Trim());
        }

        return 1;

    }
    
    
    private bool AddClientToServerConfig(string clientName, string clientPublicKey, string clientIp, string presharedKey)
    {
        

        string clientConfig = $@"

### {clientName}
[Peer]
PublicKey = {clientPublicKey}
PresharedKey = {presharedKey}
AllowedIPs = {clientIp}";

        // Добавляем клиента в конфигурацию WireGuard на сервере
        System.IO.File.AppendAllText(_wgConfigPath, clientConfig.Trim());
        _cliUtil.RunCommand(@"sudo systemctl restart wg-quick@wg0");
        return true;
    }
    
}