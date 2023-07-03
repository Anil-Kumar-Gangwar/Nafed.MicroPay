function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function NumberOnly(e) {
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        e.preventDefault();
    }
}

function DecimalNumberOnly(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    //    if (charCode > 187 || charCode > 222 || charCode > 31 && (charCode < 48 || charCode > 57) && (charCode <= 36 || charCode > 46) && (charCode > 185 || charCode < 223)) {
    //        return false;
    //    }
    if (charCode == 8 || charCode == 37) {
        return true;
    } else if (charCode == 46 && evt.path[0].value.indexOf('.') != -1) {
        return false;
    } else if (charCode > 31 && charCode != 46 && charCode != 44 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

//for datetime picker dropdown on mozilla
//$.fn.modal.Constructor.prototype.enforceFocus = function () { };

function ConvertMoneywithComma(MoneyVal) {
    if (MoneyVal != "" && MoneyVal != null) {
        MoneyVal = MoneyVal.toString();
        if (MoneyVal.trim() != "") {
            var val = MoneyVal.replace(/,/g, '');
            if (val >= 1000) {
                var values = [];
                values = val.split('.');
                var firstval = values[0];
                var len = firstval.length;
                var last3 = firstval.substr(len - 3, len);
                var beforelast3 = firstval.substr(0, len - 3);
                var lastval = values.length > 1 ? values[1] : "";

                var splitString = beforelast3.split("");
                var reverseArray = splitString.reverse();
                var rev = reverseArray.join("");

                var result = "";
                for (var i = 0; i < rev.length; i++) {
                    var c = rev[i];
                    if (i % 2 != 0)
                        result += c;
                    else
                        result += "," + c;
                }

                var splitString2 = result.split("");
                var reverseArray2 = splitString2.reverse();
                var res = reverseArray2.join("");
                return res = res + last3 + (lastval == "" ? ".00" : ("." + lastval));
            }
            else {
                if (MoneyVal.includes("."))
                    return MoneyVal;
                else
                    return MoneyVal + ".00";
            }
        }
        else
            return MoneyVal;

    }
    else
        return MoneyVal;
}


function CFormat(Id) {
    var MoneyVal = $('#' + Id).val();
    if (MoneyVal != "" && MoneyVal != null) {
        if (MoneyVal.trim() != "") {
            var val = MoneyVal.replace(/,/g, '');
            if (val >= 1000) {
                var values = [];
                values = val.split('.');
                var firstval = values[0];
                var len = firstval.length;
                var last3 = firstval.substr(len - 3, len);
                var beforelast3 = firstval.substr(0, len - 3);
                var lastval = values.length > 1 ? values[1] : "";

                var splitString = beforelast3.split("");
                var reverseArray = splitString.reverse();
                var rev = reverseArray.join("");

                var result = "";
                for (var i = 0; i < rev.length; i++) {
                    var c = rev[i];
                    if (i % 2 != 0)
                        result += c;
                    else
                        result += "," + c;
                }
                var splitString2 = result.split("");
                var reverseArray2 = splitString2.reverse();
                var res = reverseArray2.join("");
                res = res + last3 + (lastval == "" ? ".00" : ("." + lastval));
                $('#' + Id).val(res);
            }
        }
    }
    else
        $('#' + Id).val('');
}

function CFormatNumberOnly(Id) {
    var MoneyVal = $('#' + Id).val();
    if (MoneyVal != "" && MoneyVal != null) {
        if (MoneyVal.trim() != "") {
            var val = MoneyVal.replace(/,/g, '');
            if (val >= 1000) {
                var values = [];
                values = val.split('.');
                var firstval = values[0];
                var len = firstval.length;
                var last3 = firstval.substr(len - 3, len);
                var beforelast3 = firstval.substr(0, len - 3);
                var lastval = values.length > 1 ? values[1] : "";
                var splitString = beforelast3.split("");
                var reverseArray = splitString.reverse();
                var rev = reverseArray.join("");
                var result = "";
                for (var i = 0; i < rev.length; i++) {
                    var c = rev[i];
                    if (i % 2 != 0)
                        result += c;
                    else
                        result += "," + c;
                }
                var splitString2 = result.split("");
                var reverseArray2 = splitString2.reverse();
                var res = reverseArray2.join("");
                //res = res + last3 + (lastval == "" ? ".00" : ("." + lastval));
                res = res + last3;
                $('#' + Id).val(res);
            }
        }
    }
    else
        $('#' + Id).val('');
}

function ChangeDecimalToNumber(MoneyVal) {
    //alert(MoneyVal);
    if (MoneyVal != "" && MoneyVal != null) {
        MoneyVal = MoneyVal.toString();
        if (MoneyVal.trim() != "") {
            var val = MoneyVal.replace(/,/g, '');
            var values = [];
            values = val.split('.');
            var firstval = values[0];
            var len = firstval.length;
            var last3 = firstval.substr(len - 3, len);
            var beforelast3 = firstval.substr(0, len - 3);
            var lastval = values.length > 1 ? values[1] : "";

            var splitString = beforelast3.split("");
            var reverseArray = splitString.reverse();
            var rev = reverseArray.join("");

            var result = "";
            for (var i = 0; i < rev.length; i++) {
                var c = rev[i];
                if (i % 2 != 0)
                    result += c;
                else
                    result += "," + c;
            }

            var splitString2 = result.split("");
            var reverseArray2 = splitString2.reverse();
            var res = reverseArray2.join("");
            //res = res + last3 + (lastval == "" ? ".00" : ("." + lastval));
            res = res + last3;
            return res;
        }
        else
            return MoneyVal;
    }
    else
        return MoneyVal;
}



function RemoveComma(MoneyVal) {
    if (MoneyVal != "" && MoneyVal != null) {
        var val = MoneyVal.replace(/,/g, '');
        return val;
    }
    else
        return MoneyVal;
}

function NumberFormat(Value, Id) {

    var NVal = '';
    if (Value == '' && Value == null)
        NVal = $('#' + Id).val();
    else
        NVal = Value;
    NVal = NVal.toString();
    if (NVal.trim() != "") {
        var Negative = NVal.substr(0, 1) == "-" ? "-" : "";
        var val = NVal.replace(/,/g, '');
        val = Negative != "" ? NVal.replace('-', '') : val;
        if (val >= 1000) {
            var firstval = val;
            var len = firstval.length;
            var last3 = firstval.substr(len - 3, len);
            var beforelast3 = firstval.substr(0, len - 3);
            var splitString = beforelast3.split("");
            var reverseArray = splitString.reverse();
            var rev = reverseArray.join("");

            var result = "";
            for (var i = 0; i < rev.length; i++) {
                var c = rev[i];
                if (i % 2 != 0)
                    result += c;
                else
                    result += "," + c;
            }

            var splitString2 = result.split("");
            var reverseArray2 = splitString2.reverse();
            var res = reverseArray2.join("");
            res = res + last3
            res = Negative != "" ? Negative + res : res;
            return res;
        }
        else
            return NVal;
    }
    else
        return NVal;
}

//function allowGeneralCharacter(obj, e) {
//    var value = $("#" + obj.id).val()
//    var regex = /^[a-zA-Z ()., ]*$/;
//    var isSplChar = regex.test(value);


//    if (!isSplChar) {
//        var chr = String.fromCharCode(e.keyCode);
//        // var chr = String.fromCharCode(!e.charCode ? e.which : e.charCode);
//        var NewVal = value.replace(chr, '');

//        $("#" + obj.id).val(NewVal);

//        $.bootstrapGrowl("<i class='fa fa-exclamation-triangle' aria-hidden='true'></i> Only alphabets and ()., characters are allowed.", {
//            type: 'warning',
//            align: 'center',
//            width: 'auto',
//            allow_dismiss: true,
//            delay: 5000
//        });

//        return;
//    }
//    else
//        return;
//}

//function allowAlphaNumericCharacter(obj) {

//    var value = $("#" + obj.id).val()
//    var regex = /^[a-zA-Z0-9]*$/;
//    var isSplChar = regex.test(value);

//    if (!isSplChar) {
//        $.bootstrapGrowl("<i class='fa fa-exclamation-triangle' aria-hidden='true'></i> Only alphabets and ()., characters are allowed.", {
//            type: 'warning',
//            align: 'center',
//            width: 'auto',
//            allow_dismiss: true,
//            delay: 5000
//        });

//        $("#" + obj.id).focus();
//        return;
//    }
//    else
//        return;
//}

function onFocus(el) {
    if (el.value == '0' || el.value == '0.00' || el.value == '.' || el.value == '0.0' || el.value == '.0')
        el.value = '';
}



//-------------------------through View-------------------------
//$(".delete-link").click(function () {

//    var i = $(".delete-link").index(this) + 1;
//    $("#delete-link_" + i).hide();
//    $("#delete-confirm_" + i).show();
//});

//$(".delete-confirm").click(function () {
//    var i = $(".delete-confirm").index(this) + 1;
//    $("#delete-confirm_" + i).hide();
//    $("#delete-link_" + i).show();
//});

//----------------------------------------------------------------



////----------------------through Javascript-----------------------

//function P_DeleteFunc(rowNo) {

//    var deleteLink = $("#ALink_DeleteLink" + rowNo);
//    deleteLink.hide();
//    var confirmButton = $("#ALink_DeleteConfirm" + rowNo);
//    confirmButton.show();
//}

//function P_DeleteCancel(rowNo) {
//    var confirmButton = $("#ALink_DeleteConfirm" + rowNo);
//    confirmButton.hide();
//    var deletelink = $("#ALink_DeleteLink" + rowNo);
//    deletelink.show();
//}

////----------------------------------------------------------------
//-------------------------through View-------------------------

$(".table").on('click', '.delete-link', function () {

    var deleteLink = $(this);
    deleteLink.hide();
    var confirmButton = deleteLink.siblings(".delete-confirm");
    confirmButton.show();
    return false
});

$(".table").on('click', '.delete-cancel', function () {

    //    var Id = this.id;
    //    var i = Id.substring(Id.indexOf("_") + 1, Id.length);
    //    $("#delete-confirm_" + i).hide();
    //    $("#delete-link_" + i).show();

    var confirmButton = $(".delete-confirm");
    confirmButton.hide();
    var deletelink = $(".delete-link");
    deletelink.show();
    return false
});
//----------------------------------------------------------------

function GetCombination(FlagValue) {
    var res = "";
    $.ajax({
        url: '/SurveyBank/GetCombination',
        type: "GET",
        dataType: "JSON",
        async: false,
        data: { Value: FlagValue },
        success: function (data) {
            res = data;
        }
    });

    return res;
}


function DateTime(datetime) {

    //-----------------For Get Date Time------------------
    var str = (datetime).split(" ");
    var strdate = str[0].split("-");

    var monthNames = [
                                    "Jan", "Feb", "Mar",
                                    "Apr", "May", "Jun", "Jul",
                                    "Aug", "Sep", "Oct",
                                    "Nov", "Dec"
    ];


    var DateMonthYear = parseInt(strdate[0]) + ' ' + monthNames[parseInt(strdate[1]) - 1] + ' ' + parseInt(strdate[2]);


    //--------------------For Get Time------------------
    var str1 = (datetime).split(" ");
    var strtime = str1[1].split(":");
    var HourIndex = [
                                    "01", "02", "03",
                                    "04", "05", "06", "07",
                                    "08", "09", "10",
                                    "11", "12",
                                    "01", "02", "03",
                                    "04", "05", "06", "07",
                                    "08", "09", "10",
                                    "11", "12"
    ];

    var MinuteIndex = [
                                    "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
                                    "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
                                    "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
                                    "31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
                                    "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
                                    "51", "52", "53", "54", "55", "56", "57", "58", "59", "60",
    ];

    var Hour = parseInt(strtime[0]);
    var Min = parseInt(strtime[1]);

    var Time = 0;
    if (Hour < 12) {
        Time = HourIndex[Hour] + ':' + MinuteIndex[Min] + ' AM';

    }

    else if ((Hour > 12) || (Hour == 12)) {
        Time = HourIndex[Hour - 1] + ':' + MinuteIndex[Min] + ' PM';

    }

    return DateMonthYear + '  ' + Time;

}


//Convert string into sentence case


$('option').each(function () {
    t = $(this).text();
    t = t.toLowerCase().split(' ');
    var res = '';
    for (var i = 0; i < t.length; i++) {
        res += t[i].substring(0, 1).toUpperCase() + t[i].substring(1) + ' ';

    }
    $(this).text(res);
});