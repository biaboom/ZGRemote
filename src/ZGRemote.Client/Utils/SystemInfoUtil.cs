using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace ZGRemote.Client.Utils
{
    public class SystemInfoUtil
    {
        public static string UserName;

        public static string MachineName;

        public static string OsVersion;

        public static string OsCaption;

        static SystemInfoUtil()
        {
            UserName = Environment.UserName;
            MachineName = Environment.MachineName;
            OsVersion = Environment.OSVersion.ToString();

            
            ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
            
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                OsCaption = mo.GetPropertyValue("Caption").ToString();
                break;
            }
            mc.Dispose();
        }
    }
}
