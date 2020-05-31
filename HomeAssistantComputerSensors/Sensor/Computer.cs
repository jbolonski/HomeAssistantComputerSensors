using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace HomeAssistantComputerSensors.Sensor
{
    class Computer
    {
        public async Task<double> GetCpuUsageFoCurrentProcess()
        {
            var currentProcess = Process.GetCurrentProcess();

            return await GetCpuUsageForProcess(currentProcess);
        }

        private async Task<double> GetCpuUsageForProcess(System.Diagnostics.Process currentProcess)
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = currentProcess.TotalProcessorTime;

            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = currentProcess.TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }


        public async Task<double> GetCpuUsage()
        {
            double totalUsage = 0;
            foreach (var proc in Process.GetProcesses())
            {
                totalUsage += await GetCpuUsageForProcess(proc);
            }
            return totalUsage;
        }


        public double GetMemoryUsage()
        {
            double totalSize = 0;
            foreach( var proc in Process.GetProcesses())
            {
                totalSize += proc.WorkingSet64 / 1024.0;
            }

            return totalSize;
        }
    }
}
