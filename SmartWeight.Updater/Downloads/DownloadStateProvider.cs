using Entities;
using System;
using System.Management;
using System.Threading.Tasks;
using System.Windows;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public class CheckProvider
    { 
    
    }

    public class InstallProvider
    { 
        
    }

    public abstract class UpdateProvider
    {
        public abstract Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null);

        protected string GetServicePath(string serviceName)
        {
            using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + serviceName + "'"))
            {
                wmiService.Get();
                return (wmiService["PathName"]?
                    .ToString())?
                    .Replace("Server.exe", "")
                    .Replace("\"", "");
            }
        }

        protected string GetServiceExecutionPath(string serviceName)
        {
            using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + serviceName + "'"))
            {
                wmiService.Get();
                return (wmiService["PathName"]?
                    .ToString());
            }
        }
    }
}
