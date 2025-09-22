using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using IDS.Pinger.Properties;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml;

namespace IDS.Pinger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] HostIP = Settings.Default.HostIP.Split('|');
            string[] Hostname = Settings.Default.Hostname.Split('|');
            if (HostIP.Length != Hostname.Length) throw new Exception("Fehler in Konfiguration - Die Anzahl von HostIP und Hostname müssen gleich sein");
            Ping myPing = new Ping();
            for(int i =0; i<HostIP.Length; i++)
            {
                PingReply myPingReply = myPing.Send(HostIP[i],120);
                string logstring = " | " + Hostname[i] + " [" + HostIP[i] + "] | " + myPingReply.Status.ToString();
                if (myPingReply.Status != IPStatus.Success)
                    logstring.WriteErrorLog();
                else
                    logstring.WriteSuccessLog();
            }
        }
    }

    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Recursively create directory
        /// </summary>
        /// <param name="dirInfo">Folder path to create.</param>
        public static void CreateDirectory(this DirectoryInfo dirInfo)
        {
            if (dirInfo.Parent != null && !dirInfo.Exists) CreateDirectory(dirInfo.Parent);
            if (!dirInfo.Exists) dirInfo.Create();
        }

        /// <summary>
        /// Write Message to Success Logfile
        /// </summary>
        /// <param name="message"></param>
        public static void WriteSuccessLog(this String message)
        {
            try
            {

                string logfile = ConfigurationManager.AppSettings["Logfile_Success"];
                var dir = new DirectoryInfo(@logfile);
                dir.Parent.CreateDirectory();
                using (StreamWriter outfile = new StreamWriter(@logfile, true))
                {
                    outfile.WriteLine(DateTime.Now.ToString() + message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("cannot write to success log - "+ex.Message);
            }
        }

        /// <summary>
        /// Write Message to Error Logfile
        /// </summary>
        /// <param name="message"></param>
        public static void WriteErrorLog(this String message)
        {
            try
            {
                string logfile = ConfigurationManager.AppSettings["Logfile_Error"];
                var dir = new DirectoryInfo(@logfile);
                dir.Parent.CreateDirectory();
                using (StreamWriter outfile = new StreamWriter(@logfile, true))
                {
                    outfile.WriteLine(DateTime.Now.ToString() + message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("cannot write to error log file - " + ex.Message);
            }
        }

    }
}
