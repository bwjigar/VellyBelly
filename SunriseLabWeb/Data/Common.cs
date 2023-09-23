using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SunriseLabWeb.Data
{
    public class Common
    {
        public string gUserIPAddresss(bool GetLan = false)
        {
            try
            {
                string visitorIPAddress = HttpContext.Current.Request.Headers["CF-CONNECTING-IP"];

                if (String.IsNullOrEmpty(visitorIPAddress))
                    visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (String.IsNullOrEmpty(visitorIPAddress))
                    visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (string.IsNullOrEmpty(visitorIPAddress))
                    visitorIPAddress = HttpContext.Current.Request.UserHostAddress;


                if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
                {
                    GetLan = true;
                    visitorIPAddress = string.Empty;
                }
                if (!GetLan && Dns.GetHostEntry(Dns.GetHostName()).AddressList
                                   .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                   .ToString() == visitorIPAddress)
                {
                    GetLan = true;
                    visitorIPAddress = string.Empty;
                }
                string ip = "192.168.0.155";
                if (visitorIPAddress.Contains(ip))
                {
                    GetLan = true;
                    visitorIPAddress = string.Empty;
                }

                if (GetLan && string.IsNullOrEmpty(visitorIPAddress))
                {
                    try
                    {
                        string url = "http://checkip.dyndns.org";
                        System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                        System.Net.WebResponse resp = req.GetResponse();
                        System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                        string response = sr.ReadToEnd().Trim();
                        string[] a = response.Split(':');
                        string a2 = a[1].Substring(1);
                        string[] a3 = a2.Split('<');
                        string a4 = a3[0];
                        visitorIPAddress = a4;
                    }
                    catch
                    {
                        return null;
                    }
                }
                return visitorIPAddress;
            }
            catch
            {
                return null;
            }

        }
    }
}