var pgSize = 50;
var showEntryVar = null;
var total_record = null;
var UserType = null;
var orderBy = '';
var Regular = true, Fancy = false;
var ErrorMsg = [];
$(document).ready(function () {
    BindKeyToSymbolList();
    $('.sym-sec').on('click', function () {
        $('.sym-sec').toggleClass('active');
    });
    $('#btnSave').click(function () {
        GetUserList();
    });
    $('#btnReset').click(function () {
        Reset();
    });
    $("#mytable1").on('click', '.RemoveCriteria', function () {
        $(this).closest('tr').remove();
        if (parseInt($("#mytable1 #myTableBody1").find('tr').length) == 0) {
            $("#mytable1").hide();
            $("#divButton").hide();
        }
        else {
            $("#divButton").show();
        }
        var idd = 1;
        $("#mytable1 #myTableBody1 tr").each(function () {
            $(this).find("th:eq(0)").html(idd);
            idd += 1;
        });
    });
    Bind_RColor();
    FcolorBind();
    $('#ColorModal').on('show.bs.modal', function (event) {
        color_ddl_close();
    });
    $('#chk_Regular_All').change(function () {
        R_F_All_Only_Checkbox_Clr_Rst("-1");
        Regular_All = $(this).is(':checked');
        SetSearchParameter();
    });
    $('#chk_Fancy_All').change(function () {
        R_F_All_Only_Checkbox_Clr_Rst("-1");
        Fancy_All = $(this).is(':checked');
        SetSearchParameter();
    });
    UserDDL();
    $("#mytable tbody").sortable({
        update: function () {
            SetTableOrder();
        }
    });
});
function Back() {
    window.location.href = '/Lab/LabList';
}
var Reset = function () {
    window.location = '/Lab/LabDetail';
}
function SetTableOrder() {
    var OrderNo = 1;
    $("#mytable tbody tr").each(function () {
        ($(this).find(".ColumnOrder").html("<center>" + OrderNo + "</center>"));
        OrderNo = OrderNo + 1;
    });
};
function PricingMethodDDL() {
    if ($("#ddlPriceMethod").val() == "") {
        document.getElementById("txtValue").disabled = true;
        document.getElementById("txtDisc").disabled = true;
        $("#txtValue").show();
        $("#txtDisc").hide();
        $("#txtValue").val("");
    }
    else {
        if ($("#ddlPriceMethod").val() == "Discount") {
            document.getElementById("txtDisc").disabled = false;
            $("#txtDisc").show();
            $("#txtValue").hide();
            $("#txtDisc").val("");
        }
        if ($("#ddlPriceMethod").val() == "Value") {
            document.getElementById("txtValue").disabled = false;
            $("#txtValue").show();
            $("#txtDisc").hide();
            $("#txtValue").val("");
        }
    }
    $("#lblPMErr").html("");
    $("#lblPMErr").hide();
}
function DisValValid(type, evt) {
    if (type == "Value") {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
    if (type == "Discount") {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode != 46 && charCode != 45 && charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
}
function LenCheckr(type) {
    if (type == "Value") {
        if (parseFloat($("#txtValue").val()) > 100 || parseFloat($("#txtValue").val()) < 0) {
            setTimeout(function () {
                $("#lblPMErr").html("Allowed only 0.00 to 100 percent.");
                $("#lblPMErr").show();
            }, 50);
            return $("#txtValue").val("");
        }
        else if (parseFloat($("#txtValue").val()) == NaN) {
            setTimeout(function () {
                $("#lblPMErr").html("Allowed only 0.00 to 100 percent.");
                $("#lblPMErr").show();
            }, 50);
            return $("#txtValue").val("");
        }
        else {
            $("#lblPMErr").html("");
            $("#lblPMErr").hide();
        }
    }
    if (type == "Discount") {
        if (parseFloat($("#txtDisc").val()) < -100 || parseFloat($("#txtDisc").val()) > -0) {
            setTimeout(function () {
                $("#lblPMErr").html("Allowed only -0.00 to -100 percent.");
                $("#lblPMErr").show();
            }, 50);
            return $("#txtDisc").val("");
        }
        else if (parseFloat($("#txtDisc").val()) == NaN) {
            setTimeout(function () {
                $("#lblPMErr").html("Allowed only -0.00 to -100 percent.");
                $("#lblPMErr").show();
            }, 50);
            return $("#txtDisc").val("");
        }
        else {
            $("#lblPMErr").html("");
            $("#lblPMErr").hide();
        }
    }
}
var ModalShow = function (Title, ParameterLabel, ObjLst, DivSpace) {
    if (ParameterLabel == "Vendor") {
        if (!$("#ModalInner").hasClass("modal-lg")) {
            $("#ModalInner").addClass("modal-lg");
        }
    }
    else {
        if ($("#ModalInner").hasClass("modal-lg")) {
            $("#ModalInner").removeClass("modal-lg");
        }
    }
    $('#exampleModalLabel').text(Title);
    $('#divModal').removeClass("ng-hide").addClass("ng-show");

    var content = '<ul id="popupul" class="color-whit-box">';
    var c = 0, IsAllActiveC = 0;
    var list = [];
    list = ObjLst;
    list.forEach(function (item) {
        content += '<li id="li_' + ParameterLabel + '_' + c + '" onclick="ItemClicked(\'' + ParameterLabel + '\',\'' + item.sName + '\',\'' + c + '\', this);" class="';
        if (item.isActive) {
            content += 'active';

            if (ParameterLabel == "Vendor" || ParameterLabel == "Location" || ParameterLabel == "Shape" || ParameterLabel == "Color"
                || ParameterLabel == "Clarity" || ParameterLabel == "Cut") {
                IsAllActiveC = parseInt(IsAllActiveC) + 1;
            }
        }
        content += '">' + item.sName + '</li>';
        c = parseInt(c) + 1;
    });
    content += '</ul>';
    $('#divModal').empty();
    $('#divModal').append(content);

    $("#mpdal-footer").append('<button type="button" class="btn btn-primary" ng-click="ResetSelectedAttr(' + ParameterLabel + ');">Reset</button><button type="button" class="btn btn-secondary" data-dismiss="modal">Done</button>');

    if (ParameterLabel == "Vendor" || ParameterLabel == "Location" || ParameterLabel == "Shape" || ParameterLabel == "Color"
        || ParameterLabel == "Clarity" || ParameterLabel == "Cut") {
        if (IsAllActiveC == ObjLst.length - 1) {
            $("#li_" + ParameterLabel + "_0").addClass('active');
        }
    }

    $("#divmyModalbtn").html('<button type="button" class="btn btn-inverse-dark btn-fw" onclick="ResetmyModal(\'' + DivSpace + '\',\'' + ParameterLabel + '\');">Reset</button>' +
        '<button type="button" class="btn btn-inverse-success btn-fw" data-dismiss="modal">Done</button>');
    $('#myModal').modal('toggle');
}
var ResetmyModal = function (DivSpace, ParameterLabel) {
    var list = [];
    if (ParameterLabel == 'Vendor') {
        list = VendorList;
    }
    if (ParameterLabel == 'Location') {
        list = LocationList;
    }
    if (ParameterLabel == 'Shape') {
        list = ShapeList;
    }
    if (ParameterLabel == 'Clarity') {
        list = ClarityList;
    }
    if (ParameterLabel == 'Cut') {
        list = CutList;
    }
    if (ParameterLabel == 'Polish') {
        list = PolishList;
    }
    if (ParameterLabel == 'Sym') {
        list = SymmList;
    }
    if (ParameterLabel == 'Fls') {
        list = FlsList;
    }
    if (ParameterLabel == 'Lab') {
        list = LabList;
    }
    if (ParameterLabel == 'BGM') {
        list = BgmList;
    }
    if (ParameterLabel == 'CrownBlack') {
        list = CrnBlackList;
    }
    if (ParameterLabel == 'TableBlack') {
        list = TblBlackList;
    }
    if (ParameterLabel == 'CrownWhite') {
        list = CrnWhiteList;
    }
    if (ParameterLabel == 'TableWhite') {
        list = TblWhiteList;
    }
    if (ParameterLabel == 'TableOpen') {
        list = TblOpenList;
    }
    if (ParameterLabel == 'CrownOpen') {
        list = CrnOpenList;
    }
    if (ParameterLabel == 'PavOpen') {
        list = PavOpenList;
    }
    if (ParameterLabel == 'GirdleOpen') {
        list = GrdleOpenList;
    }

    for (var j = 0; j <= list.length - 1; j++) {
        $("#li_" + ParameterLabel + "_" + j).removeClass('active');
    }

    ResetSelectedAttr(DivSpace, list);
}
var ResetSelectedAttr = function (attr, obj) {
    _.each(obj, function (itm) {
        itm.isActive = false;
    });
    $(attr).empty();
}
var ItemClicked = function (ParameterLabel, item, c, curritem) {
    var list = [];
    if (ParameterLabel == 'Vendor') {
        list = VendorList;
    }
    if (ParameterLabel == 'Location') {
        list = LocationList;
    }
    if (ParameterLabel == 'Shape') {
        list = ShapeList;
    }
    if (ParameterLabel == 'Carat') {
        list = PointerList;
    }
    if (ParameterLabel == 'Color') {
        list = ColorList;
    }
    if (ParameterLabel == 'Clarity') {
        list = ClarityList;
    }
    if (ParameterLabel == 'Cut') {
        list = CutList;
    }
    if (ParameterLabel == 'Polish') {
        list = PolishList;
    }
    if (ParameterLabel == 'Sym') {
        list = SymmList;
    }
    if (ParameterLabel == 'Lab') {
        list = LabList;
    }
    if (ParameterLabel == 'Fls') {
        list = FlsList;
    }
    if (ParameterLabel == 'BGM') {
        list = BgmList;
    }
    if (ParameterLabel == 'CrownBlack') {
        list = CrnBlackList;
    }
    if (ParameterLabel == 'TableBlack') {
        list = TblBlackList;
    }
    if (ParameterLabel == 'CrownWhite') {
        list = CrnWhiteList;
    }
    if (ParameterLabel == 'TableWhite') {
        list = TblWhiteList;
    }
    if (ParameterLabel == 'TableOpen') {
        list = TblOpenList;
    }
    if (ParameterLabel == 'CrownOpen') {
        list = CrnOpenList;
    }
    if (ParameterLabel == 'PavOpen') {
        list = PavOpenList;
    }
    if (ParameterLabel == 'GirdleOpen') {
        list = GrdleOpenList;
    }

    if (item == "ALL") {
        if (ParameterLabel == "Vendor" || ParameterLabel == "Location" || ParameterLabel == "Shape" || ParameterLabel == "Color"
            || ParameterLabel == "Clarity" || ParameterLabel == "Cut") {
            //if (ParameterLabel == "Color" && item == "ALL") {
            //    R_F_All_Only_Checkbox_Clr_Rst("1");
            //}
            //else {
            for (var j = 0; j <= list.length - 1; j++) {
                if (list[j].sName != "ALL") {
                    var itm = _.find(list, function (i) {
                        return i.sName == list[j].sName
                    });
                    if ($("#li_" + ParameterLabel + "_0").hasClass("active")) {
                        itm.isActive = true;
                        $("#li_" + ParameterLabel + "_" + j).addClass('active');
                    }
                    else {
                        itm.isActive = false;
                        $("#li_" + ParameterLabel + "_" + j).removeClass('active');
                    }
                    //itm.isActive = !itm.isActive;
                }
                else {
                    $("#li_" + ParameterLabel + "_0").toggleClass('active');
                }
            }
            //}
        }
        else if (ParameterLabel == "Carat") {
            _pointerlst = PointerList;
            $(".divCheckedPointerValue").empty();
            for (var j = 0; j <= list.length - 1; j++) {
                list[j].isActive = true;
                $('.divCheckedPointerValue').append('<li id="C_' + list[j].iSr + '" class="carat-li-top allcrt">' + list[j].sName + '<i class="fa fa-times-circle" aria-hidden="true" onclick="NewSizeGroupRemove(' + list[j].iSr + ');"></i></li>');
            }
        }
    }
    else {
        if (ParameterLabel == "Color") {
            R_F_All_Only_Checkbox_Clr_Rst("-0");
        }
        var itm = _.find(list, function (i) { return i.sName == item });
        if ($("#li_" + ParameterLabel + "_" + c).hasClass("active")) {
            itm.isActive = false;
            $("#li_" + ParameterLabel + "_" + c).removeClass('active');
        }
        else {
            itm.isActive = true;
            $("#li_" + ParameterLabel + "_" + c).addClass('active');
        }

        if (ParameterLabel == "Vendor" || ParameterLabel == "Location" || ParameterLabel == "Shape" || ParameterLabel == "Color"
            || ParameterLabel == "Clarity" || ParameterLabel == "Cut") {
            var IsAllActiveC = 0;
            for (var j = 0; j <= list.length - 1; j++) {
                if (list[j].sName != "ALL") {
                    if (list[j].isActive == true) {
                        IsAllActiveC = parseInt(IsAllActiveC) + 1;
                    }
                }
            }
            if (IsAllActiveC == list.length - 1) {
                $("#li_" + ParameterLabel + "_0").addClass('active');
            }
            else {
                $("#li_" + ParameterLabel + "_0").removeClass('active');
            }
        }
        //$(curritem).toggleClass('active');
    }
    SetSearchParameter();
}
function NewSizeGroupRemove(id) {
    $('#C_' + id).remove();
    var cList = _.reject(_pointerlst, function (e) { return e.iSr == id });
    _pointerlst = cList;
    SetSearchParameter();
}
function Bind_RColor() {
    $("#divCheckedColorValue1").empty();
    for (var j = 0; j <= ColorList.length - 1; j++) {
        $('#divCheckedColorValue1').append('<li id="li_Color_' + ColorList[j].iSr + '" onclick="ItemClicked(\'Color\',\'' + ColorList[j].sName + '\',\'' + j + '\', this);">' + ColorList[j].sName + '</li>');
    }
}
function ActiveOrNot(id) {
    if ($("#" + id).hasClass("btn-spn-opt-active")) {
        //$("#" + id).removeClass("btn-spn-opt-active");
        //if (id == "Regular") {
        //    Regular = false;
        //}
        //if (id == "Fancy") {
        //    Fancy = false;
        //}
    }
    else {
        $("#" + id).addClass("btn-spn-opt-active");
        if (id == "Regular") {
            Regular = true;
            Fancy = false;
            $("#Fancy").removeClass("btn-spn-opt-active");
            $("#div_Regular").show();
            $("#div_Fancy").hide();
        }
        if (id == "Fancy") {
            Fancy = true;
            Regular = false;
            $("#Regular").removeClass("btn-spn-opt-active");
            $("#div_Regular").hide();
            $("#div_Fancy").show();
        }
        R_F_All_Only_Checkbox_Clr_Rst("1");
        ResetCheckColors();
    }
}
function R_F_All_Only_Checkbox_Clr_Rst(where) {
    if (where != "-1") {
        $('#chk_Regular_All').prop('checked', false);
        $('#chk_Fancy_All').prop('checked', false);
    }
    if (where == "-1") {
        where = "1";
    }
    if (where != "-0") {
        for (var j = 0; j <= ColorList.length - 1; j++) {
            if (ColorList[j].sName != "ALL") {
                ColorList[j].isActive = false;
            }
            $("#li_Color_" + j).removeClass('active');
        }
    }
    if (where == "-0") {
        where = "1";
    }

    Regular_All = false;
    Fancy_All = false;
    if (where == "1") {
        Check_Color_1 = [];
        $('#c1_spanselected').html('' + Check_Color_1.length + ' - Selected');
        $('#ddl_INTENSITY input[type="checkbox"]').prop('checked', false);
        Check_Color_2 = [];
        $('#c2_spanselected').html('' + Check_Color_2.length + ' - Selected');
        $('#ddl_OVERTONE input[type="checkbox"]').prop('checked', false);
        Check_Color_3 = [];
        $('#c3_spanselected').html('' + Check_Color_3.length + ' - Selected');
        $('#ddl_FANCY_COLOR input[type="checkbox"]').prop('checked', false);
    }


}
function INTENSITYShow() {
    setTimeout(function () {
        if (C1 == 0) {
            $("#sym-sec0 .carat-dropdown-main").hide();
            $("#sym-sec2 .carat-dropdown-main").hide();
            $("#sym-sec3 .carat-dropdown-main").hide();
            $("#sym-sec1 .carat-dropdown-main").show();
            C1 = 1;
            KTS = 0, C2 = 0, C3 = 0;
        }
        else {
            $("#sym-sec1 .carat-dropdown-main").hide();
            C1 = 0, KTS = 0, C2 = 0, C3 = 0;
        }
    }, 2);
}
function OVERTONEShow() {
    setTimeout(function () {
        if (C2 == 0) {
            $("#sym-sec0 .carat-dropdown-main").hide();
            $("#sym-sec1 .carat-dropdown-main").hide();
            $("#sym-sec3 .carat-dropdown-main").hide();
            $("#sym-sec2 .carat-dropdown-main").show();
            C2 = 1;
            C1 = 0, KTS = 0, C3 = 0;
        }
        else {
            $("#sym-sec2 .carat-dropdown-main").hide();
            C1 = 0, KTS = 0, C2 = 0, C3 = 0;
        }
    }, 2);
}
function FANCY_COLORShow() {
    setTimeout(function () {
        if (C3 == 0) {
            $("#sym-sec0 .carat-dropdown-main").hide();
            $("#sym-sec1 .carat-dropdown-main").hide();
            $("#sym-sec2 .carat-dropdown-main").hide();
            $("#sym-sec3 .carat-dropdown-main").show();
            C3 = 1;
            C1 = 0, KTS = 0, C2 = 0;
        }
        else {
            $("#sym-sec3 .carat-dropdown-main").hide();
            C1 = 0, KTS = 0, C2 = 0, C3 = 0;
        }
    }, 2);
}
function Key_to_symbolShow() {
    setTimeout(function () {
        if (KTS == 0) {
            $("#sym-sec1 .carat-dropdown-main").hide();
            $("#sym-sec2 .carat-dropdown-main").hide();
            $("#sym-sec3 .carat-dropdown-main").hide();
            $("#sym-sec0 .carat-dropdown-main").show();
            KTS = 1;
            C1 = 0, C2 = 0, C3 = 0;
        }
        else {
            $("#sym-sec0 .carat-dropdown-main").hide();
            C1 = 0, KTS = 0, C2 = 0, C3 = 0;
        }
    }, 2);
}
function FcolorBind() {
    for (var i = 0; i <= INTENSITY.length - 1; i++) {
        $('#ddl_INTENSITY').append('<div class="col-12 pl-0 pr-0 ng-scope">'
            + '<ul class="row m-0">'
            + '<li class="carat-dropdown-chkbox">'
            + '<div class="main-cust-check">'
            + '<label class="cust-rdi-bx mn-check" style="vertical-align: sub;margin-bottom: 0.1rem;">'
            + '<input type="checkbox" class="checkradio f_clr_clk" id="CHK_I_' + i + '" name="CHK_I_' + i + '" onclick="GetCheck_INTENSITY_List(\'' + INTENSITY[i] + '\',' + i + ');">'
            + '<span class="cust-rdi-check" style="font-size:15px;">'
            + '<i class="fa fa-check"></i>'
            + '</span>'
            + '</label>'
            + '</div>'
            + '</li>'
            + '<li class="col" style="text-align: left;margin-left: -15px;">'
            + '<span>' + INTENSITY[i] + '</span>'
            + '</li>'
            + '</ul>'
            + '</div>');
    }
    $('#ddl_INTENSITY').append('<div class="ps-scrollbar-x-rail" style="left: 0px; bottom: 0px;"><div class="ps-scrollbar-x" tabindex="0" style="left: 0px; width: 0px;"></div></div><div class="ps-scrollbar-y-rail" style="top: 0px; right: 0px;"><div class="ps-scrollbar-y" tabindex="0" style="top: 0px; height: 0px;"></div></div>');

    for (var j = 0; j <= OVERTONE.length - 1; j++) {
        $('#ddl_OVERTONE').append('<div class="col-12 pl-0 pr-0 ng-scope">'
            + '<ul class="row m-0">'
            + '<li class="carat-dropdown-chkbox">'
            + '<div class="main-cust-check">'
            + '<label class="cust-rdi-bx mn-check" style="vertical-align: sub;margin-bottom: 0.1rem;">'
            + '<input type="checkbox" class="checkradio f_clr_clk" id="CHK_O_' + j + '" name="CHK_O_' + j + '" onclick="GetCheck_OVERTONE_List(\'' + OVERTONE[j] + '\',' + j + ');">'
            + '<span class="cust-rdi-check" style="font-size:15px;">'
            + '<i class="fa fa-check"></i>'
            + '</span>'
            + '</label>'
            + '</div>'
            + '</li>'
            + '<li class="col" style="text-align: left;margin-left: -15px;">'
            + '<span>' + OVERTONE[j] + '</span>'
            + '</li>'
            + '</ul>'
            + '</div>');
    }
    $('#ddl_OVERTONE').append('<div class="ps-scrollbar-x-rail" style="left: 0px; bottom: 0px;"><div class="ps-scrollbar-x" tabindex="0" style="left: 0px; width: 0px;"></div></div><div class="ps-scrollbar-y-rail" style="top: 0px; right: 0px;"><div class="ps-scrollbar-y" tabindex="0" style="top: 0px; height: 0px;"></div></div>');

    for (var k = 0; k <= FANCY_COLOR.length - 1; k++) {
        $('#ddl_FANCY_COLOR').append('<div class="col-12 pl-0 pr-0 ng-scope">'
            + '<ul class="row m-0">'
            + '<li class="carat-dropdown-chkbox">'
            + '<div class="main-cust-check">'
            + '<label class="cust-rdi-bx mn-check" style="vertical-align: sub;margin-bottom: 0.1rem;">'
            + '<input type="checkbox" class="checkradio f_clr_clk" id="CHK_F_' + k + '" name="CHK_F_' + k + '" onclick="GetCheck_FANCY_COLOR_List(\'' + FANCY_COLOR[k] + '\',' + k + ');" style="cursor:pointer;">'
            + '<span class="cust-rdi-check" style="font-size:15px;">'
            + '<i class="fa fa-check"></i>'
            + '</span>'
            + '</label>'
            + '</div>'
            + '</li>'
            + '<li class="col" style="text-align: left;margin-left: -15px;">'
            + '<span>' + FANCY_COLOR[k] + '</span>'
            + '</li>'
            + '</ul>'
            + '</div>');
    }
    $('#ddl_FANCY_COLOR').append('<div class="ps-scrollbar-x-rail" style="left: 0px; bottom: 0px;"><div class="ps-scrollbar-x" tabindex="0" style="left: 0px; width: 0px;"></div></div><div class="ps-scrollbar-y-rail" style="top: 0px; right: 0px;"><div class="ps-scrollbar-y" tabindex="0" style="top: 0px; height: 0px;"></div></div>');
}
function GetCheck_INTENSITY_List(item, id) {
    R_F_All_Only_Checkbox_Clr_Rst("0");
    var res = _.filter(Check_Color_1, function (e) { return (e.Symbol == item) });
    if (id == "0") {
        Check_Color_1 = [];
        if ($("#CHK_I_0").prop("checked") == true) {
            for (var i = 1; i <= INTENSITY.length - 1; i++) {
                Check_Color_1.push({
                    "NewID": Check_Color_1.length + 1,
                    "Symbol": INTENSITY[i],
                });
                $("#CHK_I_" + i).prop('checked', true);
            }
        }
        else {
            for (var i = 0; i <= INTENSITY.length - 1; i++) {
                $("#CHK_I_" + i).prop('checked', false);
            }
        }
        $('#c1_spanselected').html('' + Check_Color_1.length + ' - Selected');
    }
    else {
        $("#CHK_I_0").prop('checked', false);
        if (res.length == 0) {
            Check_Color_1.push({
                "NewID": Check_Color_1.length + 1,
                "Symbol": item,
            });
        }
        else {
            for (var i = 0; i <= Check_Color_1.length - 1; i++) {
                if (Check_Color_1[i].Symbol == item) {
                    $("#CHK_I_" + id).prop('checked', false);
                    Check_Color_1.splice(i, 1);
                }
            }
        }
        if (INTENSITY.length - 1 == Check_Color_1.length) {
            $("#CHK_I_0").prop('checked', true);
        }
        $('#c1_spanselected').html('' + Check_Color_1.length + ' - Selected');
    }

    setTimeout(function () {
        $("#sym-sec1 .carat-dropdown-main").show();
    }, 2);
    Set_FancyColor();
}
function GetCheck_OVERTONE_List(item, id) {
    R_F_All_Only_Checkbox_Clr_Rst("0");
    var res = _.filter(Check_Color_2, function (e) { return (e.Symbol == item) });
    if (id == "0") {
        Check_Color_2 = [];
        if ($("#CHK_O_0").prop("checked") == true) {
            for (var i = 1; i <= OVERTONE.length - 1; i++) {
                Check_Color_2.push({
                    "NewID": Check_Color_2.length + 1,
                    "Symbol": OVERTONE[i],
                });
                $("#CHK_O_" + i).prop('checked', true);
            }
        }
        else {
            for (var i = 0; i <= OVERTONE.length - 1; i++) {
                $("#CHK_O_" + i).prop('checked', false);
            }
        }
        $('#c2_spanselected').html('' + Check_Color_2.length + ' - Selected');
    }
    else {
        $("#CHK_O_0").prop('checked', false);
        if (res.length == 0) {
            Check_Color_2.push({
                "NewID": Check_Color_2.length + 1,
                "Symbol": item,
            });
        }
        else {
            for (var i = 0; i <= Check_Color_2.length - 1; i++) {
                if (Check_Color_2[i].Symbol == item) {
                    $("#CHK_O_" + id).prop('checked', false);
                    Check_Color_2.splice(i, 1);
                }
            }
        }
        if (OVERTONE.length - 1 == Check_Color_2.length) {
            $("#CHK_O_0").prop('checked', true);
        }
        $('#c2_spanselected').html('' + Check_Color_2.length + ' - Selected');
    }
    setTimeout(function () {
        $("#sym-sec2 .carat-dropdown-main").show();
    }, 2);
    Set_FancyColor();
}
function GetCheck_FANCY_COLOR_List(item, id) {
    R_F_All_Only_Checkbox_Clr_Rst("0");
    var res = _.filter(Check_Color_3, function (e) { return (e.Symbol == item) });
    if (id == "0") {
        Check_Color_3 = [];
        if ($("#CHK_F_0").prop("checked") == true) {
            for (var i = 1; i <= FANCY_COLOR.length - 1; i++) {
                Check_Color_3.push({
                    "NewID": Check_Color_3.length + 1,
                    "Symbol": FANCY_COLOR[i],
                });
                $("#CHK_F_" + i).prop('checked', true);
            }
        }
        else {
            for (var i = 0; i <= FANCY_COLOR.length - 1; i++) {
                $("#CHK_F_" + i).prop('checked', false);
            }
        }
        $('#c3_spanselected').html('' + Check_Color_3.length + ' - Selected');
    }
    else {
        $("#CHK_F_0").prop('checked', false);
        if (res.length == 0) {
            Check_Color_3.push({
                "NewID": Check_Color_3.length + 1,
                "Symbol": item,
            });
        }
        else {
            for (var i = 0; i <= Check_Color_3.length - 1; i++) {
                if (Check_Color_3[i].Symbol == item) {
                    $("#CHK_F_" + id).prop('checked', false);
                    Check_Color_3.splice(i, 1);
                }
            }
        }
        if (FANCY_COLOR.length - 1 == Check_Color_3.length) {
            $("#CHK_F_0").prop('checked', true);
        }
        $('#c3_spanselected').html('' + Check_Color_3.length + ' - Selected');
    }
    setTimeout(function () {
        $("#sym-sec3 .carat-dropdown-main").show();
    }, 2);
    Set_FancyColor();
}
function resetINTENSITY() {
    Check_Color_1 = [];
    $('#c1_spanselected').html('' + Check_Color_1.length + ' - Selected');
    $('#ddl_INTENSITY input[type="checkbox"]').prop('checked', false);
    C1 = 1;
    INTENSITYShow();
    SetSearchParameter();
}
function resetOVERTONE() {
    Check_Color_2 = [];
    $('#c2_spanselected').html('' + Check_Color_2.length + ' - Selected');
    $('#ddl_OVERTONE input[type="checkbox"]').prop('checked', false);
    C2 = 1;
    OVERTONEShow();
    SetSearchParameter();
}
function resetFANCY_COLOR() {
    Check_Color_3 = [];
    $('#c3_spanselected').html('' + Check_Color_3.length + ' - Selected');
    $('#ddl_FANCY_COLOR input[type="checkbox"]').prop('checked', false);
    C3 = 1;
    FANCY_COLORShow();
    SetSearchParameter();
}
function Set_FancyColor() {
    FC = "";
    if (Check_Color_1.length != 0) {
        FC += (FC == "" ? "" : "</br>") + "<b>INTENSITY :</b>";
        FC += _.pluck(_.filter(Check_Color_1), 'Symbol').join(",");
    }
    if (Check_Color_2.length != 0) {
        FC += (FC == "" ? "" : "</br>") + "<b>OVERTONE :</b>";
        FC += _.pluck(_.filter(Check_Color_2), 'Symbol').join(",");
    }
    if (Check_Color_3.length != 0) {
        FC += (FC == "" ? "" : "</br>") + "<b>FANCY COLOR :</b>";
        FC += _.pluck(_.filter(Check_Color_3), 'Symbol').join(",");
    }
    $(".divCheckedColorValue").empty();
    $(".divCheckedColorValue").append(FC);
    $(".divCheckedColorValue").attr({
        "title": FC
    });
}
function color_ddl_close() {
    $("#sym-sec1 .carat-dropdown-main").hide();
    $("#sym-sec2 .carat-dropdown-main").hide();
    $("#sym-sec3 .carat-dropdown-main").hide();
}
function numvalid(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function Reset_API_Filter() {
    ResetSelectedAttr('.divCheckedVendorValue', VendorList);
    ResetSelectedAttr('.divCheckedLocationValue', LocationList);
    ResetSelectedAttr('.divCheckedShapeValue', ShapeList);
    ResetCheckCarat();
    ResetCheckColors();
    Regular = true;
    Fancy = false;
    $("#Regular").addClass("btn-spn-opt-active");
    $("#Fancy").removeClass("btn-spn-opt-active");
    $("#div_Regular").show();
    $("#div_Fancy").hide();
    ResetSelectedAttr('.divCheckedColorValue', ColorList);
    ResetSelectedAttr('.divCheckedClarityValue', ClarityList);
    ResetSelectedAttr('.divCheckedCutValue', CutList);
    ResetSelectedAttr('.divCheckedPolValue', PolishList);
    ResetSelectedAttr('.divCheckedSymValue', SymmList);
    ResetSelectedAttr('.divCheckedFlsValues', FlsList);
    ResetSelectedAttr('.divCheckedLabValue', LabList);
    $("#FromLength").val("");
    $("#ToLength").val("");
    $("#FromWidth").val("");
    $("#ToWidth").val("");
    $("#FromDepth").val("");
    $("#ToDepth").val("");
    $("#FromDepthPer").val("");
    $("#ToDepthPer").val("");
    $("#FromTablePer").val("");
    $("#ToTablePer").val("");
    $("#FromCrAng").val("");
    $("#ToCrAng").val("");
    $("#FromCrHt").val("");
    $("#ToCrHt").val("");
    $("#FromPavAng").val("");
    $("#ToPavAng").val("");
    $("#FromPavHt").val("");
    $("#ToPavHt").val("");
    resetKeytoSymbol();
    ResetSelectedAttr('.divCheckedBGMValue', BgmList);
    ResetSelectedAttr('.divCheckedCrnBlackValue', CrnBlackList);
    ResetSelectedAttr('.divCheckedTblBlackValue', TblBlackList);
    ResetSelectedAttr('.divCheckedCrnWhiteValue', CrnWhiteList);
    ResetSelectedAttr('.divCheckedTblWhiteValue', TblWhiteList);
    ResetSelectedAttr('.divCheckedTblOpenValue', TblOpenList);
    ResetSelectedAttr('.divCheckedCrnOpenValue', CrnOpenList);
    ResetSelectedAttr('.divCheckedPavOpenValue', PavOpenList);
    ResetSelectedAttr('.divCheckedGrdleOpenValue', GrdleOpenList);
    $(".imgAll").prop("checked", true);
    $(".vdoAll").prop("checked", true);

    $("#ddlPriceMethod").val("Discount");
    $("#txtValue").hide();
    $("#txtDisc").show();
    $("#txtDisc").val("");
    //document.getElementById("txtDisc").disabled = true;
    //document.getElementById("txtValue").disabled = true;

    $("#chkLength").prop("checked", false);
    $("#chkWidth").prop("checked", false);
    $("#chkDepth").prop("checked", false);
    $("#chkDepthPer").prop("checked", false);
    $("#chkTablePer").prop("checked", false);
    $("#chkCrAng").prop("checked", false);
    $("#chkCrHt").prop("checked", false);
    $("#chkPavAng").prop("checked", false);
    $("#chkPavHt").prop("checked", false);
    $("#chkKTS").prop("checked", false);

    vendorlst = "";
    locationLst = "";
    shapeLst = "";
    pointerlst = "";
    _pointerlst = [];
    colorLst = "";
    clarityLst = "";
    cutlst = "";
    Pollst = "";
    Symlst = "";
    labLst = "";
    flslst = "";
    bgmlst = "";
    crnblacklst = "";
    tblblacklst = "";
    crnwhitelst = "";
    tblwhitelst = "";
    tblopenlst = "";
    crnopenlst = "";
    pavopenlst = "";
    grdleopenlst = "";

    CheckedVendorValue = "";
    CheckedLocationValue = "";
    CheckedShapeValue = "";
    CheckedPointerValue = "";
    CheckedColorValue = "";
    CheckedClarityValue = "";
    CheckedCutValue = "";
    CheckedPolValue = "";
    CheckedSymValue = "";
    CheckedLabValue = "";
    CheckedFLsValue = "";
    CheckedBgmValue = "";
    CheckedCrnBlackValue = "";
    CheckedTblBlackValue = "";
    CheckedCrnWhiteValue = "";
    CheckedTblWhiteValue = "";
}
function BindKeyToSymbolList() {
    $('.loading-overlay-image-container').show();
    $('.loading-overlay').show();

    $.ajax({
        url: "/Lab/GetKeyToSymbolList",
        async: false,
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {
            var KeytoSymbolList = data.Data;
            $('#searchkeytosymbol').html("");
            if (KeytoSymbolList.length > 0) {
                $.each(KeytoSymbolList, function (i, itm) {
                    $('#searchkeytosymbol').append('<div class="col-12 pl-0 pr-0 ng-scope">'
                        + '<ul class="row m-0">'
                        + '<li class="carat-dropdown-chkbox">'
                        + '<div class="main-cust-check">'
                        + '<label class="cust-rdi-bx mn-check" style="vertical-align: sub;margin-bottom: 0.1rem;">'
                        + '<input type="radio" class="checkradio" id="CHK_KTS_Radio_' + (i + 1) + '" name="radio' + (i + 1) + '" onclick="GetCheck_KTS_List(\'' + itm.sSymbol + '\');">'
                        + '<span class="cust-rdi-check">'
                        + '<i class="fa fa-check"></i>'
                        + '</span>'
                        + '</label>'
                        + '<label class="cust-rdi-bx mn-time" style="vertical-align: sub;margin-bottom: 0.1rem;">'
                        + '<input type="radio" id="UNCHK_KTS_Radio_' + (i + 1) + '" class="checkradio" name="radio' + (i + 1) + '" onclick="GetUnCheck_KTS_List(\'' + itm.sSymbol + '\');">'
                        + '<span class="cust-rdi-check">'
                        + '<i class="fa fa-times"></i>'
                        + '</span>'
                        + '</label>'
                        + '</div>'
                        + '</li>'
                        + '<li class="col">'
                        + '<span>' + itm.sSymbol + '</span>'
                        + '</li>'
                        + '</ul>'
                        + '</div>')
                });
                $('#searchkeytosymbol').append('<div class="ps-scrollbar-x-rail" style="left: 0px; bottom: 0px;"><div class="ps-scrollbar-x" tabindex="0" style="left: 0px; width: 0px;"></div></div><div class="ps-scrollbar-y-rail" style="top: 0px; right: 0px;"><div class="ps-scrollbar-y" tabindex="0" style="top: 0px; height: 0px;"></div></div>');
            }
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $('.loading-overlay-image-container').hide();
            $('.loading-overlay').hide();
        }
    });
}
function GetCheck_KTS_List(item) {
    var SList = _.reject(UnCheckKeyToSymbolList, function (e) { return e.Symbol == item });
    UnCheckKeyToSymbolList = SList;

    var res = _.filter(CheckKeyToSymbolList, function (e) { return (e.Symbol == item) });
    if (res.length == 0) {
        CheckKeyToSymbolList.push({
            "NewID": CheckKeyToSymbolList.length + 1,
            "Symbol": item,
        });
        $('#spanselected').html('' + CheckKeyToSymbolList.length + '-Select');
        $('#spanunselected').html('' + UnCheckKeyToSymbolList.length + '-Deselect');
    }
}
function GetUnCheck_KTS_List(item) {
    var SList = _.reject(CheckKeyToSymbolList, function (e) { return e.Symbol == item });
    CheckKeyToSymbolList = SList

    var res = _.filter(UnCheckKeyToSymbolList, function (e) { return (e.Symbol == item) });
    if (res.length == 0) {
        UnCheckKeyToSymbolList.push({
            "NewID": UnCheckKeyToSymbolList.length + 1,
            "Symbol": item,
        });
        $('#spanselected').html('' + CheckKeyToSymbolList.length + '-Select');
        $('#spanunselected').html('' + UnCheckKeyToSymbolList.length + '-Deselect');
    }
}
function resetKeytoSymbol() {
    CheckKeyToSymbolList = [];
    UnCheckKeyToSymbolList = [];
    $('#spanselected').html('' + CheckKeyToSymbolList.length + '-Select');
    $('#spanunselected').html('' + UnCheckKeyToSymbolList.length + '-Deselect');
    $('#searchkeytosymbol input[type="radio"]').prop('checked', false);
}
var LeaveTextBox = function (ele, fromid, toid) {
    $("#" + fromid).val($("#" + fromid).val() == "" ? "0.00" : $("#" + fromid).val() == undefined ? "0.00" : parseFloat($("#" + fromid).val()).toFixed(2));
    $("#" + toid).val($("#" + toid).val() == "" ? "0.00" : $("#" + toid).val() == undefined ? "0.00" : parseFloat($("#" + toid).val()).toFixed(2));

    var fromvalue = parseFloat($("#" + fromid).val()).toFixed(2) == "" ? 0 : parseFloat($("#" + fromid).val()).toFixed(2);
    var tovalue = parseFloat($("#" + toid).val()).toFixed(2) == "" ? 0 : parseFloat($("#" + toid).val()).toFixed(2);
    if (ele == "FROM") {
        if (parseFloat(parseFloat(fromvalue).toFixed(2)) > parseFloat(parseFloat(tovalue).toFixed(2))) {
            $("#" + toid).val(fromvalue);
            if (fromvalue == 0) {
                $("#" + fromid).val("");
                $("#" + toid).val("");
            }
        }
    }
    else if (ele == "TO") {
        if (parseFloat(parseFloat(tovalue).toFixed(2)) < parseFloat(parseFloat(fromvalue).toFixed(2))) {
            $("#" + fromid).val($("#" + toid).val());
            if (tovalue == 0) {
                $("#" + fromid).val("");
                $("#" + toid).val("");
            }
        }
    }
    if (parseFloat(parseFloat($("#" + fromid).val())) == "0" && parseFloat(parseFloat($("#" + toid).val())) == "0") {
        $("#" + fromid).val("");
        $("#" + toid).val("");
    }
}
//var LeaveTextBox = function (ele, type) {
//    if (type == "LENGTH") {debugger
//        $("#FromLength").val($("#FromLength").val() == "" ? "0.00" : $("#FromLength").val() == undefined ? "0.00" : parseFloat($("#FromLength").val()).toFixed(2));
//        $("#ToLength").val($("#ToLength").val() == "" ? "0.00" : $("#ToLength").val() == undefined ? "0.00" : parseFloat($("#ToLength").val()).toFixed(2));
//        debugger
//        var fromLength = parseFloat($("#FromLength").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromLength").val()).toFixed(2);
//        var toLength = parseFloat($("#ToLength").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToLength").val()).toFixed(2);
//        debugger
//        if (ele == "FROM") {
//            debugger
//            if (parseFloat(parseFloat(fromLength).toFixed(2)) > parseFloat(parseFloat(toLength).toFixed(2))) {
//                $("#ToLength").val(fromLength);
//                if (fromLength == 0) {
//                    $("#FromLength").val("");
//                    $("#ToLength").val("");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            debugger
//            if (parseFloat(parseFloat(toLength).toFixed(2)) < parseFloat(parseFloat(fromLength).toFixed(2))) {
//                $("#FromLength").val($("#ToLength").val());
//                if (toLength == 0) {
//                    $("#FromLength").val("");
//                    $("#ToLength").val("");
//                }
//            }
//        } debugger
//        if (parseFloat($("#FromLength").val()) == "0" && parseFloat($("#ToLength").val()) == "0") {
//            debugger
//            $("#FromLength").val("");
//            $("#ToLength").val("");
//        }
//    }
//    if (type == "WIDTH") {
//        $("#FromWidth").val($("#FromWidth").val() == "" ? "0.00" : $("#FromWidth").val() == undefined ? "0.00" : parseFloat($("#FromWidth").val()).toFixed(2));
//        $("#ToWidth").val($("#ToWidth").val() == "" ? "0.00" : $("#ToWidth").val() == undefined ? "0.00" : parseFloat($("#ToWidth").val()).toFixed(2));

//        var fromWidth = parseFloat($("#FromWidth").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromWidth").val()).toFixed(2);
//        var toWidth = parseFloat($("#ToWidth").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToWidth").val()).toFixed(2);
//        if (ele == "FROM") {
//            if (parseFloat(fromWidth).toFixed(2) > parseFloat(toWidth).toFixed(2)) {
//                $("#ToWidth").val(fromWidth);
//                if (fromWidth == 0) {
//                    $("#FromWidth").val("0.00");
//                    $("#ToWidth").val("0.00");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            if (parseFloat(toWidth).toFixed(2) < parseFloat(fromWidth).toFixed(2)) {
//                $("#FromWidth").val($("#ToWidth").val());
//                if (toWidth == 0) {
//                    $("#FromWidth").val("0.00");
//                    $("#ToWidth").val("0.00");
//                }
//            }
//        }
//        if (parseFloat($("#FromWidth").val()) == "0" && parseFloat($("#ToWidth").val()) == "0") {
//            $("#FromWidth").val("");
//            $("#ToWidth").val("");
//        }
//    }
//    if (type == "DEPTH") {
//        $("#FromDepth").val($("#FromDepth").val() == "" ? "0.00" : $("#FromDepth").val() == undefined ? "0.00" : parseFloat($("#FromDepth").val()).toFixed(2));
//        $("#ToDepth").val($("#ToDepth").val() == "" ? "0.00" : $("#ToDepth").val() == undefined ? "0.00" : parseFloat($("#ToDepth").val()).toFixed(2));

//        var fromDepth = parseFloat($("#FromDepth").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromDepth").val()).toFixed(2);
//        var toDepth = parseFloat($("#ToDepth").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToDepth").val()).toFixed(2);
//        if (ele == "FROM") {
//            if (parseFloat(fromDepth).toFixed(2) > parseFloat(toDepth).toFixed(2)) {
//                $("#ToDepth").val(fromDepth);
//                if (fromDepth == 0) {
//                    $("#FromDepth").val("0.00");
//                    $("#ToDepth").val("0.00");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            if (parseFloat(toDepth).toFixed(2) < parseFloat(fromDepth).toFixed(2)) {
//                $("#FromDepth").val($("#ToDepth").val());
//                if (toDepth == 0) {
//                    $("#FromDepth").val("0.00");
//                    $("#ToDepth").val("0.00");
//                }
//            }
//        }
//        if (parseFloat($("#FromDepth").val()) == "0" && parseFloat($("#ToDepth").val()) == "0") {
//            $("#FromDepth").val("");
//            $("#ToDepth").val("");
//        }
//    }
//    if (type == "DEPTHPER") {
//        $("#FromDepthPer").val($("#FromDepthPer").val() == "" ? "0.00" : $("#FromDepthPer").val() == undefined ? "0.00" : parseFloat($("#FromDepthPer").val()).toFixed(2));
//        $("#ToDepthPer").val($("#ToDepthPer").val() == "" ? "0.00" : $("#ToDepthPer").val() == undefined ? "0.00" : parseFloat($("#ToDepthPer").val()).toFixed(2));

//        var fromDepthPer = parseFloat($("#FromDepthPer").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromDepthPer").val()).toFixed(2);
//        var toDepthPer = parseFloat($("#ToDepthPer").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToDepthPer").val()).toFixed(2);
//        if (ele == "FROM") {
//            if (parseFloat(fromDepthPer).toFixed(2) > parseFloat(toDepthPer).toFixed(2)) {
//                $("#ToDepthPer").val(fromDepthPer);
//                if (fromDepthPer == 0) {
//                    $("#FromDepthPer").val("0.00");
//                    $("#ToDepthPer").val("0.00");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            if (parseFloat(toDepthPer).toFixed(2) < parseFloat(fromDepthPer).toFixed(2)) {
//                $("#FromDepthPer").val($("#ToDepthPer").val());
//                if (toDepthPer == 0) {
//                    $("#FromDepthPer").val("0.00");
//                    $("#ToDepthPer").val("0.00");
//                }
//            }
//        }
//        if (parseFloat($("#FromDepthPer").val()) == "0" && parseFloat($("#ToDepthPer").val()) == "0") {
//            $("#FromDepthPer").val("");
//            $("#ToDepthPer").val("");
//        }
//    }
//    if (type == "TABLEPER") {
//        $("#FromTablePer").val($("#FromTablePer").val() == "" ? "0.00" : $("#FromTablePer").val() == undefined ? "0.00" : parseFloat($("#FromTablePer").val()).toFixed(2));
//        $("#ToTablePer").val($("#ToTablePer").val() == "" ? "0.00" : $("#ToTablePer").val() == undefined ? "0.00" : parseFloat($("#ToTablePer").val()).toFixed(2));

//        var fromTablePer = parseFloat($("#FromTablePer").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromTablePer").val()).toFixed(2);
//        var toTablePer = parseFloat($("#ToTablePer").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToTablePer").val()).toFixed(2);
//        if (ele == "FROM") {
//            if (parseFloat(fromTablePer).toFixed(2) > parseFloat(toTablePer).toFixed(2)) {
//                $("#ToTablePer").val(fromTablePer);
//                if (fromTablePer == 0) {
//                    $("#FromTablePer").val("0.00");
//                    $("#ToTablePer").val("0.00");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            if (parseFloat(toTablePer).toFixed(2) < parseFloat(fromTablePer).toFixed(2)) {
//                $("#FromTablePer").val($("#ToTablePer").val());
//                if (toTablePer == 0) {
//                    $("#FromTablePer").val("0.00");
//                    $("#ToTablePer").val("0.00");
//                }
//            }
//        }
//        if (parseFloat($("#FromTablePer").val()) == "0" && parseFloat($("#ToTablePer").val()) == "0") {
//            $("#FromTablePer").val("");
//            $("#ToTablePer").val("");
//        }
//    }
//    if (type == "CRANG") {
//        $("#FromCrAng").val($("#FromCrAng").val() == "" ? "0.00" : $("#FromCrAng").val() == undefined ? "0.00" : parseFloat($("#FromCrAng").val()).toFixed(2));
//        $("#ToCrAng").val($("#ToCrAng").val() == "" ? "0.00" : $("#ToCrAng").val() == undefined ? "0.00" : parseFloat($("#ToCrAng").val()).toFixed(2));

//        var fromCrAng = parseFloat($("#FromCrAng").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromCrAng").val()).toFixed(2);
//        var toCrAng = parseFloat($("#ToCrAng").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToCrAng").val()).toFixed(2);
//        if (ele == "FROM") {
//            if (parseFloat(fromCrAng).toFixed(2) > parseFloat(toCrAng).toFixed(2)) {
//                $("#ToCrAng").val(fromCrAng);
//                if (fromCrAng == 0) {
//                    $("#FromCrAng").val("0.00");
//                    $("#ToCrAng").val("0.00");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            if (parseFloat(toCrAng).toFixed(2) < parseFloat(fromCrAng).toFixed(2)) {
//                $("#FromCrAng").val($("#ToCrAng").val());
//                if (toCrAng == 0) {
//                    $("#FromCrAng").val("0.00");
//                    $("#ToCrAng").val("0.00");
//                }
//            }
//        }
//        if (parseFloat($("#FromCrAng").val()) == "0" && parseFloat($("#ToCrAng").val()) == "0") {
//            $("#FromCrAng").val("");
//            $("#ToCrAng").val("");
//        }
//    }
//    if (type == "CRHT") {
//        $("#FromCrHt").val($("#FromCrHt").val() == "" ? "0.00" : $("#FromCrHt").val() == undefined ? "0.00" : parseFloat($("#FromCrHt").val()).toFixed(2));
//        $("#ToCrHt").val($("#ToCrHt").val() == "" ? "0.00" : $("#ToCrHt").val() == undefined ? "0.00" : parseFloat($("#ToCrHt").val()).toFixed(2));

//        var fromCrHt = parseFloat($("#FromCrHt").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromCrHt").val()).toFixed(2);
//        var toCrHt = parseFloat($("#ToCrHt").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToCrHt").val()).toFixed(2);
//        if (ele == "FROM") {
//            if (parseFloat(fromCrHt).toFixed(2) > parseFloat(toCrHt).toFixed(2)) {
//                $("#ToCrHt").val(fromCrHt);
//                if (fromCrHt == 0) {
//                    $("#FromCrHt").val("0.00");
//                    $("#ToCrHt").val("0.00");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            if (parseFloat(toCrHt).toFixed(2) < parseFloat(fromCrHt).toFixed(2)) {
//                $("#FromCrHt").val($("#ToCrHt").val());
//                if (toCrHt == 0) {
//                    $("#FromCrHt").val("0.00");
//                    $("#ToCrHt").val("0.00");
//                }
//            }
//        }
//        if (parseFloat($("#FromCrHt").val()) == "0" && parseFloat($("#ToCrHt").val()) == "0") {
//            $("#FromCrHt").val("");
//            $("#ToCrHt").val("");
//        }
//    }
//    if (type == "PAVANG") {
//        $("#FromPavAng").val($("#FromPavAng").val() == "" ? "0.00" : $("#FromPavAng").val() == undefined ? "0.00" : parseFloat($("#FromPavAng").val()).toFixed(2));
//        $("#ToPavAng").val($("#ToPavAng").val() == "" ? "0.00" : $("#ToPavAng").val() == undefined ? "0.00" : parseFloat($("#ToPavAng").val()).toFixed(2));

//        var fromPavAng = parseFloat($("#FromPavAng").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromPavAng").val()).toFixed(2);
//        var toPavAng = parseFloat($("#ToPavAng").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToPavAng").val()).toFixed(2);
//        if (ele == "FROM") {
//            if (parseFloat(fromPavAng).toFixed(2) > parseFloat(toPavAng).toFixed(2)) {
//                $("#ToPavAng").val(fromPavAng);
//                if (fromPavAng == 0) {
//                    $("#FromPavAng").val("0.00");
//                    $("#ToPavAng").val("0.00");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            if (parseFloat(toPavAng).toFixed(2) < parseFloat(fromPavAng).toFixed(2)) {
//                $("#FromPavAng").val($("#ToPavAng").val());
//                if (toPavAng == 0) {
//                    $("#FromPavAng").val("0.00");
//                    $("#ToPavAng").val("0.00");
//                }
//            }
//        }
//        if (parseFloat($("#FromPavAng").val()) == "0" && parseFloat($("#ToPavAng").val()) == "0") {
//            $("#FromPavAng").val("");
//            $("#ToPavAng").val("");
//        }
//    }
//    if (type == "PAVHT") {
//        $("#FromPavHt").val($("#FromPavHt").val() == "" ? "0.00" : $("#FromPavHt").val() == undefined ? "0.00" : parseFloat($("#FromPavHt").val()).toFixed(2));
//        $("#ToPavHt").val($("#ToPavHt").val() == "" ? "0.00" : $("#ToPavHt").val() == undefined ? "0.00" : parseFloat($("#ToPavHt").val()).toFixed(2));

//        var fromPavHt = parseFloat($("#FromPavHt").val()).toFixed(2) == "" ? 0 : parseFloat($("#FromPavHt").val()).toFixed(2);
//        var toPavHt = parseFloat($("#ToPavHt").val()).toFixed(2) == "" ? 0 : parseFloat($("#ToPavHt").val()).toFixed(2);
//        if (ele == "FROM") {
//            if (parseFloat(fromPavHt).toFixed(2) > parseFloat(toPavHt).toFixed(2)) {
//                $("#ToPavHt").val(fromPavHt);
//                if (fromPavHt == 0) {
//                    $("#FromPavHt").val("0.00");
//                    $("#ToPavHt").val("0.00");
//                }
//            }
//        }
//        else if (ele == "TO") {
//            if (parseFloat(toPavHt).toFixed(2) < parseFloat(fromPavHt).toFixed(2)) {
//                $("#FromPavHt").val($("#ToPavHt").val());
//                if (toPavHt == 0) {
//                    $("#FromPavHt").val("0.00");
//                    $("#ToPavHt").val("0.00");
//                }
//            }
//        }
//        if (parseFloat($("#FromPavHt").val()) == "0" && parseFloat($("#ToPavHt").val()) == "0") {
//            $("#FromPavHt").val("");
//            $("#ToPavHt").val("");
//        }
//    }
//}
var GetError = function () {
    ErrorMsg = [];
    if ($("#ddlPriceMethod").val() == "") {
        ErrorMsg.push({
            'Error': "Please Select Price Method.",
        });
        ErrorMsg.push({
            'Error': "Please Enter Percent of Price Method.",
        });
    }
    else if ($("#ddlPriceMethod").val() != "") {
        if ($("#ddlPriceMethod").val() == "Value" && $("#txtValue").val() == "") {
            ErrorMsg.push({
                'Error': "Please Enter Value Percent.",
            });
        }
        if ($("#ddlPriceMethod").val() == "Discount" && $("#txtDisc").val() == "") {
            ErrorMsg.push({
                'Error': "Please Enter Discount Percent.",
            });
        }
    }
    return ErrorMsg;
}
function AddNewRow() {
    ErrorMsg = GetError();
    if (ErrorMsg.length > 0) {
        $("#divError").empty();
        ErrorMsg.forEach(function (item) {
            $("#divError").append('<li>' + item.Error + '</li>');
        });
        $('#ErrorModel').modal('toggle');
    }
    else {
        loaderShow();
        $("#btnAddNewRow").attr("disabled", true);
        $("#mytable1").show();

        var KeyToSymLst_Check1 = _.pluck(CheckKeyToSymbolList, 'Symbol').join(",");
        var KeyToSymLst_uncheck1 = _.pluck(UnCheckKeyToSymbolList, 'Symbol').join(",");

        var cntRow = parseInt($("#mytable1 #myTableBody1").find('tr').length) + 1;

        var Vendor = _.pluck(_.filter(VendorList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Location = _.pluck(_.filter(LocationList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Shape = _.pluck(_.filter(ShapeList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Carat = _.pluck(_.filter(_pointerlst, function (e) { return e.isActive == true }), 'sName').join(",");
        var Color_Type = (Regular_All == true ? "Regular" : (Fancy_All == true ? "Fancy" : ""));
        var Color = _.pluck(_.filter(ColorList, function (e) { return e.isActive == true }), 'sName').join(",");
        var F_INTENSITY = _.pluck(_.filter(Check_Color_1), 'Symbol').join(",");
        var F_OVERTONE = _.pluck(_.filter(Check_Color_2), 'Symbol').join(",");
        var F_FANCY_COLOR = _.pluck(_.filter(Check_Color_3), 'Symbol').join(",");
        var MixColor = "";
        if (Color != "") {
            MixColor = Color;
        }
        else if (FC != "") {
            MixColor = FC;
        }
        if (Color_Type != "") {
            MixColor = (Color_Type == "Regular" ? "<b>REGULAR ALL</b>" : Color_Type == "Fancy" ? "<b>FANCY ALL</b>" : "");
        }
        var Clarity = _.pluck(_.filter(ClarityList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Cut = _.pluck(_.filter(CutList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Polish = _.pluck(_.filter(PolishList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Sym = _.pluck(_.filter(SymmList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Fls = _.pluck(_.filter(FlsList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Lab = _.pluck(_.filter(LabList, function (e) { return e.isActive == true }), 'sName').join(",");

        var FromLength = $("#FromLength").val() == "" || $("#FromLength").val() == undefined ? "" : parseFloat($("#FromLength").val());
        var ToLength = $("#ToLength").val() == "" || $("#ToLength").val() == undefined ? "" : parseFloat($("#ToLength").val());
        var Length_IsBlank = (document.getElementById("chkLength").checked == true ? true : "");

        var FromWidth = $("#FromWidth").val() == "" || $("#FromWidth").val() == undefined ? "" : parseFloat($("#FromWidth").val());
        var ToWidth = $("#ToWidth").val() == "" || $("#ToWidth").val() == undefined ? "" : parseFloat($("#ToWidth").val());
        var Width_IsBlank = (document.getElementById("chkWidth").checked == true ? true : "");

        var FromDepth = $("#FromDepth").val() == "" || $("#FromDepth").val() == undefined ? "" : parseFloat($("#FromDepth").val());
        var ToDepth = $("#ToDepth").val() == "" || $("#ToDepth").val() == undefined ? "" : parseFloat($("#ToDepth").val());
        var Depth_IsBlank = (document.getElementById("chkDepth").checked == true ? true : "");

        var FromDepthinPer = $("#FromDepthPer").val() == "" || $("#FromDepthPer").val() == undefined ? "" : parseFloat($("#FromDepthPer").val());
        var ToDepthinPer = $("#ToDepthPer").val() == "" || $("#ToDepthPer").val() == undefined ? "" : parseFloat($("#ToDepthPer").val());
        var DepthPer_IsBlank = (document.getElementById("chkDepthPer").checked == true ? true : "");

        var FromTableinPer = $("#FromTablePer").val() == "" || $("#FromTablePer").val() == undefined ? "" : parseFloat($("#FromTablePer").val());
        var ToTableinPer = $("#ToTablePer").val() == "" || $("#ToTablePer").val() == undefined ? "" : parseFloat($("#ToTablePer").val());
        var TablePer_IsBlank = (document.getElementById("chkTablePer").checked == true ? true : "");

        var FromCrAng = $("#FromCrAng").val() == "" || $("#FromCrAng").val() == undefined ? "" : parseFloat($("#FromCrAng").val());
        var ToCrAng = $("#ToCrAng").val() == "" || $("#ToCrAng").val() == undefined ? "" : parseFloat($("#ToCrAng").val());
        var CrAng_IsBlank = (document.getElementById("chkCrAng").checked == true ? true : "");

        var FromCrHt = $("#FromCrHt").val() == "" || $("#FromCrHt").val() == undefined ? "" : parseFloat($("#FromCrHt").val());
        var ToCrHt = $("#ToCrHt").val() == "" || $("#ToCrHt").val() == undefined ? "" : parseFloat($("#ToCrHt").val());
        var CrHt_IsBlank = (document.getElementById("chkCrHt").checked == true ? true : "");

        var FromPavAng = $("#FromPavAng").val() == "" || $("#FromPavAng").val() == undefined ? "" : parseFloat($("#FromPavAng").val());
        var ToPavAng = $("#ToPavAng").val() == "" || $("#ToPavAng").val() == undefined ? "" : parseFloat($("#ToPavAng").val());
        var PavAng_IsBlank = (document.getElementById("chkPavAng").checked == true ? true : "");

        var FromPavHt = $("#FromPavHt").val() == "" || $("#FromPavHt").val() == undefined ? "" : parseFloat($("#FromPavHt").val());
        var ToPavHt = $("#ToPavHt").val() == "" || $("#ToPavHt").val() == undefined ? "" : parseFloat($("#ToPavHt").val());
        var PavHt_IsBlank = (document.getElementById("chkPavHt").checked == true ? true : "");

        var Keytosymbol = KeyToSymLst_Check1 + (KeyToSymLst_Check1 == "" || KeyToSymLst_uncheck1 == "" ? "" : "-") + KeyToSymLst_uncheck1;
        var dCheckKTS = KeyToSymLst_Check1;
        var dUNCheckKTS = KeyToSymLst_uncheck1;
        var KTS_IsBlank = (document.getElementById("chkKTS").checked == true ? true : "");

        var BGM = _.pluck(_.filter(BgmList, function (e) { return e.isActive == true }), 'sName').join(",");
        var CrownBlack = _.pluck(_.filter(CrnBlackList, function (e) { return e.isActive == true }), 'sName').join(",");
        var TableBlack = _.pluck(_.filter(TblBlackList, function (e) { return e.isActive == true }), 'sName').join(",");
        var CrownWhite = _.pluck(_.filter(CrnWhiteList, function (e) { return e.isActive == true }), 'sName').join(",");
        var TableWhite = _.pluck(_.filter(TblWhiteList, function (e) { return e.isActive == true }), 'sName').join(",");
        var TableOpen = _.pluck(_.filter(TblOpenList, function (e) { return e.isActive == true }), 'sName').join(",");
        var CrownOpen = _.pluck(_.filter(CrnOpenList, function (e) { return e.isActive == true }), 'sName').join(",");
        var PavOpen = _.pluck(_.filter(PavOpenList, function (e) { return e.isActive == true }), 'sName').join(",");
        var GirdleOpen = _.pluck(_.filter(GrdleOpenList, function (e) { return e.isActive == true }), 'sName').join(",");
        var Image = $('#rdoImage:checked').val();
        var Video = $('#rdoVideo:checked').val();
        var PriceMethod = $("#ddlPriceMethod").val();
        var per = "";
        if ($("#ddlPriceMethod").val() == "Discount") {
            per = $("#txtDisc").val();
        }
        if ($("#ddlPriceMethod").val() == "Value") {
            per = $("#txtValue").val();
        }
        var Percentage = per;

        var html = "<tr id='tr'>";
        html += "<th class='Row Fi-Criteria' style=''>" + cntRow.toString() + "</th>";
        html += "<td style=''><span class='Fi-Criteria Vendor'>" + Vendor + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria Location'>" + Location + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Shape'>" + Shape + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Carat'>" + Carat + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ColorType'>" + Color_Type + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria Color'>" + Color + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria dCheckINTENSITY'>" + F_INTENSITY + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria dCheckOVERTONE'>" + F_OVERTONE + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria dCheckFANCY_COLOR'>" + F_FANCY_COLOR + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria MixColor' style='line-height: 1rem;'>" + MixColor + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Clarity'>" + Clarity + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Cut'>" + Cut + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Polish'>" + Polish + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Sym'>" + Sym + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Fls'>" + Fls + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria BGM'>" + BGM + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Lab'>" + Lab + "</span></td>";
        
        html += "<td style='display:none;'><span class='Fi-Criteria FromLength'>" + FromLength + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToLength'>" + ToLength + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria Length_IsBlank'>" + Length_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromLength == "" && ToLength == "" ? "" : FromLength + "-" + ToLength) + "" + (Length_IsBlank == 1 ? (FromLength.toString() != "" && ToLength.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style='display:none;'><span class='Fi-Criteria FromWidth'>" + FromWidth + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToWidth'>" + ToWidth + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria Width_IsBlank'>" + Width_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromWidth == "" && ToWidth == "" ? "" : FromWidth + "-" + ToWidth) + "" + (Width_IsBlank == 1 ? (FromWidth.toString() != "" && ToWidth.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style='display:none;'><span class='Fi-Criteria FromDepth'>" + FromDepth + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToDepth'>" + ToDepth + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria Depth_IsBlank'>" + Depth_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromDepth == "" && ToDepth == "" ? "" : FromDepth + "-" + ToDepth) + "" + (Depth_IsBlank == 1 ? (FromDepth.toString() != "" && ToDepth.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style='display:none;'><span class='Fi-Criteria FromDepthinPer'>" + FromDepthinPer + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToDepthinPer'>" + ToDepthinPer + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria DepthPer_IsBlank'>" + DepthPer_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromDepthinPer == "" && ToDepthinPer == "" ? "" : FromDepthinPer + "-" + ToDepthinPer) + "" + (DepthPer_IsBlank == 1 ? (FromDepthinPer.toString() != "" && ToDepthinPer.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style='display:none;'><span class='Fi-Criteria FromTableinPer'>" + FromTableinPer + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToTableinPer'>" + ToTableinPer + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria TablePer_IsBlank'>" + TablePer_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromTableinPer == "" && ToTableinPer == "" ? "" : FromTableinPer + "-" + ToTableinPer) + "" + (TablePer_IsBlank == 1 ? (FromTableinPer.toString() != "" && ToTableinPer.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style='display:none;'><span class='Fi-Criteria FromCrAng'>" + FromCrAng + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToCrAng'>" + ToCrAng + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria CrAng_IsBlank'>" + CrAng_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromCrAng == "" && ToCrAng == "" ? "" : FromCrAng + "-" + ToCrAng) + "" + (CrAng_IsBlank == 1 ? (FromCrAng.toString() != "" && ToCrAng.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style='display:none;'><span class='Fi-Criteria FromCrHt'>" + FromCrHt + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToCrHt'>" + ToCrHt + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria CrHt_IsBlank'>" + CrHt_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromCrHt == "" && ToCrHt == "" ? "" : FromCrHt + "-" + ToCrHt) + "" + (CrHt_IsBlank == 1 ? (FromCrHt.toString() != "" && ToCrHt.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style='display:none;'><span class='Fi-Criteria FromPavAng'>" + FromPavAng + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToPavAng'>" + ToPavAng + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria PavAng_IsBlank'>" + PavAng_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromPavAng == "" && ToPavAng == "" ? "" : FromPavAng + "-" + ToPavAng) + "" + (PavAng_IsBlank == 1 ? (FromPavAng.toString() != "" && ToPavAng.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style='display:none;'><span class='Fi-Criteria FromPavHt'>" + FromPavHt + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria ToPavHt'>" + ToPavHt + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria PavHt_IsBlank'>" + PavHt_IsBlank + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria'>" + (FromPavHt == "" && ToPavHt == "" ? "" : FromPavHt + "-" + ToPavHt) + "" + (PavHt_IsBlank == 1 ? (FromPavHt.toString() != "" && ToPavHt.toString() != "" ? ", BLANK" : "BLANK") : "") + "</span></td>";

        html += "<td style=''><span class='Fi-Criteria Image'>" + Image + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria Video'>" + Video + "</span></td>";

        html += "<td style=''><span class='Fi-Criteria' style='line-height: 16px;'>" + Keytosymbol + "" + (KTS_IsBlank == 1 ? (Keytosymbol.toString() != "" ? "<br/>BLANK" : "BLANK") : "") + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria Keytosymbol' style='line-height: 16px;'>" + Keytosymbol + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria dCheckKTS'>" + dCheckKTS + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria dUNCheckKTS'>" + dUNCheckKTS + "</span></td>";
        html += "<td style='display:none;'><span class='Fi-Criteria KTS_IsBlank'>" + KTS_IsBlank + "</span></td>";

        html += "<td style=''><span class='Fi-Criteria TableBlack'>" + TableBlack + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria CrownBlack'>" + CrownBlack + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria TableWhite'>" + TableWhite + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria CrownWhite'>" + CrownWhite + "</span></td>";
        
        html += "<td style=''><span class='Fi-Criteria TableOpen'>" + TableOpen + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria CrownOpen'>" + CrownOpen + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria PavOpen'>" + PavOpen + "</span></td>";
        html += "<td style=''><span class='Fi-Criteria GirdleOpen'>" + GirdleOpen + "</span></td>";

        html += "<td style=''><span class='Fi-Criteria PriceMethod'>" + PriceMethod + "</span></td>";
        html += "<td style=''><center><span class='Fi-Criteria Percentage'>" + Percentage + "</span></center></td>";

        html += "<td style='width: 50px'>" + '<center><i style="cursor:pointer;" class="error RemoveCriteria"><img src="/Content/images/trash-delete-icon.png" style="width: 19px;border-radius: 0px;height: 22px;"></i></center>' + "</td>";
        html += "</tr>";

        $("#mytable1 #myTableBody1").append(html);

        $("#btnAddNewRow").attr("disabled", false);
        Reset_API_Filter();
        if (parseInt($("#mytable1 #myTableBody1").find('tr').length) == 0) {
            $("#divButton").hide();
        }
        else {
            $("#divButton").show();
        }
        loaderHide();
    }
}
function UserDDL() {
    if ($("#For_iUserId").val() != "") {
        loaderShow();
        var data = {};
        data.iUserid = $("#For_iUserId").val()

        $.ajax({
            type: 'POST',
            url: '/Lab/UserwiseCompany_select',
            data: data,
            dataType: "json",
            success: function (data) {
                loaderHide();
                if (data.Status == "1") {
                    $("#lblCompanyName").html(data.Data[0].CompanyName);
                } else {
                    if (data.Message.indexOf('Something Went wrong') > -1) {
                        MoveToErrorPage(0);
                    }
                    toastr.error(data.Message);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                loaderHide();
                toastr.error(textStatus);
            }
        });
    }
    else {
        $("#lblCompanyName").html("");
    }
}
function chebox_fill(icolumnId) {
    if (icolumnId == "header") {
        $("#chebox_fillImg_Header").removeClass('img-block');
        $("#chebox_fillImg_Header").addClass('img-none');

        $("#chebox_emptyImg_Header").removeClass('img-none');
        $("#chebox_emptyImg_Header").addClass('img-block');

        $(".chebox-fill").addClass('img-none');
        $(".chebox-fill").removeClass('img-block');

        $(".chebox-empty").removeClass('img-none');
        $(".chebox-empty").addClass('img-block');

    } else {
        $("#chebox_fillImg_" + icolumnId).addClass('img-none');
        $("#chebox_fillImg_" + icolumnId).removeClass('img-block');

        $("#chebox_emptyImg_" + icolumnId).removeClass('img-none');
        $("#chebox_emptyImg_" + icolumnId).addClass('img-block');
    }
}
function chebox_empty(icolumnId) {
    if (icolumnId == "header") {
        $("#chebox_emptyImg_Header").removeClass('img-block');
        $("#chebox_emptyImg_Header").addClass('img-none');

        $("#chebox_fillImg_Header").removeClass('img-none');
        $("#chebox_fillImg_Header").addClass('img-block');

        $(".chebox-fill").removeClass('img-none');
        $(".chebox-fill").addClass('img-block');

        $(".chebox-empty").addClass('img-none');
        $(".chebox-empty").removeClass('img-block');

    } else {
        $("#chebox_fillImg_" + icolumnId).removeClass('img-none');
        $("#chebox_fillImg_" + icolumnId).addClass('img-block');

        $("#chebox_emptyImg_" + icolumnId).addClass('img-none');
        $("#chebox_emptyImg_" + icolumnId).removeClass('img-block');
    }
}
function SaveData() {
    if ($("#UserName").val() == undefined || $("#UserName").val() == "") {
        toastr.warning("Please Enter User Name.", { timeOut: 2500 });
        $("#UserName").focus();
        return;
    }
    if ($("#Password").val() == undefined || $("#Password").val() == "") {
        toastr.warning("Please Enter Password.", { timeOut: 2500 });
        $("#Password").focus();
        return;
    }
    if ($("#ExportType").val() == undefined || $("#ExportType").val() == "") {
        toastr.warning("Please Select Export Type.", { timeOut: 2500 });
        $("#ExportType").focus();
        return;
    }
    if ($("#APIName").val() == undefined || $("#APIName").val() == "") {
        toastr.warning("Please Enter API Name.", { timeOut: 2500 });
        $("#APIName").focus();
        return;
    }
    if ($("#For_iUserId").val() == undefined || $("#For_iUserId").val() == "") {
        toastr.warning("Please Select User.", { timeOut: 2500 });
        $("#For_iUserId").focus();
        return;
    }
    if ($("#mytable1 #myTableBody1").find('tr').length == 0) {
        toastr.warning("Please Add Minimum 1 Filter Criteria.");
        return;
    }
    
    var List2 = [];
    $("#mytable1 #myTableBody1 tr").each(function () {
        
        var Index = $(this).index();
        var Vendor = $(this).find('.Vendor').html();
        var Location = $(this).find('.Location').html();
        var Shape = $(this).find('.Shape').html();
        var Carat = $(this).find('.Carat').html();
        var ColorType = $(this).find('.ColorType').html();
        var Color = $(this).find('.Color').html();
        var INTENSITY = $(this).find('.dCheckINTENSITY').html();
        var OVERTONE = $(this).find('.dCheckOVERTONE').html();
        var FANCY_COLOR = $(this).find('.dCheckFANCY_COLOR').html();
        var Clarity = $(this).find('.Clarity').html();
        var Cut = $(this).find('.Cut').html();
        var Polish = $(this).find('.Polish').html();
        var Sym = $(this).find('.Sym').html();
        var Fls = $(this).find('.Fls').html();
        var Lab = $(this).find('.Lab').html();

        var FromLength = $(this).find('.FromLength').html();
        var ToLength = $(this).find('.ToLength').html();
        var Length_IsBlank = $(this).find('.Length_IsBlank').html();

        var FromWidth = $(this).find('.FromWidth').html();
        var ToWidth = $(this).find('.ToWidth').html();
        var Width_IsBlank = $(this).find('.Width_IsBlank').html();

        var FromDepth = $(this).find('.FromDepth').html();
        var ToDepth = $(this).find('.ToDepth').html();
        var Depth_IsBlank = $(this).find('.Depth_IsBlank').html();

        var FromDepthinPer = $(this).find('.FromDepthinPer').html();
        var ToDepthinPer = $(this).find('.ToDepthinPer').html();
        var DepthPer_IsBlank = $(this).find('.DepthPer_IsBlank').html();

        var FromTableinPer = $(this).find('.FromTableinPer').html();
        var ToTableinPer = $(this).find('.ToTableinPer').html();
        var TablePer_IsBlank = $(this).find('.TablePer_IsBlank').html();

        var FromCrAng = $(this).find('.FromCrAng').html();
        var ToCrAng = $(this).find('.ToCrAng').html();
        var CrAng_IsBlank = $(this).find('.CrAng_IsBlank').html();

        var FromCrHt = $(this).find('.FromCrHt').html();
        var ToCrHt = $(this).find('.ToCrHt').html();
        var CrHt_IsBlank = $(this).find('.CrHt_IsBlank').html();

        var FromPavAng = $(this).find('.FromPavAng').html();
        var ToPavAng = $(this).find('.ToPavAng').html();
        var PavAng_IsBlank = $(this).find('.PavAng_IsBlank').html();

        var FromPavHt = $(this).find('.FromPavHt').html();
        var ToPavHt = $(this).find('.ToPavHt').html();
        var PavHt_IsBlank = $(this).find('.PavHt_IsBlank').html();

        var Keytosymbol = $(this).find('.Keytosymbol').html();
        var dCheckKTS = $(this).find('.dCheckKTS').html();
        var dUNCheckKTS = $(this).find('.dUNCheckKTS').html();
        var KTS_IsBlank = $(this).find('.KTS_IsBlank').html();

        var BGM = $(this).find('.BGM').html();
        var CrownBlack = $(this).find('.CrownBlack').html();
        var TableBlack = $(this).find('.TableBlack').html();
        var CrownWhite = $(this).find('.CrownWhite').html();
        var TableWhite = $(this).find('.TableWhite').html();
        var TableOpen = $(this).find('.TableOpen').html();
        var CrownOpen = $(this).find('.CrownOpen').html();
        var PavOpen = $(this).find('.PavOpen').html();
        var GirdleOpen = $(this).find('.GirdleOpen').html();
        var Image = $(this).find('.Image').html();
        var Video = $(this).find('.Video').html();
        var PriceMethod = $(this).find('.PriceMethod').html();
        var Percentage = $(this).find('.Percentage').html();
        
        List2.push({
            iVendor: Vendor,
            iLocation: Location,
            sShape: Shape,
            sPointer: Carat,
            sColorType: ColorType,
            sColor: Color,
            sINTENSITY: INTENSITY,
            sOVERTONE: OVERTONE,
            sFANCY_COLOR: FANCY_COLOR,
            sClarity: Clarity,
            sCut: Cut,
            sPolish: Polish,
            sSymm: Sym,
            sFls: Fls,
            sLab: Lab,

            dFromLength: FromLength,
            dToLength: ToLength,
            Length_IsBlank: Length_IsBlank,

            dFromWidth: FromWidth,
            dToWidth: ToWidth,
            Width_IsBlank: Width_IsBlank,

            dFromDepth: FromDepth,
            dToDepth: ToDepth,
            Depth_IsBlank: Depth_IsBlank,

            dFromDepthPer: FromDepthinPer,
            dToDepthPer: ToDepthinPer,
            DepthPer_IsBlank: DepthPer_IsBlank,

            dFromTablePer: FromTableinPer,
            dToTablePer: ToTableinPer,
            TablePer_IsBlank: TablePer_IsBlank,

            dFromCrAng: FromCrAng,
            dToCrAng: ToCrAng,
            CrAng_IsBlank: CrAng_IsBlank,

            dFromCrHt: FromCrHt,
            dToCrHt: ToCrHt,
            CrHt_IsBlank: CrHt_IsBlank,

            dFromPavAng: FromPavAng,
            dToPavAng: ToPavAng,
            PavAng_IsBlank: PavAng_IsBlank,

            dFromPavHt: FromPavHt,
            dToPavHt: ToPavHt,
            PavHt_IsBlank: PavHt_IsBlank,

            dKeyToSymbol: Keytosymbol,
            dCheckKTS: dCheckKTS,
            dUNCheckKTS: dUNCheckKTS,
            KTS_IsBlank: KTS_IsBlank,

            sBGM: BGM,
            sCrownBlack: CrownBlack,
            sTableBlack: TableBlack,
            sCrownWhite: CrownWhite,
            sTableWhite: TableWhite,
            sTableOpen: TableOpen,
            sCrownOpen: CrownOpen,
            sPavOpen: PavOpen,
            sGirdleOpen: GirdleOpen,
            Img: Image,
            Vdo: Video,
            PriceMethod: PriceMethod,
            PricePer: Percentage
        });
    });
    
    var Arr1 = [];
    var Arr2 = [];
    $("#mytable tbody tr").each(function () {
        var Index = $(this).index();
        var icolumnId = $(this).find("td:eq(4)").html().trim();
        var ColumnName = $(this).find("td:eq(2)").html().trim();
        var EditColumnName = $(this).find("input").val();
        if ($('#chebox_fillImg_' + icolumnId).hasClass('img-block')) {
            var Visibility = true;
        }
        else {
            var Visibility = false;
        }
        Arr2.push({ iPriority: Index, sUser_ColumnName: ColumnName, IsActive: Visibility, EditColumnName: EditColumnName, icolumnId: icolumnId });
        Arr1 = _.filter(Arr2, function (e) { return e.IsActive == true });
    });

    if (Arr1.length == 0) {
        toastr.warning("Please Select Minimum 1 Column.");
        return;
    }

    var List1 = [];
    Arr1.forEach(function (e) {
        List1.push({
            "icolumnId": e.icolumnId,
            "iPriority": e.iPriority + 1,
            "sUser_ColumnName": e.sUser_ColumnName,
            "sCustMiseCaption": e.EditColumnName,
        });
    });

    loaderShow();

    var obj = {};
    obj.UserName = $("#UserName").val();
    obj.Password = $("#Password").val();
    obj.ExportType = $("#ExportType").val();
    obj.APIStatus = document.getElementById("chkAPIStatus").checked,
        obj.APIName = $("#APIName").val();
    obj.For_iUserId = $("#For_iUserId").val();
    obj.iTransId = $("#iTransId").val();
    obj.Filters = List2;
    obj.ColumnsSettings = List1;
    obj.Type = ($("#iTransId").val() == 0 ? 'Insert' : 'Edit');
    
    $.ajax({
        url: "/Lab/SaveLab",
        async: false,
        type: "POST",
        dataType: "json",
        data: JSON.stringify({ savelab_req: obj }),
        contentType: "application/json; charset=utf-8",
        success: function (data, textStatus, jqXHR) {
            loaderHide();
            if (data.Status == "1") {
                toastr.success(data.Message, { timeOut: 2500 });
                setTimeout(function () {
                    location.href = "/Lab/LabList";
                }, 1000);
            }
            else if (data.Status == "0") {
                toastr.error(data.Message, { timeOut: 2500 });
            }
            else if (data.Status == "00") {
                toastr.warning(data.Message, { timeOut: 2500 });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            loaderHide();
            toastr.error(textStatus);
        }
    });
}
var BindPara = function (iTransId) {
    //$.ajax({
    //    type: "POST",
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    url: '/Lab/GetLab',
    //    data: "{'sTransId':'" + iTransId + "'}",
    //    success: function (data) {
    //        if (data.Data != null) {
    //            for (var i = 0; i < data.Data.length; i++) {
    //                if (data.Data[i].iTransId == iTransId) {
    //                    ColumnsettingList(data.Data[i].ColumnsSettings);
    //                }
    //            }
    //        }
    //    }
    //});
    ColumnsettingList(Column);
}
var ColumnsettingList = function (Detaillist) {
    $.ajax({
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        url: '/Lab/GetApiColumns',
        data: {},
        dataType: 'json',
        success: function (data) {
            ColumnList = [];
            NewColumnList = [];
            data.Data.forEach(function (item, index) {
                item.icolumnId = item.iid,
                    item.iPriority = index + 1;
                item.IsActive = false;
                item.sCustMiseCaption = item.caption;
                item.sUser_ColumnName = item.caption;
            });
            ColumnList = data.Data;

            //if ($("#IsModify").val() == 'True') {
            if (Detaillist.length > 0) {
                ColumnList.forEach(function (e) {
                    Detaillist.forEach(function (item) {
                        if (e.iid == item.icolumnId) {
                            e.IsActive = true;
                            e.CustomeCaption = item.sCustMiseCaption;
                            e.iid = parseFloat(item.icolumnId);
                        }
                        e.CustomeCaption = e.sCustMiseCaption;
                        e.iid = e.iid;
                    });
                });
                // $scope.TR_lst =$filter('orderBy')($scope.ColumnList, function (e) { return e.IsActive == true });
                var TR_lst = _.filter(ColumnList, function (e) { return e.IsActive == false });
                var NTR_lst = _.sortBy(Detaillist, function (e) { return e.iPriority });

                //$scope.ColumnList.forEach(function (e) {
                NTR_lst.forEach(function (item) {
                    item.CustomeCaption = item.sCustMiseCaption;
                    item.caption = item.sUser_ColumnName;
                    item.IsActive = true;
                    item.iid = parseFloat(item.icolumnId);
                });

                var Arr = (NTR_lst.concat(TR_lst));
                Arr.forEach(function (e, i) {
                    e.iPriority = i + 1;
                });
                var List = [];
                Arr.forEach(function (e) {
                    List.push({
                        "icolumnId": e.iid,
                        "iPriority": e.iPriority,
                        "sUser_ColumnName": e.caption,
                        "sCustMiseCaption": e.CustomeCaption,
                        "IsActive": e.IsActive
                    });
                });

                ColumnList = List; //$scope.Arr;
                CreateTable_Column(ColumnList)
            }
            // }
        }
    });
}
var CreateTable_Column = function (Columns) {
    var trs = "";
    $("#myTableBody").empty();
    Columns.forEach(function (item) {
        trs += "";
        trs += '<tr>'
        trs += '<td id="lblCoolName" style="display: none;"></td>';
        trs += '<td><i style="cursor: move;" class="fa fa-bars" aria-hidden="true"></i></td>';
        trs += '<td id="lblFieldName" class="onbinding">' + item.sUser_ColumnName + '</td>';
        trs += '<td class="CustName">';
        trs += '<input type="text" class="form-control form-control-sm" value="' + item.sCustMiseCaption + '" maxlength="100">';
        trs += '</td>';
        trs += '<td id="lblColId" style="display: none; " class="onbinding">' + item.icolumnId + '</td>';
        trs += '<td id="lblOrder" class="ColumnOrder onbinding"><center>' + item.iPriority + '</center></td>';
        trs += '<td><center>';
        if (item.IsActive) {
            trs += '<img src="/Content/images/chebox-fill.png" class="chebox-fill img-block" id="chebox_fillImg_' + item.icolumnId + '" onclick="chebox_fill(' + item.icolumnId + ')" style="cursor:pointer;width: 20px;height: 19px;" />';
            trs += '<img src="/Content/images/chebox-empty.png" class="chebox-empty img-none" id="chebox_emptyImg_' + item.icolumnId + '" onclick="chebox_empty(' + item.icolumnId + ')" style="cursor:pointer;width: 20px;height: 19px;" />';
        }
        else {
            trs += '<img src="/Content/images/chebox-fill.png" class="chebox-fill img-none" id="chebox_fillImg_' + item.icolumnId + '" onclick="chebox_fill(' + item.icolumnId + ')" style="cursor:pointer;width: 20px;height: 19px;" />';
            trs += '<img src="/Content/images/chebox-empty.png" class="chebox-empty img-block" id="chebox_emptyImg_' + item.icolumnId + '" onclick="chebox_empty(' + item.icolumnId + ')" style="cursor:pointer;width: 20px;height: 19px;" />';
        }
        trs += '</center></td>';
        trs += '</tr>';

    });
    $("#myTableBody").html(trs);
}

function Lab_Column_Auto_Select() {
    var Type = $("#ddlLab_Column_Auto").val();
    if (Type != "") {
        loaderShow();
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: '/Lab/Lab_Column_Auto_Select',
            data: "{'Type':'" + Type + "'}",
            success: function (data) {
                if (data.Data != null) {
                    ColumnsettingList(data.Data);
                    loaderHide();
                }
            }
        });
    }
    else {
        if ($("#iTransId").val() != "0") {
            BindPara($("#iTransId").val());
        }
        else if ($("#iTransId").val() == "0") {
            ColumnsettingList(Column);
        }
    }
}
