using Lib.Model;
using Newtonsoft.Json;
using SunriseLabWeb.Data;
using SunriseLabWeb.Helper;
using SunriseLabWeb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SunriseLabWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        API _api = new API();
        Data.Common _common = new Data.Common();
        public ActionResult Index()
        {
            UserLogin _obj = new UserLogin();
            if (Request.Cookies["Username"] != null && Request.Cookies["Password"] != null && Request.Cookies["IsRemember"] != null)
            {
                _obj.Username = Request.Cookies["Username"].Value.ToString();
                _obj.Password = Request.Cookies["Password"].Value.ToString();
                _obj.isRemember = Convert.ToBoolean(Request.Cookies["IsRemember"].Value);
            }
            ViewBag.Message = "Please enter correct username and password";
            //ViewData["URL"] = ConfigurationManager.AppSettings["SunriseWebsiteLogoutURL"];
            return View(_obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserLogin _obj)
        {
            if (ModelState.IsValid)
            {
                string _ipAddress = _common.gUserIPAddresss();
                var input = new LoginRequest
                {
                    UserName = _obj.Username,
                    Password = _obj.Password,
                    Source = "",
                    IpAddress = _ipAddress,
                    UDID = "",
                    LoginMode = "",
                    DeviseType = "Web",
                    DeviceName = "",
                    AppVersion = "",
                    Location = "",
                    Login = "",
                    grant_type = "password"
                };
                string inputJson = string.Join("&", input.GetType()
                                                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                            .Where(p => p.GetValue(input, null) != null)
                                   .Select(p => $"{p.Name}={Uri.EscapeDataString(p.GetValue(input).ToString())}"));


                string _response = _api.CallAPIUrlEncoded(Constants.UserLogin, inputJson);
                if (_response.ToLower().Contains(@"""error") && _response.ToLower().Contains(@"""error_description"))
                {
                    OAuthErrorMsg _authErrorMsg = new OAuthErrorMsg();
                    _authErrorMsg = (new JavaScriptSerializer()).Deserialize<OAuthErrorMsg>(_response);
                    inputJson = (new JavaScriptSerializer()).Serialize(input);
                    string _keyresponse = _api.CallAPIUrlEncoded(Constants.KeyAccountData, inputJson);
                    ServiceResponse<KeyAccountDataResponse> _objresponse = (new JavaScriptSerializer()).Deserialize<ServiceResponse<KeyAccountDataResponse>>(_keyresponse);

                    TempData["Message"] = _authErrorMsg.error_description;
                }
                else
                {
                    LoginFullResponse _data = new LoginFullResponse();
                    try
                    {
                        _data = (new JavaScriptSerializer()).Deserialize<LoginFullResponse>(_response);
                    }
                    catch (WebException ex)
                    {
                        var webException = ex as WebException;
                        if ((Convert.ToString(webException.Status)).ToUpper() == "PROTOCOLERROR")
                        {
                            OAuthErrorMsg error =
                                JsonConvert.DeserializeObject<OAuthErrorMsg>(
                               API.ExtractResponseString(webException));
                            TempData["Message"] = error.error_description;
                        }
                        TempData["Message"] = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = ex.Message;
                    }

                    if (_data.UserID > 0)
                    {
                        SessionFacade.TokenNo = _data.access_token;
                        inputJson = (new JavaScriptSerializer()).Serialize(input);
                        string _keyresponse = _api.CallAPI(Constants.KeyAccountData, inputJson);
                        ServiceResponse<KeyAccountDataResponse> _objresponse = (new JavaScriptSerializer()).Deserialize<ServiceResponse<KeyAccountDataResponse>>(_keyresponse);

                        string _imageResponse = _api.CallAPI(Constants.GetUserProfilePicture, string.Empty);

                        if (_objresponse.Data != null && _objresponse.Data.Count > 0)
                        {
                            SessionFacade.UserSession = _objresponse.Data.FirstOrDefault();
                            SessionFacade.UserSession.ProfileImage = _imageResponse.Replace("\"", "");

                            //var obj = _objresponse.Data.FirstOrDefault();

                            //Response.Cookies["Userid_DNA"].Value = obj.iUserid.Value.ToString();

                            //var _input1 = new
                            //{
                            //    IPAddress = GetIpValue(),
                            //    UserId = obj.iUserid,
                            //    Type = "STORED"
                            //};
                            //var _inputJson_1 = (new JavaScriptSerializer()).Serialize(_input1);
                            //string _Response_1 = _api.CallAPI(Constants.IP_Wise_Login_Detail, _inputJson_1);

                        }
                        if (_obj.isRemember)
                        {
                            Response.Cookies["UserName"].Value = _obj.Username;
                            Response.Cookies["Password"].Value = _obj.Password;
                            Response.Cookies["IsRemember"].Value = _obj.isRemember.ToString();
                        }

                        //return RedirectToAction("LabList", "Lab");
                        return RedirectToAction("", "Dashboard");
                    }
                    else
                    {
                        TempData["Message"] = _data.Message;
                    }
                }
            }
            return View(_obj);

        }
        public string GetIpValue()
        {
            string ipAdd = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAdd))
            {
                ipAdd = Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                // lblIPAddress.Text = ipAdd;
            }
            return ipAdd;
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            SessionFacade.Abandon();

            //var _input1 = new
            //{
            //    IPAddress = GetIpValue(),
            //    UserId = 0,
            //    Type = "EXPIRED"
            //};
            //var _inputJson_1 = (new JavaScriptSerializer()).Serialize(_input1);
            //string _Response_1 = _api.CallAPI(Constants.IP_Wise_Login_Detail, _inputJson_1);

            //string _response = _api.CallAPIWithoutToken(Constants.LogoutWithoutToken, "");

            //ViewData["URL"] = ConfigurationManager.AppSettings["SunriseWebsiteLogoutURL"];
            //return View();
            return RedirectToAction("Index", "Login");
        }
        public JsonResult LoginCheck()
        {
            string _response = _api.CallAPI("/LabStock/LoginCheck", String.Empty);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data != null ? _data.Message : "UNAUTHORIZED", JsonRequestBehavior.AllowGet);
        }
    }
}