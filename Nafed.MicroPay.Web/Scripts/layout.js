jQuery(document).ready(function ($) {
    var alterClass = function () {
        var ww = document.body.clientWidth;
        if (ww < 450) {
            $('#banner').addClass('small');
            $('.logo').addClass('sm_logo');

          
            var sc = $(window).scrollTop(); 
            if (sc == 0) {
                $("#banner").addClass("small");
                $('.logo').addClass('sm_logo');
            }

        } else if (ww >= 451) {
            $('#banner').removeClass('small');
            $('.logo').removeClass('sm_logo');
        };
    };
    $(window).resize(function () {
        alterClass();
    });
    //Fire it when the page first loads:
    alterClass();
});




$(window).scroll(function () {

    var sc = $(window).scrollTop(); 
     var ww = document.body.clientWidth;
     if (sc == 0 && ww < 450) {
        $("#banner").addClass("small");
        $('.logo').addClass('sm_logo');
    }
    else if (sc > 30) {
        $("#banner").addClass("small");
        $(".logo").addClass("sm_logo");
        $(".content-x").addClass("content1");
    }
  
    else {
        $("#banner").removeClass("small")
        $(".logo").removeClass("sm_logo")
        $(".content-x").removeClass("content1");
    }
});



$(document).ready(function () {

    //$(document).find('.pull-right').removeClass('pull-right').addClass('float-right');

    //$('.modal').on('show.bs.modal', '.modal', function (e) {
    //    $(this).find('.pull-right').removeClass('pull-right').addClass('float-right');
    //});

    $('.collapse').on('show.bs.collapse', function () {
        var id = $(this).attr('id');
        $('a[href="#' + id + '"]').closest('.panel-heading').addClass('active-faq');
        $('a[href="#' + id + '"] .panel-title span').html('<i class="glyphicon glyphicon-minus"></i>');
    });
    $('.collapse').on('hide.bs.collapse', function () {
        var id = $(this).attr('id');
        $('a[href="#' + id + '"]').closest('.panel-heading').removeClass('active-faq');
        $('a[href="#' + id + '"] .panel-title span').html('<i class="glyphicon glyphicon-plus"></i>');
    });
    $('.advance-search select > option').each(function () {
        $(this).addClass("newoption");
        $(this).text($(this).text().toUpperCase());
       
    });



    ///==== Adding jquery datatable reference to all master grid ==== (Sujeet G)
    var table = $('#tblGrid').DataTable();

    //=== re-adjust datatable column on bootstrap tab click event === (Sujeet G / 08-Jan-2020)
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $($.fn.dataTable.tables(true)).DataTable()
           .columns.adjust()
           .responsive.recalc();
    });


    hidecolumnsfromdatatableexport(table);
    $('#tblGrid_length').find('select').addClass('custom-select');
    //========= End==================================================

    $('.TextEditor').summernote({
        placeholder: 'Enter Help',
        tabsize: 2,
        height: 200,
        styleWithSpan: false,
        toolbar: [
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['color', ['color']],
            ['para', ['ul', 'ol']]
        ]
    });
});



(function ($) {
    $('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
        if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
        }
        var $subMenu = $(this).next(".dropdown-menu");
        $subMenu.toggleClass('show');

        $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
            $('.dropdown-submenu .show').removeClass("show");
        });
        return false;
    });
})(jQuery)


$(document).on("click", "[data-toggle=\"confirm\"]", function (e) {
    e.preventDefault();
    var lHref = $(this).attr('href');
    var lText = this.attributes.getNamedItem("data-title") ? this.attributes.getNamedItem("data-title").value : "Are you sure you want to delete this record ?";
    // If data-title is not set use default text

    bootbox.confirm("<span style='font-size:medium;'> <i class='fa fa-question-circle' aria-hidden='true'></i> &nbsp;" + lText + "</span>", function (confirmed) {
        if (confirmed) {
            window.location.href = lHref;

        }
    });
});



$(document).on("click", "[data-toggle=\"ajax-confirm\"]", function (e) {
    e.preventDefault();
    // var lHref = $(this).attr('href');
    var lText = this.attributes.getNamedItem("data-title") ? this.attributes.getNamedItem("data-title").value : "Are you sure you want to delete this record ?";
    // If data-title is not set use default text

    bootbox.confirm("<span style='font-size:medium;'> <i class='fa fa-question-circle' aria-hidden='true'></i> &nbsp;" + lText + "</span>", function (confirmed) {
        if (confirmed) {
            return true;
        } else {
            return false;
        }
    });
});

$(document).on("click", "[data-toggle=\"fileClose\"]", function (e) {
    e.preventDefault();
    var lHref = $(this).attr('href');
    var lText = this.attributes.getNamedItem("data-title") ? this.attributes.getNamedItem("data-title").value : "Are you sure you to close this file.?";
    // If data-title is not set use default text

    bootbox.confirm("<span style='font-size:medium;'> <i class='fa fa-question-circle' aria-hidden='true'></i> &nbsp;" + lText + "</span>", function (confirmed) {
        if (confirmed) {
            window.location.href = lHref;

        }
    });
});


function commonAlert(msg) {
    bootbox.alert({
        message: "<span style='font-size:medium;'>" + msg + "</span>",
        size: 'small',
        callback: function () { /* your callback code */ }
    });
}

var events = [{ start: '2017/09/30', end: '2017/10/07', summary: "Example Event", mask: true }, { start: '2017/10/08', end: '2017/10/13', summary: "Example Event #3", mask: true }];

$('#calendar').calendar({ 'events': events });

var calendar1 = new Date();
calendar1.setDate(1);
calendar1.setMonth(calendar1.getMonth() - 1);

$('#calendar-1').calendar({ color: 'orange', date: calendar1 });

var calendar2 = new Date();
calendar2.setDate(1);
calendar2.setMonth(calendar2.getMonth() + 1);

$('#calendar-2').calendar({ color: 'purple', date: calendar2, events: events });

var calendar3 = new Date();
calendar3.setDate(1);
calendar3.setMonth(calendar3.getMonth() + 2);

$('#calendar-3').calendar({ color: 'yellow', date: calendar3 });

var calendar4 = new Date();
calendar4.setDate(1);
calendar4.setMonth(calendar4.getMonth() + 3);

$('#calendar-4').calendar({ color: 'blue', date: calendar4 });

var calendar5 = new Date();
calendar5.setDate(1);
calendar5.setMonth(calendar5.getMonth() + 4);

$('#calendar-5').calendar({ color: 'green', date: calendar5 });

var calendar6 = new Date();
calendar6.setDate(1);
calendar6.setMonth(calendar6.getMonth() + 5);

$('#calendar-6').calendar({ color: 'grey', date: calendar6 });

var calendar7 = new Date();
calendar7.setDate(1);
calendar7.setMonth(calendar7.getMonth() + 6);

$('#calendar-7').calendar({ color: 'pink', date: calendar7 });

function hidecolumnsfromdatatableexport(table) {
    $(table).find('thead').each(function (i, el) {
        $(this).find('tr').each(function (i, el) {
            $(this).find('th').each(function (i, el) {
                if ($(this).text().trim() == "Edit" || $(this).text().trim() == "Delete" || $(this).text().trim() == "Action" || $(this).text().trim() == "View" || $(this).text().trim() == "Profile Picture") {
                    $(this).removeClass("sorting");
                }
            });

        });

    });
}

function integerOnly(evt) {
    var charCode = evt.which ? evt.which : event.keyCode;
    if (charCode <= 57 && charCode >= 48) {
        return true;
    } else {
        return false;
    }
}

$(function () {
    // Create the close button
    var closebtn = $('<button/>', {
        type: "button",
        text: 'x',
        id: 'close-preview',
        style: 'font-size: initial;',
    });
    closebtn.attr("class", "close pull-right");
    // Set the popover default content
    $('.image-preview').popover({
        trigger: 'manual',
        html: true,
        title: "<strong>Preview</strong>" + $(closebtn)[0].outerHTML,
        content: "There's no image",
        placement: 'bottom'
    });
    // Clear event
    $('.image-preview-clear').click(function () {
        $('.image-preview').attr("data-content", "").popover('hide');
        $('.image-preview-filename').val("");
        $('.image-preview-clear').hide();
        $('.image-preview-input input:file').val("");
        $(".image-preview-input-title").text("Browse");
    });
    // Create the preview image
    $(".image-preview-input input:file").change(function () {
        //var img = $('<img/>', {
        //    id: 'dynamic',
        //    width: 250,
        //    height: 200
        //});
        var file = this.files[0];
        var reader = new FileReader();
        // Set preview image into the popover data-content
        reader.onload = function (e) {
            $(".image-preview-input-title").text("Change");
            $(".image-preview-clear").show();
            $(".image-preview-filename").val(file.name);
            //img.attr('src', e.target.result);
            //$(".image-preview").attr("data-content", $(img)[0].outerHTML).popover("show");
        }
        reader.readAsDataURL(file);
    });

    $(".image-preview-input-multiple input:file").change(function () {
        var files = $("#file").get(0).files;
        var totalFiles = files.length;
        var fileName = "";
        for (var i = 0; i < files.length; i++) {
            fileName += files[i].name + ",";
        }
        $(".image-preview-filename").val(fileName.replace(/,\s*$/, ""));
        $(".image-preview-input-title").text("Change");
        $(".image-preview-clear").show();
    });
});
