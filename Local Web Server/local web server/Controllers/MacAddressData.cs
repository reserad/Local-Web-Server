using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Local_Web_Server.Models;
using System.Web;
using System.Threading;

namespace Local_Web_Server.Controllers
{
    public class MacAddressData
    {
        public static Queue targets = Queue.Synchronized(new Queue());
        public static Queue results = Queue.Synchronized(new Queue());
        public static List<string> getIPS() 
        {
            List<string> IPs = new List<string>();

           //Load ipaddresses into targets queue
            for (int i = 2; i < 255; i++)
            {
                targets.Enqueue(string.Format("192.168.1.{0}", i));
                //targets.Enqueue(string.Format("192.168.0.{0}", i));
            }

            int threadIsAliveCount = 0;

           //Specify number of threads
            Thread[] t = new Thread[40];

           //Start up the threads
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new Thread(new ThreadStart(pinger));
                t[i].Start();
                threadIsAliveCount++;
            }

            //Loop if threads are still active
            while (threadIsAliveCount > 0)
            {
                threadIsAliveCount = 0;
                foreach (Thread et in t)
                {
                    if (et.IsAlive)
                    {
                        threadIsAliveCount++;
                    }
                }

                //Dequeue results and output to console
                while (results.Count > 0)
                {
                    IPs.Add(results.Dequeue().ToString());
                }
            }

            return IPs;
        }

        static void pinger()
        {
            while (targets.Count > 0)
            {
                string addr = targets.Dequeue().ToString();
                if (new Ping().Send(addr, 1000).Status == IPStatus.Success)
                {
                    results.Enqueue(addr);
                }
            }

            //Delay thread termination by 500ms to allow dequeueing of results.
            Thread.Sleep(500);
        }

        public static List<ComputerData> PutAllTogether() 
        {
            List<string> IPs = new List<string>();
            List<string> RefinedIPs = new List<string>();
            IPs = getIPS();

            foreach (var ip in IPs) 
            {
                if (!RefinedIPs.Contains(ip)) 
                {
                    RefinedIPs.Add(ip);
                }
            }

            List<ComputerData> cd = new List<ComputerData>();
            foreach (var ip in RefinedIPs)
            {
                ComputerData data = new ComputerData();
                data.IPAddress = ip;
                data.Mac = GetMacAddress(ip);
                cd.Add(data);
            }

            return cd; 
        }

        public static string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a " + ipAddress;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                         + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                         + "-" + substrings[7] + "-"
                         + substrings[8].Substring(0, 2);
                return macAddress;
            }

            else
            {
                return "not found";
            }
        }
        public static string GetHostName(string ip) 
        {
            return Dns.GetHostEntry(ip).HostName;
        }
    }
}