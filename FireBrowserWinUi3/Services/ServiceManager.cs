using System.Diagnostics;
using System.ServiceProcess;
using System;

public class ServiceManager
{
    private const string ServiceName = "SecureVaultService";

    // Use Package.Current.InstalledLocation to dynamically get the path in MSIX
    private static string GetServicePath()
    {
        var packageLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
        var servicePath = System.IO.Path.Combine(packageLocation.Path, "Assets", "Apps", "SecureVaultService.exe");
        return servicePath;
    }

    public static void InstallService()
    {
        var exePath = GetServicePath();
        ExecutePowerShellCommand($@"
                New-Service -Name '{ServiceName}' -Binary '{exePath}' -DisplayName 'Secure Vault Service' -Description 'A service that manages secure vault operations.' -StartupType Automatic
                Start-Service -Name '{ServiceName}'
            ");
    }

    public static void StartService()
    {
        using (ServiceController sc = new ServiceController(ServiceName))
        {
            if (sc.Status != ServiceControllerStatus.Running)
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
            }
        }
    }

    public static void StopService()
    {
        using (ServiceController sc = new ServiceController(ServiceName))
        {
            if (sc.Status == ServiceControllerStatus.Running)
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
            }
        }
    }

    public static void UninstallService()
    {
        ExecutePowerShellCommand($@"
                Stop-Service -Name '{ServiceName}' -ErrorAction SilentlyContinue
                Remove-Service -Name '{ServiceName}' -ErrorAction SilentlyContinue
            ");
    }

    private static void ExecutePowerShellCommand(string command)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"PowerShell command failed with exit code {process.ExitCode}. Output: {output}");
            }
        }
    }
}