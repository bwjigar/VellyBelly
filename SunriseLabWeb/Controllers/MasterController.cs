using Lib.Model;
using SunriseLabWeb.Data;
using SunriseLabWeb.Filter;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SunriseLabWeb.Controllers
{
    [AuthorizeActionFilterAttribute]
    public class MasterController : Controller
    {
        API _api = new API();
        public ActionResult SchemeMas()
        {
            return View();
        }
        public ActionResult SchemeDet()
        {
            return View();
        }
        public JsonResult CustomerMast_Get_MemberId_Wise(MemberIdSearch_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.CustomerMast_Get_MemberId_Wise, inputJson);
            ServiceResponse<MemberIdSearch_Res> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<MemberIdSearch_Res>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult MemberWise_SchemeDet_Get(MemberIdSearch_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.MemberWise_SchemeDet_Get, inputJson);
            ServiceResponse<MemberWise_SchemeDet_Get_Res> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<MemberWise_SchemeDet_Get_Res>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult MemberWise_SchemeDet_Save(MemberWise_SchemeDet_Save_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.MemberWise_SchemeDet_Save, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult MemberWise_SchemeDet_Select(MemberWise_SchemeDet_Select_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string response = _api.CallAPI(Constants.MemberWise_SchemeDet_Select, inputJson);
            ServiceResponse<MemberWise_SchemeDet_Select_Res> data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<MemberWise_SchemeDet_Select_Res>>(response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MemSchemeAllocMas()
        {
            return View();
        }
        public ActionResult MemSchemeAllocDet()
        {
            return View();
        }
        public JsonResult MemberWise_SchemeAlloc_Get(MemberIdSearch_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.MemberWise_SchemeAlloc_Get, inputJson);
            ServiceResponse<MemberWise_SchemeDet_Get_Res> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<MemberWise_SchemeDet_Get_Res>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult MemberWise_SchemeAllocation_Save(MemberWise_SchemeDet_Save_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.MemberWise_SchemeAllocation_Save, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult MemberWise_SchemeAllocation_Select(MemberWise_SchemeDet_Select_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string response = _api.CallAPI(Constants.MemberWise_SchemeAllocation_Select, inputJson);
            ServiceResponse<MemberWise_SchemeDet_Select_Res> data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<MemberWise_SchemeDet_Select_Res>>(response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Scheme_Delete(MemberIdSearch_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.Scheme_Delete, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult AMCDetail()
        {
            return View();
        }
        public JsonResult Year_Mas()
        {
            string response = _api.CallAPI(Constants.Year_Mas, string.Empty);
            ServiceResponse<Year_Mas_Res> data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<Year_Mas_Res>>(response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Bank_Mas()
        {
            string response = _api.CallAPI(Constants.Bank_Mas, string.Empty);
            ServiceResponse<Bank_Mas_Res> data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<Bank_Mas_Res>>(response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CustomerWise_AMC_Detail_Get(CustomerWise_AMC_Detail_Get_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string response = _api.CallAPI(Constants.CustomerWise_AMC_Detail_Get, inputJson);
            ServiceResponse<CustomerWise_AMC_Detail_Get_Res> data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<CustomerWise_AMC_Detail_Get_Res>>(response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AMC_Detail_Save(AMC_Detail_Save_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.AMC_Detail_Save, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AMCPaymentReceipt()
        {
            return View();
        }
        public JsonResult AMC_Payment_Receipt_Save(AMC_Payment_Receipt_Save_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.AMC_Payment_Receipt_Save, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AMC_Payment_Receipt_Get(AMC_Payment_Receipt_Get_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string response = _api.CallAPI(Constants.AMC_Payment_Receipt_Get, inputJson);
            ServiceResponse<AMC_Payment_Receipt_Get_Res> data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<AMC_Payment_Receipt_Get_Res>>(response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}