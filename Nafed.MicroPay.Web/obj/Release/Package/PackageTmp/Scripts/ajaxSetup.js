
//==========Session Timeout for ajax Request ===============
$(document).ajaxError(function (xhr, props) {

    if (props.status === 308) {

        //  alert('timeout occur.');

        // window.location.href = "~/Login/LogOff";
        location.reload();

    } else {

    }
    $('#waitLoading').modal('hide');
});


$(document).ajaxError(function () {

    $('#waitLoading').modal('hide');
});

//$(form).submit(function () {
//    $('#waitLoading').modal('show');
//   // return true;
//});


$(document).ajaxStart(function () {
    $('#waitLoading').modal('show');
});

$(document).ajaxComplete(function () {

    $('#waitLoading').modal('hide');
    $('#tblGrid').DataTable().destroy();
    var table = $('#tblGrid').DataTable();
    var table = $("#tblGrid");

    hidecolumnsfromdatatableexport(table);
});
$(document).ajaxSuccess(function () {

    $('#waitLoading').modal('hide');
});

//var $document = $(document);

//handleAjaxError = function (XMLHttpRequest, textStatus, errorThrown) {
//    alert(XMLHttpRequest.status);
//    if (XMLHttpRequest.status == 401) {
//        //refresh the page, as we are not longer authorized
//        location.reload();
//    }
//};
//$document.ajaxComplete(function (event, XMLHttpRequest, AjaxOptions) {

//    handleAjaxError(XMLHttpRequest);
//});
//$(document).ajaxStart(function () {
//    $('#waitLoading').modal('show');
//});

//$(document).ajaxComplete(function () {
//    $('#waitLoading').modal('hide');
//});

//$(document).ajaxStop(function () {
//    $('#waitLoading').modal('hide');
//});
//===========================================================

function hidecolumnsfromdatatableexport(table) {
    table.find('thead').each(function (i, el) {
        $(this).find('tr').each(function (i, el) {
            $(this).find('th').each(function (i, el) {
                if ($(this).text().trim() == "Edit" || $(this).text().trim() == "Delete" || $(this).text().trim() == "Action" || $(this).text().trim() == "View" || $(this).text().trim() == "Profile Picture") {
                    $(this).removeClass("sorting");
                }
            });

        });

    });
}