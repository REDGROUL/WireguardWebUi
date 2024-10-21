using System.Diagnostics;

namespace WireguardWebUi.Utils;

public class CliUtil
{
    public string RunCommand(string command)
    {
        var processInfo = new ProcessStartInfo("bash", $"-c \"sudo {command}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processInfo);
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Ошибка выполнения команды: {command}\nОшибка: {error}");
        }

        return output.Trim();
    }
}