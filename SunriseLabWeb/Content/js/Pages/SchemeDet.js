var Customer_Id = "", MemberId = "";
var SchemeDet = [];
function MemberIdSearch(CustomerId) {
    Customer_Id = CustomerId;

    if (Customer_Id != "" || $("#txtMemberId").val() != "") {
        loaderShow();
        var data = {};
        data.MemberId = $("#txtMemberId").val();
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

                    $("#txtMemberId").val(data.Data[0].MemberId);
                    $("#txtMemberName").val(data.Data[0].Customer_Name);
                    $("#txtAddress").val(data.Data[0].Address);
                    $("#txtPhoneno").val(data.Data[0].Mobile);

                    //if (data.Data[0].SchemeDetAvail == true) {
                    MemberWise_SchemeDet_Get(MemberId, Customer_Id);
                    //}
                } else {
                    Reset();
                    toastr.error("Member Id Not Found or Primary Id Not Exists");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                loaderHide();
                toastr.error(textStatus);
            }
        });
    }
}
function MemberWise_SchemeDet_Get(MemberId, CustomerId) {
    loaderShow();
    var data = {};
    data.MemberId = MemberId;
    data.Customer_Id = CustomerId;

    $.ajax({
        type: 'POST',
        url: '/Master/MemberWise_SchemeDet_Get',
        data: data,
        dataType: "json",
        success: function (data) {
            loaderHide();
            if (data.Status == "1" && data.Data != null && data.Data.length > 0) {
                $("#divScheme").show();
                $('#myTableBody').html("");
                $("#divButton").show();
                SchemeDet = data.Data;

                var iSr = document.getElementById('myTableBody').rows.length;
                _(data.Data).each(function (obj, i) {
                    iSr = iSr + 1;
                    $('#myTableBody').append(
                        '<tr>' +
                        '<td>' + iSr + '</td>' +
                        '<td>' + obj.SchemeName + '</td>' +
                        '<td>' + obj.SchemeType + '</td>' +
                        '<td>' +
                        '<input type="hidden" class="hdnSchemeId" value="' + obj.SchemeId + '" />' +
                        '<input type="text" maxlength="7" Validity="' + obj.Validity + '" AllocatedPkg="' + obj.AllocatedPkg + '" onblur="LenCheckr(\'' + obj.SchemeId + '\')" id="txtpkg' + obj.SchemeId + '" class="form-control txtpkg" value="' + (obj.Pkg != null ? obj.Pkg : '') + '" autocomplete="off" style="height: 30px;">' +
                        '</td>' +
                        '</tr>');
                });
                
                $(".txtpkg").keypress(function (e) {
                    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                        return false;
                    }
                });

            } else {
                toastr.error("Scheme(s) Not Exists");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            loaderHide();
            toastr.error(textStatus);
        }
    });
}
function Reset() {
    Customer_Id = "", MemberId = "";
    $("#txtMemberName").val("");
    $("#txtAddress").val("");
    $("#txtPhoneno").val("");
    $("#divScheme").hide();
    $('#myTableBody').html("");
    $("#divButton").hide();
}
function ResetAll() {
    $("#txtMemberId").val("");
    Reset();
    $("#divButton").hide();
}
function Back() {
    window.location.href = '/Master/SchemeMas';
}
function SaveData() {
    var List = [];
    $("#mytable #myTableBody tr").each(function () {
        if ($(this).find('.txtpkg').val() != "") {
            List.push({
                CustomerId: Customer_Id,
                MemberId: MemberId,
                SchemeId: $(this).find('.hdnSchemeId').val(),
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
            url: "/Master/MemberWise_SchemeDet_Save",
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
                    toastr.success("Scheme Detail Save Successfully");
                    setTimeout(function () {
                        window.location.href = "/Master/SchemeMas";
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
function LenCheckr(id) {
    
    var txtpkg = $("#txtpkg" + id).val() == '' ? 0 : $("#txtpkg" + id).val();

    if ($("#txtpkg" + id).attr('validity') == "false" && parseInt($("#txtpkg" + id).attr('allocatedpkg')) > 0) {
        debugger
        if (txtpkg == 0 || parseInt($("#txtpkg" + id).attr('allocatedpkg')) > txtpkg) {
            debugger
            _(SchemeDet).each(function (obj, i) {
                debugger
                if (obj.SchemeId == id) {
                    debugger
                    setTimeout(function () {
                        toastr.error("Please Delete Transaction Record");
                    }, 50);
                    return $("#txtpkg" + id).val((obj.Pkg != null ? obj.Pkg : ''));
                }
            });
        }
    }
    else if ($("#txtpkg" + id).val() == "") {
        setTimeout(function () {
            toastr.error("Allowed only greater than 0");
        }, 50);
        //return $("#txtpkg" + id).val("");
        $("#txtpkg" + id).val("");
    }

    debugger
    _(SchemeDet).each(function (obj, i) {
        debugger
        if (obj.SchemeId == id) {
            debugger
            obj.Pkg = $("#txtpkg" + id).val();
        }
    });
    debugger
    return $("#txtpkg" + id).val($("#txtpkg" + id).val());
}
$(document).ready(function () {
    if (getParameterByName("C") != null) {
        MemberIdSearch(getParameterByName("C"));
    }
});