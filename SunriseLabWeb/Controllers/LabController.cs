using Lib.Model;
using Newtonsoft.Json;
using SunriseLabWeb.Data;
using SunriseLabWeb.Filter;
using SunriseLabWeb.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SunriseLabWeb.Controllers
{
    [AuthorizeActionFilterAttribute]
    public class LabController : Controller
    {
        API _api = new API();
        Data.Common _common = new Data.Common();

        #region Thread :: Lab Stock Data Upload AUTO
        public JsonResult Index()
        {
            Thread LabData = new Thread(LabDataUpload_Ora_Thread);
            LabData.Start();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public static void LabDataUpload_Ora_Thread()
        {
            API _api = new API();
            LabDataUpload_Ora_Req req = new LabDataUpload_Ora_Req();
            req.Type = "AUTO";
            req.DataTransferType = "MACRO";

            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPIUrlEncodedWithWebReq(Constants.LabDataUpload_Ora, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
        }
        #endregion

        #region Thread :: Lab Stock Data Manual Upload in Lab Stock API Module

        public JsonResult LabAPI_LabDataUpload_Ora(LabDataUpload_Ora_Req req)
        {
            String arg = req.DataTransferType;

            Thread LabData = new Thread(LabAPI_LabDataUpload_Ora_Thread);
            LabData.Start(arg);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public static void LabAPI_LabDataUpload_Ora_Thread(object arg)
        {
            String arg1 = arg.ToString();
            API _api = new API();
            LabDataUpload_Ora_Req req = new LabDataUpload_Ora_Req();
            req.Type = "MANUAL";
            req.DataTransferType = arg1;

            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPIUrlEncodedWithWebReq(Constants.LabDataUpload_Ora, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
        }
        // LAB Stock API in Status Get
        public JsonResult LabStockStatusGet()
        {
            var input = new { };
            string inputJson = (new JavaScriptSerializer()).Serialize(input);
            string _response = _api.CallAPI(Constants.LabStockStatusGet, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Thread :: Lab Stock Data Delete
        public JsonResult LabStockDataDelete()
        {
            Thread LabData = new Thread(LabStockDataDelete_Thread);
            LabData.Start();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public static void LabStockDataDelete_Thread()
        {
            API _api = new API();
            string _response = _api.CallAPIUrlEncodedWithWebReq(Constants.LabStockDataDelete, string.Empty);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
        }

        #endregion

        public ActionResult Download()
        {
            return View();
        }
        public ActionResult DownloadAction(LabStockDownload_Req req)
        {
            string username = DecodeServerName(req.UN);
            string password = DecodeServerName(req.PD);

            if (username == "" || password == "")
            {
                Lab_CommonResponse _data1 = new Lab_CommonResponse();
                _data1.Status = "0";
                _data1.Error = "401 : Unauthorized Request";
                return Json(_data1, JsonRequestBehavior.AllowGet);
            }

            req.Username = username;
            req.Password = password;
            req.UN = null;
            req.PD = null;
            req.IPAddress = _common.gUserIPAddresss();

            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPIUrlEncodedWithWebReq(Constants.LabDataGetURLApi, inputJson);
            Lab_CommonResponse _data = new Lab_CommonResponse();
            _data = (new JavaScriptSerializer()).Deserialize<Lab_CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);

            //if (_data.Status == "1")
            //{
            //    if (_data.ExportType == "XML" || _data.ExportType == "JSON")
            //    {
            //        string path = _data.Message;
            //        string[] pathArr = path.Split('\\');
            //        string[] fileArr = pathArr.Last().Split('.');
            //        string fileName = fileArr.Last().ToString();

            //        Response.ContentType = fileArr.Last();
            //        Response.AddHeader("Content-Disposition", "attachment;filename=\"" + pathArr.Last() + "\"");
            //        Response.TransmitFile(_data.Message);
            //        Response.End();
            //        return Json("Success", JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {
            //        return Json(_data, JsonRequestBehavior.AllowGet);
            //    }
            //}
            //else
            //{
            //    return Json(_data, JsonRequestBehavior.AllowGet);
            //}


            //if (_data.Status == "1")
            //{
            //    if (_data.ExportType != "JSON(TEXT)")
            //    {
            //        //Thread.Sleep(120000); 

            //        string path = _data.Message;
            //        string[] pathArr = path.Split('\\');
            //        string[] fileArr = pathArr.Last().Split('.');
            //        string fileName = fileArr.Last().ToString();

            //        Response.ContentType = fileArr.Last();
            //        Response.AddHeader("Content-Disposition", "attachment;filename=\"" + pathArr.Last() + "\"");
            //        Response.TransmitFile(_data.Message);
            //        Response.End();
            //        return Json("Success", JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {
            //        var webClient = new WebClient();
            //        var json = webClient.DownloadString(_data.Message);

            //        //dynamic obj = JsonConvert.DeserializeObject(json.ToString());
            //        //var abc = obj.First;

            //        return new JsonResult()
            //        {
            //            Data = json,
            //            ContentType = "application/json",
            //            ContentEncoding = Encoding.UTF8,
            //            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            //            MaxJsonLength = Int32.MaxValue
            //        };
            //    }
            //}
            //else
            //{
            //    return Json(_data.Error, JsonRequestBehavior.AllowGet);
            //}
        }
        public ActionResult LabList()
        {
            return View();
        }
        public ActionResult LabDetail()
        {
            var TransId = Convert.ToInt32(Request.QueryString.Count > 0 && Request.QueryString["TransId"] != "" ? Request.QueryString["TransId"] : "0");
            var input = new
            {
                ListValue = "DP"
            };
            string inputJson = (new JavaScriptSerializer()).Serialize(input);
            string _response = _api.CallAPI(Constants.GetSearchParameter, inputJson);
            ServiceResponse<ListValueResponse> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<ListValueResponse>>(_response);
            UploadMethodModel apiFilter = new UploadMethodModel();
            ServiceResponse<GetLab_Response> _data1;

            if (TransId != 0)
            {
                var input1 = new
                {
                    sTransId = TransId
                };
                string inputJson1 = (new JavaScriptSerializer()).Serialize(input1);
                string _response1 = _api.CallAPI(Constants.GetLab, inputJson1);
                _data1 = (new JavaScriptSerializer()).Deserialize<ServiceResponse<GetLab_Response>>(_response1);

                apiFilter.UserName = _data1.Data[0].UserName;
                apiFilter.Password = _data1.Data[0].Password;
                apiFilter.ExportType = _data1.Data[0].ExportType;
                apiFilter.APIStatus = _data1.Data[0].APIStatus;
                apiFilter.APIName = _data1.Data[0].APIName;
                apiFilter.For_iUserId = _data1.Data[0].For_iUserId;
                apiFilter.For_CompName = _data1.Data[0].For_CompName;
                apiFilter.APIUrl = _data1.Data[0].APIUrl;

                List<APIFiltersSettingsModel> columns2 = new List<APIFiltersSettingsModel>();
                int index2 = 0;
                foreach (var item in _data1.Data[0].Filters)
                {
                    string mixcolor = String.Empty, LengthTitle = String.Empty, WidthTitle = String.Empty, DepthTitle = String.Empty, 
                        DepthPerTitle = String.Empty, TablePerTitle = String.Empty, CrAngTitle = String.Empty, CrHtTitle = String.Empty, 
                        PavAngTitle = String.Empty, PavHtTitle = String.Empty, KeyToSymbolTitle = String.Empty;

                    if (!String.IsNullOrEmpty(item.sColor))
                    {
                        mixcolor += item.sColor;
                    }
                    if (!String.IsNullOrEmpty(item.sINTENSITY))
                    {
                        mixcolor += (mixcolor == "" ? "" : "</br>") + "<b>INTENSITY :</b>";
                        mixcolor += item.sINTENSITY;
                    }
                    if (!String.IsNullOrEmpty(item.sOVERTONE))
                    {
                        mixcolor += (mixcolor == "" ? "" : "</br>") + "<b>OVERTONE :</b>";
                        mixcolor += item.sOVERTONE;
                    }
                    if (!String.IsNullOrEmpty(item.sFANCY_COLOR))
                    {
                        mixcolor += (mixcolor == "" ? "" : "</br>") + "<b>FANCY COLOR :</b>";
                        mixcolor += item.sFANCY_COLOR;
                    }
                    if (!String.IsNullOrEmpty(item.sColorType))
                    {
                        mixcolor += (item.sColorType == "Regular" ? "<b>REGULAR ALL</b>" : item.sColorType == "Fancy" ? "<b>FANCY ALL</b>" : "");
                    }

                    LengthTitle = (item.dFromLength != null && item.dToLength != null ? item.dFromLength + "-" + item.dToLength: "");
                    LengthTitle += (item.Length_IsBlank == true ? (item.dFromLength != null && item.dToLength != null ? ", BLANK" : "BLANK") : "");

                    WidthTitle = (item.dFromWidth != null && item.dToWidth != null ? item.dFromWidth + "-" + item.dToWidth : "");
                    WidthTitle += (item.Width_IsBlank == true ? (item.dFromWidth != null && item.dToWidth != null ? ", BLANK" : "BLANK") : "");

                    DepthTitle = (item.dFromDepth != null && item.dToDepth != null ? item.dFromDepth + "-" + item.dToDepth : "");
                    DepthTitle += (item.Depth_IsBlank == true ? (item.dFromDepth != null && item.dToDepth != null ? ", BLANK" : "BLANK") : "");

                    DepthPerTitle = (item.dFromDepthPer != null && item.dToDepthPer != null ? item.dFromDepthPer + "-" + item.dToDepthPer : "");
                    DepthPerTitle += (item.DepthPer_IsBlank == true ? (item.dFromDepthPer != null && item.dToDepthPer != null ? ", BLANK" : "BLANK") : "");

                    TablePerTitle = (item.dFromTablePer != null && item.dToTablePer != null ? item.dFromTablePer + "-" + item.dToTablePer : "");
                    TablePerTitle += (item.TablePer_IsBlank == true ? (item.dFromTablePer != null && item.dToTablePer != null ? ", BLANK" : "BLANK") : "");

                    CrAngTitle = (item.dFromCrAng != null && item.dToCrAng != null ? item.dFromCrAng + "-" + item.dToCrAng : "");
                    CrAngTitle += (item.CrAng_IsBlank == true ? (item.dFromCrAng != null && item.dToCrAng != null ? ", BLANK" : "BLANK") : "");

                    CrHtTitle = (item.dFromCrHt != null && item.dToCrHt != null ? item.dFromCrHt + "-" + item.dToCrHt : "");
                    CrHtTitle += (item.CrHt_IsBlank == true ? (item.dFromCrHt != null && item.dToCrHt != null ? ", BLANK" : "BLANK") : "");

                    PavAngTitle = (item.dFromPavAng != null && item.dToPavAng != null ? item.dFromPavAng + "-" + item.dToPavAng : "");
                    PavAngTitle += (item.PavAng_IsBlank == true ? (item.dFromPavAng != null && item.dToPavAng != null ? ", BLANK" : "BLANK") : "");

                    PavHtTitle = (item.dFromPavHt != null && item.dToPavHt != null ? item.dFromPavHt + "-" + item.dToPavHt : "");
                    PavHtTitle += (item.PavHt_IsBlank == true ? (item.dFromPavHt != null && item.dToPavHt != null ? ", BLANK" : "BLANK") : "");

                    KeyToSymbolTitle = item.dKeyToSymbol;
                    KeyToSymbolTitle += (item.KTS_IsBlank == true ? (item.dKeyToSymbol != null ? " <br/>BLANK" : "BLANK") : "");


                    columns2.Add(new APIFiltersSettingsModel()
                    {
                        Sr = item.Sr,
                        iVendor = item.iVendor,
                        iLocation = item.iLocation,
                        sShape = item.sShape,
                        sPointer = item.sPointer,
                        sColorType = item.sColorType,
                        sColor = item.sColor,
                        sINTENSITY = item.sINTENSITY,
                        sOVERTONE = item.sOVERTONE,
                        sFANCY_COLOR = item.sFANCY_COLOR,
                        MixColor = mixcolor,
                        sClarity = item.sClarity,
                        sCut = item.sCut,
                        sPolish = item.sPolish,
                        sSymm = item.sSymm,
                        sFls = item.sFls,
                        sLab = item.sLab,
                        dFromLength = item.dFromLength,
                        dToLength = item.dToLength,
                        dFromWidth = item.dFromWidth,
                        dToWidth = item.dToWidth,
                        dFromDepth = item.dFromDepth,
                        dToDepth = item.dToDepth,
                        dFromDepthPer = item.dFromDepthPer,
                        dToDepthPer = item.dToDepthPer,
                        dFromTablePer = item.dFromTablePer,
                        dToTablePer = item.dToTablePer,
                        dFromCrAng = item.dFromCrAng,
                        dToCrAng = item.dToCrAng,
                        dFromCrHt = item.dFromCrHt,
                        dToCrHt = item.dToCrHt,
                        dFromPavAng = item.dFromPavAng,
                        dToPavAng = item.dToPavAng,
                        dFromPavHt = item.dFromPavHt,
                        dToPavHt = item.dToPavHt,
                        dKeyToSymbol = item.dKeyToSymbol,
                        dCheckKTS = item.dCheckKTS,
                        dUNCheckKTS = item.dUNCheckKTS,
                        sBGM = item.sBGM,
                        sCrownBlack = item.sCrownBlack,
                        sTableBlack = item.sTableBlack,
                        sCrownWhite = item.sCrownWhite,
                        sTableWhite = item.sTableWhite,
                        sTableOpen = item.sTableOpen,
                        sCrownOpen = item.sCrownOpen,
                        sPavOpen = item.sPavOpen,
                        sGirdleOpen = item.sGirdleOpen,
                        Length_IsBlank = item.Length_IsBlank,
                        Width_IsBlank = item.Width_IsBlank,
                        Depth_IsBlank = item.Depth_IsBlank,
                        DepthPer_IsBlank = item.DepthPer_IsBlank,
                        TablePer_IsBlank = item.TablePer_IsBlank,
                        CrAng_IsBlank = item.CrAng_IsBlank,
                        CrHt_IsBlank = item.CrHt_IsBlank,
                        PavAng_IsBlank = item.PavAng_IsBlank,
                        PavHt_IsBlank = item.PavHt_IsBlank,
                        KTS_IsBlank = item.KTS_IsBlank,
                        Img = item.Img,
                        Vdo = item.Vdo,
                        PriceMethod = item.PriceMethod,
                        PricePer = item.PricePer,
                        LengthTitle = LengthTitle,
                        WidthTitle = WidthTitle,
                        DepthTitle = DepthTitle,
                        DepthPerTitle = DepthPerTitle,
                        TablePerTitle = TablePerTitle,
                        CrAngTitle = CrAngTitle,
                        CrHtTitle = CrHtTitle,
                        PavAngTitle = PavAngTitle,
                        PavHtTitle = PavHtTitle,
                        KeyToSymbolTitle = KeyToSymbolTitle
                    }); 
                    index2++;
                }
                apiFilter.APIFilters = columns2.ToList();


                List<ColumnsSettingsModel> columns3 = new List<ColumnsSettingsModel>();
                int index3 = 0;
                foreach (var item in _data1.Data[0].ColumnsSettings)
                {
                    columns3.Add(new ColumnsSettingsModel()
                    {
                        IsActive = item.IsActive,
                        icolumnId = item.icolumnId,
                        iPriority = item.iPriority,
                        sCustMiseCaption = item.sCustMiseCaption,
                        sUser_ColumnName = item.sUser_ColumnName,
                    });
                    index3++;
                }
                apiFilter.ColumnList = columns3.ToList();
            }
            else
            {
                _response = _api.CallAPI(Constants.GetApiColumnsDetails, string.Empty);
                ServiceResponse<ApiColumns> _coldata = (new JavaScriptSerializer()).Deserialize<ServiceResponse<ApiColumns>>(_response);
                List<ColumnsSettingsModel> columns = new List<ColumnsSettingsModel>();
                int index = 0;
                foreach (var item in _coldata.Data)
                {
                    columns.Add(new ColumnsSettingsModel()
                    {
                        icolumnId = item.iid,
                        iPriority = index + 1,
                        IsActive = false,
                        sCustMiseCaption = item.caption,
                        sUser_ColumnName = item.caption
                    });
                    index++;
                }
                apiFilter.ColumnList = columns.OrderBy(i => i.iPriority).ToList();
                apiFilter.APIStatus = true;
                apiFilter.For_iUserId = Convert.ToInt32(SessionFacade.UserSession.iUserid);
            }
            string _response2 = _api.CallAPI(Constants.GetUserMas, string.Empty);
            ServiceResponse<ColumnsUserResponse> _data2 = (new JavaScriptSerializer()).Deserialize<ServiceResponse<ColumnsUserResponse>>(_response2);
            List<SelectListItem> col2 = new List<SelectListItem>();
            foreach (var a in _data2.Data)
            {
                col2.Add(new SelectListItem
                {
                    Text = a.sUsername + (a.sCompName != "" && a.sCompName != null ? " [" + a.sCompName + "]" : ""),
                    Value = a.iUserid.ToString()
                });
            }
            apiFilter.iTransId = TransId;
            apiFilter.For_iUserIds = col2.OrderBy(i => i.Text).ToList();

            _data.Data.Add(new ListValueResponse { Id = -0, Value = "ALL", ListType = "Shape" });
            apiFilter.ShapeList = _data.Data.Where(a => a.ListType.ToLower() == "shape").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(o => o.iSr).ToList();

            apiFilter.PointerList = _data.Data.Where(a => a.ListType.ToLower() == "pointer").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(o => o.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = -0, Value = "ALL", ListType = "Color" });
            apiFilter.ColorList = _data.Data.Where(a => a.ListType.ToLower() == "color").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(o => o.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = -0, Value = "ALL", ListType = "Cut" });
            apiFilter.CutList = _data.Data.Where(a => a.ListType.ToLower() == "cut").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(o => o.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = -0, Value = "ALL", ListType = "Clarity" });
            apiFilter.ClarityList = _data.Data.Where(a => a.ListType.ToLower() == "clarity").Select(b => new ListingModel() { iSr = (b.Value == "ALL" ? b.Id : b.Id + 1), sName = b.Value }).OrderBy(o => o.iSr).ToList();

            apiFilter.PolishList = _data.Data.Where(a => a.ListType.ToLower() == "polish").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).ToList();
            apiFilter.SymmList = _data.Data.Where(a => a.ListType.ToLower() == "symm").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).ToList();
            apiFilter.FlsList = _data.Data.Where(a => a.ListType.ToLower() == "fls").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).ToList();
            apiFilter.ShadeList = _data.Data.Where(a => a.ListType.ToLower() == "shade").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).ToList();
            apiFilter.NattsList = _data.Data.Where(a => a.ListType.ToLower() == "table_natts").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).ToList();
            apiFilter.InclusionList = _data.Data.Where(a => a.ListType.ToLower() == "table_incl").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).ToList();
            apiFilter.LabList = _data.Data.Where(a => a.ListType.ToLower() == "lab").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).ToList();

            _data.Data.Add(new ListValueResponse { Id = -0, Value = "ALL", ListType = "Location" });
            apiFilter.LocationList = _data.Data.Where(a => a.ListType.ToLower() == "location").Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "bgm" }); 
            apiFilter.BGMList = _data.Data.Where(a => a.ListType.ToLower() == "bgm" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "crown_natts" });
            apiFilter.CrnBlackList = _data.Data.Where(a => a.ListType.ToLower() == "crown_natts" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "crown_incl" });
            apiFilter.CrnWhiteList = _data.Data.Where(a => a.ListType.ToLower() == "crown_incl" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "table_natts" });
            apiFilter.TblBlackList = _data.Data.Where(a => a.ListType.ToLower() == "table_natts" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "table_incl" });
            apiFilter.TblWhiteList = _data.Data.Where(a => a.ListType.ToLower() == "table_incl" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "tableopen" });
            apiFilter.TblOpenList = _data.Data.Where(a => a.ListType.ToLower() == "tableopen" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "crownopen" });
            apiFilter.CrnOpenList = _data.Data.Where(a => a.ListType.ToLower() == "crownopen" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "pavilionopen" });
            apiFilter.PavOpenList = _data.Data.Where(a => a.ListType.ToLower() == "pavilionopen" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            _data.Data.Add(new ListValueResponse { Id = 4, Value = "BLANK", ListType = "girdleopen" });
            apiFilter.GrdleOpenList = _data.Data.Where(a => a.ListType.ToLower() == "girdleopen" && a.Id > 0).Select(b => new ListingModel() { iSr = b.Id, sName = b.Value }).OrderBy(c => c.iSr).ToList();

            string _response_V = _api.CallAPI(Constants.VendorInfo, string.Empty);
            ServiceResponse<VendorResponse> _data_V = (new JavaScriptSerializer()).Deserialize<ServiceResponse<VendorResponse>>(_response_V);
            _data_V.Data.Add(new VendorResponse { Id = 0, SUPPLIER = "ALL" });
            apiFilter.SupplierList = _data_V.Data.Select((b, i) => new ListingModel() { iSr = (b.SUPPLIER == "ALL" ? 0 : i + 1), sName = b.SUPPLIER }).OrderBy(o => o.iSr).ToList();

            apiFilter.ExportTypeList = new List<SelectListItem>() {
                new SelectListItem { Value = "EXCEL(.xlsx)", Text = "EXCEL(.xlsx)" },
                new SelectListItem { Value = "EXCEL(.xls)", Text = "EXCEL(.xls)" },
                new SelectListItem { Value = "CSV",         Text = "CSV" },
                //new SelectListItem { Value = "XML",         Text = "XML" },
                //new SelectListItem { Value = "JSON",        Text = "JSON" },
                //new SelectListItem { Value = "JSON(Text)",        Text = "JSON(Text)" }
            };

            return View(apiFilter);
        }
        public JsonResult GetKeyToSymbolList()
        {
            string _response = _api.CallAPI(Constants.GetKeyToSymbolList, string.Empty);
            ServiceResponse<KeyToSymbolResponse> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<KeyToSymbolResponse>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UserwiseCompany_select(int iUserid)
        {
            var input = new
            {
                iUserid = iUserid
            };
            string inputJson = (new JavaScriptSerializer()).Serialize(input);
            string _response = _api.CallAPI(Constants.UserwiseCompany_select, inputJson);
            ServiceResponse<UserwiseCompany_select> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<UserwiseCompany_select>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveLab(SaveLab_Req savelab_req)
        {
            Uri url = new Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
            string AbsoluteUri = url.AbsoluteUri;
            string AbsolutePath = url.AbsolutePath;
            string mainurl = AbsoluteUri.Replace(AbsolutePath, "");
            string DecodedUsername = EncodeServerName(savelab_req.UserName);
            string DecodedPassword = EncodeServerName(savelab_req.Password);

            savelab_req.APIUrl = mainurl + "/Lab/Download?UN=" + DecodedUsername + "&PD=" + DecodedPassword + "&TransId=";

            string inputJson = (new JavaScriptSerializer()).Serialize(savelab_req);
            string _response = _api.CallAPI(Constants.SaveLab, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLab(GetLab_Request getlab_request)
        {
            string inputJson1 = (new JavaScriptSerializer()).Serialize(getlab_request);
            string _response1 = _api.CallAPI(Constants.GetLab, inputJson1);
            ServiceResponse<GetLab_Response> _data1 = (new JavaScriptSerializer()).Deserialize<ServiceResponse<GetLab_Response>>(_response1);
            return Json(_data1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApiColumns()
        {
            string _response = _api.CallAPI(Constants.GetApiColumnsDetails, string.Empty);
            ServiceResponse<ApiColumns> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<ApiColumns>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Lab_Column_Auto_Select(Lab_Column_Auto_Select_Req req)
        {
            string _inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.Lab_Column_Auto_Select, _inputJson);
            ServiceResponse<ColumnsSettingsModel> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<ColumnsSettingsModel>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public static string EncodeServerName(string serverName)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(serverName));
        }
        public static string DecodeServerName(string encodedServername)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(encodedServername));
            }
            catch(Exception ex)
            {
                return "";
            }
            
        }
        public JsonResult hardik()
        {
            string response = _api.CallAPI("/LabStock/hardik", string.Empty);
            CommonResponse data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}