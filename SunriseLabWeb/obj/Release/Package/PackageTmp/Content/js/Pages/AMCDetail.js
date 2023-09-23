$(document).ready(function (e) {
    $(".amcapply").hide();
    Year_Mas();
});
function Year_Mas() {
    loaderShow();

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
function ddlYearChange() {
    if ($("#ddlYear").val() != "") {
        loaderShow();
        var data = {};
        data.Year_Code = $("#ddlYear").val();
        data.Assigned = $("#ddlAMCCharges").val();

        $.ajax({
            type: 'POST',
            url: '/Master/CustomerWise_AMC_Detail_Get',
            data: data,
            dataType: "json",
            success: function (data) {
                loaderHide();
                if (data.Status == "1" && data.Data != null && data.Data.length > 0) {
                    $("#divMem").show();
                    $(".amcapply").show();
                    $('#divMem #myTableBody').html("");
                    $("#divButton").show();
                    SchemeDet = data.Data;

                    var iSr = document.getElementById('myTableBody').rows.length;
                    _(data.Data).each(function (obj, i) {
                        iSr = iSr + 1;
                        $('#divMem #myTableBody').append(
                            '<tr>' +
                                '<td>' + iSr + '</td>' +
                                '<td>' + obj.MemberId + '</td>' +
                                '<td>' + obj.Customer_Name + '</td>' +
                                '<td>' +
                                '<input type="hidden" class="hdnCustomer_Id" value="' + obj.Customer_Id + '" />' +
                                '<input ' + (obj.Pay_Start == true ? "disabled" : '') + ' type="text" maxlength="10" id="txtAMC_' + obj.Customer_Id + '" class="form-control AMCAmount" value="' + (obj.AMCAmount != null && obj.AMCAmount != 0 ? obj.AMCAmount : '') + '" autocomplete="off" style="height: 30px;">' +
                                '</td>' +
                                '</tr>');
                    });

                    $(".AMCAmount").keypress(function (e) {
                        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                            return false;
                        }
                    });
                    window.scrollBy(50, 200);
                } else {
                    $("#divMem").hide();
                    $(".amcapply").hide();
                    $('#divMem #myTableBody').html("");
                    toastr.error("Member(s) Not Exists !");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                loaderHide();
                toastr.error(textStatus);
            }
        });
    }
    else {
        $("#divMem").hide();
        $(".amcapply").hide();
        $('#divMem #myTableBody').html("");
    }
}
function Set() {
    if ($("#txtAmt").val() != "") {
        $("#divMem #mytable #myTableBody tr").each(function () {
            if ($(this).find('.AMCAmount').val() == "") {
                $(this).find('.AMCAmount').val($("#txtAmt").val());
            }
        });
        $("#txtAmt").val("");
    }
    else {
        toastr.error("Please set AMC Amount first !");
        $("#txtAmt").focus();
    }
}
function SaveData() {
    var List = [];
    $("#divMem #mytable #myTableBody tr").each(function () {
        if ($(this).find('.AMCAmount').val() != "") {
            List.push({
                Customer_Id: $(this).find('.hdnCustomer_Id').val(),
                Year_Code: $("#ddlYear").val(),
                AMCAmount: $(this).find('.AMCAmount').val()
            });
        }
    });
    if (List.length == 0) {
        toastr.error("Please Set Any one Customer in AMC Amount");
    }
    else {
        var i = 0;
        for (i = 0; i < List.chunk(200).length; i++) {
            var obj = {};
            obj.amcsave = List.chunk(200)[i];

            loaderShow();
            $.ajax({
                url: "/Master/AMC_Detail_Save",
                async: true,
                type: "POST",
                dataType: "json",
                data: JSON.stringify({ req: obj }),
                contentType: "application/json; charset=utf-8",
                success: function (data, textStatus, jqXHR) {
                    loaderHide();
                    if (data.Status == "0") {
                        toastr.error(data.Message);
                    }
                    //else if (data.Status == "1") {
                    //}
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    loaderHide();
                    toastr.error(textStatus);
                }
            });
        }

        if (List.chunk(200).length == i) {
            toastr.success("AMC Detail Save Successfully");
            //setTimeout(function () {
            $("#ddlAMCCharges").val("");
            ddlYearChange();
            //}, 2000);
        }
    }
}
Object.defineProperty(Array.prototype, 'chunk', {
    value: function (chunkSize) {
        var R = [];
        for (var i = 0; i < this.length; i += chunkSize)
            R.push(this.slice(i, i + chunkSize));
        return R;
    }
});