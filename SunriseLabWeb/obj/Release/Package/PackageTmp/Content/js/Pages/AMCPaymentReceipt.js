//var Bank_Mas = [];
var Customer_Id = "", MemberId = "", Year_Code = "", AMCAmount = "";

$(document).ready(function (e) {
    Mas();
    GetCustomer_List();
    $("#txtCustName").focusout(function () {
        CustNameSelectRequired();
    });

});
function Mas() {
    loaderShow();

    //$.ajax({
    //    type: 'POST',
    //    url: '/Master/Bank_Mas',
    //    dataType: "json",
    //    success: function (data) {
    //        Bank_Mas = [];
    //        if (data.Status == "1" && data.Data != null) {
    //            Bank_Mas = data.Data;
    //        }
    //    },
    //    error: function (jqXHR, textStatus, errorThrown) {
    //        toastr.error(textStatus);
    //    }
    //});

    $.ajax({
        type: 'POST',
        url: '/Master/Year_Mas',
        dataType: "json",
        success: function (data) {
            loaderHide();
            if (data.Status == "1" && data.Data != null) {
                $('#ddlYear').html('<option value="">Select</option>');
                _(data.Data).each(function (obj, i) {
                    $('#ddlYear').append('<option value="' + obj.Year_Code + '">' + obj.Year + '</option>');
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            loaderHide();
            toastr.error(textStatus);
        }
    });
}
var CustList = [];
function GetCustomer_List() {
    $('#divpayment #myTableBody').html("");
    $("#divpayment").hide();

    if ($("#ddlYear").val() == "") {
        ejAutocomplete([]);
        CustnmRst();
        CustNameSelectRequired();

        $('#divpayment #myTableBody').html("");
        $("#divpayment").hide();
    }
    else {
        loaderShow();
        setTimeout(function () {
            var data = {};
            data.Year_Code = $("#ddlYear").val();
            data.Assigned = "1";

            $.ajax({
                url: '/Master/CustomerWise_AMC_Detail_Get',
                async: false,
                type: "POST",
                data: data,
                success: function (data, textStatus, jqXHR) {
                    loaderHide();
                    if (data.Data != null) {
                        CustList = data.Data;

                        CustnmRst();
                        CustNameSelectRequired();

                        for (var i = 0; i < CustList.length; i++) {
                            CustList[i].Customer_Id = CustList[i].Customer_Id + "__" + CustList[i].Year_Code;
                        }
                        ejAutocomplete(CustList);
                    }
                }
            });

        }, 50);
    }
}
function ejAutocomplete(CustList) {
    $('#txtCustName').ejAutocomplete({
        dataSource: CustList,
        filterType: 'contains',
        fields: { key: "Customer_Id" },
        highlightSearch: true,
        watermarkText: "Search with Member Id, Customer Name",
        width: "100%",
        showPopupButton: true,
        popupHeight: '300px',
        showRoundedCorner: true,
        emptyResultText: 'No Customer Found',
        multiColumnSettings: {
            enable: true,
            showHeader: true,
            stringFormat: "{1}",
            searchColumnIndices: [0, 1],
            columns: [
                { "field": "MemberId", "headerText": "Member Id" },
                { "field": "Customer_Name", "headerText": "Customer Name" }
            ]
        },
        close: function (arg) {
            Customer_Id = $("#txtCustName_hidden").val().split("__")[0];
            Year_Code = $("#txtCustName_hidden").val().split("__")[1];

            for (var i = 0; i < CustList.length; i++) {
                if ($("#txtCustName_hidden").val() == CustList[i].Customer_Id) {
                    MemberId = CustList[i].MemberId;
                }
            }
            if (Customer_Id != "" && MemberId != "" && Year_Code != "") {
                AMC_Payment_Receipt_Get();
            }
        }
    });
}
function CustnmRst() {
    $("#txtCustName_hidden").val("");
    $('#divpayment #myTableBody').html("");
    $("#divpayment").hide();

    Customer_Id = "", MemberId = "", Year_Code = "", AMCAmount = "";
}
function CustNameSelectRequired() {
    if ($("#txtCustName_hidden").val() != undefined) {
        setTimeout(function () {
            if ($("#txtCustName_hidden").val().split("__").length != 2) {
                $("#txtCustName").val("");
                $("#txtCustName_hidden").val("");
            }
        }, 250);
    }
}
function AMC_Payment_Receipt_Get() {
    if (Customer_Id != "" && MemberId != "" && Year_Code != "") {
        loaderShow();
        var data = {};
        data.Customer_Id = Customer_Id;
        data.MemberId = MemberId;
        data.Year_Code = Year_Code;

        for (var i = 0; i < CustList.length; i++) {
            if ($("#txtCustName_hidden").val() == CustList[i].Customer_Id) {
                AMCAmount = CustList[i].AMCAmount;
            }
        }

        $.ajax({
            url: '/Master/AMC_Payment_Receipt_Get',
            async: false,
            type: "POST",
            data: data,
            success: function (data, textStatus, jqXHR) {
                loaderHide();
                $("#divpayment").show();
                $('#divpayment #myTableBody').html("");

                if (data.Data != null && data.Data.length > 0) {
                    _(data.Data).each(function (obj, i) {
                        AMCAmount = parseFloat(AMCAmount) - parseFloat(obj.Paid_Amt);
                    });
                    
                    if (AMCAmount > 0) {
                        $('#divpayment #myTableBody').html(
                            '<tr>' +
                            '<td style="text-align: center;">' + SetCurrentDate() + '</td>' +
                            '<td style="text-align: center;">' + MemberId + '</td>' +
                            '<td style="text-align: right;">' + formatNumber(AMCAmount) + '</td>' +
                            '<td>' +
                            '<input type="text" onkeyup="LenCheckr(\'' + AMCAmount + '\')"  maxlength="10" id="txt_PaidAmt_1" class="form-control form-control-sm PaidAmt" value="" autocomplete="off">' +
                            '</td>' +
                            '</tr>');
                        $("#btnSave").show();
                    }
                    else {
                        $("#btnSave").hide();
                    }

                    _(data.Data).each(function (obj, i) {
                        $('#divpayment #myTableBody').append(
                            '<tr>' +
                            '<td style="text-align: center;">' + obj.Payment_Date + '</td>' +
                            '<td style="text-align: center;">' + obj.MemberId + '</td>' +
                            '<td style="text-align: right;">' + formatNumber(obj.Amt_To_Pay) + '</td>' +
                            '<td style="text-align: right;">' + formatNumber(obj.Paid_Amt) + '</td>' +
                            '</tr>');
                    });
                }
                else {
                    $('#divpayment #myTableBody').html(
                        '<tr>' +
                        '<td style="text-align: center;">' + SetCurrentDate() + '</td>' +
                        '<td style="text-align: center;">' + MemberId + '</td>' +
                        '<td style="text-align: right;">' + formatNumber(AMCAmount) + '</td>' +
                        '<td>' +
                        '<input type="text" onkeyup="LenCheckr(\'' + AMCAmount + '\')"  maxlength="10" id="txt_PaidAmt_1" class="form-control form-control-sm PaidAmt" value="" autocomplete="off">' +
                        '</td>' +
                        '</tr>');
                    $("#btnSave").show();
                }
                $("#txt_PaidAmt_1").keypress(function (e) {
                    //if the letter is not digit then display error and don't type anything 
                    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                        return false;
                    }
                });
            }
        });
    }
}
function LenCheckr(AMCAmount) {
    if (parseFloat($("#txt_PaidAmt_1").val()) == "0") {
        return $("#txt_PaidAmt_1").val("");
    }
    else {
        if (parseFloat($("#txt_PaidAmt_1").val()) > parseFloat(AMCAmount)) {
            setTimeout(function () {
                toastr.error("Allowed only less than or equals to AMC Amount");
            }, 50);
            return $("#txt_PaidAmt_1").val("");
        }
    }
}
function SaveData() {
    if (parseFloat($("#txt_PaidAmt_1").val()) == "0") {
        setTimeout(function () {
            toastr.error("Allowed only greater than 0");
        }, 50);
        return $("#txt_PaidAmt_0").val("");
    }
    else {
        loaderShow();
        var data = {};
        data.Customer_Id = Customer_Id;
        data.MemberId = MemberId;
        data.Year_Code = Year_Code;
        data.Amt_To_Pay = AMCAmount;
        data.Paid_Amt = $("#txt_PaidAmt_1").val();

        $.ajax({
            url: '/Master/AMC_Payment_Receipt_Save',
            async: false,
            type: "POST",
            data: data,
            success: function (data, textStatus, jqXHR) {
                loaderHide();
                if (data.Status == "0") {
                    toastr.error(data.Message);
                }
                else if (data.Status == "1") {
                    toastr.success(data.Message);
                    setTimeout(function () {
                        AMC_Payment_Receipt_Get();
                    }, 1000);
                }
            }
        });
    }
}
var SetCurrentDate = function () {
    var m_names = new Array("Jan", "Feb", "Mar",
        "Apr", "May", "Jun", "Jul", "Aug", "Sep",
        "Oct", "Nov", "Dec");
    var d = new Date();
    var curr_date = (d.getDate() < 10 ? '0' + d.getDate() : d.getDate())
    var curr_month = d.getMonth();
    var curr_year = d.getFullYear();
    var FinalDate = (curr_date + "-" + m_names[curr_month]
        + "-" + curr_year);

    return FinalDate;
}
function formatNumber(number) {
    return (parseFloat(number).toFixed(2)).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
}