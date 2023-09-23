using Lib.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Master")]
    public class MasterController : ApiController
    {
        [HttpPost]
        public IHttpActionResult CustomerMast_Get_MemberId_Wise([FromBody]JObject data)
        {
            MemberIdSearch_Req req = new MemberIdSearch_Req();
            try
            {
                req = JsonConvert.DeserializeObject<MemberIdSearch_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberIdSearch_Res>
                {
                    Data = new List<MemberIdSearch_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (!string.IsNullOrEmpty(req.MemberId))
                    para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, req.MemberId));
                else
                    para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, DBNull.Value)); 
                
                if (req.Customer_Id > 0)
                    para.Add(db.CreateParam("Customer_Id", DbType.Int64, ParameterDirection.Input, req.Customer_Id));
                else
                    para.Add(db.CreateParam("Customer_Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                DataTable dtData = db.ExecuteSP("CustomerMast_Get_MemberId_Wise", para.ToArray(), false);

                List<MemberIdSearch_Res> list = new List<MemberIdSearch_Res>();
                list = DataTableExtension.ToList<MemberIdSearch_Res>(dtData);

                return Ok(new ServiceResponse<MemberIdSearch_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ServiceResponse<MemberIdSearch_Res>
                {
                    Data = new List<MemberIdSearch_Res>(),
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult MemberWise_SchemeDet_Get([FromBody]JObject data)
        {
            MemberIdSearch_Req req = new MemberIdSearch_Req();
            try
            {
                req = JsonConvert.DeserializeObject<MemberIdSearch_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Get_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Get_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                para.Add(db.CreateParam("User_Id", DbType.Int64, ParameterDirection.Input, userID)); 
                
                if (!string.IsNullOrEmpty(req.MemberId))
                    para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, req.MemberId));
                else
                    para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (req.Customer_Id > 0)
                    para.Add(db.CreateParam("Customer_Id", DbType.Int64, ParameterDirection.Input, req.Customer_Id));
                else
                    para.Add(db.CreateParam("Customer_Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                DataTable dtData = db.ExecuteSP("MemberWise_SchemeDet_Get", para.ToArray(), false);

                List<MemberWise_SchemeDet_Get_Res> list = new List<MemberWise_SchemeDet_Get_Res>();
                list = DataTableExtension.ToList<MemberWise_SchemeDet_Get_Res>(dtData);

                return Ok(new ServiceResponse<MemberWise_SchemeDet_Get_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Get_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Get_Res>(),
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult MemberWise_SchemeDet_Save([FromBody]JObject data)
        {
            MemberWise_SchemeDet_Save_Req req = new MemberWise_SchemeDet_Save_Req();
            try
            {
                req = JsonConvert.DeserializeObject<MemberWise_SchemeDet_Save_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Message = "Input Parameters are not in the proper format",
                    Status = "0",
                    Error = ""
                });
            }

            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                
                DataTable dt = new DataTable();
                dt.Columns.Add("CustomerId", typeof(string));
                dt.Columns.Add("MemberId", typeof(string));
                dt.Columns.Add("SchemeId", typeof(string));
                dt.Columns.Add("Pkg", typeof(string));
                dt.Columns.Add("EntryBy", typeof(string));

                if (req.schemesave.Count() > 0)
                {
                    for (int i = 0; i < req.schemesave.Count(); i++)
                    {
                        DataRow dr = dt.NewRow();

                        dr["CustomerId"] = req.schemesave[i].CustomerId.ToString();
                        dr["MemberId"] = req.schemesave[i].MemberId.ToString();
                        dr["SchemeId"] = req.schemesave[i].SchemeId.ToString();
                        dr["Pkg"] = req.schemesave[i].Pkg;
                        dr["EntryBy"] = userID.ToString();

                        dt.Rows.Add(dr);
                    }
                }

                Database db = new Database();
                DataTable dtData = new DataTable();
                List<SqlParameter> para = new List<SqlParameter>();

                SqlParameter param = new SqlParameter("tableCol", SqlDbType.Structured);
                param.Value = dt;
                para.Add(param);

                dtData = db.ExecuteSP("MemberWise_SchemeDet_Save", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    return Ok(new CommonResponse
                    {
                        Message = dtData.Rows[0]["Message"].ToString(),
                        Status = dtData.Rows[0]["Status"].ToString(),
                        Error = ""
                    }); 
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Message = "Something Went wrong.\nPlease try again later",
                        Status = "0",
                        Error = ""
                    }); 
                }
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
        [HttpPost]
        public IHttpActionResult MemberWise_SchemeDet_Select([FromBody]JObject data)
        {
            MemberWise_SchemeDet_Select_Req req = new MemberWise_SchemeDet_Select_Req();
            try
            {
                req = JsonConvert.DeserializeObject<MemberWise_SchemeDet_Select_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Select_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Select_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (req.sPgNo != null)
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, req.sPgNo));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, DBNull.Value));
                
                if (req.sPgSize != null)
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, req.sPgSize));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.OrderBy))
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, req.OrderBy));
                else
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("MemberWise_SchemeDet_Select", para.ToArray(), false);

                List<MemberWise_SchemeDet_Select_Res> list = new List<MemberWise_SchemeDet_Select_Res>();
                list = DataTableExtension.ToList<MemberWise_SchemeDet_Select_Res>(dt);

                return Ok(new ServiceResponse<MemberWise_SchemeDet_Select_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Select_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Select_Res>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult MemberWise_SchemeAlloc_Get([FromBody]JObject data)
        {
            MemberIdSearch_Req req = new MemberIdSearch_Req();
            try
            {
                req = JsonConvert.DeserializeObject<MemberIdSearch_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Get_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Get_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                para.Add(db.CreateParam("User_Id", DbType.Int64, ParameterDirection.Input, userID));

                if (!string.IsNullOrEmpty(req.MemberId))
                    para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, req.MemberId));
                else
                    para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (req.Customer_Id > 0)
                    para.Add(db.CreateParam("Customer_Id", DbType.Int64, ParameterDirection.Input, req.Customer_Id));
                else
                    para.Add(db.CreateParam("Customer_Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                DataTable dtData = db.ExecuteSP("MemberWise_SchemeAlloc_Get", para.ToArray(), false);

                List<MemberWise_SchemeDet_Get_Res> list = new List<MemberWise_SchemeDet_Get_Res>();
                list = DataTableExtension.ToList<MemberWise_SchemeDet_Get_Res>(dtData);

                return Ok(new ServiceResponse<MemberWise_SchemeDet_Get_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Get_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Get_Res>(),
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult MemberWise_SchemeAllocation_Save([FromBody]JObject data)
        {
            MemberWise_SchemeDet_Save_Req req = new MemberWise_SchemeDet_Save_Req();
            try
            {
                req = JsonConvert.DeserializeObject<MemberWise_SchemeDet_Save_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Message = "Input Parameters are not in the proper format",
                    Status = "0",
                    Error = ""
                });
            }

            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);

                DataTable dt = new DataTable();
                dt.Columns.Add("CustomerId", typeof(string));
                dt.Columns.Add("MemberId", typeof(string));
                dt.Columns.Add("SchemeId", typeof(string));
                dt.Columns.Add("Pkg", typeof(string));
                dt.Columns.Add("EntryBy", typeof(string));

                if (req.schemesave.Count() > 0)
                {
                    for (int i = 0; i < req.schemesave.Count(); i++)
                    {
                        DataRow dr = dt.NewRow();

                        dr["CustomerId"] = req.schemesave[i].CustomerId.ToString();
                        dr["MemberId"] = req.schemesave[i].MemberId.ToString();
                        dr["SchemeId"] = req.schemesave[i].SchemeId.ToString();
                        dr["Pkg"] = req.schemesave[i].Pkg;
                        dr["EntryBy"] = userID.ToString();
                        
                        dt.Rows.Add(dr);
                    }
                }

                Database db = new Database();
                DataTable dtData = new DataTable();
                List<SqlParameter> para = new List<SqlParameter>();

                SqlParameter param = new SqlParameter("tableCol", SqlDbType.Structured);
                param.Value = dt;
                para.Add(param);

                dtData = db.ExecuteSP("MemberWise_SchemeAllocation_Save", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    return Ok(new CommonResponse
                    {
                        Message = dtData.Rows[0]["Message"].ToString(),
                        Status = dtData.Rows[0]["Status"].ToString(),
                        Error = ""
                    });
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Message = "Something Went wrong.\nPlease try again later",
                        Status = "0",
                        Error = ""
                    });
                }
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
        [HttpPost]
        public IHttpActionResult MemberWise_SchemeAllocation_Select([FromBody]JObject data)
        {
            MemberWise_SchemeDet_Select_Req req = new MemberWise_SchemeDet_Select_Req();
            try
            {
                req = JsonConvert.DeserializeObject<MemberWise_SchemeDet_Select_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Select_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Select_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (req.sPgNo != null)
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, req.sPgNo));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (req.sPgSize != null)
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, req.sPgSize));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.OrderBy))
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, req.OrderBy));
                else
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("MemberWise_SchemeAllocation_Select", para.ToArray(), false);

                List<MemberWise_SchemeDet_Select_Res> list = new List<MemberWise_SchemeDet_Select_Res>();
                list = DataTableExtension.ToList<MemberWise_SchemeDet_Select_Res>(dt);

                return Ok(new ServiceResponse<MemberWise_SchemeDet_Select_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Select_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Select_Res>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Scheme_Delete([FromBody]JObject data)
        {
            MemberIdSearch_Req req = new MemberIdSearch_Req();
            try
            {
                req = JsonConvert.DeserializeObject<MemberIdSearch_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberIdSearch_Res>
                {
                    Data = new List<MemberIdSearch_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (!string.IsNullOrEmpty(req.Type))
                    para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, req.Type));
                else
                    para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, DBNull.Value)); 
                
                if (!string.IsNullOrEmpty(req.MemberId))
                    para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, req.MemberId));
                else
                    para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (req.Customer_Id > 0)
                    para.Add(db.CreateParam("Customer_Id", DbType.Int64, ParameterDirection.Input, req.Customer_Id));
                else
                    para.Add(db.CreateParam("Customer_Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                DataTable dtData = db.ExecuteSP("Scheme_Delete", para.ToArray(), false);

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
        public IHttpActionResult Year_Mas()
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();
                DataTable dtData = db.ExecuteSP("Year_Mas_Get", para.ToArray(), false);

                List<Year_Mas_Res> list = new List<Year_Mas_Res>();
                list = DataTableExtension.ToList<Year_Mas_Res>(dtData);

                return Ok(new ServiceResponse<Year_Mas_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ServiceResponse<Year_Mas_Res>
                {
                    Data = new List<Year_Mas_Res>(),
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
        public IHttpActionResult Bank_Mas()
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();
                DataTable dtData = db.ExecuteSP("Bank_Mas_Get", para.ToArray(), false);

                List<Bank_Mas_Res> list = new List<Bank_Mas_Res>();
                list = DataTableExtension.ToList<Bank_Mas_Res>(dtData);

                return Ok(new ServiceResponse<Bank_Mas_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ServiceResponse<Bank_Mas_Res>
                {
                    Data = new List<Bank_Mas_Res>(),
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult CustomerWise_AMC_Detail_Get([FromBody]JObject data)
        {
            CustomerWise_AMC_Detail_Get_Req req = new CustomerWise_AMC_Detail_Get_Req();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerWise_AMC_Detail_Get_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CustomerWise_AMC_Detail_Get_Res>
                {
                    Data = new List<CustomerWise_AMC_Detail_Get_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (req.Year_Code > 0)
                    para.Add(db.CreateParam("Year_Code", DbType.String, ParameterDirection.Input, req.Year_Code));
                else
                    para.Add(db.CreateParam("Year_Code", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Assigned))
                {
                    if (req.Assigned == "1")
                        para.Add(db.CreateParam("Assigned", DbType.Boolean, ParameterDirection.Input, true));
                    else
                        para.Add(db.CreateParam("Assigned", DbType.Boolean, ParameterDirection.Input, false));
                }
                else
                {
                    para.Add(db.CreateParam("Assigned", DbType.Boolean, ParameterDirection.Input, DBNull.Value));
                }

                DataTable dtData = db.ExecuteSP("CustomerWise_AMC_Detail_Get", para.ToArray(), false);

                List<CustomerWise_AMC_Detail_Get_Res> list = new List<CustomerWise_AMC_Detail_Get_Res>();
                list = DataTableExtension.ToList<CustomerWise_AMC_Detail_Get_Res>(dtData);

                return Ok(new ServiceResponse<CustomerWise_AMC_Detail_Get_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ServiceResponse<CustomerWise_AMC_Detail_Get_Res>
                {
                    Data = new List<CustomerWise_AMC_Detail_Get_Res>(),
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult AMC_Detail_Save([FromBody]JObject data)
        {
            AMC_Detail_Save_Req req = new AMC_Detail_Save_Req();
            try
            {
                req = JsonConvert.DeserializeObject<AMC_Detail_Save_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Message = "Input Parameters are not in the proper format",
                    Status = "0",
                    Error = ""
                });
            }

            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Customer_Id", typeof(string));
                dt.Columns.Add("Year_Code", typeof(string));
                dt.Columns.Add("AMCAmount", typeof(string));

                if (req.amcsave.Count() > 0)
                {
                    for (int i = 0; i < req.amcsave.Count(); i++)
                    {
                        DataRow dr = dt.NewRow();

                        dr["Customer_Id"] = req.amcsave[i].Customer_Id.ToString();
                        dr["Year_Code"] = req.amcsave[i].Year_Code.ToString();
                        dr["AMCAmount"] = req.amcsave[i].AMCAmount.ToString();

                        dt.Rows.Add(dr);
                    }
                }

                Database db = new Database();
                DataTable dtData = new DataTable();
                List<SqlParameter> para = new List<SqlParameter>();

                SqlParameter param = new SqlParameter("tableCol", SqlDbType.Structured);
                param.Value = dt;
                para.Add(param);

                dtData = db.ExecuteSP("AMC_Detail_Save", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    return Ok(new CommonResponse
                    {
                        Message = dtData.Rows[0]["Message"].ToString(),
                        Status = dtData.Rows[0]["Status"].ToString(),
                        Error = ""
                    });
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Message = "Something Went wrong.\nPlease try again later",
                        Status = "0",
                        Error = ""
                    });
                }
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

        [HttpPost]
        public IHttpActionResult AMC_Payment_Receipt_Save([FromBody]JObject data)
        {
            AMC_Payment_Receipt_Save_Req req = new AMC_Payment_Receipt_Save_Req();
            try
            {
                req = JsonConvert.DeserializeObject<AMC_Payment_Receipt_Save_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<MemberWise_SchemeDet_Get_Res>
                {
                    Data = new List<MemberWise_SchemeDet_Get_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);

                para.Add(db.CreateParam("Customer_Id", DbType.Int32, ParameterDirection.Input, req.Customer_Id));
                para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, req.MemberId));
                para.Add(db.CreateParam("Year_Code", DbType.Int32, ParameterDirection.Input, req.Year_Code));
                para.Add(db.CreateParam("Amt_To_Pay", DbType.Decimal, ParameterDirection.Input, req.Amt_To_Pay));
                para.Add(db.CreateParam("Paid_Amt", DbType.Decimal, ParameterDirection.Input, req.Paid_Amt));
                para.Add(db.CreateParam("EntryBy", DbType.Int64, ParameterDirection.Input, userID));

                DataTable dtData = db.ExecuteSP("AMC_Payment_Receipt_Save", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    return Ok(new CommonResponse
                    {
                        Message = dtData.Rows[0]["Message"].ToString(),
                        Status = dtData.Rows[0]["Status"].ToString(),
                        Error = ""
                    });
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Message = "Something Went wrong.\nPlease try again later",
                        Status = "0",
                        Error = ""
                    });
                }
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
        [HttpPost]
        public IHttpActionResult AMC_Payment_Receipt_Get([FromBody]JObject data)
        {
            AMC_Payment_Receipt_Get_Req req = new AMC_Payment_Receipt_Get_Req();
            try
            {
                req = JsonConvert.DeserializeObject<AMC_Payment_Receipt_Get_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<AMC_Payment_Receipt_Get_Res>
                {
                    Data = new List<AMC_Payment_Receipt_Get_Res>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                para.Add(db.CreateParam("Customer_Id", DbType.Int32, ParameterDirection.Input, req.Customer_Id));
                para.Add(db.CreateParam("MemberId", DbType.String, ParameterDirection.Input, req.MemberId));
                para.Add(db.CreateParam("Year_Code", DbType.String, ParameterDirection.Input, req.Year_Code));

                DataTable dtData = db.ExecuteSP("AMC_Payment_Receipt_Get", para.ToArray(), false);

                List<AMC_Payment_Receipt_Get_Res> list = new List<AMC_Payment_Receipt_Get_Res>();
                list = DataTableExtension.ToList<AMC_Payment_Receipt_Get_Res>(dtData);

                return Ok(new ServiceResponse<AMC_Payment_Receipt_Get_Res>
                {
                    Data = list,
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                return Ok(new ServiceResponse<AMC_Payment_Receipt_Get_Res>
                {
                    Data = new List<AMC_Payment_Receipt_Get_Res>(),
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
    }
}
