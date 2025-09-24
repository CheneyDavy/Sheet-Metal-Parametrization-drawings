using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.Utils
{
    public class LicenseUtil
    {

        public static CheckResult Check()
        {
            List<string> macs = new List<string>();
            {
                macs.Add("70-08-94-49-BF-B9");//cx
                macs.Add("00-2B-67-65-5C-FE");//pl

                //已购买用户
                macs.Add("FC-6D-77-B6-CB-E1");
            }

            var result = CheckMac(macs.ToArray());//cx
            if (result.IsPass)
            {
                return result;
            }

            result = CheckTime("2025-09-20 00:00:00");
            if (result.IsPass)
            {
                result = CheckMac("FC-6D-77-B6-CB-E1");//试用
            }
            return result;
        }

        public static CheckResult CheckTime(string time)
        {
            CheckResult result = new CheckResult();
            DateTime dt = Convert.ToDateTime(time);
            if (DateTime.Compare(DateTime.Now, dt) > 0)
            {
                result.IsPass = false;
                result.Message = "试用期已过，请联系供应商！";
            }
            else
            {
                result.IsPass = true;
            }
            return result;
        }

        public static CheckResult CheckMac(params string[] correctMacs)
        {
            CheckResult result = new CheckResult();
            if (correctMacs == null)
            {
                correctMacs = new string[] { };
            }
            correctMacs = correctMacs.Select(x => x.ToLower()).ToArray();
            var macs = GetMacAddress();
            foreach (string mac in correctMacs)
            {
                if (macs.Contains(mac))
                {
                    result.IsPass = true;
                    return result;
                }
            }

            result.IsPass = false;
            result.Message = "非授权Mac，请联系供应商！";
            return result;
        }

        private static List<string> GetMacAddress()
        {
            List<string> strMac = new List<string>();
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    foreach (ManagementObject mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            strMac.Add(mo["MacAddress"].ToString().ToLower().Replace(":", "-"));
                        }
                    }
                }
            }
            return strMac;
        }

    }

    public class CheckResult
    {
        public bool IsPass { get; set; }
        public string Message { get; set; }
    }
}
