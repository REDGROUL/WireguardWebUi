using System.Text;

namespace WireguardWebUi.Builders;

public class WgConfigsBuilder
{
    private readonly IWebHostEnvironment _environment;
    public string clientName { get; set; }
    public string clientPrivateKey { get; set; }
    public string clientPublicKey{ get; set; }
    public string clientAddressPort { set; get; }
    public string clientPresharedKey{ get; set; }
    
    public string serverPublicKey{ get; set; }
    public string dns { get; set; }
    
    public string BuildClientConfig()
    {
        StringBuilder sb = new();
        sb.AppendLine("[Interface]");
        sb.AppendLine($"PrivateKey = {clientPrivateKey}");
        sb.AppendLine($"Address = {clientAddressPort},fd42:42:42::8/128");
        sb.AppendLine($"DNS = {dns}");
        sb.AppendLine();
        sb.AppendLine("[Peer]");
        sb.AppendLine($"PublicKey = {serverPublicKey}");
        sb.AppendLine($"PresharedKey = {clientPresharedKey}");
        sb.AppendLine("Endpoint = 93.3.3.3");
        sb.AppendLine("AllowedIPs = 0.0.0.0/0, ::0,fd42:42:42::8/128");
        return sb.ToString();
    }

    public string BuildServerConfig()
    {
        StringBuilder sb = new();
        sb.AppendLine();
        sb.AppendLine($"### Client {clientName}");
        sb.AppendLine("[Peer]");
        sb.AppendLine($"PublicKey = {clientPublicKey}");
        sb.AppendLine($"PresharedKey = {clientPresharedKey}");
        sb.AppendLine($"AllowedIPs = {clientAddressPort},fd42:42:42::8/128");
        return sb.ToString();
    }
    
    
}