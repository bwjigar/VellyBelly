var gridOptions = {};
var GridpgSize = 50;
var rowData = [];
var iTransId = 0;
var _Customer_Id = "", _MemberId = "";

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

var AddNew = function () {
    window.location.href = '/Master/SchemeDet';
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
    //$("#txtSearch").val("");
    //DatePicker();
    btnSearch();
}

var columnDefs = [
    { headerName: "Sr.", field: "iSr", width: 40, sortable: false },
    { headerName: "Entry Date", field: "EntryDate", width: 135, sortable: true },
    { headerName: "Customer_Id", field: "Customer_Id", width: 130, hide:true },
    { headerName: "Member Id", field: "MemberId", width: 100 },
    { headerName: "Customer Name", field: "Customer_Name", width: 300 },
    { headerName: "Mobile", field: "Mobile", width: 100 },
    { headerName: "Action", field: "bIsAction", width: 60, cellRenderer: UserDetailPage },
];
function UserDetailPage(params) {
    var str = '<a onclick="EditData(\'' + params.data.Customer_Id + '\')" ><i class="fa fa-pencil-square-o" aria-hidden="true" style="font-size: 17px;cursor:pointer;"></i></a>'

    if (params.data.SchemeAllocated == false) {
        str += '&nbsp;&nbsp;<a onclick="DeleteUserDetail(\'' + params.data.Customer_Id + '\',\'' + params.data.MemberId + '\')" ><i class="fa fa-trash-o" aria-hidden="true" style="font-size: 17px;cursor:pointer;"></i></a>';
    }
    else {
        str += '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
    }
    return str;
}
function EditData(Customer_Id) {
    location.href = '/Master/SchemeDet?C=' + Customer_Id;
}

function DeleteUserDetail(Customer_Id, MemberId) {
    _Customer_Id = Customer_Id;
    _MemberId = MemberId;

    $("#Remove").modal("show");
}

function ClearRemoveModel() {
    _Customer_Id = "";
    _MemberId = "";

    $("#Remove").modal("hide");
}
function DeleteData() {
    loaderShow();

    var obj = {};
    obj.Type = "SchemeDet";
    obj.MemberId = _MemberId;
    obj.Customer_Id = _Customer_Id;
    $.ajax({
        url: "/Master/Scheme_Delete",
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
                ClearRemoveModel();
                toastr.success("Scheme Detail Delete Successfully");
                GetDataList();
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            loaderHide();
            toastr.error(textStatus);
        }
    });
}


var gridDiv = document.querySelector('#myGrid');
function GetDataList() {
    loaderShow();
    if (gridOptions.api != undefined) {
        gridOptions.api.destroy();
    }

    gridOptions = {
        wrapText: true,
        masterDetail: true,
        detailCellRenderer: 'myDetailCellRenderer',
        groupDefaultExpanded: 1,
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
        //formData.append('dtFromDate', $('#txtFromDate').val());
        //formData.append('dtToDate', $("#txtToDate").val());
        //formData.append('sSearch', $("#txtSearch").val());
        formData.append('sPgNo', PageNo);
        formData.append('sPgSize', GridpgSize);
        formData.append('OrderBy', OrderBy);

        $.ajax({
            url: "/Master/MemberWise_SchemeDet_Select",
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
                            //toastr.warning(data.Message);
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
function btnSearch() {
    GetDataList();
}
$(document).ready(function (e) {
    //DatePicker();
    GetDataList();
    contentHeight();
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

