using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Lib.Model
{
    public class VendorResponse
    {
        public int Id { get; set; }
        public string SUPPLIER { get; set; }
    }
    public class ListValueRequest
    {
        public string ListValue { get; set; }
    }
    public class ListValueResponse
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string ListType { get; set; }
        public string UrlValue { get; set; }
        public string UrlValueHov { get; set; }
    }
    public class UploadMethodModel
    {
        public long? iTransId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExportType { get; set; }
        public string APIName { get; set; }
        public bool APIStatus { get; set; }
        public string APIUrl { get; set; }
        public int For_iUserId { get; set; }
        public string For_CompName { get; set; }

        public List<SelectListItem> For_iUserIds { get; set; }
        public List<ListingModel> SupplierList { get; set; }
        public List<ListingModel> ShapeList { get; set; }
        public List<ListingModel> PointerList { get; set; }
        public List<ListingModel> ColorList { get; set; }
        public List<ListingModel> ClarityList { get; set; }
        public List<ListingModel> CutList { get; set; }
        public List<ListingModel> PolishList { get; set; }
        public List<ListingModel> SymmList { get; set; }
        public List<ListingModel> FlsList { get; set; }
        public List<ListingModel> ShadeList { get; set; }
        public List<ListingModel> NattsList { get; set; }
        public List<ListingModel> InclusionList { get; set; }
        public List<ListingModel> LabList { get; set; }
        public List<ListingModel> LocationList { get; set; }
        public List<ListingModel> BGMList { get; set; }
        public List<ListingModel> CrnBlackList { get; set; }
        public List<ListingModel> CrnWhiteList { get; set; }
        public List<ListingModel> TblBlackList { get; set; }
        public List<ListingModel> TblWhiteList { get; set; }
        public List<SelectListItem> ExportTypeList { get; set; }
        public List<SelectListItem> OccuranceList { get; set; }
        public List<SelectListItem> SeparatorList { get; set; }
        public List<SelectListItem> PricingMethodList { get; set; }
        public List<SelectListItem> VendorList { get; set; }
        public List<SelectListItem> FtpTypeList { get; set; }
        public List<ListingModel> TblOpenList { get; set; }
        public List<ListingModel> CrnOpenList { get; set; }
        public List<ListingModel> PavOpenList { get; set; }
        public List<ListingModel> GrdleOpenList { get; set; }
        public List<ColumnsSettingsModel> ColumnList { get; set; }
        public List<APIFiltersSettingsModel> APIFilters { get; set; }

        public UploadMethodModel()
        {
            SupplierList = new List<ListingModel>();
            ShapeList = new List<ListingModel>();
            PointerList = new List<ListingModel>();
            ColorList = new List<ListingModel>();
            ClarityList = new List<ListingModel>();
            CutList = new List<ListingModel>();
            PolishList = new List<ListingModel>();
            SymmList = new List<ListingModel>();
            FlsList = new List<ListingModel>();
            ShadeList = new List<ListingModel>();
            NattsList = new List<ListingModel>();
            InclusionList = new List<ListingModel>();
            LabList = new List<ListingModel>();
            LocationList = new List<ListingModel>();
            OccuranceList = new List<SelectListItem>();
            ExportTypeList = new List<SelectListItem>();
            SeparatorList = new List<SelectListItem>();
            For_iUserIds = new List<SelectListItem>();
            VendorList = new List<SelectListItem>();

            ColumnList = new List<ColumnsSettingsModel>();
            APIFilters = new List<APIFiltersSettingsModel>();
        }
    }
    public class ListingModel
    {
        public long iSr { get; set; }
        public string sName { get; set; }
        public bool isActive { get; set; }
    }
    public class ColumnsSettingsModel
    {
        public bool IsActive { get; set; }
        public int icolumnId { get; set; }
        public int iPriority { get; set; }
        public string sCustMiseCaption { get; set; }
        public string sUser_ColumnName { get; set; }
    }
    public class APIFiltersSettingsModel
    {
        public long? Id { get; set; }
        public long Sr { get; set; }
        public long iTransId { get; set; }
        public string iVendor { get; set; }
        public string iLocation { get; set; }
        public string sShape { get; set; }
        public string sPointer { get; set; }
        public string sColorType { get; set; }
        public string sColor { get; set; }
        public string sINTENSITY { get; set; }
        public string sOVERTONE { get; set; }
        public string sFANCY_COLOR { get; set; }
        public string MixColor { get; set; }
        public string sClarity { get; set; }
        public string sCut { get; set; }
        public string sPolish { get; set; }
        public string sSymm { get; set; }
        public string sFls { get; set; }
        public string sLab { get; set; }

        public string LengthTitle { get; set; }
        public string dFromLength { get; set; }
        public string dToLength { get; set; }
        
        public string WidthTitle { get; set; }
        public string dFromWidth { get; set; }
        public string dToWidth { get; set; }

        public string DepthTitle { get; set; }
        public string dFromDepth { get; set; }
        public string dToDepth { get; set; }
        
        public string DepthPerTitle { get; set; }
        public string dFromDepthPer { get; set; }
        public string dToDepthPer { get; set; }

        public string TablePerTitle { get; set; }
        public string dFromTablePer { get; set; }
        public string dToTablePer { get; set; }

        public string CrAngTitle { get; set; }
        public string dFromCrAng { get; set; }
        public string dToCrAng { get; set; }

        public string CrHtTitle { get; set; }
        public string dFromCrHt { get; set; }
        public string dToCrHt { get; set; }

        public string PavAngTitle { get; set; }
        public string dFromPavAng { get; set; }
        public string dToPavAng { get; set; }

        public string PavHtTitle { get; set; }
        public string dFromPavHt { get; set; }
        public string dToPavHt { get; set; }

        public string KeyToSymbolTitle { get; set; }
        public string dKeyToSymbol { get; set; }
        public string dCheckKTS { get; set; }
        public string dUNCheckKTS { get; set; }
        public string sBGM { get; set; }
        public string sCrownBlack { get; set; }
        public string sTableBlack { get; set; }
        public string sCrownWhite { get; set; }
        public string sTableWhite { get; set; }
        public string sTableOpen { get; set; }
        public string sCrownOpen { get; set; }
        public string sPavOpen { get; set; }
        public string sGirdleOpen { get; set; }
        public string Img { get; set; }
        public string Vdo { get; set; }
        public string PriceMethod { get; set; }
        public double? PricePer { get; set; }

        public Boolean? Length_IsBlank { get; set; }
        public Boolean? Width_IsBlank { get; set; }
        public Boolean? Depth_IsBlank { get; set; }
        public Boolean? DepthPer_IsBlank { get; set; }
        public Boolean? TablePer_IsBlank { get; set; }
        public Boolean? CrAng_IsBlank { get; set; }
        public Boolean? CrHt_IsBlank { get; set; }
        public Boolean? PavAng_IsBlank { get; set; }
        public Boolean? PavHt_IsBlank { get; set; }
        public Boolean? KTS_IsBlank { get; set; }
    }
    public class KeyToSymbolResponse
    {
        public string sSymbol { get; set; }
    }
    public class ColumnsUserResponse
    {
        public long iSr { get; set; }
        public long iUserid { get; set; }
        public string sFullName { get; set; }
        public string sCompName { get; set; }
        public string sUsername { get; set; }
    }
    public class UserwiseCompany_select
    {
        public int iUserid { get; set; }
        public string CompanyName { get; set; }
    }
    public class ApiColumns
    {
        public int iid { get; set; }
        public string caption { get; set; }
        public string Heading { get; set; }
        public int icolumnId { get; set; }
        public int iPriority { get; set; }
        public bool IsActive { get; set; }
        public string sCustMiseCaption { get; set; }
        public string sUser_ColumnName { get; set; }
        public string Display_Order { get; set; }
    }
    public class SaveLab_Req
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExportType { get; set; }
        public Boolean APIStatus { get; set; }
        public string APIName { get; set; }
        public int For_iUserId { get; set; }
        public int iTransId { get; set; }
        public string APIUrl { get; set; }
        public List<SaveLab_Filters> Filters { get; set; }
        public List<SaveLab_ColumnsSettings> ColumnsSettings { get; set; }
        public string Type { get; set; }
        public SaveLab_Req()
        {
            Filters = new List<SaveLab_Filters>();
            ColumnsSettings = new List<SaveLab_ColumnsSettings>();
        }
    }
    public class SaveLab_Filters
    {
        public long? Id { get; set; }
        public string Sr { get; set; }
        public string iVendor { get; set; }
        public string iLocation { get; set; }
        public string sShape { get; set; }
        public string sPointer { get; set; }
        public string sColorType { get; set; }
        public string sColor { get; set; }
        public string sINTENSITY { get; set; }
        public string sOVERTONE { get; set; }
        public string sFANCY_COLOR { get; set; }
        public string sClarity { get; set; }
        public string sCut { get; set; }
        public string sPolish { get; set; }
        public string sSymm { get; set; }
        public string sFls { get; set; }
        public string sLab { get; set; }

        public Single? dFromLength { get; set; }
        public Single? dToLength { get; set; }
        public Boolean? Length_IsBlank { get; set; }
        public Single? dFromWidth { get; set; }
        public Single? dToWidth { get; set; }
        public Boolean? Width_IsBlank { get; set; }
        public Single? dFromDepth { get; set; }
        public Single? dToDepth { get; set; }
        public Boolean? Depth_IsBlank { get; set; }
        public Single? dFromDepthPer { get; set; }
        public Single? dToDepthPer { get; set; }
        public Boolean? DepthPer_IsBlank { get; set; }
        public Single? dFromTablePer { get; set; }
        public Single? dToTablePer { get; set; }
        public Boolean? TablePer_IsBlank { get; set; }
        public Single? dFromCrAng { get; set; }
        public Single? dToCrAng { get; set; }
        public Boolean? CrAng_IsBlank { get; set; }
        public Single? dFromCrHt { get; set; }
        public Single? dToCrHt { get; set; }
        public Boolean? CrHt_IsBlank { get; set; }
        public Single? dFromPavAng { get; set; }
        public Single? dToPavAng { get; set; }
        public Boolean? PavAng_IsBlank { get; set; }
        public Single? dFromPavHt { get; set; }
        public Single? dToPavHt { get; set; }
        public Boolean? PavHt_IsBlank { get; set; }
        public string dKeyToSymbol { get; set; }
        public string dCheckKTS { get; set; }
        public string dUNCheckKTS { get; set; }
        public Boolean? KTS_IsBlank { get; set; }

        public string sBGM { get; set; }
        public string sCrownBlack { get; set; }
        public string sTableBlack { get; set; }
        public string sCrownWhite { get; set; }
        public string sTableWhite { get; set; }
        public string sTableOpen { get; set; }
        public string sCrownOpen { get; set; }
        public string sPavOpen { get; set; }
        public string sGirdleOpen { get; set; }
        public string Img { get; set; }
        public string Vdo { get; set; }
        public string PriceMethod { get; set; }
        public double? PricePer { get; set; }
    }
    public class SaveLab_ColumnsSettings
    {
        public int iTransId { get; set; }
        public int icolumnId { get; set; }
        public string sUser_ColumnName { get; set; }
        public string sCustMiseCaption { get; set; }
        public int iPriority { get; set; }
        public bool IsActive { get; set; }
        public string sColumnName { get; set; }
        public string sCaption { get; set; }
        public string sUserCaption { get; set; }
        public int iSeqNo { get; set; }
    }
    public class GetLab_Request
    {
        public string sTransId { get; set; }
        public string dtFromDate { get; set; }
        public string dtToDate { get; set; }
        public string sSearch { get; set; }
        public string sPgNo { get; set; }
        public string sPgSize { get; set; }
        public string OrderBy { get; set; }
        public string UserId { get; set; }
    }
    public class GetLab_Response
    {
        public long iTotalRec { get; set; }
        public long iSr { get; set; }
        public long iTransId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExportType { get; set; }
        public string APIName { get; set; }
        public int For_iUserId { get; set; }
        public string APIUrl { get; set; }
        public Boolean APIStatus { get; set; }
        public int iUserId { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string For_FullName { get; set; }
        public string For_Username { get; set; }
        public string For_CompName { get; set; }
        public string AssistBy { get; set; }
        public List<APIFiltersSettingsModel> Filters { get; set; }
        public List<ColumnsSettingsModel> ColumnsSettings { get; set; }
        public string Type { get; set; }
        public GetLab_Response()
        {
            Filters = new List<APIFiltersSettingsModel>();
            ColumnsSettings = new List<ColumnsSettingsModel>();
        }
    }
    public class LabStockDownload_Req
    {
        public string UN { get; set; }
        public string Username { get; set; }
        public string PD { get; set; }
        public string Password { get; set; }
        public int TransId { get; set; }
        public string IPAddress { get; set; }
    }
    public class Lab_Column_Auto_Select_Req
    {
        public string Type { get; set; }
    }
    public class LabDataUpload_Ora_Req
    {
        public string Type { get; set; }
        public string DataTransferType { get; set; }
    }
}
