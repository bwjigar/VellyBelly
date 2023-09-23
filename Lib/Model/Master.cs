using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Model
{
    public class MemberIdSearch_Req
    {
        public string Type { get; set; }
        public string MemberId { get; set; }
        public int Customer_Id { get; set; }
    }
    public class MemberIdSearch_Res
    {
        public string MemberId { get; set; }
        public int Customer_Id { get; set; }
        public string Customer_Name { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimaryMember { get; set; }
        public bool SchemeDetAvail { get; set; }
    }
    public class MemberWise_SchemeDet_Get_Res
    {
        public string Date { get; set; }
        public int SchemeId { get; set; }
        public string SchemeName { get; set; }
        public string SchemeType { get; set; }
        public bool Validity { get; set; }
        public string MemberId { get; set; }
        public int Customer_Id { get; set; }
        public string Pkg { get; set; }
        public string AllocatedPkg { get; set; }
    }
    public class MemberWise_SchemeDet_Save_Req
    {
        public List<ObjMemberWise_SchemeDet> schemesave { get; set; }
        public MemberWise_SchemeDet_Save_Req()
        {
            schemesave = new List<ObjMemberWise_SchemeDet>();
        }
    }
    public class ObjMemberWise_SchemeDet
    {
        public string CustomerId { get; set; }
        public string MemberId { get; set; }
        public string SchemeId { get; set; }
        public string Pkg { get; set; }
    }
    public class MemberWise_SchemeDet_Select_Req
    {
        public string sPgNo { get; set; }
        public string sPgSize { get; set; }
        public string OrderBy { get; set; }
    }
    public class MemberWise_SchemeDet_Select_Res
    {
        public long iTotalRec { get; set; }
        public long iSr { get; set; }
        public int Customer_Id { get; set; }
        public string MemberId { get; set; }
        public string Customer_Name { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string EntryDate { get; set; }
        public bool SchemeAllocated { get; set; }
    }
    public class Year_Mas_Res
    {
        public long Year_Code { get; set; }
        public string Year { get; set; }
        public DateTime  From_Date { get; set; }
        public DateTime To_Date { get; set; }
    }
    public class Bank_Mas_Res
    {
        public int Bank_Code { get; set; }
        public string Bank_Name { get; set; }
    }
    public class CustomerWise_AMC_Detail_Get_Req
    {
        public int Year_Code { get; set; }
        public string Assigned { get; set; }
    }
    public class CustomerWise_AMC_Detail_Get_Res
    {
        public int Customer_Id { get; set; }
        public string Customer_Name { get; set; }
        public string MemberId { get; set; }
        public decimal AMCAmount { get; set; }
        public int Year_Code { get; set; }
        public bool Pay_Start { get; set; }
    }
    public class AMC_Detail_Save_Req
    {
        public List<ObjAMC_Detail_Save> amcsave { get; set; }
        public AMC_Detail_Save_Req()
        {
            amcsave = new List<ObjAMC_Detail_Save>();
        }
    }
    public class ObjAMC_Detail_Save
    {
        public string Customer_Id { get; set; }
        public string Year_Code { get; set; }
        public string AMCAmount { get; set; }
    }
    public class AMC_Payment_Receipt_Save_Req
    {
        public int Customer_Id { get; set; }
        public string MemberId { get; set; }
        public int Year_Code { get; set; }
        public decimal Amt_To_Pay { get; set; }
        public decimal Paid_Amt { get; set; }
        public int EntryBy { get; set; }
    }
    public class AMC_Payment_Receipt_Get_Req
    {
        public int Customer_Id { get; set; }
        public string MemberId { get; set; }
        public int Year_Code { get; set; }
    }
    public class AMC_Payment_Receipt_Get_Res
    {
        public long iSr { get; set; }
        public int Id { get; set; }
        public int Customer_Id { get; set; }
        public string MemberId { get; set; }
        public int Year_Code { get; set; }
        public string Payment_Date { get; set; }
        public decimal Amt_To_Pay { get; set; }
        public decimal Paid_Amt { get; set; }
    }

}
