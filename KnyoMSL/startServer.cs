using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace KnyoMSL
{
    public class startServer
    {
        public string path,serverName;
        public int minM = 512;
        public int maxM = 2048;
        public string javaPath = "java";
        public string otherArgs = "";

        // public string cs = "-XX:+UseConcMarkSweepGC -XX:+CMSParallelRemarkEnabled -XX:+ExplicitGCInvokesConcurrent -XX:+CMSScavengeBeforeRemark -XX:CMSInitiatingOccupancyFraction=70 -XX:+UseCMSInitiatingOccupancyOnly -XX:TargetSurvivorRatio=90 -XX:ParallelGCThreads=6";
        // public string arg = "-server -d64 -Xincgc -XX:+AggressiveOpts -XX:+UseCompressedOops -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:+DisableExplicitGC ";
        
        public void creatStartBat()
        {
            string bat = $@"
@echo off
COLOR 0b
title {serverName}
""{javaPath}"" -Xmx{maxM}M -Xms{minM}M -Xss512K {otherArgs} -jar ""{path}"" nogui
";
            File.WriteAllText("Start.bat",bat);
        }

        public void generateMknyo()
        {
            int AvailableM = int.Parse((GetAvailablePhysicalMemory() / 1024 / 1024).ToString());
            if ( AvailableM < 1536 )
            {
                this.maxM = AvailableM / 2;
                this.minM = 512;
                if (maxM < minM)
                    this.minM = 256;
            }
            else
            {
                this.maxM = AvailableM / 4 * 3;
                this.minM = 1024;
            }
        }

        // P.S. 下列两个函数来自于 CSDN-郝伟博士

        public long GetAvailablePhysicalMemory()
        {
            long capacity = 0;
            try
            {
                foreach (ManagementObject mo1 in new ManagementClass("Win32_PerfFormattedData_PerfOS_Memory").GetInstances())
                    capacity += long.Parse(mo1.Properties["AvailableBytes"].Value.ToString());
            }
            catch (Exception ex)
            {
                capacity = -1;
                Console.WriteLine(ex.Message);
            }
            return capacity;
        }


        public long GetPhisicalMemory()
        {
            long capacity = 0;
            try
            {
                foreach (ManagementObject mo1 in new ManagementClass("Win32_PhysicalMemory").GetInstances())
                    capacity += long.Parse(mo1.Properties["Capacity"].Value.ToString());
            }
            catch (Exception ex)
            {
                capacity = -1;
                Console.WriteLine(ex.Message);
            }
            return capacity;
        }

    }
}
