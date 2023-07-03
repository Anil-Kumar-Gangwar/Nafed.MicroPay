/**
 * Theme: Hyper - Responsive Bootstrap 4 Admin Dashboard
 * Author: Coderthemes
 * Module/App: Form Advanced Page
 */


!function($) {
    'use strict';

    var AdvanceFormApp = function() {
        this.$body = $('body'),
        this.$window = $(window)
    };


    /** 
     * Initlizes the select2
    */
    AdvanceFormApp.prototype.initSelect2 = function() {
        // Select2
        $(".select2").select2();

        // Select2 with limiting
        $(".select2-limiting").select2({
            maximumSelectionLength: 2
        });
    },

    /** 
     * Initlized mask
    */
    AdvanceFormApp.prototype.initMask = function() {
        $('.date').mask('00/00/0000');
        $('.time').mask('00:00:00');
        $('.date_time').mask('00/00/0000 00:00:00');
        $('.cep').mask('00000-000');
        $('.crazy_cep').mask('0-00-00-00');
        $('.phone').mask('0000-0000');
        $('.sp_celphones').mask('(00) 00000-0000');
        $('.phone_us').mask('(000) 000-0000');
        $('.mixed').mask('AAA 000-S0S');
        $('.cpf').mask('000.000.000-00', { reverse: true });
        $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
        $('.money').mask('000.000.000.000.000,00', { reverse: true });
        $('.money2').mask("#.##0,00", { reverse: true });
        $('.ip_address').mask('099.099.099.099');
        $('.percent').mask('##0,00%', { reverse: true });
    },

    AdvanceFormApp.prototype.initDateRange = function() {
        $('#singledaterange').daterangepicker({
            "cancelClass": "btn-light",
            "applyButtonClasses": "btn-success"
        });
        $('#daterangetime').daterangepicker({
            timePicker: true,
            "cancelClass": "btn-light",
            startDate: moment().startOf('hour'),
            endDate: moment().startOf('hour').add(32, 'hour'),
            locale: {
                format: 'DD/MM hh:mm A'
            }
        });
        $('#birthdatepicker').daterangepicker({
            singleDatePicker: true
        });

        //
        var start = moment().subtract(29, 'days');
        var end = moment();

        function cb(start, end) {
            $('#reportrange span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
        }

        $('#reportrange').daterangepicker({
            "cancelClass": "btn-light",
            startDate: start,
            endDate: end,
            ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            }
        }, cb);

        cb(start, end);
        
    },

    /** 
     * Initilize
    */
   AdvanceFormApp.prototype.init = function() {
        var $this = this;
        this.initSelect2();
        this.initMask();
        this.initDateRange();
    },

    $.AdvanceFormApp = new AdvanceFormApp, $.AdvanceFormApp.Constructor = AdvanceFormApp


}(window.jQuery),
    //initializing main application module
function($) {
    "use strict";
    $.AdvanceFormApp.init();
}(window.jQuery);
