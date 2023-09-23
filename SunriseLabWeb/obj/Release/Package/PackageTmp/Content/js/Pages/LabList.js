﻿var gridOptions = {};
var GridpgSize = 50;
var rowData = [];
var iTransId = 0;
var ISFirstTime = 0;
var Excel_OrderBy, Excel_PageNo, Excel_GridpgSize;

var showEntryHtml = '<div class="show_entry show_entry1"><label>'
    + 'Show <select onchange = "onPageSizeChanged()" id = "ddlPagesize" class="" >'
    + '<option value="50">50</option>'
    + '<option value="100">100</option>'
    + '<option value="200">200</option>'
    + '</select> entries'
    + '</label>'
    + '</div>';

function onPageSizeChanged() {
    var value = $("#ddlPagesize").val();
    GridpgSize = Number(value);
    GetDataList();
}
var SetCurrentDate = function () {
    var m_names = new Array("Jan", "Feb", "Mar",
        "Apr", "May", "Jun", "Jul", "Aug", "Sep",
        "Oct", "Nov", "Dec");
    var d = new Date();
    var curr_date = d.getDate();
    var curr_month = d.getMonth();
    var curr_year = d.getFullYear();
    var FinalDate = (curr_date + "-" + m_names[curr_month]
        + "-" + curr_year);

    return FinalDate;
}
var lastWeekDate = new Date();
var m_names = new Array("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");
var today = new Date();
var lastWeekDate = new Date(today.setDate(today.getDate() - 7));
var date = new Date(lastWeekDate),
    mnth = ("0" + (date.getMonth() + 1)).slice(-2),
    day = ("0" + date.getDate()).slice(-2);
var F_date = [day, m_names[mnth - 1], date.getFullYear()].join("-");

var Back = function () {
    window.location.href = '/Lab/LabDetail';
}
function formatNumber(number) {
    return (parseFloat(number).toFixed(2)).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
}
function contentHeight() {
    var winH = $(window).height(),
        header = $(".card").height(),
        contentHei = winH - (header + 140);
    $("#myGrid").css("height", contentHei);
}

var Clear = function () {
    //window.location = '/Admin_APIView/Admin_APIView';
    $("#txtSearch").val("");
    DatePicker();
    btnSearch();
}

var columnDefs = [
    { headerName: "Sr.", field: "iSr", width: 40, sortable: false },
    { headerName: "Created Date & Time", field: "CreatedDate", width: 135, sortable: true },
    { headerName: "Created By", field: "CreatedBy", width: 130, sortable: true },
    { headerName: "User Name", field: "For_Username", width: 130, sortable: true },
    { headerName: "Company Name", field: "For_CompName", width: 260, sortable: true },
    { headerName: "Customer Name", field: "For_FullName", width: 130, sortable: true },
    { headerName: "AssistBy", field: "AssistBy", width: 130, sortable: true },
    { headerName: "API Name", field: "APIName", width: 115, sortable: true },
    { headerName: "Export Type", field: "ExportType", width: 95, sortable: true },
    { headerName: "API Url", field: "APIUrl", width: 440, sortable: false },
    { headerName: "Active", field: "APIStatus", sortable: true, tooltip: function (params) { return (params.value); }, width: 58, cellRenderer: Status, },
    { headerName: "Last Updated Date", field: "UpdatedDate", width: 130, sortable: true },
    { headerName: "Action", field: "bIsAction", tooltip: function (params) { return (params.value); }, width: 60, cellRenderer: UserDetailPage, },

];
function Status(params) {
    if (params.data.APIStatus == true) {
        return "<span class='Yes'> Yes </span>";
    }
    else {
        return "<span class='No'> No </span>";
    }
}
function UserDetailPage(params) {
    return '<a hrfe="#" onclick="EditData(\'' + params.data.iTransId + '\')" ><i class="fa fa-pencil-square-o" aria-hidden="true" style="font-size: 17px;cursor:pointer;"></i></a>';
}
function EditData(row) {
    location.href = '/Lab/LabDetail?TransId=' + row;
}

var gridDiv = document.querySelector('#myGrid');
function GetDataList() {
    loaderShow();
    if (gridOptions.api != undefined) {
        gridOptions.api.destroy();
    }

    gridOptions = {
        //rowHeight: 45,
        wrapText: true,
        masterDetail: true,
        detailCellRenderer: 'myDetailCellRenderer',
        //detailRowHeight: 70,
        groupDefaultExpanded: 1,
        //components: {
        //    dateIndicator: dateIndicator
        //},
        defaultColDef: {
            enableSorting: true,
            sortable: false,
            width: 150,
        },
        pagination: true,
        icons: {
            groupExpanded:
                '<i class="fa fa-minus-circle"/>',
            groupContracted:
                '<i class="fa fa-plus-circle"/>'
        },
        rowSelection: 'multiple',
        overlayLoadingTemplate: '<span class="ag-overlay-loading-center">NO DATA TO SHOW..</span>',
        suppressRowClickSelection: true,
        columnDefs: columnDefs,
        rowModelType: 'serverSide',
        cacheBlockSize: GridpgSize, // you can have your custom page size
        paginationPageSize: GridpgSize, //pagesize
        paginationNumberFormatter: function (params) {
            return '[' + params.value.toLocaleString() + ']';
        }
    };

    new agGrid.Grid(gridDiv, gridOptions);
    gridOptions.api.setServerSideDatasource(datasource1);

    showEntryVar = setInterval(function () {
        if ($('#myGrid .ag-paging-panel').length > 0) {
            $(showEntryHtml).appendTo('#myGrid .ag-paging-panel');
            $('#ddlPagesize').val(GridpgSize);
        }
        clearInterval(showEntryVar);
    }, 500);

    setTimeout(function () {
        var allColumnIds = [];
        gridOptions.columnApi.getAllColumns().forEach(function (column) {
            allColumnIds.push(column.colId);
        });

        //gridOptions.columnApi.autoSizeColumns(allColumnIds, false);
    }, 1000);
}

const datasource1 = {
    getRows(params) {
        var OrderBy = '', PageNo = gridOptions.api.paginationGetCurrentPage() + 1;
        if (params.request.sortModel.length > 0) {
            OrderBy = '' + params.request.sortModel[0].colId + ' ' + params.request.sortModel[0].sort + ''
        }
        var formData = new FormData();
        if (ISFirstTime == 1) {
            formData.append('dtFromDate', '01-Jan-2020');
        }
        else if (ISFirstTime == 0) {
            formData.append('dtFromDate', $('#txtFromDate').val());
        }
        formData.append('dtToDate', $("#txtToDate").val());
        formData.append('sSearch', $("#txtSearch").val());
        formData.append('sPgNo', PageNo);
        formData.append('sPgSize', GridpgSize);
        formData.append('OrderBy', OrderBy);

        Excel_OrderBy = OrderBy;
        Excel_PageNo = PageNo;
        Excel_GridpgSize = GridpgSize;

        $.ajax({
            url: "/Lab/GetLab",
            async: false,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, textStatus, jqXHR) {
                if (data.Status != undefined) {
                    if (data.Status == "1") {
                        if (data.Data != null && data.Data.length > 0) {
                            params.successCallback(data.Data, data.Data[0].iTotalRec);
                        }
                        else {
                            params.successCallback([], 0);
                            toastr.warning(data.Message);
                        }
                    } else {
                        params.successCallback([], 0);
                        toastr.error(data.Message);
                    }
                    loaderHide();
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                params.successCallback([], 0);
                loaderHide();
            }
        });

    }
};
function ExcelGet() {
    loaderShow();
    var formData1 = new FormData();

    if (ISFirstTime == 1) {
        formData1.append('fromDate', '01-Jan-2020');
    }
    else if (ISFirstTime == 0) {
        formData1.append('fromDate', $('#txtFromDate').val());
    }
    formData1.append('ToDate', $('#txtToDate').val());
    formData1.append('Search', $('#txtSearch').val());
    formData1.append('PageNo', Excel_PageNo);
    formData1.append('PageSize', Excel_GridpgSize);
    formData1.append('OrderBy', Excel_OrderBy);

    $.ajax({
        url: "/Api/UploadMethodExcelGet",
        async: false,
        type: "POST",
        data: formData1,
        processData: false,
        contentType: false,
        success: function (data, textStatus, jqXHR) {
            if (data.indexOf('Something Went wrong') > -1) {
                MoveToErrorPage(0);
            }
            else if (data.indexOf('No data') > -1) {
                toastr.error(data);
            }
            else {
                location.href = data;
            }
            loaderHide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            params.successCallback([], 0);
            MoveToErrorPage(0);
            loaderHide();
        }
    });
}
function btnSearch() {
    ISFirstTime = 0;
    GetDataList();
}
$(document).ready(function (e) {
    DatePicker();
    ISFirstTime = 1;
    GetDataList();
    contentHeight();
    LabStockStatusGet_Second();
});
function DatePicker() {
    $('#txtFromDate').val(F_date);
    $('#txtToDate').val(SetCurrentDate());

    $('#txtFromDate').daterangepicker({
        singleDatePicker: true,
        startDate: F_date,
        showDropdowns: true,
        locale: {
            separator: "-",
            format: 'DD-MMM-YYYY'
        },
        minYear: 1901,
        maxYear: parseInt(moment().format('YYYY'), 10)
    }).on('change', function (e) {
        greaterThanDate(e);
    });
    $('#txtToDate').daterangepicker({
        singleDatePicker: true,
        startDate: SetCurrentDate(),
        showDropdowns: true,
        locale: {
            separator: "-",
            format: 'DD-MMM-YYYY'
        },
        minYear: 1901,
        maxYear: parseInt(moment().format('YYYY'), 10)
    }).on('change', function (e) {
        greaterThanDate(e);
    });
}
function greaterThanDate(evt) {
    var fDate = $.trim($('#txtFromDate').val());
    var tDate = $.trim($('#txtToDate').val());
    if (fDate != "" && tDate != "") {
        if (new Date(tDate) >= new Date(fDate)) {
            return true;
        }
        else {
            evt.currentTarget.value = "";
            toastr.warning("To date must be greater than From date !");
            DatePicker();
            return false;
        }
    }
    else {
        return true;
    }
}

function LabStockUpload(UploadType) {
    $("#lblLabStockUploadMsg").html("");
    $(".btnLabStockUpload").hide();
    debugger
    var data = {};
    data.DataTransferType = UploadType;

    $.ajax({
        url: "/Lab/LabAPI_LabDataUpload_Ora",
        async: false,
        data: data,
        dataType: "json",
        success: function (data) {
            debugger
            $("#lblLabStockUploadMsg").html("");
            $(".btnLabStockUpload").hide();

            setTimeout(function () {
                LabStockStatusGet();
            }, 500);
        }
    });
}
function LabStockStatusGet() {
    $.ajax({
        url: "/Lab/LabStockStatusGet",
        async: false,
        type: "POST",
        data: null,
        success: function (data, textStatus, jqXHR) {
            if (data.Status == "1") {
                $("#lblLabStockUploadMsg").html(data.Message);
                document.getElementById("lblLabStockUploadMsg").style.color = "green";
                $(".btnLabStockUpload").show();
            } else {
                $("#lblLabStockUploadMsg").html(data.Message);
                document.getElementById("lblLabStockUploadMsg").style.color = "red";
                $(".btnLabStockUpload").hide();
            }
        }
    });
}
function LabStockStatusGet_Second() {
    LabStockStatusGet();
    setInterval(LabStockStatusGet, 5000);
}