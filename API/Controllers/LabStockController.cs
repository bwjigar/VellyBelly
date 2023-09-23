using API.Models;
//using ClosedXML.Excel;
using Lib.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
//using Syncfusion.XlsIO;

namespace API.Controllers
{
    [Authorize]
    [RoutePrefix("api/LabStock")]
    public class LabStockController : ApiController
    {
        public static int Log_TransId;
        public static int Log_UserId;
        public static int TotCount = 0;

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult LabDataUpload_Ora([FromBody]JObject data)
        {
            JObject test1 = JObject.Parse(data.ToString());
            LabDataUpload_Ora_Req obj = new LabDataUpload_Ora_Req();
            obj = JsonConvert.DeserializeObject<LabDataUpload_Ora_Req>(((Newtonsoft.Json.Linq.JProperty)test1.Last).Name.ToString());

            Database_Lab db = new Database_Lab(Request);
            List<IDbDataParameter> para = new List<IDbDataParameter>();

            para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, obj.Type.ToUpper()));
            para.Add(db.CreateParam("DataTransferType", DbType.String, ParameterDirection.Input, obj.DataTransferType.ToUpper()));

            DataTable dt0 = db.ExecuteSP("LabDataUpload_Status_Get", para.ToArray(), false);

            string path = HttpContext.Current.Server.MapPath("~/Lab_Upload_From_Oracle_Log.txt");
            if (!File.Exists(@"" + path + ""))
            {
                File.Create(@"" + path + "").Dispose();
            }
            StringBuilder sb = new StringBuilder();

            if (dt0.Rows[0]["STATUS"].ToString() == "1")
            {
                try
                {
                    LabDataUpload_Status("Insert", obj.DataTransferType, "MAS", "");

                    string Lab_MAS_SP_NAME = ConfigurationManager.AppSettings["Lab_MAS_SP_NAME"];
                    string Lab_Detail_SP_NAME = string.Empty;

                    if (obj.DataTransferType == "MACRO")
                    {
                        Lab_Detail_SP_NAME = ConfigurationManager.AppSettings["Lab_MACRO_SP_NAME"];
                    }
                    else if (obj.DataTransferType == "WITHOUT_MACRO")
                    {
                        Lab_Detail_SP_NAME = ConfigurationManager.AppSettings["Lab_WITHOUT_MACRO_SP_NAME"];
                    }

                    Oracle_DBAccess oracleDbAccess = new Oracle_DBAccess();
                    List<OracleParameter> paramList = new List<OracleParameter>();
                    DateTime date = DateTime.Now;
                    List<SqlParameter> para2 = new List<SqlParameter>();
                    string TRANS_ID = string.Empty, Message = string.Empty;
                    int Count = 0;
                    string DataTransferType = obj.DataTransferType.Replace('_', ' ');

                    OracleParameter param11 = new OracleParameter("vrec", OracleDbType.RefCursor);
                    param11.Direction = ParameterDirection.Output;
                    paramList.Add(param11);

                    DataTable dt = oracleDbAccess.CallSP(Lab_MAS_SP_NAME, paramList);

                    Count = 0;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        Count = dt.Rows.Count;

                        db = new Database_Lab(Request);
                        para2 = new List<SqlParameter>();

                        SqlParameter sqlparam1 = new SqlParameter("table", SqlDbType.Structured);
                        sqlparam1.Value = dt;
                        para2.Add(sqlparam1);

                        DataTable dt2 = db.ExecuteSP("LabData_Mas_Ora_Insert", para2.ToArray(), false);

                        if (dt2 != null && dt2.Rows.Count > 0)
                        {
                            if (dt2.Rows[0]["Message"].ToString() == "SUCCESS" && dt2.Rows[0]["Status"].ToString() == "1")
                            {
                                db = new Database_Lab(Request);
                                para = new List<IDbDataParameter>();
                                para.Add(db.CreateParam("Transfer_Type", DbType.String, ParameterDirection.Input, obj.DataTransferType));
                                DataTable dt6 = db.ExecuteSP("TransId_Get", para.ToArray(), false);

                                if (dt6 != null && dt6.Rows.Count > 0)
                                {
                                    if (dt6.Rows[0]["Message"].ToString() == "SUCCESS" && dt6.Rows[0]["Status"].ToString() == "1" && dt6.Rows[0]["TRANS_ID"].ToString() != "")
                                    {
                                        TRANS_ID = dt6.Rows[0]["TRANS_ID"].ToString();

                                        LabDataUpload_Status("Update", obj.DataTransferType, "DETAIL", TRANS_ID);

                                        string fromtime = Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt");

                                        string Last_Time = string.Empty;

                                        db = new Database_Lab(Request);
                                        para = new List<IDbDataParameter>();
                                        DataTable dt3 = db.ExecuteSP("LabStock_Fetch_Time_Get", para.ToArray(), false);

                                        if (dt3 != null && dt3.Rows.Count > 0)
                                        {
                                            Last_Time = dt3.Rows[0]["LabStock_Fetching_Data_To_Time"].ToString();
                                        }
                                        else
                                        {
                                            Last_Time = Common.GetHKTime().ToString("HH:mm");
                                        }

                                        string Current_Time = Common.GetHKTime().ToString("HH:mm");

                                        oracleDbAccess = new Oracle_DBAccess();
                                        paramList = new List<OracleParameter>();
                                        date = DateTime.Now;

                                        OracleParameter param1 = new OracleParameter("p_for_comp", OracleDbType.Int32);
                                        param1.Value = 1;
                                        paramList.Add(param1);

                                        OracleParameter param2 = new OracleParameter("p_trans_id", OracleDbType.NVarchar2);
                                        param2.Value = TRANS_ID;
                                        paramList.Add(param2);

                                        OracleParameter param3 = new OracleParameter("para_from_date", OracleDbType.Date);
                                        param3.Value = string.Format("{0:dd-MMM-yyyy}", date);
                                        paramList.Add(param3);

                                        OracleParameter param4 = new OracleParameter("para_to_date", OracleDbType.Date);
                                        param4.Value = string.Format("{0:dd-MMM-yyyy}", date);
                                        paramList.Add(param4);

                                        OracleParameter param5 = new OracleParameter("vrec", OracleDbType.RefCursor);
                                        param5.Direction = ParameterDirection.Output;
                                        paramList.Add(param5);

                                        OracleParameter param6 = new OracleParameter("p_pre_sold_flag", OracleDbType.NVarchar2);
                                        param6.Value = "B";
                                        paramList.Add(param6);

                                        string _fromtime = Common.GetHKTime().ToString("yyyy-MM-dd HH:mm:ss");

                                        DataTable dt4 = oracleDbAccess.CallSP(Lab_Detail_SP_NAME, paramList);

                                        string _totime = Common.GetHKTime().ToString("yyyy-MM-dd HH:mm:ss");

                                        Count = 0;
                                        if (dt4 != null && dt4.Rows.Count > 0)
                                        {
                                            if (obj.DataTransferType == "MACRO")
                                            {
                                                db = new Database_Lab(Request);
                                                para = new List<IDbDataParameter>();
                                                DataTable dt7 = db.ExecuteSP("LabStock_WITHOUT_MACRO_Delete", para.ToArray(), false);
                                            }

                                            Count = dt4.Rows.Count;

                                            db = new Database_Lab(Request);
                                            para2 = new List<SqlParameter>();

                                            SqlParameter sqlparam2 = new SqlParameter("table", SqlDbType.Structured);
                                            sqlparam2.Value = dt4;
                                            para2.Add(sqlparam2);

                                            DataTable dt5 = db.ExecuteSP("LabData_Ora_Insert", para2.ToArray(), false);

                                            LabDataUpload_Status("Delete", obj.DataTransferType, "", "");

                                            Message = string.Empty;
                                            if (dt5 != null)
                                            {
                                                Message = dt5.Rows[0]["Message"].ToString();
                                            }
                                            string totime = Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt");

                                            if (Message == "SUCCESS")
                                            {
                                                db = new Database_Lab(Request);
                                                para = new List<IDbDataParameter>();

                                                para.Add(db.CreateParam("LabStock_Fetching_Data_From_Time", DbType.String, ParameterDirection.Input, Last_Time));
                                                para.Add(db.CreateParam("LabStock_Fetching_Data_To_Time", DbType.String, ParameterDirection.Input, Current_Time));
                                                para.Add(db.CreateParam("LabStock_Fetching_From", DbType.String, ParameterDirection.Input, _fromtime));
                                                para.Add(db.CreateParam("LabStock_Fetching_To", DbType.String, ParameterDirection.Input, _totime));
                                                para.Add(db.CreateParam("TotalStock", DbType.String, ParameterDirection.Input, Count));
                                                para.Add(db.CreateParam("TRANS_ID", DbType.String, ParameterDirection.Input, TRANS_ID));
                                                para.Add(db.CreateParam("Transfer_Type", DbType.String, ParameterDirection.Input, obj.DataTransferType));

                                                DataTable dt1 = db.ExecuteSP("LabStock_Fetch_Time_Insert", para.ToArray(), false);

                                                LabStock_Upload_History_Insert("SUCCESS " + Count + " " + DataTransferType + " Lab Data Found" + (TRANS_ID != "" ? " for TRANS ID : " + TRANS_ID : "") + " upload time " + fromtime + " to " + totime);

                                                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                                sb.Append("SUCCESS " + Count + " " + DataTransferType + " Lab Data Found" + (TRANS_ID != "" ? " for TRANS ID : " + TRANS_ID : "") + " upload time " + fromtime + " to " + totime + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                                sb.AppendLine("");
                                                File.AppendAllText(path, sb.ToString());
                                                sb.Clear();

                                                return Ok(new CommonResponse
                                                {
                                                    Message = "SUCCESS " + Count + " " + DataTransferType + " Lab Data Found, upload time " + fromtime + " to " + totime + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                                                    Status = "1",
                                                    Error = ""
                                                });
                                            }
                                            else
                                            {
                                                LabStock_Upload_History_Insert("Lab Data upload in issue");

                                                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                                sb.Append("Lab Data upload in issue" + (Message != "" && Message != null ? " " + Message : "") + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                                sb.AppendLine("");
                                                File.AppendAllText(path, sb.ToString());
                                                sb.Clear();
                                                return Ok(new CommonResponse
                                                {
                                                    Message = "Lab Data upload in issue" + (Message != "" && Message != null ? " " + Message : "") + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                                                    Status = "1",
                                                    Error = ""
                                                });
                                            }
                                        }
                                        else
                                        {
                                            LabDataUpload_Status("Delete", obj.DataTransferType, "", "");
                                            LabStock_Upload_History_Insert("No Lab Data Found" + (TRANS_ID != "" ? " for TRANS ID : " + TRANS_ID : ""));

                                            sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                            sb.Append("No Lab Data Found" + (TRANS_ID != "" ? " for TRANS ID : " + TRANS_ID : "") + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                            sb.AppendLine("");
                                            File.AppendAllText(path, sb.ToString());
                                            sb.Clear();
                                            return Ok(new CommonResponse
                                            {
                                                Message = "No Lab Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                                                Status = "1",
                                                Error = ""
                                            });
                                        }
                                    }
                                    else if (dt6.Rows[0]["Message"].ToString() == "SUCCESS" && dt6.Rows[0]["Status"].ToString() == "1" && dt6.Rows[0]["TRANS_ID"].ToString() == "")
                                    {
                                        LabDataUpload_Status("Delete", obj.DataTransferType, "", "");
                                        LabStock_Upload_History_Insert("New TRANS ID Not Found");

                                        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                        sb.Append("New TRANS ID Not Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                        sb.AppendLine("");
                                        File.AppendAllText(path, sb.ToString());
                                        sb.Clear();
                                        return Ok(new CommonResponse
                                        {
                                            Message = "New TRANS ID Not Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                                            Status = "1",
                                            Error = ""
                                        });
                                    }
                                    else
                                    {
                                        Message = (dt6.Rows[0]["Message"].ToString() != "" && dt6.Rows[0]["Message"].ToString() != null ? " " + dt6.Rows[0]["Message"].ToString() : "");

                                        LabDataUpload_Status("Delete", obj.DataTransferType, "", "");
                                        LabStock_Upload_History_Insert("TRANS ID Get in issue" + Message);

                                        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                        sb.Append("TRANS ID Get in issue" + Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                        sb.AppendLine("");
                                        File.AppendAllText(path, sb.ToString());
                                        sb.Clear();
                                        return Ok(new CommonResponse
                                        {
                                            Message = "TRANS ID Get in issue" + Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                                            Status = "1",
                                            Error = ""
                                        });
                                    }
                                }
                                else
                                {
                                    LabDataUpload_Status("Delete", obj.DataTransferType, "", "");
                                    LabStock_Upload_History_Insert("TRANS ID Get in issue");

                                    sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                    sb.Append("TRANS ID Get in issue, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                    sb.AppendLine("");
                                    File.AppendAllText(path, sb.ToString());
                                    sb.Clear();
                                    return Ok(new CommonResponse
                                    {
                                        Message = "TRANS ID Get in issue, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                                        Status = "1",
                                        Error = ""
                                    });
                                }
                            }
                            else
                            {
                                Message = (dt2.Rows[0]["Message"].ToString() != "" && dt2.Rows[0]["Message"].ToString() != null ? " " + dt2.Rows[0]["Message"].ToString() : ""); 
                                
                                LabDataUpload_Status("Delete", obj.DataTransferType, "", "");
                                LabStock_Upload_History_Insert("Lab Mas Data upload in issue" + Message);

                                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                sb.Append("Lab Mas Data upload in issue" + Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                sb.AppendLine("");
                                File.AppendAllText(path, sb.ToString());
                                sb.Clear();
                                return Ok(new CommonResponse
                                {
                                    Message = "Lab Mas Data upload in issue" + Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                                    Status = "1",
                                    Error = ""
                                });
                            }
                        }
                        else
                        {
                            LabDataUpload_Status("Delete", obj.DataTransferType, "", "");
                            LabStock_Upload_History_Insert("Lab Mas Data upload in issue");

                            sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                            sb.Append("Lab Mas Data upload in issue, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                            sb.AppendLine("");
                            File.AppendAllText(path, sb.ToString());
                            sb.Clear();
                            return Ok(new CommonResponse
                            {
                                Message = "Lab Mas Data upload in issue, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                                Status = "1",
                                Error = ""
                            });
                        }
                    }
                    else
                    {
                        LabDataUpload_Status("Delete", obj.DataTransferType, "", "");
                        LabStock_Upload_History_Insert("No Lab Mas Data Found");

                        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                        sb.Append("No Lab Mas Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                        sb.AppendLine("");
                        File.AppendAllText(path, sb.ToString());
                        sb.Clear();
                        return Ok(new CommonResponse
                        {
                            Message = "No Lab Mas Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                            Status = "1",
                            Error = ""
                        });
                    }
                }
                catch (Exception ex)
                {
                    LabDataUpload_Status("Delete", obj.DataTransferType, "", "");
                    LabStock_Upload_History_Insert(ex.Message);

                    Common.InsertErrorLog(ex, null, Request);
                    sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                    sb.Append(ex.Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                    sb.AppendLine("");
                    File.AppendAllText(path, sb.ToString());
                    sb.Clear();
                    return Ok(new CommonResponse
                    {
                        Message = ex.Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                        Status = "0",
                        Error = ex.StackTrace
                    });
                }
            }
            else
            {
                if (dt0.Rows[0]["MESSAGE"].ToString() != "")
                {
                    LabStock_Upload_History_Insert(dt0.Rows[0]["MESSAGE"].ToString());

                    sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                    sb.Append(dt0.Rows[0]["MESSAGE"].ToString() + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                    sb.AppendLine("");
                    File.AppendAllText(path, sb.ToString());
                    sb.Clear();
                }

                return Ok(new CommonResponse
                {
                    Message = dt0.Rows[0]["MESSAGE"].ToString() + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                    Status = "1",
                    Error = ""
                });
            }




            //JObject test1 = JObject.Parse(data.ToString());
            //LabDataUpload_Ora_Req obj = new LabDataUpload_Ora_Req();
            //obj = JsonConvert.DeserializeObject<LabDataUpload_Ora_Req>(((Newtonsoft.Json.Linq.JProperty)test1.Last).Name.ToString());

            //Database_Lab db = new Database_Lab(Request);
            //System.Collections.Generic.List<System.Data.IDbDataParameter> para;
            //para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            //para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, obj.Type.ToUpper()));

            //DataTable dt3 = db.ExecuteSP("LabDataUpload_Status_Get", para.ToArray(), false);

            //string path = HttpContext.Current.Server.MapPath("~/Lab_Upload_From_Oracle_Log.txt");
            //if (!File.Exists(@"" + path + ""))
            //{
            //    File.Create(@"" + path + "").Dispose();
            //}
            //StringBuilder sb = new StringBuilder();

            //if (dt3.Rows[0]["STATUS"].ToString() == "1")
            //{
            //    try
            //    {
            //        string lab_data_Det_SP_NAME = ConfigurationManager.AppSettings["lab_data_Det_SP_NAME"];

            //        Oracle_DBAccess oracleDbAccess = new Oracle_DBAccess();
            //        List<OracleParameter> paramList = new List<OracleParameter>();
            //        DateTime date = DateTime.Now;
            //        DataTable dt = new DataTable();
            //        DataTable dt2 = new DataTable();
            //        List<SqlParameter> para2 = new List<SqlParameter>();
            //        string TRANS_ID = string.Empty, Message = string.Empty;
            //        int Count = 0;

            //        LabDataUpload_Status("Insert");

            //        string fromtime = Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt");

            //        string Last_Time = string.Empty;

            //        db = new Database_Lab(Request);
            //        List<IDbDataParameter> para3 = new List<IDbDataParameter>();
            //        DataTable dt4 = db.ExecuteSP("LabStock_Fetch_Time_Get", para3.ToArray(), false);

            //        if (dt4 != null && dt4.Rows.Count > 0)
            //        {
            //            Last_Time = dt4.Rows[0]["LabStock_Fetching_Data_To_Time"].ToString();
            //        }
            //        else
            //        {
            //            Last_Time = Common.GetHKTime().ToString("HH:mm");
            //        }

            //        string Current_Time = Common.GetHKTime().ToString("HH:mm");

            //        oracleDbAccess = new Oracle_DBAccess();
            //        paramList = new List<OracleParameter>();
            //        date = DateTime.Now;

            //        OracleParameter param1 = new OracleParameter("p_for_comp", OracleDbType.Int32);
            //        param1.Value = 1;
            //        paramList.Add(param1);

            //        OracleParameter param2 = new OracleParameter("p_time", OracleDbType.NVarchar2);
            //        param2.Value = Last_Time;
            //        paramList.Add(param2);

            //        OracleParameter param3 = new OracleParameter("vrec", OracleDbType.RefCursor);
            //        param3.Direction = ParameterDirection.Output;
            //        paramList.Add(param3);

            //        string _fromtime = Common.GetHKTime().ToString("yyyy-MM-dd HH:mm:ss");

            //        dt = oracleDbAccess.CallSP(lab_data_Det_SP_NAME, paramList);

            //        string _totime = Common.GetHKTime().ToString("yyyy-MM-dd HH:mm:ss");

            //        Count = 0;
            //        if (dt != null && dt.Rows.Count > 0)
            //        {
            //            Count = dt.Rows.Count;

            //            db = new Database_Lab(Request);
            //            dt2 = new DataTable();
            //            para2 = new List<SqlParameter>();

            //            SqlParameter sqlparam2 = new SqlParameter("table", SqlDbType.Structured);
            //            sqlparam2.Value = dt;
            //            para2.Add(sqlparam2);

            //            dt2 = db.ExecuteSP("LabData_Ora_Insert", para2.ToArray(), false);

            //            LabDataUpload_Status("Delete");

            //            Message = string.Empty;
            //            if (dt2 != null)
            //            {
            //                Message = dt2.Rows[0]["Message"].ToString();
            //            }
            //            string totime = Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt");

            //            if (Message == "SUCCESS")
            //            {
            //                db = new Database_Lab(Request);
            //                List<IDbDataParameter> para1 = new List<IDbDataParameter>();

            //                para1.Add(db.CreateParam("LabStock_Fetching_Data_From_Time", DbType.String, ParameterDirection.Input, Last_Time));
            //                para1.Add(db.CreateParam("LabStock_Fetching_Data_To_Time", DbType.String, ParameterDirection.Input, Current_Time));
            //                para1.Add(db.CreateParam("LabStock_Fetching_From", DbType.String, ParameterDirection.Input, _fromtime));
            //                para1.Add(db.CreateParam("LabStock_Fetching_To", DbType.String, ParameterDirection.Input, _totime));
            //                para1.Add(db.CreateParam("TotalStock", DbType.String, ParameterDirection.Input, Count));
            //                para1.Add(db.CreateParam("TRANS_ID", DbType.String, ParameterDirection.Input, TRANS_ID));

            //                DataTable dt1 = db.ExecuteSP("LabStock_Fetch_Time_Insert", para1.ToArray(), false);

            //                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //                sb.Append(Message + " " + Count + " Lab Data Found for " + Last_Time + " to " + Current_Time + " Hour, process time " + fromtime + " to " + totime + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //                sb.AppendLine("");
            //                File.AppendAllText(path, sb.ToString());
            //                sb.Clear();

            //                return Ok(new CommonResponse
            //                {
            //                    Message = Message + " " + Count + " Lab Data Found, process time " + fromtime + " to " + totime + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                    Status = "1",
            //                    Error = ""
            //                });
            //            }
            //            else
            //            {

            //                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //                sb.Append("Lab Data upload in issue" + (Message != "" && Message != null ? " " + Message : "") + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //                sb.AppendLine("");
            //                File.AppendAllText(path, sb.ToString());
            //                sb.Clear();
            //                return Ok(new CommonResponse
            //                {
            //                    Message = "Lab Data upload in issue" + (Message != "" && Message != null ? " " + Message : "") + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                    Status = "1",
            //                    Error = ""
            //                });
            //            }
            //        }
            //        else
            //        {
            //            LabDataUpload_Status("Delete");

            //            sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //            sb.Append("No Lab Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //            sb.AppendLine("");
            //            File.AppendAllText(path, sb.ToString());
            //            sb.Clear();
            //            return Ok(new CommonResponse
            //            {
            //                Message = "No Lab Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                Status = "1",
            //                Error = ""
            //            });
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        LabDataUpload_Status("Delete");
            //        Common.InsertErrorLog(ex, null, Request);
            //        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //        sb.Append(ex.Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //        sb.AppendLine("");
            //        File.AppendAllText(path, sb.ToString());
            //        sb.Clear();
            //        return Ok(new CommonResponse
            //        {
            //            Message = ex.Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //            Status = "0",
            //            Error = ex.StackTrace
            //        });
            //    }
            //}
            //else
            //{
            //    if (dt3.Rows[0]["MESSAGE"].ToString() != "")
            //    {
            //        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //        sb.Append(dt3.Rows[0]["MESSAGE"].ToString() + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //        sb.AppendLine("");
            //        File.AppendAllText(path, sb.ToString());
            //        sb.Clear();
            //    }

            //    return Ok(new CommonResponse
            //    {
            //        Message = dt3.Rows[0]["MESSAGE"].ToString() + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //        Status = "1",
            //        Error = ""
            //    });
            //}






            //JObject test1 = JObject.Parse(data.ToString());
            //LabDataUpload_Ora_Req obj = new LabDataUpload_Ora_Req();
            //obj = JsonConvert.DeserializeObject<LabDataUpload_Ora_Req>(((Newtonsoft.Json.Linq.JProperty)test1.Last).Name.ToString());

            //Database_Lab db = new Database_Lab(Request);
            //System.Collections.Generic.List<System.Data.IDbDataParameter> para;
            //para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            //para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, obj.Type.ToUpper()));

            //DataTable dt3 = db.ExecuteSP("LabDataUpload_Status_Get", para.ToArray(), false);

            //string path = HttpContext.Current.Server.MapPath("~/Lab_Upload_From_Oracle_Log.txt");
            //if (!File.Exists(@"" + path + ""))
            //{
            //    File.Create(@"" + path + "").Dispose();
            //}
            //StringBuilder sb = new StringBuilder();

            //if (dt3.Rows[0]["STATUS"].ToString() == "1")
            //{
            //    try
            //    {
            //        string Lab_MAS_SP_NAME = ConfigurationManager.AppSettings["Lab_MAS_SP_NAME"];
            //        string Lab_Detail_SP_NAME = ConfigurationManager.AppSettings["Lab_MANUAL_SP_NAME"];
            //        //string Lab_Detail_SP_NAME = ConfigurationManager.AppSettings["Lab_MACRO_SP_NAME"];




            //        Oracle_DBAccess oracleDbAccess = new Oracle_DBAccess();
            //        List<OracleParameter> paramList = new List<OracleParameter>();
            //        DateTime date = DateTime.Now;
            //        DataTable dt = new DataTable();
            //        DataTable dt2 = new DataTable();
            //        List<SqlParameter> para2 = new List<SqlParameter>();
            //        string TRANS_ID = string.Empty, Message = string.Empty;
            //        int Count = 0;

            //        OracleParameter param11 = new OracleParameter("vrec", OracleDbType.RefCursor);
            //        param11.Direction = ParameterDirection.Output;
            //        paramList.Add(param11);

            //        dt = oracleDbAccess.CallSP(Lab_MAS_SP_NAME, paramList);

            //        Count = 0;
            //        if (dt != null && dt.Rows.Count > 0)
            //        {
            //            Count = dt.Rows.Count;

            //            db = new Database_Lab(Request);
            //            dt2 = new DataTable();
            //            para2 = new List<SqlParameter>();

            //            SqlParameter sqlparam1 = new SqlParameter("table", SqlDbType.Structured);
            //            sqlparam1.Value = dt;
            //            para2.Add(sqlparam1);

            //            dt2 = db.ExecuteSP("LabData_Mas_Ora_Insert", para2.ToArray(), false);

            //            if (dt2 != null && dt2.Rows.Count > 0)
            //            {
            //                if (dt2.Rows[0]["Message"].ToString() == "SUCCESS" && dt2.Rows[0]["Status"].ToString() == "1" && dt2.Rows[0]["TRANS_ID"].ToString() == "")
            //                {
            //                    sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //                    sb.Append("New TRANS ID Not Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //                    sb.AppendLine("");
            //                    File.AppendAllText(path, sb.ToString());
            //                    sb.Clear();
            //                    return Ok(new CommonResponse
            //                    {
            //                        Message = "New TRANS ID Not Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                        Status = "1",
            //                        Error = ""
            //                    });
            //                }
            //                else if (dt2.Rows[0]["Message"].ToString() == "SUCCESS" && dt2.Rows[0]["Status"].ToString() == "1" && dt2.Rows[0]["TRANS_ID"].ToString() != "")
            //                {
            //                    TRANS_ID = dt2.Rows[0]["TRANS_ID"].ToString();

            //                    LabDataUpload_Status("Insert");

            //                    string fromtime = Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt");

            //                    string Last_Time = string.Empty;

            //                    db = new Database_Lab(Request);
            //                    List<IDbDataParameter> para3 = new List<IDbDataParameter>();
            //                    DataTable dt4 = db.ExecuteSP("LabStock_Fetch_Time_Get", para3.ToArray(), false);

            //                    if (dt4 != null && dt4.Rows.Count > 0)
            //                    {
            //                        Last_Time = dt4.Rows[0]["LabStock_Fetching_Data_To_Time"].ToString();
            //                    }
            //                    else
            //                    {
            //                        Last_Time = Common.GetHKTime().ToString("HH:mm");
            //                    }

            //                    string Current_Time = Common.GetHKTime().ToString("HH:mm");

            //                    oracleDbAccess = new Oracle_DBAccess();
            //                    paramList = new List<OracleParameter>();
            //                    date = DateTime.Now;

            //                    OracleParameter param1 = new OracleParameter("p_for_comp", OracleDbType.Int32);
            //                    param1.Value = 1;
            //                    paramList.Add(param1);

            //                    OracleParameter param2 = new OracleParameter("p_trans_id", OracleDbType.NVarchar2);
            //                    param2.Value = TRANS_ID;
            //                    paramList.Add(param2);

            //                    OracleParameter param3 = new OracleParameter("para_from_date", OracleDbType.Date);
            //                    param3.Value = string.Format("{0:dd-MMM-yyyy}", date);
            //                    paramList.Add(param3);

            //                    OracleParameter param4 = new OracleParameter("para_to_date", OracleDbType.Date);
            //                    param4.Value = string.Format("{0:dd-MMM-yyyy}", date);
            //                    paramList.Add(param4);

            //                    OracleParameter param5 = new OracleParameter("vrec", OracleDbType.RefCursor);
            //                    param5.Direction = ParameterDirection.Output;
            //                    paramList.Add(param5);

            //                    OracleParameter param6 = new OracleParameter("p_pre_sold_flag", OracleDbType.NVarchar2);
            //                    param6.Value = "B";
            //                    paramList.Add(param6);

            //                    string _fromtime = Common.GetHKTime().ToString("yyyy-MM-dd HH:mm:ss");

            //                    dt = oracleDbAccess.CallSP(Lab_Detail_SP_NAME, paramList);

            //                    string _totime = Common.GetHKTime().ToString("yyyy-MM-dd HH:mm:ss");

            //                    Count = 0;
            //                    if (dt != null && dt.Rows.Count > 0)
            //                    {
            //                        Count = dt.Rows.Count;

            //                        db = new Database_Lab(Request);
            //                        dt2 = new DataTable();
            //                        para2 = new List<SqlParameter>();

            //                        SqlParameter sqlparam2 = new SqlParameter("table", SqlDbType.Structured);
            //                        sqlparam2.Value = dt;
            //                        para2.Add(sqlparam2);

            //                        dt2 = db.ExecuteSP("LabData_Ora_Insert", para2.ToArray(), false);

            //                        LabDataUpload_Status("Delete");

            //                        Message = string.Empty;
            //                        if (dt2 != null)
            //                        {
            //                            Message = dt2.Rows[0]["Message"].ToString();
            //                        }
            //                        string totime = Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt");

            //                        if (Message == "SUCCESS")
            //                        {
            //                            db = new Database_Lab(Request);
            //                            List<IDbDataParameter> para1 = new List<IDbDataParameter>();

            //                            para1.Add(db.CreateParam("LabStock_Fetching_Data_From_Time", DbType.String, ParameterDirection.Input, Last_Time));
            //                            para1.Add(db.CreateParam("LabStock_Fetching_Data_To_Time", DbType.String, ParameterDirection.Input, Current_Time));
            //                            para1.Add(db.CreateParam("LabStock_Fetching_From", DbType.String, ParameterDirection.Input, _fromtime));
            //                            para1.Add(db.CreateParam("LabStock_Fetching_To", DbType.String, ParameterDirection.Input, _totime));
            //                            para1.Add(db.CreateParam("TotalStock", DbType.String, ParameterDirection.Input, Count));
            //                            para1.Add(db.CreateParam("TRANS_ID", DbType.String, ParameterDirection.Input, TRANS_ID));

            //                            DataTable dt1 = db.ExecuteSP("LabStock_Fetch_Time_Insert", para1.ToArray(), false);

            //                            sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //                            sb.Append(Message + " " + Count + " Lab Data Found" + (TRANS_ID != "" ? " for TRANS ID : " + TRANS_ID : "") + " upload time " + fromtime + " to " + totime + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //                            sb.AppendLine("");
            //                            File.AppendAllText(path, sb.ToString());
            //                            sb.Clear();

            //                            return Ok(new CommonResponse
            //                            {
            //                                Message = Message + " " + Count + " Lab Data Found, upload time " + fromtime + " to " + totime + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                                Status = "1",
            //                                Error = ""
            //                            });
            //                        }
            //                        else
            //                        {

            //                            sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //                            sb.Append("Lab Data upload in issue" + (Message != "" && Message != null ? " " + Message : "") + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //                            sb.AppendLine("");
            //                            File.AppendAllText(path, sb.ToString());
            //                            sb.Clear();
            //                            return Ok(new CommonResponse
            //                            {
            //                                Message = "Lab Data upload in issue" + (Message != "" && Message != null ? " " + Message : "") + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                                Status = "1",
            //                                Error = ""
            //                            });
            //                        }
            //                    }
            //                    else
            //                    {
            //                        LabDataUpload_Status("Delete");

            //                        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //                        sb.Append("No Lab Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //                        sb.AppendLine("");
            //                        File.AppendAllText(path, sb.ToString());
            //                        sb.Clear();
            //                        return Ok(new CommonResponse
            //                        {
            //                            Message = "No Lab Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                            Status = "1",
            //                            Error = ""
            //                        });
            //                    }
            //                }
            //                else
            //                {
            //                    Message = (dt2.Rows[0]["Message"].ToString() != "" && dt2.Rows[0]["Message"].ToString() != null ? dt2.Rows[0]["Message"].ToString() + " " : "");

            //                    sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //                    sb.Append(Message + "Lab Mas Data upload in issue, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //                    sb.AppendLine("");
            //                    File.AppendAllText(path, sb.ToString());
            //                    sb.Clear();
            //                    return Ok(new CommonResponse
            //                    {
            //                        Message = Message + "Lab Mas Data upload in issue, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                        Status = "1",
            //                        Error = ""
            //                    });
            //                }
            //            }
            //            else
            //            {
            //                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //                sb.Append("Lab Mas Data upload in issue, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //                sb.AppendLine("");
            //                File.AppendAllText(path, sb.ToString());
            //                sb.Clear();
            //                return Ok(new CommonResponse
            //                {
            //                    Message = "Lab Mas Data upload in issue, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                    Status = "1",
            //                    Error = ""
            //                });
            //            }
            //        }
            //        else
            //        {
            //            sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //            sb.Append("No Lab Mas Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //            sb.AppendLine("");
            //            File.AppendAllText(path, sb.ToString());
            //            sb.Clear();
            //            return Ok(new CommonResponse
            //            {
            //                Message = "No Lab Mas Data Found, Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //                Status = "1",
            //                Error = ""
            //            });
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        LabDataUpload_Status("Delete");
            //        Common.InsertErrorLog(ex, null, Request);
            //        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //        sb.Append(ex.Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //        sb.AppendLine("");
            //        File.AppendAllText(path, sb.ToString());
            //        sb.Clear();
            //        return Ok(new CommonResponse
            //        {
            //            Message = ex.Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //            Status = "0",
            //            Error = ex.StackTrace
            //        });
            //    }
            //}
            //else
            //{
            //    if (dt3.Rows[0]["MESSAGE"].ToString() != "")
            //    {
            //        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
            //        sb.Append(dt3.Rows[0]["MESSAGE"].ToString() + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
            //        sb.AppendLine("");
            //        File.AppendAllText(path, sb.ToString());
            //        sb.Clear();
            //    }

            //    return Ok(new CommonResponse
            //    {
            //        Message = dt3.Rows[0]["MESSAGE"].ToString() + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
            //        Status = "1",
            //        Error = ""
            //    });
            //}
        }
        public void LabDataUpload_Status(string Type, string DataTransferType, string SP_Type, string TRANS_ID)
        {
            Database_Lab db = new Database_Lab(Request);
            System.Collections.Generic.List<System.Data.IDbDataParameter> para;
            para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, Type));
            para.Add(db.CreateParam("DataTransferType", DbType.String, ParameterDirection.Input, DataTransferType));
            para.Add(db.CreateParam("SP_Type", DbType.String, ParameterDirection.Input, SP_Type));
            para.Add(db.CreateParam("TRANS_ID", DbType.String, ParameterDirection.Input, TRANS_ID));
            DataTable dt = db.ExecuteSP("LabDataUpload_Status", para.ToArray(), false);
        }
        public void LabStock_Upload_History_Insert(string Message)
        {
            Database_Lab db = new Database_Lab(Request);
            System.Collections.Generic.List<System.Data.IDbDataParameter> para;
            para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            para.Add(db.CreateParam("Message", DbType.String, ParameterDirection.Input, Message));
            DataTable dt = db.ExecuteSP("LabStock_Upload_History_Insert", para.ToArray(), false);
        }


        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult LabStockDataDelete()
        {
            string path = HttpContext.Current.Server.MapPath("~/Lab_Stock_Data_Delete_Log.txt");
            if (!File.Exists(@"" + path + ""))
            {
                File.Create(@"" + path + "").Dispose();
            }
            StringBuilder sb = new StringBuilder();
            try
            {
                Database_Lab db = new Database_Lab(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                string fromtime = Common.GetHKTime().ToString("hh:mm:ss tt");
                DataTable dt = db.ExecuteSP("LabStockDataDelete", para.ToArray(), false);
                string totime = Common.GetHKTime().ToString("hh:mm:ss tt");

                string msg = "";
                if (dt != null && dt.Rows[0]["Status"].ToString() == "1" && dt.Rows[0]["Message"].ToString() == "SUCCESS")
                {
                    msg = dt.Rows[0]["Message"].ToString();
                    sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                    sb.Append(dt.Rows[0]["Message"].ToString() + " Lab Data Delete Successfully Execution time " + fromtime + " to " + totime + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                    sb.AppendLine("");
                    File.AppendAllText(path, sb.ToString());
                    sb.Clear();

                    return Ok(new CommonResponse
                    {
                        Message = dt.Rows[0]["Message"].ToString() + " Lab Data Delete Successfully Execution time " + fromtime + " to " + totime + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"),
                        Status = "1",
                        Error = ""
                    });
                }
                else
                {
                    sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                    sb.Append((msg == "" ? "Lab Data Not Delete" : msg) + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                    sb.AppendLine("");
                    File.AppendAllText(path, sb.ToString());
                    sb.Clear();
                    return Ok(new CommonResponse
                    {
                        Message = (msg == "" ? "Lab Data Not Delete" : msg),
                        Status = "1",
                        Error = ""
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                sb.Append(ex.Message + ", Log Time : " + Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                sb.AppendLine("");
                File.AppendAllText(path, sb.ToString());
                sb.Clear();
                return Ok(new CommonResponse
                {
                    Message = ex.Message,
                    Status = "0",
                    Error = ex.StackTrace
                });
            }
        }

        [HttpPost]
        public IHttpActionResult GetListValue([FromBody] JObject data)
        {
            ListValueRequest listValueRequest = new ListValueRequest();
            try
            {
                listValueRequest = JsonConvert.DeserializeObject<ListValueRequest>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<ListValueRequest>
                {
                    Data = new List<ListValueRequest>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }
            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                Database db = new Database(Request);
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                if (listValueRequest.ListValue != null)
                    para.Add(db.CreateParam("ListType", DbType.String, ParameterDirection.Input, listValueRequest.ListValue));
                else
                    para.Add(db.CreateParam("ListType", DbType.String, ParameterDirection.Input, DBNull.Value));

                // Change By hitesh on [21-03-2017] as per Priyanka & Disha
                para.Add(db.CreateParam("iUserId", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(userID)));
                para.Add(db.CreateParam("iEmpId", DbType.Int32, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("GetListValue", para.ToArray(), false);
                List<ListValueResponse> listValueResponses = new List<ListValueResponse>();
                listValueResponses = DataTableExtension.ToList<ListValueResponse>(dt);
                if (listValueResponses.Count > 0)
                {
                    return Ok(new ServiceResponse<ListValueResponse>
                    {
                        Data = listValueResponses,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<ListValueResponse>
                    {
                        Data = listValueResponses,
                        Message = "Something Went wrong.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<ListValueResponse>
                {
                    Data = new List<ListValueResponse>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult GetVendorInfo()
        {
            try
            {
                Database db = new Database(Request);

                Oracle_DBAccess oracleDbAccess = new Oracle_DBAccess();
                List<OracleParameter> paramList = new List<OracleParameter>();

                OracleParameter param1 = new OracleParameter("p_for_comp", OracleDbType.Int32);
                param1.Value = 1;
                paramList.Add(param1);

                OracleParameter param3 = new OracleParameter("vrec", OracleDbType.RefCursor);
                param3.Direction = ParameterDirection.Output;
                paramList.Add(param3);

                System.Data.DataTable dt = oracleDbAccess.CallSP("get_lab_supplier", paramList);

                List<VendorResponse> vendorresponse = new List<VendorResponse>();
                vendorresponse = DataTableExtension.ToList<VendorResponse>(dt);
                if (vendorresponse.Count > 0)
                {
                    return Ok(new ServiceResponse<VendorResponse>
                    {
                        Data = vendorresponse,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<VendorResponse>
                    {
                        Data = vendorresponse,
                        Message = "Something Went wrong.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<VendorResponse>
                {
                    Data = new List<VendorResponse>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult GetKeyToSymbol()
        {
            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                DataTable dt = db.ExecuteSP("get_key_to_symbol", para.ToArray(), false);
                List<KeyToSymbolResponse> keyToSymbolResponses = new List<KeyToSymbolResponse>();
                keyToSymbolResponses = DataTableExtension.ToList<KeyToSymbolResponse>(dt);
                if (keyToSymbolResponses.Count > 0)
                {
                    return Ok(new ServiceResponse<KeyToSymbolResponse>
                    {
                        Data = keyToSymbolResponses,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<KeyToSymbolResponse>
                    {
                        Data = keyToSymbolResponses,
                        Message = "Something Went wrong.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<KeyToSymbolResponse>
                {
                    Data = new List<KeyToSymbolResponse>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult LabStockStatusGet()
        {
            try
            {
                Database_Lab db = new Database_Lab();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();
                DataTable dt = db.ExecuteSP("LabDataUpload_Status_Get_ForManualUpload", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Ok(new CommonResponse
                    {
                        Error = "",
                        Message = dt.Rows[0]["Message"].ToString(),
                        Status = dt.Rows[0]["Status"].ToString()
                    });
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Error = "",
                        Message = "Something Went wrong.\nPlease try again later",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse
                {
                    Error = "",
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult GetUserMas()
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();
                DataTable dt = db.ExecuteSP("UserMas_SelectByPara", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<ColumnsUserResponse> list = new List<ColumnsUserResponse>();
                    list = DataTableExtension.ToList<ColumnsUserResponse>(dt);

                    return Ok(new ServiceResponse<ColumnsUserResponse>
                    {
                        Data = list,
                        Message = "Success",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<ColumnsUserResponse>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ServiceResponse<ColumnsUserResponse>
                {
                    Data = null,
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult UserwiseCompany_select([FromBody]JObject data)
        {
            UserwiseCompany_select userwisecompany_select = new UserwiseCompany_select();

            try
            {
                userwisecompany_select = JsonConvert.DeserializeObject<UserwiseCompany_select>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok();
            }

            try
            {
                Database db = new Database();
                System.Collections.Generic.List<System.Data.IDbDataParameter> para;
                para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

                if (userwisecompany_select.iUserid > 0)
                    para.Add(db.CreateParam("iUserid", DbType.Int64, ParameterDirection.Input, userwisecompany_select.iUserid));
                else
                    para.Add(db.CreateParam("iUserid", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                System.Data.DataTable dtData = db.ExecuteSP("UserwiseCompany_select", para.ToArray(), false);

                List<UserwiseCompany_select> list = new List<UserwiseCompany_select>();
                list = DataTableExtension.ToList<UserwiseCompany_select>(dtData);

                if (list.Count > 0)
                {
                    return Ok(new ServiceResponse<UserwiseCompany_select>
                    {
                        Data = list,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<UserwiseCompany_select>
                    {
                        Data = new List<UserwiseCompany_select>(),
                        Message = "No Record Found",
                        Status = "1"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<UserwiseCompany_select>
                {
                    Data = new List<UserwiseCompany_select>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult GetApiColumnsDetails()
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();
                DataTable dt = db.ExecuteSP("Lab_Column_Select", para.ToArray(), false);

                List<ApiColumns> apiCols = new List<ApiColumns>();
                apiCols = DataTableExtension.ToList<ApiColumns>(dt);
                if (apiCols.Count > 0)
                {
                    return Ok(new ServiceResponse<ApiColumns>
                    {
                        Data = apiCols,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<ApiColumns>
                    {
                        Data = apiCols,
                        Message = "Something Went wrong.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<ApiColumns>
                {
                    Data = new List<ApiColumns>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Lab_Column_Auto_Select([FromBody]JObject data)
        {
            Lab_Column_Auto_Select_Req lab_column_auto_select_req = new Lab_Column_Auto_Select_Req();
            try
            {
                lab_column_auto_select_req = JsonConvert.DeserializeObject<Lab_Column_Auto_Select_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok();
            }
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (!string.IsNullOrEmpty(lab_column_auto_select_req.Type))
                    para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, lab_column_auto_select_req.Type));
                else
                    para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("Lab_Column_Auto_Select", para.ToArray(), false);

                List<ColumnsSettingsModel> apiCols = new List<ColumnsSettingsModel>();
                apiCols = DataTableExtension.ToList<ColumnsSettingsModel>(dt);
                if (apiCols.Count > 0)
                {
                    return Ok(new ServiceResponse<ColumnsSettingsModel>
                    {
                        Data = apiCols,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<ColumnsSettingsModel>
                    {
                        Data = apiCols,
                        Message = "Something Went wrong.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<ApiColumns>
                {
                    Data = new List<ApiColumns>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult SaveLab([FromBody]JObject data)
        {
            SaveLab_Req savelab_req = new SaveLab_Req();
            try
            {
                savelab_req = JsonConvert.DeserializeObject<SaveLab_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CommonResponse>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                var db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();
                para.Add(db.CreateParam("iUserId", DbType.Int64, ParameterDirection.Input, Convert.ToInt64(userID)));
                para.Add(db.CreateParam("iTransId", DbType.Int64, ParameterDirection.Input, savelab_req.iTransId));
                para.Add(db.CreateParam("APIUrl", DbType.String, ParameterDirection.Input, savelab_req.APIUrl));
                para.Add(db.CreateParam("APIStatus", DbType.String, ParameterDirection.Input, savelab_req.APIStatus));
                para.Add(db.CreateParam("APIName", DbType.String, ParameterDirection.Input, savelab_req.APIName));
                para.Add(db.CreateParam("For_iUserId", DbType.Int64, ParameterDirection.Input, savelab_req.For_iUserId));
                para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, savelab_req.UserName));
                para.Add(db.CreateParam("Password", DbType.String, ParameterDirection.Input, savelab_req.Password));
                para.Add(db.CreateParam("ExportType", DbType.String, ParameterDirection.Input, savelab_req.ExportType));
                para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, savelab_req.Type));

                string savelab_req_filters = Common.ToXML<List<SaveLab_Filters>>(savelab_req.Filters);
                para.Add(db.CreateParam("Filters", DbType.String, ParameterDirection.Input, savelab_req_filters));

                string savelab_req_columnssettings = Common.ToXML<List<SaveLab_ColumnsSettings>>(savelab_req.ColumnsSettings);
                para.Add(db.CreateParam("ColumnsSettings", DbType.String, ParameterDirection.Input, savelab_req_columnssettings));

                DataTable dtData = db.ExecuteSP("LabMaster_Save", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    return Ok(new CommonResponse
                    {
                        Error = "",
                        Message = dtData.Rows[0]["Message"].ToString(),
                        Status = dtData.Rows[0]["Status"].ToString()
                    });
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Error = "",
                        Message = "Something Went wrong.\nPlease try again later",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CommonResponse>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult GetLab([FromBody]JObject data)
        {
            GetLab_Request getlab_request = new GetLab_Request();
            try
            {
                getlab_request = JsonConvert.DeserializeObject<GetLab_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<GetLab_Request>
                {
                    Data = new List<GetLab_Request>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }
            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();
                if (!String.IsNullOrEmpty(getlab_request.sTransId))
                    para.Add(db.CreateParam("TransId", DbType.Int64, ParameterDirection.Input, getlab_request.sTransId));
                else
                    para.Add(db.CreateParam("TransId", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(getlab_request.sSearch))
                    para.Add(db.CreateParam("sSearch", DbType.String, ParameterDirection.Input, getlab_request.sSearch));
                else
                    para.Add(db.CreateParam("sSearch", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("UserId", DbType.Int64, ParameterDirection.Input, Convert.ToInt64(userID)));

                if (!string.IsNullOrEmpty(getlab_request.dtFromDate))
                    para.Add(db.CreateParam("dtFromDate", DbType.String, ParameterDirection.Input, getlab_request.dtFromDate));
                else
                    para.Add(db.CreateParam("dtFromDate", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(getlab_request.dtToDate))
                    para.Add(db.CreateParam("dtToDate", DbType.String, ParameterDirection.Input, getlab_request.dtToDate));
                else
                    para.Add(db.CreateParam("dtToDate", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (getlab_request.sPgNo != null)
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, getlab_request.sPgNo));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, DBNull.Value));
                if (getlab_request.sPgSize != null)
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, getlab_request.sPgSize));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(getlab_request.OrderBy))
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, getlab_request.OrderBy));
                else
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("LabMaster_Select", para.ToArray(), false);
                List<GetLab_Response> getlab_response = new List<GetLab_Response>();
                getlab_response = DataTableExtension.ToList<GetLab_Response>(dt);
                if (getlab_response.Count > 0)
                {
                    if (!String.IsNullOrEmpty(getlab_request.sTransId))
                    {
                        List<ColumnsSettingsModel> columnssettings;
                        foreach (var getlab in getlab_response)
                        {
                            db = new Database();
                            para = new List<IDbDataParameter>();
                            para.Add(db.CreateParam("iTransId", DbType.Int64, ParameterDirection.Input, Convert.ToInt64(getlab.iTransId)));
                            dt = db.ExecuteSP("LabMaster_Columns_Select", para.ToArray(), false);
                            if (dt.Rows.Count > 0)
                            {
                                columnssettings = new List<ColumnsSettingsModel>();
                                columnssettings = DataTableExtension.ToList<ColumnsSettingsModel>(dt);
                                getlab.ColumnsSettings = columnssettings;
                            }
                        }

                        List<APIFiltersSettingsModel> filters;
                        foreach (var getlab in getlab_response)
                        {
                            db = new Database();
                            para = new List<IDbDataParameter>();
                            para.Add(db.CreateParam("iTransId", DbType.Int64, ParameterDirection.Input, Convert.ToInt64(getlab.iTransId)));
                            dt = db.ExecuteSP("LabMaster_Filters_Select", para.ToArray(), false);
                            if (dt.Rows.Count > 0)
                            {
                                filters = new List<APIFiltersSettingsModel>();
                                filters = DataTableExtension.ToList<APIFiltersSettingsModel>(dt);
                                getlab.Filters = filters;
                            }
                        }
                    }
                    return Ok(new ServiceResponse<GetLab_Response>
                    {
                        Data = getlab_response,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<GetLab_Response>
                    {
                        Data = new List<GetLab_Response>(),
                        Message = "No Record Found",
                        Status = "1"
                    });
                }
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<GetLab_Response>
                {
                    Data = new List<GetLab_Response>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [NonAction]
        private DataTable Lab_viewAll(int iPgNo, int iPgSize, int TransId = 0)
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (TransId > 0)
                    para.Add(db.CreateParam("TransId", DbType.Int64, ParameterDirection.Input, TransId));
                else
                    para.Add(db.CreateParam("TransId", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (iPgNo > 0)
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, iPgNo));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (iPgSize > 0)
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, iPgSize));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("LabMaster_Select", para.ToArray(), false);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void LabMaster_Download_Log_Ins(int TransId, int UserId, int TotCount, Boolean Status, string Error, string FileName, string IPAddress)
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();
                para.Add(db.CreateParam("UserId", DbType.Int32, ParameterDirection.Input, Convert.ToInt64(UserId)));
                para.Add(db.CreateParam("TransId", DbType.Int32, ParameterDirection.Input, Convert.ToInt64(TransId)));
                para.Add(db.CreateParam("TotCount", DbType.Int32, ParameterDirection.Input, Convert.ToInt64(TotCount)));
                para.Add(db.CreateParam("Status", DbType.Boolean, ParameterDirection.Input, Status));
                para.Add(db.CreateParam("Error", DbType.String, ParameterDirection.Input, Error.ToString()));
                para.Add(db.CreateParam("FileName", DbType.String, ParameterDirection.Input, FileName.ToString()));
                para.Add(db.CreateParam("IPAddress", DbType.String, ParameterDirection.Input, IPAddress.ToString()));
                DataTable dt = db.ExecuteSP("LabMaster_Download_Log_Ins", para.ToArray(), false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult LabDataGetURLApi([FromBody] JObject data)
        {
            JObject test1 = JObject.Parse(data.ToString());
            LabStockDownload_Req labstockdownload_req = new LabStockDownload_Req();
            try
            {
                labstockdownload_req = JsonConvert.DeserializeObject<LabStockDownload_Req>(((Newtonsoft.Json.Linq.JProperty)test1.Last).Name.ToString());

            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new Lab_CommonResponse
                {
                    Message = "",
                    Status = "0",
                    Error = "Input Parameters are not in the proper format"
                });
            }

            try
            {
                Lab_CommonResponse resp = new Lab_CommonResponse();
                DataTable _dt = Lab_viewAll(1, 1000, Convert.ToInt32(labstockdownload_req.TransId));
                if (_dt.Rows.Count > 0)
                {
                    Log_TransId = Convert.ToInt32(_dt.Rows[0]["iTransId"].ToString());
                    Log_UserId = Convert.ToInt32(_dt.Rows[0]["iUserId"].ToString());

                    DataView _dv = new DataView(_dt);
                    _dv.RowFilter = "UserName = '" + labstockdownload_req.Username.Trim() + "' AND Password = '" + labstockdownload_req.Password.Trim() + "'";
                    _dt = _dv.ToTable();
                    int TC = 0, TC1 = 0;
                    if (_dt.Rows.Count > 0)
                    {
                        TC = _dt.Rows.Count;
                    }
                    DataView _dv1 = new DataView(_dt);
                    _dv1.RowFilter = "APIStatus = 'True'";
                    _dt = _dv1.ToTable();
                    if (_dt.Rows.Count > 0)
                    {
                        TC1 = _dt.Rows.Count;
                    }

                    if (TC != 0 && TC1 == 0)
                    {
                        LabMaster_Download_Log_Ins(Log_TransId, Log_UserId, 0, false, "LabDataGetURLApi : 403 : In Active", "", labstockdownload_req.IPAddress);
                        return Ok(new Lab_CommonResponse
                        {
                            Message = "",
                            Status = "0",
                            Error = "403 : In Active"
                        });
                    }
                    if (TC == 0)
                    {
                        LabMaster_Download_Log_Ins(Log_TransId, Log_UserId, 0, false, "LabDataGetURLApi : 401 : Unauthorized Request", "", labstockdownload_req.IPAddress);
                        return Ok(new Lab_CommonResponse
                        {
                            Message = "",
                            Status = "0",
                            Error = "401 : Unauthorized Request"
                        });
                    }

                    if (_dt.Rows[0]["iTransId"].ToString() == "" || _dt.Rows[0]["iUserId"].ToString() == "")
                    {
                        LabMaster_Download_Log_Ins(Log_TransId, Log_UserId, 0, false, "LabDataGetURLApi : 404 : Trans Id / User Id Not Found", "", labstockdownload_req.IPAddress);
                        return Ok(new Lab_CommonResponse
                        {
                            Message = "",
                            Status = "0",
                            Error = "404 : Trans Id / User Id Not Found"
                        });
                    }

                    Database db = new Database();
                    List<IDbDataParameter> para = new List<IDbDataParameter>();

                    //if (!String.IsNullOrEmpty(_dt.Rows[0]["iTransId"].ToString()))
                        para.Add(db.CreateParam("TransId", DbType.Int64, ParameterDirection.Input, _dt.Rows[0]["iTransId"].ToString()));
                    //else
                    //    para.Add(db.CreateParam("TransId", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                    //if (!String.IsNullOrEmpty(_dt.Rows[0]["iUserId"].ToString()))
                        para.Add(db.CreateParam("UserId", DbType.Int64, ParameterDirection.Input, _dt.Rows[0]["iUserId"].ToString()));
                    //else
                    //    para.Add(db.CreateParam("UserId", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                    DataTable dt = db.ExecuteSP("SelectLabMethodData", para.ToArray(), false);
                    string filename = "";

                    if (dt.Rows.Count > 0)
                    {
                        TotCount = dt.Rows.Count;

                        string tempPath = HostingEnvironment.MapPath("~/Temp/Lab_EXPORT/");
                        string _path = ConfigurationManager.AppSettings["data"];
                        _path += "Lab_EXPORT/";
                        DateTime now = DateTime.Now;
                        string DATE = " " + now.Day + "" + now.Month + "" + now.Year + "" + now.Hour + "" + now.Minute + "" + now.Second;
                        string FileForUpload = "";

                        if (!Directory.Exists(tempPath))
                        {
                            Directory.CreateDirectory(tempPath);
                        }
                        if (_dt.Rows[0]["ExportType"].ToString().ToUpper() == "XML")
                        {
                            filename = tempPath + _dt.Rows[0]["APIName"].ToString() + DATE + ".xml";
                            if (File.Exists(filename))
                            {
                                File.Delete(filename);
                            }

                            dt.TableName = "Records";
                            dt.WriteXml(tempPath + _dt.Rows[0]["APIName"].ToString() + DATE + ".xml");
                            FileForUpload = _path + _dt.Rows[0]["APIName"].ToString() + DATE + ".xml";
                        }
                        else if (_dt.Rows[0]["ExportType"].ToString().ToUpper() == "CSV")
                        {
                            filename = tempPath + _dt.Rows[0]["APIName"].ToString() + DATE + ".csv";
                            if (File.Exists(filename))
                            {
                                File.Delete(filename);
                            }

                            StringBuilder sb = new StringBuilder();
                            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                            sb.AppendLine(string.Join(",", columnNames));

                            foreach (DataRow row in dt.Rows)
                            {
                                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString().Replace(",", " "));
                                sb.AppendLine(string.Join(",", fields));
                            }
                            File.WriteAllText(filename, sb.ToString());

                            FileForUpload = _path + _dt.Rows[0]["APIName"].ToString() + DATE + ".csv";
                        }
                        else if (_dt.Rows[0]["ExportType"].ToString().ToUpper() == "EXCEL(.XLSX)" || _dt.Rows[0]["ExportType"].ToString().ToUpper() == "EXCEL(.XLS)")
                        {
                            if (_dt.Rows[0]["ExportType"].ToString().ToUpper() == "EXCEL(.XLSX)")
                            {
                                filename = tempPath + _dt.Rows[0]["APIName"].ToString() + DATE + ".xlsx";
                                FileForUpload = _path + _dt.Rows[0]["APIName"].ToString() + DATE + ".xlsx";
                            }
                            else
                            {
                                filename = tempPath + _dt.Rows[0]["APIName"].ToString() + DATE + ".xls";
                                FileForUpload = _path + _dt.Rows[0]["APIName"].ToString() + DATE + ".xls";
                            }

                            if (File.Exists(filename))
                            {
                                File.Delete(filename);
                            }

                            //XLWorkbook wb = new XLWorkbook();
                            //wb.Worksheets.Add(dt, "Hardik");
                            //wb.SaveAs(filename);


                            //using (ExcelEngine excelEngine = new ExcelEngine())
                            //{
                            //    IApplication application = excelEngine.Excel;
                            //    application.DefaultVersion = ExcelVersion.Excel2016;
                            //    IWorkbook workbook = application.Workbooks.Create(1);
                            //    IWorksheet worksheet = workbook.Worksheets[0];
                            //    worksheet.ImportDataTable(dt, true, 1, 1);
                            //    workbook.SaveAs(filename);
                            //}


                            //FileInfo newFile = new FileInfo(filename);
                            //ExcelPackage pck = new ExcelPackage(newFile);
                            //ExcelWorksheet ws = pck.Workbook.Worksheets.Add(_dt.Rows[0]["APIName"].ToString());
                            //pck.Workbook.Properties.Author = "SUNRISE DIAMOND";
                            //pck.Workbook.Properties.Title = "SUNRISE DIAMOND PVT. LTD.";
                            //if (dt.Rows.Count > 0)
                            //{
                            //    int j = 1, p = 0; ;
                            //    int dtColumnCount = dt.Columns.Count;
                            //    foreach (DataColumn c in dt.Columns)  //loop through the columns. 
                            //    {
                            //        ws.Cells[2, j].Value = c.ColumnName.ToString();
                            //        j++;
                            //    }
                            //    for (int i = 3; i < dt.Rows.Count + 3; i++)
                            //    {
                            //        for (int k = 1; k <= dtColumnCount; k++)
                            //        {
                            //            ws.Cells[i, k].Value = dt.Rows[p][k - 1].ToString();
                            //        }
                            //        p++;
                            //    }
                            //    //for (int i = 0; i < dt.Rows.Count;)
                            //    //{
                            //    //    ws.Cells[i + 1, 1].Value = dt.Rows[i][0].ToString();
                            //    //    ws.Cells[i + 1, 2].Value = dt.Rows[i][1].ToString();
                            //    //    ws.Cells[i + 1, 3].Value = dt.Rows[i][2].ToString();
                            //    //    i++;
                            //    //}
                            //}
                            //pck.Save();


                            FileInfo newFile = new FileInfo(filename);
                            using (ExcelPackage pck = new ExcelPackage(newFile))
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(_dt.Rows[0]["APIName"].ToString());
                                pck.Workbook.Properties.Author = "SUNRISE DIAMOND";
                                pck.Workbook.Properties.Title = "SUNRISE DIAMOND PVT. LTD.";

                                ws.Cells["A2"].LoadFromDataTable(dt, true);

                                ws.View.FreezePanes(3, 1);

                                int rowStart = ws.Dimension.Start.Row;
                                int rowEnd = ws.Dimension.End.Row;
                                int columnEnd = ws.Dimension.End.Column;

                                removingGreenTagWarning(ws, ws.Cells[3, 1, rowEnd, columnEnd].Address);

                                var headerCells = ws.Cells[2, 1, 2, ws.Dimension.Columns];
                                headerCells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSkyBlue);
                                headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                headerCells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                headerCells.Style.Font.Size = 10;
                                headerCells.Style.Font.Bold = true;
                                headerCells.AutoFilter = true;
                                headerCells.AutoFitColumns();

                                headerCells.Style.Border.Left.Style = headerCells.Style.Border.Right.Style
                                        = headerCells.Style.Border.Top.Style = headerCells.Style.Border.Bottom.Style
                                        = ExcelBorderStyle.Medium;

                                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d3d3d3"));

                                ws.Row(1).Height = 40;
                                ws.Row(2).Height = 40;
                                ws.Row(2).Style.WrapText = true;

                                ws.Cells[1, 1, 1, columnEnd].Style.Font.Bold = true;
                                ws.Cells[1, 1, 1, columnEnd].Style.Font.Size = 11;
                                ws.Cells[1, 1, 1, columnEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[1, 1, 1, columnEnd].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[1, 1, 1, columnEnd].Style.Font.Color.SetColor(System.Drawing.Color.Black);

                                ws.Cells[3, 1, rowEnd, columnEnd].Style.Font.Size = 9;
                                ws.Cells[3, 1, rowEnd, columnEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                string ColCaption = "";

                                //-----------Ref No---------------------------------
                                ColCaption = GetColumnUserCaption("Ref No", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int RefNo = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(RefNo);
                                    ws.Cells[2, RefNo].AutoFitColumns(12);

                                    ws.Cells[1, RefNo].Formula = "ROUND(SUBTOTAL(103," + Alpha + (rowStart + 1) + ":" + Alpha + rowEnd + "),2)";
                                    ws.Cells[1, RefNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[1, RefNo].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                    ws.Cells[1, RefNo].Style.Numberformat.Format = "#,##";

                                    ExcelStyle cellStyleHeader_Total = ws.Cells[1, RefNo].Style;
                                    cellStyleHeader_Total.Border.Left.Style = cellStyleHeader_Total.Border.Right.Style
                                            = cellStyleHeader_Total.Border.Top.Style = cellStyleHeader_Total.Border.Bottom.Style
                                            = ExcelBorderStyle.Medium;
                                }
                                //-----------Lab---------------------------------
                                ColCaption = GetColumnUserCaption("Lab", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Lab = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Lab);
                                    ws.Cells[2, Lab].AutoFitColumns(6);
                                }
                                //-----------Certi---------------------------------
                                ColCaption = GetColumnUserCaption("Certi", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Certi = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Certi);
                                    ws.Cells[2, Certi].AutoFitColumns(9);

                                    for (int i = 3; i <= rowEnd; i++)
                                    {
                                        string Certi_Link = Convert.ToString(ws.Cells[i, Certi].Value);
                                        if (Certi_Link != "")
                                        {
                                            ws.Cells[i, Certi].Formula = "=HYPERLINK(\"" + Certi_Link + "\",\" Certi \")";
                                            ws.Cells[i, Certi].Style.Font.UnderLine = true;
                                            ws.Cells[i, Certi].Style.Font.Color.SetColor(Color.Blue);
                                        }
                                    }
                                }
                                //-----------HD Image---------------------------------
                                ColCaption = GetColumnUserCaption("HD Image", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int HDImage = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(HDImage);
                                    ws.Cells[2, HDImage].AutoFitColumns(9);

                                    for (int i = 3; i <= rowEnd; i++)
                                    {
                                        string Image_Link = Convert.ToString(ws.Cells[i, HDImage].Value);
                                        if (Image_Link != "")
                                        {
                                            ws.Cells[i, HDImage].Formula = "=HYPERLINK(\"" + Image_Link + "\",\" Image \")";
                                            ws.Cells[i, HDImage].Style.Font.UnderLine = true;
                                            ws.Cells[i, HDImage].Style.Font.Color.SetColor(Color.Blue);
                                        }
                                    }
                                }
                                //-----------HD Video---------------------------------
                                ColCaption = GetColumnUserCaption("HD Video", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int HDVideo = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(HDVideo);
                                    ws.Cells[2, HDVideo].AutoFitColumns(9);

                                    for (int i = 3; i <= rowEnd; i++)
                                    {
                                        string Movie_Link = Convert.ToString(ws.Cells[i, HDVideo].Value);
                                        if (Movie_Link != "")
                                        {
                                            ws.Cells[i, HDVideo].Formula = "=HYPERLINK(\"" + Movie_Link + "\",\" Movie \")";
                                            ws.Cells[i, HDVideo].Style.Font.UnderLine = true;
                                            ws.Cells[i, HDVideo].Style.Font.Color.SetColor(Color.Blue);
                                        }
                                    }
                                }
                                //-----------Supplier Stock ID---------------------------------
                                ColCaption = GetColumnUserCaption("Supplier Stock ID", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int SupplierStockID = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(SupplierStockID);
                                    ws.Cells[2, SupplierStockID].AutoFitColumns(14);
                                }
                                //-----------Certi No---------------------------------
                                ColCaption = GetColumnUserCaption("Certi No", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int CertiNo = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(CertiNo);
                                    ws.Cells[2, CertiNo].AutoFitColumns(12);
                                }
                                //-----------Supplier---------------------------------
                                ColCaption = GetColumnUserCaption("Supplier", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Supplier = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Supplier);
                                    ws.Cells[2, Supplier].AutoFitColumns(16);
                                }
                                //-----------Shape---------------------------------
                                ColCaption = GetColumnUserCaption("Shape", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Shape = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Shape);
                                    ws.Cells[2, Shape].AutoFitColumns(15);
                                }
                                //-----------Pointer---------------------------------
                                ColCaption = GetColumnUserCaption("Pointer", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Pointer = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Pointer);
                                    ws.Cells[2, Pointer].AutoFitColumns(10);

                                    ws.Cells[3, Pointer, rowEnd, Pointer].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[3, Pointer, rowEnd, Pointer].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c6e0b4"));
                                }
                                //-----------BGM---------------------------------
                                ColCaption = GetColumnUserCaption("BGM", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int BGM = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(BGM);
                                    ws.Cells[2, BGM].AutoFitColumns(10);
                                }
                                //-----------Color---------------------------------
                                ColCaption = GetColumnUserCaption("Color", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Color = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Color);
                                    ws.Cells[2, Color].AutoFitColumns(9);
                                }
                                //-----------Clarity---------------------------------
                                ColCaption = GetColumnUserCaption("Clarity", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Clarity = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Clarity);
                                    ws.Cells[2, Clarity].AutoFitColumns(9);
                                }
                                //-----------Cts---------------------------------
                                ColCaption = GetColumnUserCaption("Cts", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Cts = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Cts);
                                    ws.Cells[2, Cts].AutoFitColumns(12);
                                    //ws.Column(Cts).Style.Numberformat.Format = "0.00";

                                    ws.Cells[1, Cts].Formula = "ROUND(SUBTOTAL(109," + Alpha + (rowStart + 1) + ":" + Alpha + rowEnd + "),2)";
                                    ws.Cells[1, Cts].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[1, Cts].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                    ws.Cells[1, Cts].Style.Numberformat.Format = "#,##0.00";

                                    ExcelStyle cellStyleHeader_Cts = ws.Cells[1, Cts].Style;
                                    cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                            = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                            = ExcelBorderStyle.Medium;
                                    ws.Cells[3, Cts, rowEnd, Cts].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Rap Rate---------------------------------
                                ColCaption = GetColumnUserCaption("Rap Rate", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int RapRate = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(RapRate);
                                    ws.Cells[2, RapRate].AutoFitColumns(11);
                                    ws.Cells[3, RapRate, rowEnd, RapRate].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Rap Value---------------------------------
                                ColCaption = GetColumnUserCaption("Rap Value", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int RapValue = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(RapValue);
                                    ws.Cells[2, RapValue].AutoFitColumns(15);

                                    ws.Cells[1, RapValue].Formula = "ROUND(SUBTOTAL(109," + Alpha + (rowStart + 1) + ":" + Alpha + rowEnd + "),2)";
                                    ws.Cells[1, RapValue].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[1, RapValue].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                    ws.Cells[1, RapValue].Style.Numberformat.Format = "#,##0.00";

                                    ExcelStyle cellStyleHeader_Cts = ws.Cells[1, RapValue].Style;
                                    cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                            = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                            = ExcelBorderStyle.Medium;
                                    ws.Cells[3, RapValue, rowEnd, RapValue].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Supplier Cost Disc(%)---------------------------------
                                ColCaption = GetColumnUserCaption("Supplier Cost Disc(%)", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int SupplierCostDiscPer = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(SupplierCostDiscPer);
                                    ws.Cells[2, SupplierCostDiscPer].AutoFitColumns(12);
                                    //ws.Column(SupplierCostDiscPer).Style.Numberformat.Format = "0.00";

                                    string S_C_V_ColName = GetColumnUserCaption("Supplier Cost Value", Log_TransId);
                                    string R_V_ColName = GetColumnUserCaption("Rap Value", Log_TransId);

                                    if (S_C_V_ColName != "" && R_V_ColName != "")
                                    {
                                        int S_C_V_ColNo = GetColumnByName(ws, S_C_V_ColName);
                                        string S_C_V_Alpha = GetAlphaByColumnNo(S_C_V_ColNo);

                                        int R_V_ColNo = GetColumnByName(ws, R_V_ColName);
                                        string R_V_Alpha = GetAlphaByColumnNo(R_V_ColNo);

                                        ws.Cells[1, SupplierCostDiscPer].Formula = "IF(SUBTOTAL(109, " + R_V_Alpha + (rowStart + 1) + ": " + R_V_Alpha + rowEnd + ")=0,0,((1-(SUBTOTAL(109, " + S_C_V_Alpha + (rowStart + 1) + ":" + S_C_V_Alpha + rowEnd + ")/SUBTOTAL(109, " + R_V_Alpha + (rowStart + 1) + ": " + R_V_Alpha + rowEnd + ")))*100))";
                                        ws.Cells[1, SupplierCostDiscPer].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[1, SupplierCostDiscPer].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                        ws.Cells[1, SupplierCostDiscPer].Style.Numberformat.Format = "#,##0.00";

                                        ExcelStyle cellStyleHeader_Cts = ws.Cells[1, SupplierCostDiscPer].Style;
                                        cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                                = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                                = ExcelBorderStyle.Medium;
                                    }

                                    ws.Cells[3, SupplierCostDiscPer, rowEnd, SupplierCostDiscPer].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[3, SupplierCostDiscPer, rowEnd, SupplierCostDiscPer].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ff99cc"));
                                    //ws.Cells[3, SupplierCostDiscPer, rowEnd, SupplierCostDiscPer].Style.Font.Bold = true;
                                    ws.Cells[3, SupplierCostDiscPer, rowEnd, SupplierCostDiscPer].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Supplier Cost Value---------------------------------
                                ColCaption = GetColumnUserCaption("Supplier Cost Value", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int SupplierCostValue = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(SupplierCostValue);
                                    ws.Cells[2, SupplierCostValue].AutoFitColumns(15);
                                    //ws.Column(SupplierCostValue).Style.Numberformat.Format = "0.00";

                                    ws.Cells[1, SupplierCostValue].Formula = "ROUND(SUBTOTAL(109," + Alpha + (rowStart + 1) + ":" + Alpha + rowEnd + "),2)";
                                    ws.Cells[1, SupplierCostValue].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[1, SupplierCostValue].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                    ws.Cells[1, SupplierCostValue].Style.Numberformat.Format = "#,##0.00";

                                    ExcelStyle cellStyleHeader_Cts = ws.Cells[1, SupplierCostValue].Style;
                                    cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                            = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                            = ExcelBorderStyle.Medium;

                                    ws.Cells[3, SupplierCostValue, rowEnd, SupplierCostValue].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[3, SupplierCostValue, rowEnd, SupplierCostValue].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ff99cc"));
                                    //ws.Cells[3, SupplierCostValue, rowEnd, SupplierCostValue].Style.Font.Bold = true;
                                    ws.Cells[3, SupplierCostValue, rowEnd, SupplierCostValue].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Sunrise Disc(%)---------------------------------
                                ColCaption = GetColumnUserCaption("Sunrise Disc(%)", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int SunriseDiscPer = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(SunriseDiscPer);
                                    ws.Cells[2, SunriseDiscPer].AutoFitColumns(12);
                                    //ws.Column(SunriseDiscPer).Style.Numberformat.Format = "0.00";

                                    string S_V_US_D_ColName = GetColumnUserCaption("Sunrise Value US($)", Log_TransId);
                                    string R_V_ColName = GetColumnUserCaption("Rap Value", Log_TransId);

                                    if (S_V_US_D_ColName != "" && R_V_ColName != "")
                                    {
                                        int S_V_US_D_ColNo = GetColumnByName(ws, S_V_US_D_ColName);
                                        string S_V_US_D_Alpha = GetAlphaByColumnNo(S_V_US_D_ColNo);

                                        int R_V_ColNo = GetColumnByName(ws, R_V_ColName);
                                        string R_V_Alpha = GetAlphaByColumnNo(R_V_ColNo);

                                        ws.Cells[1, SunriseDiscPer].Formula = "IF(SUBTOTAL(109, " + R_V_Alpha + (rowStart + 1) + ": " + R_V_Alpha + rowEnd + ")=0,0,((1-(SUBTOTAL(109, " + S_V_US_D_Alpha + (rowStart + 1) + ": " + S_V_US_D_Alpha + rowEnd + ")/SUBTOTAL(109, " + R_V_Alpha + (rowStart + 1) + ": " + R_V_Alpha + rowEnd + ")))*100))";
                                        ws.Cells[1, SunriseDiscPer].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[1, SunriseDiscPer].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                        ws.Cells[1, SunriseDiscPer].Style.Numberformat.Format = "#,##0.00";

                                        ExcelStyle cellStyleHeader_Cts = ws.Cells[1, SunriseDiscPer].Style;
                                        cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                                = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                                = ExcelBorderStyle.Medium;
                                    }

                                    ws.Cells[3, SunriseDiscPer, rowEnd, SunriseDiscPer].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[3, SunriseDiscPer, rowEnd, SunriseDiscPer].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ccffff"));
                                    //ws.Cells[3, SunriseDiscPer, rowEnd, SunriseDiscPer].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                    //ws.Cells[3, SunriseDiscPer, rowEnd, SunriseDiscPer].Style.Font.Bold = true;
                                    ws.Cells[3, SunriseDiscPer, rowEnd, SunriseDiscPer].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Sunrise Value US($)---------------------------------
                                ColCaption = GetColumnUserCaption("Sunrise Value US($)", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int SunriseValueUSDollar = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(SunriseValueUSDollar);
                                    ws.Cells[2, SunriseValueUSDollar].AutoFitColumns(15);
                                    //ws.Column(SunriseValueUSDollar).Style.Numberformat.Format = "0.00";

                                    ws.Cells[1, SunriseValueUSDollar].Formula = "ROUND(SUBTOTAL(109," + Alpha + (rowStart + 1) + ":" + Alpha + rowEnd + "),2)";
                                    ws.Cells[1, SunriseValueUSDollar].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[1, SunriseValueUSDollar].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                    ws.Cells[1, SunriseValueUSDollar].Style.Numberformat.Format = "#,##0.00";

                                    ExcelStyle cellStyleHeader_Cts = ws.Cells[1, SunriseValueUSDollar].Style;
                                    cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                            = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                            = ExcelBorderStyle.Medium;

                                    ws.Cells[3, SunriseValueUSDollar, rowEnd, SunriseValueUSDollar].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[3, SunriseValueUSDollar, rowEnd, SunriseValueUSDollar].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ccffff"));
                                    //ws.Cells[3, SunriseValueUSDollar, rowEnd, SunriseValueUSDollar].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                    //ws.Cells[3, SunriseValueUSDollar, rowEnd, SunriseValueUSDollar].Style.Font.Bold = true;
                                    ws.Cells[3, SunriseValueUSDollar, rowEnd, SunriseValueUSDollar].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Supp Base Offer(%)---------------------------------
                                ColCaption = GetColumnUserCaption("Supp Base Offer(%)", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int SuppBaseOfferPer = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(SuppBaseOfferPer);
                                    ws.Cells[2, SuppBaseOfferPer].AutoFitColumns(12);
                                    //ws.Column(SuppBaseOfferPer).Style.Numberformat.Format = "0.00";

                                    string S_B_O_V_ColName = GetColumnUserCaption("Supp Base Offer Value", Log_TransId);
                                    string R_V_ColName = GetColumnUserCaption("Rap Value", Log_TransId);

                                    if (S_B_O_V_ColName != "" && R_V_ColName != "")
                                    {
                                        int S_B_O_V_ColNo = GetColumnByName(ws, S_B_O_V_ColName);
                                        string S_B_O_V_Alpha = GetAlphaByColumnNo(S_B_O_V_ColNo);

                                        int R_V_ColNo = GetColumnByName(ws, R_V_ColName);
                                        string R_V_Alpha = GetAlphaByColumnNo(R_V_ColNo);

                                        ws.Cells[1, SuppBaseOfferPer].Formula = "IF(SUBTOTAL(109, " + R_V_Alpha + (rowStart + 1) + ": " + R_V_Alpha + rowEnd + ")=0,0,((1-(SUBTOTAL(109, " + S_B_O_V_Alpha + (rowStart + 1) + ":" + S_B_O_V_Alpha + rowEnd + ")/SUBTOTAL(109, " + R_V_Alpha + (rowStart + 1) + ": " + R_V_Alpha + rowEnd + ")))*100))";
                                        ws.Cells[1, SuppBaseOfferPer].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[1, SuppBaseOfferPer].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                        ws.Cells[1, SuppBaseOfferPer].Style.Numberformat.Format = "#,##0.00";

                                        ExcelStyle cellStyleHeader_Cts = ws.Cells[1, SuppBaseOfferPer].Style;
                                        cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                                = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                                = ExcelBorderStyle.Medium;

                                    }
                                    ws.Cells[3, SuppBaseOfferPer, rowEnd, SuppBaseOfferPer].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Supp Base Offer Value---------------------------------
                                ColCaption = GetColumnUserCaption("Supp Base Offer Value", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int SuppBaseOfferValue = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(SuppBaseOfferValue);
                                    ws.Cells[2, SuppBaseOfferValue].AutoFitColumns(15);
                                    //ws.Column(SuppBaseOfferValue).Style.Numberformat.Format = "0.00";

                                    ws.Cells[1, SuppBaseOfferValue].Formula = "ROUND(SUBTOTAL(109," + Alpha + (rowStart + 1) + ":" + Alpha + rowEnd + "),2)";
                                    ws.Cells[1, SuppBaseOfferValue].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[1, SuppBaseOfferValue].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                    ws.Cells[1, SuppBaseOfferValue].Style.Numberformat.Format = "#,##0.00";

                                    ExcelStyle cellStyleHeader_Cts = ws.Cells[1, SuppBaseOfferValue].Style;
                                    cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                            = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                            = ExcelBorderStyle.Medium;
                                    ws.Cells[3, SuppBaseOfferValue, rowEnd, SuppBaseOfferValue].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Max Slab Disc(%)---------------------------------
                                ColCaption = GetColumnUserCaption("Max Slab Disc(%)", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int MaxSlabDiscPer = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(MaxSlabDiscPer);
                                    ws.Cells[2, MaxSlabDiscPer].AutoFitColumns(15);
                                    //ws.Column(MaxSlabDiscPer).Style.Numberformat.Format = "0.00";

                                    ws.Cells[1, MaxSlabDiscPer].Formula = "ROUND(SUBTOTAL(109," + Alpha + (rowStart + 1) + ":" + Alpha + rowEnd + "),2)";
                                    ws.Cells[1, MaxSlabDiscPer].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[1, MaxSlabDiscPer].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                    ws.Cells[1, MaxSlabDiscPer].Style.Numberformat.Format = "#,##0.00";

                                    ExcelStyle cellStyleHeader_Cts = ws.Cells[1, MaxSlabDiscPer].Style;
                                    cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                            = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                            = ExcelBorderStyle.Medium;
                                    ws.Cells[3, MaxSlabDiscPer, rowEnd, MaxSlabDiscPer].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Max Slab Value---------------------------------
                                ColCaption = GetColumnUserCaption("Max Slab Value", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int MaxSlabValue = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(MaxSlabValue);
                                    ws.Cells[2, MaxSlabValue].AutoFitColumns(15);
                                    //ws.Column(MaxSlabValue).Style.Numberformat.Format = "0.00";

                                    ws.Cells[1, MaxSlabValue].Formula = "ROUND(SUBTOTAL(109," + Alpha + (rowStart + 1) + ":" + Alpha + rowEnd + "),2)";
                                    ws.Cells[1, MaxSlabValue].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[1, MaxSlabValue].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d9e1f2"));
                                    ws.Cells[1, MaxSlabValue].Style.Numberformat.Format = "#,##0.00";

                                    ExcelStyle cellStyleHeader_Cts = ws.Cells[1, MaxSlabValue].Style;
                                    cellStyleHeader_Cts.Border.Left.Style = cellStyleHeader_Cts.Border.Right.Style
                                            = cellStyleHeader_Cts.Border.Top.Style = cellStyleHeader_Cts.Border.Bottom.Style
                                            = ExcelBorderStyle.Medium;
                                    ws.Cells[3, MaxSlabValue, rowEnd, MaxSlabValue].Style.Numberformat.Format = "#,##0.00";
                                }
                                //-----------Cut---------------------------------
                                ColCaption = GetColumnUserCaption("Cut", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Cut = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Cut);
                                    ws.Cells[2, Cut].AutoFitColumns(6);

                                    string _polish = GetColumnUserCaption("Polish", Log_TransId);
                                    string _symm = GetColumnUserCaption("Symm", Log_TransId);

                                    for (int i = 3; i <= rowEnd; i++)
                                    {
                                        if (Convert.ToString(ws.Cells[i, Cut].Value) == "3EX")
                                        {
                                            ws.Cells[i, Cut].Style.Font.Bold = true;
                                            if (_polish != "")
                                            {
                                                int __polish = GetColumnByName(ws, _polish);
                                                ws.Cells[i, __polish].Style.Font.Bold = true;
                                            }
                                            if (_symm != "")
                                            {
                                                int __symm = GetColumnByName(ws, _symm);
                                                ws.Cells[i, __symm].Style.Font.Bold = true;
                                            }
                                        }
                                    }
                                }
                                //-----------Polish---------------------------------
                                ColCaption = GetColumnUserCaption("Polish", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Polish = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Polish);
                                    ws.Cells[2, Polish].AutoFitColumns(7);
                                }
                                //-----------Symm---------------------------------
                                ColCaption = GetColumnUserCaption("Symm", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Symm = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Symm);
                                    ws.Cells[2, Symm].AutoFitColumns(7);
                                }
                                //-----------Fls---------------------------------
                                ColCaption = GetColumnUserCaption("Fls", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Fls = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Fls);
                                    ws.Cells[2, Fls].AutoFitColumns(7);
                                }
                                //-----------Length---------------------------------
                                ColCaption = GetColumnUserCaption("Length", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Length = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Length);
                                    ws.Cells[2, Length].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Width---------------------------------
                                ColCaption = GetColumnUserCaption("Width", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Width = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Width);
                                    ws.Cells[2, Width].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Depth---------------------------------
                                ColCaption = GetColumnUserCaption("Depth", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Depth = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Depth);
                                    ws.Cells[2, Depth].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Depth(%)---------------------------------
                                ColCaption = GetColumnUserCaption("Depth(%)", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int DepthPer = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(DepthPer);
                                    ws.Cells[2, DepthPer].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Table(%)---------------------------------
                                ColCaption = GetColumnUserCaption("Table(%)", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int TablePer = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(TablePer);
                                    ws.Cells[2, TablePer].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Key to Symbol---------------------------------
                                ColCaption = GetColumnUserCaption("Key to Symbol", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int KeytoSymbol = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(KeytoSymbol);
                                    ws.Cells[2, KeytoSymbol].AutoFitColumns(35);
                                }
                                //-----------Gia Lab Comment---------------------------------
                                ColCaption = GetColumnUserCaption("Gia Lab Comment", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int GiaLabComment = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(GiaLabComment);
                                    ws.Cells[2, GiaLabComment].AutoFitColumns(35);
                                }
                                //-----------Girdle(%)---------------------------------
                                ColCaption = GetColumnUserCaption("Girdle(%)", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int GirdlePer = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(GirdlePer);
                                    ws.Cells[2, GirdlePer].AutoFitColumns(8);
                                }
                                //-----------Crown Angle---------------------------------
                                ColCaption = GetColumnUserCaption("Crown Angle", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int CrownAngle = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(CrownAngle);
                                    ws.Cells[2, CrownAngle].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Crown Height---------------------------------
                                ColCaption = GetColumnUserCaption("Crown Height", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int CrownHeight = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(CrownHeight);
                                    ws.Cells[2, CrownHeight].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Pav Angle---------------------------------
                                ColCaption = GetColumnUserCaption("Pav Angle", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int PavAngle = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(PavAngle);
                                    ws.Cells[2, PavAngle].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Pav Height---------------------------------
                                ColCaption = GetColumnUserCaption("Pav Height", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int PavHeight = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(PavHeight);
                                    ws.Cells[2, PavHeight].AutoFitColumns(9);
                                    ws.Column(GetColumnByName(ws, ColCaption)).Style.Numberformat.Format = "0.00";
                                }
                                //-----------Table Natts---------------------------------
                                ColCaption = GetColumnUserCaption("Table Natts", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int TableNatts = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(TableNatts);
                                    ws.Cells[2, TableNatts].AutoFitColumns(9);
                                }
                                //-----------Crown Natts---------------------------------
                                ColCaption = GetColumnUserCaption("Crown Natts", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int CrownNatts = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(CrownNatts);
                                    ws.Cells[2, CrownNatts].AutoFitColumns(9);
                                }
                                //-----------Table Inclusion---------------------------------
                                ColCaption = GetColumnUserCaption("Table Inclusion", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int TableInclusion = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(TableInclusion);
                                    ws.Cells[2, TableInclusion].AutoFitColumns(9);
                                }
                                //-----------Crown Inclusion---------------------------------
                                ColCaption = GetColumnUserCaption("Crown Inclusion", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int CrownInclusion = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(CrownInclusion);
                                    ws.Cells[2, CrownInclusion].AutoFitColumns(9);
                                }
                                //-----------Culet---------------------------------
                                ColCaption = GetColumnUserCaption("Culet", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int Culet = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(Culet);
                                    ws.Cells[2, Culet].AutoFitColumns(9);
                                }
                                //-----------Table Open---------------------------------
                                ColCaption = GetColumnUserCaption("Table Open", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int TableOpen = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(TableOpen);
                                    ws.Cells[2, TableOpen].AutoFitColumns(9);
                                }
                                //-----------Girdle Open---------------------------------
                                ColCaption = GetColumnUserCaption("Girdle Open", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int GirdleOpen = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(GirdleOpen);
                                    ws.Cells[2, GirdleOpen].AutoFitColumns(9);
                                }
                                //-----------Crown Open---------------------------------
                                ColCaption = GetColumnUserCaption("Crown Open", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int CrownOpen = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(CrownOpen);
                                    ws.Cells[2, CrownOpen].AutoFitColumns(9);
                                }
                                //-----------Pavilion Open---------------------------------
                                ColCaption = GetColumnUserCaption("Pavilion Open", Log_TransId);
                                if (ColCaption != "")
                                {
                                    int PavilionOpen = GetColumnByName(ws, ColCaption);
                                    string Alpha = GetAlphaByColumnNo(PavilionOpen);
                                    ws.Cells[2, PavilionOpen].AutoFitColumns(9);
                                }
                                pck.Save();
                            }
                        }
                        else if (_dt.Rows[0]["ExportType"].ToString().ToUpper() == "JSON" || _dt.Rows[0]["ExportType"].ToString().ToUpper() == "JSON(TEXT)")
                        {
                            filename = tempPath + _dt.Rows[0]["APIName"].ToString() + DATE + ".json";
                            if (File.Exists(filename))
                            {
                                File.Delete(filename);
                            }
                            string json = Common.DataTableToJSONWithStringBuilder(dt);
                            File.WriteAllText(tempPath + _dt.Rows[0]["APIName"].ToString() + DATE + ".json", json);
                            FileForUpload = _path + _dt.Rows[0]["APIName"].ToString() + DATE + ".json";
                        }

                        LabMaster_Download_Log_Ins(Convert.ToInt32(_dt.Rows[0]["iTransId"].ToString()), Convert.ToInt32(_dt.Rows[0]["iUserId"].ToString()), TotCount, true, "Success", FileForUpload, labstockdownload_req.IPAddress);
                        return Ok(new Lab_CommonResponse
                        {
                            ExportType = _dt.Rows[0]["ExportType"].ToString().ToUpper(),
                            Message = FileForUpload,
                            Status = "1",
                            Error = ""
                        });
                    }
                    else
                    {
                        LabMaster_Download_Log_Ins(Convert.ToInt32(_dt.Rows[0]["iTransId"].ToString()), Convert.ToInt32(_dt.Rows[0]["iUserId"].ToString()), TotCount, false, "LabDataGetURLApi : 404 Record Not Found", "", labstockdownload_req.IPAddress);
                        return Ok(new Lab_CommonResponse
                        {
                            Message = "",
                            Status = "0",
                            Error = "404 : Record Not Found"
                        });
                    }
                }
                else
                {
                    //LabMaster_Download_Log_Ins(Log_TransId, Log_UserId, TotCount, false, "LabDataGetURLApi : 400 Bad Request", "");
                    return Ok(new Lab_CommonResponse
                    {
                        Message = "",
                        Status = "0",
                        Error = "400 : Bad Request"
                    });
                }

            }
            catch (Exception ex)
            {
                LabMaster_Download_Log_Ins(Log_TransId, Log_UserId, TotCount, false, "LabDataGetURLApi : " + ex.Message.ToString(), "", labstockdownload_req.IPAddress);
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new Lab_CommonResponse
                {
                    Message = "",
                    Status = "0",
                    Error = ex.Message
                });
            }
        }
        [NonAction]
        private static string GetColumnUserCaption(string sColCaption, int iTransId)
        {
            string sUserCaption = "";
            Database db = new Database();
            DataTable dt = new DataTable();
            List<IDbDataParameter> para = new List<IDbDataParameter>();
            para.Add(db.CreateParam("iTransId", DbType.Int64, ParameterDirection.Input, Convert.ToInt64(iTransId)));
            dt = db.ExecuteSP("LabMaster_Columns_Select", para.ToArray(), false);
            if (dt.Rows.Count > 0)
            {
                List<ColumnsSettingsModel> columnsSettings = new List<ColumnsSettingsModel>();
                columnsSettings = DataTableExtension.ToList<ColumnsSettingsModel>(dt);
                var Ucaption = columnsSettings.Where(cp => cp.sUser_ColumnName.Equals(sColCaption)).SingleOrDefault();
                if (Ucaption != null)
                {
                    sUserCaption = Ucaption.sCustMiseCaption;
                }
                return sUserCaption.ToString().Trim();
            }
            return string.Empty;
        }
        public int GetColumnByName(ExcelWorksheet ws, string columnName)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            return ws.Cells["2:2"].First(c => c.Value.ToString() == columnName).Start.Column;
        }
        static String reverse(String input)
        {
            char[] reversedString = input.ToCharArray();
            Array.Reverse(reversedString);
            return new String(reversedString);
        }
        public string GetAlphaByColumnNo(int columnNumber)
        {
            // To store result (Excel column name)
            String columnName = "";
            while (columnNumber > 0)
            {
                // Find remainder
                int rem = columnNumber % 26;
                // If remainder is 0, then a
                // 'Z' must be there in output
                if (rem == 0)
                {
                    columnName += "Z";
                    columnNumber = (columnNumber / 26) - 1;
                }
                // If remainder is non-zero
                else
                {
                    columnName += (char)((rem - 1) + 'A');
                    columnNumber = columnNumber / 26;
                }
            }
            // Reverse the string
            columnName = reverse(columnName);
            return columnName;
        }
        private void removingGreenTagWarning(ExcelWorksheet template1, string address)
        {
            var xdoc = template1.WorksheetXml;
            //Create the import nodes (note the plural vs singular
            var ignoredErrors = xdoc.CreateNode(System.Xml.XmlNodeType.Element, "ignoredErrors", xdoc.DocumentElement.NamespaceURI);
            var ignoredError = xdoc.CreateNode(System.Xml.XmlNodeType.Element, "ignoredError", xdoc.DocumentElement.NamespaceURI);
            ignoredErrors.AppendChild(ignoredError);

            //Attributes for the INNER node
            var sqrefAtt = xdoc.CreateAttribute("sqref");
            sqrefAtt.Value = address;// Or whatever range is needed....

            var flagAtt = xdoc.CreateAttribute("numberStoredAsText");
            flagAtt.Value = "1";

            ignoredError.Attributes.Append(sqrefAtt);
            ignoredError.Attributes.Append(flagAtt);

            //Now put the OUTER node into the worksheet xml
            xdoc.LastChild.AppendChild(ignoredErrors);
        }
        [HttpPost]
        public IHttpActionResult LoginCheck()
        {
            try
            {
                return Ok(new CommonResponse
                {
                    Message = "OK",
                    Status = "1",
                    Error = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse
                {
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0",
                    Error = ex.StackTrace
                });
            }
        }
        [HttpPost]
        public IHttpActionResult hardik()
        {
            try
            {
                string sourcePath = "D:\\Project\\Hardik_Files\\----------------------------------WORK---------\\11-09-2021";
                string targetPath = "D:\\Project\\Hardik_Files\\----------------------------------WORK---------\\09-10-2021";

                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                }

                return Ok(new CommonResponse
                {
                    Message = "SUCCESS",
                    Status = "1",
                    Error = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse
                {
                    Message = ex.Message,
                    Status = "0",
                    Error = ""
                });
            }
        }
    }
}
