using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.ServiceProcess;

namespace SmartWeight.Background.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        public ProcessController()
        {

        }

        [HttpPost("Kill")]
        public void KillProcess(int pid)
            => Process.GetProcessById(pid).Kill();

        [HttpPost("StopService")]
        public void StopService(string serviceName)
        {
            if (OperatingSystem.IsWindows())
            {
                var service = ServiceController.GetServices()
                    .FirstOrDefault(service => service.ServiceName == serviceName);

                service?.Close();
                service?.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        [HttpPost("RunService")]
        public void RunService(string serviceName)
        {
            if (OperatingSystem.IsWindows())
            {
                ServiceController.GetServices()
                    .FirstOrDefault(service => service.ServiceName == serviceName)?
                    .Start();
            }
        }
    }
}