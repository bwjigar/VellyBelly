var MemberIdList = [];
var SchemeIdList = [];
var SchemeAlloc_Get = [];
var Customer_Id = "", MemberId = "";
var ddlScheme = "";
var row_cnt = 0;
var row = $('#myTableBody').find('tr').length;

function Back() {
    window.location.href = "/Master/MemSchemeAllocMas";
}
function ddlMemberIdFill() {
    loaderShow();

    var obj = {};
    obj.sPgNo = 0;
    $.ajax({
        type: 'POST',
        url: '/Master/MemberWise_SchemeDet_Select',
        data: obj,
        dataType: "json",
        success: function (data) {
            loaderHide();
            if (data.Status == "1" && data.Data != null) {
                MemberIdList = data.Data;

                $('#ddlMemberId').html('<option value="">Select</option>');
                _(data.Data).each(function (obj, i) {
                    $('#ddlMemberId').append('<option value="' + obj.MemberId + '">' + obj.MemberId + '</option>');
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            loaderHide();
            toastr.error(textStatus);
        }
    });
}
function ddlMemberIdChange() {
    loaderShow();

    setTimeout(function () {
        if ($("#ddlMemberId").val() != "") {
            _(MemberIdList).each(function (obj, i) {
                if (obj.MemberId == $("#ddlMemberId").val()) {
                    MemberId = obj.MemberId;
                    Customer_Id = obj.Customer_Id;

                    $("#txtMemberName").val(obj.Customer_Name);
                    $("#txtAddress").val(obj.Address);
                    $("#txtPhoneno").val(obj.Mobile);

                    SchemeListGet();

                    setTimeout(function () {
                        $('#myTableBody').html("");
                        row = $('#myTableBody').find('tr').length;
                        SchemeAlloc_Get = [];

                        if (obj.SchemeAllocated == false) {
                            AddNewRow();
                        }
                        else {
                            MemberWise_SchemeAlloc_Get();
                        }
                    }, 100);
                }
            });
        }
        else {
            Reset();
        }
        loaderHide();
    }, 100);
}
function SchemeListGet() {
    var obj = {};
    obj.MemberId = MemberId;
    obj.Customer_Id = Customer_Id;

    $.ajax({
        type: 'POST',
        url: '/Master/MemberWise_SchemeDet_Get',
        data: obj,
        dataType: "json",
        success: function (data) {
            loaderHide();
            if (data.Status == "1" && data.Data != null) {
                SchemeIdList = _.filter(data.Data, function (e) { return e.Pkg != null });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            loaderHide();
            toastr.error(textStatus);
        }
    });
}
function AddNewRow() {
    $("#divScheme").show();

    ddlScheme = "<option value=''>Select</option>";
    _(SchemeIdList).each(function (obj, i) {
        ddlScheme += "<option Customer_Id='" + obj.Customer_Id + "' MemberId='" + obj.MemberId + "' SchemeId='" + obj.SchemeId + "' SchemeType='" + obj.SchemeType + "' Validity='" + obj.Validity + "' Pkg='" + obj.Pkg + "' value='" + obj.SchemeId + "'>" + obj.SchemeName + "</option>";
    });

    row = parseInt(row) + 1;

    var id = generateUUID();
    $('#myTableBody').append(
        '<tr>' +
        '<td class="tblbody_sr" style="padding: 0px 0px 11px 12px;">' + row.toString() + '</td>' +
        '<td style="padding: 0px 0px 11px 12px;">' + SetCurrentDate() + '</td>' +
        '<td>' +
        '<select id="ddl' + id + '" onchange="ddlSchemeonChange(\'' + id + '\')" class="col-md-11 form-control form-control-sm ddlSchemeName">' + ddlScheme + '</select>' +
        '</td>' +
        '<td>' +
        '<div class="col-md-12 row">' +
        '<input type="text" id="txtpkg' + id + '" onkeyup="LenCheckr(\'' + id + '\')" maxlength="4" class="col-md-9 form-control form-control-sm txtpkg" value="" autocomplete="off">' +
        '<label style="padding: 8px 0px 0px 8px;" id="lblSType' + id + '"></label>' +
        '</div>' +
        '</td>' +
        '<td style="width: 50px"><center><img class="error RemoveRow" src="/Content/images/trash-delete-icon.png" style="width: 20px;height: auto;border-radius: 0%;cursor:pointer;margin-top: -15px;"/></center></td>' +
        '</tr>');
}
function LenCheckr(id) {
    if (parseFloat($("#txtpkg" + id).val()) == "0") {
        setTimeout(function () {
            toastr.error("Allowed only greater than 0");
        }, 50);
        return $("#txtpkg" + id).val("");
    }
    else {
        if ($("#ddl" + id).find(':selected').attr('validity') == "false") {
            var SchemeName = $("#ddl" + id).find(':selected').html();
            var SchemeType = $("#ddl" + id).find(':selected').attr('schemetype');
            var ValidPkg = parseInt($("#ddl" + id).find(':selected').attr('pkg'));

            var Pkg = 0;
            _(SchemeAlloc_Get).each(function (obj, i) {
                if (obj.SchemeName == SchemeName) {
                    Pkg += parseInt(obj.Pkg);
                }
            });
            $("#mytable #myTableBody tr").each(function () {
                if ($(this).find('.ddlSchemeName').find(':selected').html() == SchemeName) {
                    Pkg += parseInt(($(this).find('.txtpkg').val() == "" ? 0 : $(this).find('.txtpkg').val()));
                }
            });
            if (Pkg > ValidPkg) {
                setTimeout(function () {
                    toastr.error(SchemeName + " Scheme in allowed less than or equals to " + ValidPkg + " " + SchemeType);
                }, 50);
                return $("#txtpkg" + id).val("");
            }
        }
    }
}
function Reset() {
    Customer_Id = "", MemberId = "";
    $("#txtMemberName").val("");
    $("#txtAddress").val("");
    $("#txtPhoneno").val("");
    $("#divScheme").hide();
    $('#myTableBody').html("");
    $("#divButton").hide();
    SchemeIdList = [];
}

function MemberIdSearch(CustomerId) {
    Customer_Id = CustomerId;

    if (Customer_Id != "") {
        loaderShow();
        var data = {};
        data.Customer_Id = Customer_Id;

        $.ajax({
            type: 'POST',
            url: '/Master/CustomerMast_Get_MemberId_Wise',
            data: data,
            dataType: "json",
            success: function (data) {
                loaderHide();
                if (data.Status == "1" && data.Data != null && data.Data.length > 0) {
                    Customer_Id = data.Data[0].Customer_Id;
                    MemberId = data.Data[0].MemberId;

                    if (_.filter(MemberIdList, function (e) { return e.MemberId == data.Data[0].MemberId }).length > 0) {
                        $("#ddlMemberId").val(data.Data[0].MemberId);
                        ddlMemberIdChange();
                    }
                } else {
                    toastr.error("Member Id Not Found");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                loaderHide();
                toastr.error(textStatus);
            }
        });
    }
}
function ddlSchemeonChange(id) {
    $("#txtpkg" + id).val("");
    $("#lblSType" + id).html("");

    if ($("#ddl" + id).val() != "") {
        if ($("#ddl" + id).find(':selected').attr('validity') == "true") {
            $("#txtpkg" + id).val($("#ddl" + id).find(':selected').attr('pkg'));
            document.getElementById("txtpkg" + id).disabled = true;
            $("#lblSType" + id).html($("#ddl" + id).find(':selected').attr('SchemeType'));
        }
        else {
            $("#txtpkg" + id).val('');
            document.getElementById("txtpkg" + id).disabled = false;
            $("#lblSType" + id).html($("#ddl" + id).find(':selected').attr('SchemeType'));
        }
    }
}
function MemberWise_SchemeAlloc_Get() {
    var obj = {};
    obj.MemberId = MemberId;
    obj.Customer_Id = Customer_Id;
    
    $.ajax({
        type: 'POST',
        url: '/Master/MemberWise_SchemeAlloc_Get',
        data: obj,
        dataType: "json",
        success: function (data) {
            loaderHide();
            if (data.Status == "1" && data.Data != null) {
                SchemeAlloc_Get = data.Data;
                $("#divScheme").show();

                _(data.Data).each(function (obj, i) {
                    row = parseInt(row) + 1;
                    var id = generateUUID();
                    $('#myTableBody').append(
                        '<tr>' +
                        '<td class="tblbody_sr" style="padding: 10px 0px 11px 12px;">' + row.toString() + '</td>' +
                        '<td style="padding: 10px 0px 11px 12px;">' + obj.Date + '</td>' +
                        '<td style="padding: 10px 0px 11px 12px;">' + obj.SchemeName + '</td>' +
                        '<td style="padding: 10px 0px 11px 12px;">' + obj.Pkg + ' ' + obj.SchemeType + '</td>' +
                        '<td style="width: 50px"></td>' +
                        '</tr>');
                });
                AddNewRow();
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            loaderHide();
            toastr.error(textStatus);
        }
    });
}

function SaveData() {
    var List = [];
    $("#mytable #myTableBody tr").each(function () {
        if ($(this).find('.ddlSchemeName').val() != undefined && $(this).find('.txtpkg').val() != "" && $(this).find('.ddlSchemeName').val() != "") {
            List.push({
                CustomerId: Customer_Id,
                MemberId: MemberId,
                SchemeId: $(this).find('.ddlSchemeName').val(),
                Pkg: $(this).find('.txtpkg').val()
            });
        }
    });
    
    if (List.length == 0) {
        toastr.error("Please Set Any one Package");
    }
    else {
        loaderShow();

        var obj = {};
        obj.schemesave = List;
        $.ajax({
            url: "/Master/MemberWise_SchemeAllocation_Save",
            async: false,
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ req: obj }),
            contentType: "application/json; charset=utf-8",
            success: function (data, textStatus, jqXHR) {
                loaderHide();
                if (data.Status == "0") {
                    toastr.error(data.Message);
                }
                else if (data.Status == "1") {
                    toastr.success("Member Scheme Allocation Detail Save Successfully");
                    setTimeout(function () {
                        window.location.href = "/Master/MemSchemeAllocMas";
                    }, 2000);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                loaderHide();
                toastr.error(textStatus);
            }
        });
    }
}
$(document).ready(function () {
    ddlMemberIdFill();
    setTimeout(function () {
        if (getParameterByName("C") != null) {
            ddlMemberIdFill();
            MemberIdSearch(getParameterByName("C"));
        }
    }, 100);
    $("#myTableBody").on('click', '.RemoveRow', function () {
        $(this).closest('tr').remove();
        if (parseInt($("#mytable #myTableBody").find('tr').length) == SchemeAlloc_Get.length) {
            AddNewRow();
        }
        row_cnt = 1;
        row = 1;
        $("#mytable #myTableBody tr").each(function () {
            $(this).find("td:eq(0)").html(row_cnt);
            row_cnt += 1;
            row += 1;
        });
        if (row > 0) {
            row = parseInt(row) - 1;
        }
    });
});
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