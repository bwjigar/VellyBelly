using System;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Web;
using System.Threading;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Security.Claims;
using System.Net.Http;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Lib.Model
{
    public static class Common
    {
        public static DateTime GetHKTime()
        {
            DateTime dt = DateTime.Now.ToUniversalTime();
            dt = TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));

            return dt;
        }

        public static DateTime GetGMTime(DateTime ust)
        {
            DateTime dt;//= ust.ToUniversalTime();
            dt = ust.AddHours(5);
            return dt;
        }

        public static DateTime GetHKTime(DateTime ust)
        {
            DateTime dt;
            dt = GetGMTime(ust);
            dt = dt.AddHours(8);// (TimeZoneInfo.FindSystemTimeZoneById("China Standard Time").BaseUtcOffset);

            return dt;
        }
        public static string EmailHeader()
        {
            return @"<html><head><style type=""text/css"">body{font-family: Verdana,'sans-serif';font-size:12px;}p{text-align:justify;margin:10px 0 !important;}
                a{color:#1a4e94;text-decoration:none;font-weight:bold;}a:hover{color:#3c92fe;}table td{font-family: Verdana,'sans-serif' !important;font-size:12px;padding:3px;border-bottom:1px solid #dddddd;}
                </style></head><body>
                <div style=""width:100%; margin:5px auto;font-family: Verdana,'sans-serif';font-size:12px;line-height:20px; background-color:#f2f2f2;"">
                <img alt=""Sunrise Diamonds Ltd"" src=""https://sunrisediamonds.com.hk/Images/email-head.png"" width=""100%"" />
                <div style=""padding:10px;overflow-x:scroll !important;overflow-y:hidden;"">";
        }
        public static string EmailSignature()
        {
            return @"<p>Please do let us know if you have any questions. Email us on <a href=""mailto:support@sunrisediamonds.com.hk"">support@sunrisediamonds.com.hk</a></p>
                <p>Thanks and Regards,<br />Sunrise Diamond Team,<br />Room 1,14/F, Peninsula Square<br/>East Wing, 18 Sung On Street<br/>Hunghom, Kowloon<br/>Hong Kong<br/>
                <a href=""https://sunrisediamonds.com.hk"">www.sunrisediamonds.com.hk</a></p>
                </div></div></body></html>";
        }
        private static void SendMail(string fsToAdd, string fsSubject, string fsMsgBody, string fsCCAdd, int? fiOrderId, bool AdminMail, Int64? UserId, string MailFrom, bool bIsOrder)
        {
            MailMessage loMail = new MailMessage();
            SmtpClient loSmtp = new SmtpClient();
            try
            {
                loMail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"], "Sunrise Diamonds");
                loMail.To.Add(fsToAdd);
                if (!string.IsNullOrEmpty(fsCCAdd))
                    loMail.Bcc.Add(fsCCAdd);
                if (bIsOrder == false)
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CCEmail"]))
                        loMail.Bcc.Add(ConfigurationManager.AppSettings["CCEmail"]);
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BCCEmail"]))
                        loMail.Bcc.Add(ConfigurationManager.AppSettings["BCCEmail"]);
                }
                loMail.Subject = fsSubject;
                loMail.IsBodyHtml = true;

                AlternateView av = AlternateView.CreateAlternateViewFromString(fsMsgBody, null, MediaTypeNames.Text.Html);
                loMail.AlternateViews.Add(av);

                Thread email = new Thread(delegate ()
                {
                    loSmtp.Send(loMail);
                });

                email.IsBackground = true;
                email.Start();
                if (!email.IsAlive)
                {
                    email.Abort();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool EmailOfSuspendedUser(string ToAssEmail, string Name, string Username, string CompName)
        {
            try
            {
                StringBuilder loSb = new StringBuilder();
                loSb.Append(EmailHeader());

                loSb.Append(@"<p style=""font-size:18px; color:#1a4e94;"">Dear Sir/Madam,</p>");
                loSb.Append(@"<p>" + Username + " [ " + CompName + " ] has tried to login on our website [ Sunrise Lab Website ].<br />");
                loSb.Append(@" As per our company policy his/her account is suspended.<br /></p>");

                loSb.Append(EmailSignature());

                SendMail(ToAssEmail, "Unauthorised Login Attemt.", Convert.ToString(loSb), null, null, false, 0, "SuspendedUser", false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static Boolean InsertErrorLog(Exception ex, string Message, HttpRequestMessage Request)
        {
            Database db = new Database();
            System.Collections.Generic.List<System.Data.IDbDataParameter> para;
            para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            para.Add(db.CreateParam("dtErrorDate", System.Data.DbType.DateTime, System.Data.ParameterDirection.Input, GetHKTime()));

            if (ex != null)
                para.Add(db.CreateParam("sErrorTrace", System.Data.DbType.String, System.Data.ParameterDirection.Input, ex.ToString()));
            else
                para.Add(db.CreateParam("sErrorTrace", System.Data.DbType.String, System.Data.ParameterDirection.Input, null));
            if (ex != null)
                para.Add(db.CreateParam("sErrorMsg", System.Data.DbType.String, System.Data.ParameterDirection.Input, ex.Message.ToString() + Message));
            else if (Message != "")
                para.Add(db.CreateParam("sErrorMsg", System.Data.DbType.String, System.Data.ParameterDirection.Input, Message));
            else
                para.Add(db.CreateParam("sErrorMsg", System.Data.DbType.String, System.Data.ParameterDirection.Input, null));

            if (Request != null)
            {
                ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
                List<Claim> claims = principal.Claims.ToList();

                if (claims.Count > 0)
                {
                    para.Add(db.CreateParam("iUserId", System.Data.DbType.String, System.Data.ParameterDirection.Input, Convert.ToInt32(claims.Where(cl => cl.Type == "UserID").FirstOrDefault().Value)));
                    para.Add(db.CreateParam("sIPAddress", System.Data.DbType.String, System.Data.ParameterDirection.Input, Convert.ToString(claims.Where(cl => cl.Type == "IpAddress").FirstOrDefault().Value)));
                    para.Add(db.CreateParam("sErrorSite", System.Data.DbType.String, System.Data.ParameterDirection.Input, Convert.ToString(claims.Where(cl => cl.Type == "DeviseType").FirstOrDefault().Value)));
                }
            }
            para.Add(db.CreateParam("sErrorPage", System.Data.DbType.String, System.Data.ParameterDirection.Input, null));

            db.ExecuteSP("ErrorLog_Insert", para.ToArray(), false);
            return true;
        }
        public static string ToXML<T>(T obj)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            }
        }
        public static string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }

    }
}
